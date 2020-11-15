﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
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
    public sealed partial class ViewVisitInfoContentDialog : ContentDialog
    {

        private AppointmentNameInfo appointmentNameInfo;
        private bool hasInitialFinalDiagnosis;

        public ViewVisitInfoContentDialog(AppointmentNameInfo appointmentNameInfo)
        {
            this.InitializeComponent();
            this.appointmentNameInfo = appointmentNameInfo;
            this.loadVistInfo();
        }

        public ViewVisitInfoContentDialog(AppointmentNameInfo appointmentNameInfo, string finalDiagnosis)
        {
            this.InitializeComponent();
            this.appointmentNameInfo = appointmentNameInfo;
            this.loadVistInfo();
            this.finalDiagnosisRichEditBox.Document.SetText(0, finalDiagnosis);
        }

        private void loadVistInfo()
        {
            this.patientIdTextBox.Text = this.appointmentNameInfo.Appointment.PatientId.ToString();
            this.patientNameTextBox.Text = this.appointmentNameInfo.PatientName;
            this.dateTextBox.Text = this.appointmentNameInfo.Appointment.ScheduledDate.Date.ToString("MM/dd/yyyy");
            
            var visitInfo = VisitInformationDAL.GetVisitInfoFromAppointment(this.appointmentNameInfo.Appointment)[0];
            this.systolicBpTextBox.Text = visitInfo.SystolicBp;
            this.diastolicBpTextBox.Text = visitInfo.DiastolicBp;
            this.bodyTempTextBox.Text = visitInfo.BodyTemp;
            this.pulseTextBox.Text = visitInfo.Pulse;
            this.weightTextBox.Text = visitInfo.Weight;

            this.symptomsRichEditBox.Document.SetText(TextSetOptions.None, visitInfo.Symptoms);
            this.symptomsRichEditBox.IsReadOnly = true;
            
            this.initialDiagnosisRichEditBox.Document.SetText(TextSetOptions.None, visitInfo.InitialDiagnosis);
            this.initialDiagnosisRichEditBox.IsReadOnly = true;
            
            if (visitInfo.FinalDiagnosis != null)
            {
                this.finalDiagnosisRichEditBox.Document.SetText(TextSetOptions.None, visitInfo.FinalDiagnosis);
                this.finalDiagnosisRichEditBox.IsReadOnly = true;
                this.finalDiagnosisRichEditBox.IsTabStop = false;
                this.hasInitialFinalDiagnosis = true;
            }
        }

        private async void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.finalDiagnosisRichEditBox.Document.GetText(TextGetOptions.None, out var finalDiagnosis);
            this.Hide();
            if (finalDiagnosis.Trim() != string.Empty && !this.hasInitialFinalDiagnosis)
            {
                var patientId = this.appointmentNameInfo.Appointment.PatientId;
                var date = this.appointmentNameInfo.Appointment.ScheduledDate;
                this.symptomsRichEditBox.Document.GetText(0, out var symptoms);
                this.initialDiagnosisRichEditBox.Document.GetText(0, out var diagnosis);
                var visitInformation = new VisitInformation(patientId, date,
                    this.systolicBpTextBox.Text, this.diastolicBpTextBox.Text, this.bodyTempTextBox.Text,
                    this.pulseTextBox.Text, this.weightTextBox.Text, symptoms, diagnosis, finalDiagnosis.Trim());
                ConfirmFinalDiagnosisContentDialog confirmFinalDiagnosisContentDialog = new ConfirmFinalDiagnosisContentDialog(visitInformation, this.appointmentNameInfo, finalDiagnosis.Trim());
                await confirmFinalDiagnosisContentDialog.ShowAsync();
            }
        }

        private bool hasFinalDiagnosis()
        {
            this.finalDiagnosisRichEditBox.Document.GetText(TextGetOptions.None, out var finalDiagnosis);
            return finalDiagnosis.Trim() != string.Empty;
        }

        private async void viewTestsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            OrderedTestsContentDialog orderedTestsContentDialog = new OrderedTestsContentDialog(this.appointmentNameInfo, this.hasFinalDiagnosis());
            await orderedTestsContentDialog.ShowAsync();
        }
    }
}
