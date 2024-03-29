﻿using ClinicDatabaseSystem.Controller;
using ClinicDatabaseSystem.DAL;
using Microsoft.Toolkit.Uwp.UI.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ClinicDatabaseSystem.View
{
    /// <summary>
    /// Holds the admin capabilities including their ability to query.
    /// </summary>
    public sealed partial class AdminQueryPage : Page
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminQueryPage"/> class.
        /// </summary>
        public AdminQueryPage()
        {
            this.InitializeComponent();
            this.updateCurrentUserTextBlocks();
        }

        private void updateCurrentUserTextBlocks()
        {
            this.fullNameTextBlock.Text =
                LoginController.CurrentAdministrator.FirstName + " " + LoginController.CurrentAdministrator.LastName;
            this.usernameTextBlock.Text = LoginController.CurrentAdministrator.AccountId;
            this.idTextBlock.Text = LoginController.CurrentAdministrator.AdminId.ToString();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = AdministrationDAL.AdminQuery(this.queryTextBox.Text);
                this.loadDataTable(result);
            }
            catch (MySqlException sqlExc)
            {
                var messageDialog =
                    new MessageDialog("Error No: " + sqlExc.Number + ": " + sqlExc.Message, "Sql Error") {
                        CancelCommandIndex = 0, DefaultCommandIndex = 0
                    };
                await messageDialog.ShowAsync();
            }
            catch (Exception)
            {
                var messageDialog = new MessageDialog("Invalid sql query.", "Sql Error") {
                    CancelCommandIndex = 0, DefaultCommandIndex = 0
                };
                await messageDialog.ShowAsync();
            }
        }

        private void loadDataTable(DataTable result)
        {
            this.dataGrid.Columns.Clear();
            if (result == null)
            {
                this.queryErrorTextBlock.Text = "Error in sql query";
                this.queryErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }
            this.queryErrorTextBlock.Visibility = Visibility.Collapsed;
            for (var i = 0; i < result.Columns.Count; i++)
            {
                this.dataGrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = result.Columns[i].ColumnName,
                    Binding = new Binding { Path = new PropertyPath("[" + i.ToString() + "]") }
                });
            }
            var collection = new ObservableCollection<object>();
            foreach (DataRow row in result.Rows)
            {
                collection.Add(row.ItemArray);
            }

            this.dataGrid.ItemsSource = collection;
        }

        private async void viewReportButton_Click(object sender, RoutedEventArgs e)
        {
            var begDate = this.startDatePicker.Date.DateTime;
            var endDate = this.endDatePicker.Date.DateTime;

            try
            {
                var result =  AdministrationDAL.GenerateReport(begDate, endDate);
                this.loadDataTable(result);
            }
            catch (MySqlException sqlExc)
            {
                var messageDialog =
                    new MessageDialog("Error No: " + sqlExc.Number + ": " + sqlExc.SqlState, "Sql Error")
                    {
                        CancelCommandIndex = 0,
                        DefaultCommandIndex = 0
                    };
                await messageDialog.ShowAsync();
            }
            catch (Exception)
            {
                var messageDialog = new MessageDialog("Invalid sql statement.", "Sql Error")
                {
                    CancelCommandIndex = 0,
                    DefaultCommandIndex = 0
                };
                await messageDialog.ShowAsync();
            }
        }

        private void queryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.queryTextBox.Text == string.Empty)
            {
                this.executeButton.IsEnabled = false;
            }
            else
            {
                this.executeButton.IsEnabled = true;
            }
        }

        private void StartDatePicker_OnDateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            this.checkStartAndEndDates();
        }

        private void checkStartAndEndDates()
        {
            if (this.startDatePicker.SelectedDate.HasValue && this.endDatePicker.SelectedDate.HasValue && this.endDatePicker.Date < this.startDatePicker.Date)
            {
                this.datesErrorTextBlock.Text = "Start date must come before end date.";
                this.datesErrorTextBlock.Visibility = Visibility.Visible;
                this.viewReportButton.IsEnabled = false;
            }
            else if (this.startDatePicker.SelectedDate.HasValue && this.endDatePicker.SelectedDate.HasValue)
            {
                this.datesErrorTextBlock.Visibility = Visibility.Collapsed;
                this.viewReportButton.IsEnabled = true;
            }
        }

        private void EndDatePicker_OnDateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            this.checkStartAndEndDates();
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
            LoginController.CurrentAdministrator = null;
            LoginController.CurrentNurse = null;
        }
    }
}
