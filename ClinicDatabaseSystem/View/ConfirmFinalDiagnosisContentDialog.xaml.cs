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
    public sealed partial class ConfirmFinalDiagnosisContentDialog : ContentDialog
    {
        private VisitInformation visitInformation;
        private AppointmentNameInfo appointmentNameInfo;
        private string finalDiagnosis;

        public ConfirmFinalDiagnosisContentDialog(VisitInformation visitInformation, AppointmentNameInfo appointmentNameInfo, string finalDiagnosis)
        {
            this.InitializeComponent();
            this.visitInformation = visitInformation;
            this.appointmentNameInfo = appointmentNameInfo;
            this.finalDiagnosis = finalDiagnosis;
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            VisitInformationDAL.AddFinalDiagnosis(this.visitInformation, this.finalDiagnosis);
        }

        private async void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ViewVisitInfoContentDialog viewVisitInfoContentDialog =
                new ViewVisitInfoContentDialog(this.appointmentNameInfo, this.finalDiagnosis);
            await viewVisitInfoContentDialog.ShowAsync();
        }
    }
}