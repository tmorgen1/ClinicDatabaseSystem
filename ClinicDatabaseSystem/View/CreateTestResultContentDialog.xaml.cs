﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ClinicDatabaseSystem.DAL;
using ClinicDatabaseSystem.Model;
using ClinicDatabaseSystem.ViewModel;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ClinicDatabaseSystem.View
{
    public sealed partial class CreateTestResultContentDialog : ContentDialog
    {
        private TestResult testResult;
        private string testName;
        private AppointmentNameInfo appointmentNameInfo;
        private bool viewResultsOnly;

        public CreateTestResultContentDialog(TestResult testResult, AppointmentNameInfo appointmentNameInfo, string testName, bool viewResultsOnly)
        {
            this.InitializeComponent();
            this.testResult = testResult;
            this.testName = testName;
            this.appointmentNameInfo = appointmentNameInfo;
            this.viewResultsOnly = viewResultsOnly;
            this.loadInfo();
        }

        private void loadInfo()
        {
            //TODO: load all info from database that isnt the result.
            this.patientIdTextBox.Text = this.appointmentNameInfo.Appointment.PatientId.ToString();
            this.patientNameTextBox.Text = this.appointmentNameInfo.PatientName;
            this.testIdTextBox.Text = this.testResult.TestId.ToString();
            this.testNameTextBox.Text = this.testName;
            this.dateTextBox.Text = this.testResult.ResultDateTime.Date.ToString();
            this.timeTextBox.Text = this.testResult.ResultDateTime.TimeOfDay.ToString();
        }

        private bool validateResults()
        {
            this.resultsRichEditBox.Document.GetText(0, out var reasons);
            reasons = reasons.Trim();
            return reasons != string.Empty;
        }

        private void checkButtonStatus()
        {
            this.confirmButton.IsEnabled = this.validateResults();
        }

        private async void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            OrderedTestsContentDialog orderedTestsContentDialog = new OrderedTestsContentDialog(this.appointmentNameInfo, this.viewResultsOnly);
            await orderedTestsContentDialog.ShowAsync();
        }

        private async void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            this.resultsRichEditBox.Document.GetText(0, out var results);
            var newTestResult = new TestResult(this.testResult.TestId, this.testResult.PatientId, this.testResult.ResultDateTime, results);
            if (TestResultDAL.EditTestResult(newTestResult, this.testResult))
            {
                this.Hide();
                OrderedTestsContentDialog orderedTestsContentDialog = new OrderedTestsContentDialog(this.appointmentNameInfo, this.viewResultsOnly);
                await orderedTestsContentDialog.ShowAsync();
            }
        }

        private void ResultsRichEditBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            this.checkButtonStatus();
            if (!this.validateResults())
            {
                this.resultsErrorTextBlock.Text = "Must have result.";
                this.resultsErrorTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                this.resultsErrorTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void ResultsRichEditBox_OnTextChanging(RichEditBox sender, RichEditBoxTextChangingEventArgs args)
        {
            this.checkButtonStatus();
            sender.Document.GetText(0, out var value);
            if (value != string.Empty)
            {
                this.resultsErrorTextBlock.Visibility = Visibility.Collapsed;
            }
        }
    }
}