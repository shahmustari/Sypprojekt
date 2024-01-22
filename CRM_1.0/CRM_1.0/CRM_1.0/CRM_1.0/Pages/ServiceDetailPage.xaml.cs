using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace CRM.Pages
{
    public sealed partial class ServiceDetailPage : Page
    {
        public Service aktuelle;

        public ServiceDetailPage()
        {
            this.InitializeComponent();
        }

        // Override the OnNavigatedTo method to populate TextBox controls with data
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is int projectID)
            {
                // Now you have the personID, and you can use it as needed
                ProjektIDTextBox.Text = projectID.ToString();

                // Additional logic if needed...
            }
            if (e.Parameter is Service data)
            {
                aktuelle = data;

                ArtTextBox.Text = data.Art;
                DatumDatePicker.Date = data.Datum.Date;
                DatumTimePicker.Time = data.Datum.TimeOfDay; 
                InformationTextBox.Text = data.Information;
                KostenTextBox.Text = data.Kosten.ToString();
                AktivCheckBox.IsChecked = data.Aktiv;
                ProjektIDTextBox.Text = data.ProjektID.ToString();

            }
            else
            {
                DatumDatePicker.Date = new DateTime(2100, 1, 1);
                DatumTimePicker.Time = new TimeSpan(12, 0, 0);
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            loadingRing.IsActive = true;

            try
            {
                var date = DatumDatePicker.Date;
                var time = DatumTimePicker.Time;

                if (aktuelle is not Service data)
                {
                    // Erstelle einen neuen Datensatz
                    var newService = new Service
                    {
                        Art = ArtTextBox.Text,
                        Datum = new DateTime(
                            date.Year, date.Month, date.Day,
                            time.Hours, time.Minutes, time.Seconds, DateTimeKind.Utc
                            ),
                        Information = InformationTextBox.Text,
                        Kosten = string.IsNullOrWhiteSpace(KostenTextBox.Text) ? 0 : int.Parse(KostenTextBox.Text),
                        Aktiv = (bool)AktivCheckBox.IsChecked,
                        ProjektID = int.Parse(ProjektIDTextBox.Text),
                };

                    using (var db = new DB_Wittek())
                    {
                        db.Add(newService);
                        await db.SaveChangesAsync();
                    }
                    loadingRing.IsActive = false;
                }
                else
                {
                    aktuelle.Art = ArtTextBox.Text;
                    aktuelle.Datum = new DateTime(
                        date.Year, date.Month, date.Day, 
                        time.Hours, time.Minutes, time.Seconds, DateTimeKind.Utc
                        );
                    aktuelle.Information = InformationTextBox.Text;
                    aktuelle.Kosten = string.IsNullOrWhiteSpace(KostenTextBox.Text) ? 0 : int.Parse(KostenTextBox.Text);
                    aktuelle.Aktiv = (bool)AktivCheckBox.IsChecked;
                    aktuelle.ProjektID = int.Parse(ProjektIDTextBox.Text);

                    using (var db = new DB_Wittek())
                    {
                        db.Update(aktuelle);
                        await db.SaveChangesAsync();
                    }
                    loadingRing.IsActive = false;
                }


                // Zeige eine Erfolgsmeldung an
                ContentDialog dialog = new ContentDialog();

                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                dialog.XamlRoot = this.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "Datensatz wurde gespeichert";
                dialog.CloseButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Primary;

                var result = await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                loadingRing.IsActive = false;

                // Zeige eine Fehlermeldung an
                ContentDialog dialog = new ContentDialog();

                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                dialog.XamlRoot = this.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "Datensatz wurde NICHT gespeichert";
                dialog.CloseButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Primary;

                TextBlock textBox = new TextBlock
                {
                    Text = ex.Message,
                };
                dialog.Content = textBox;

                var result = await dialog.ShowAsync();
            }
        }
    }
}
