using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace CRM.Pages
{
    public sealed partial class ProjectDetailPage : Page
    {

        private DB_Wittek _dbContext;
        private Service selectedService;

        public Projekt aktuelle;
            
        public ProjectDetailPage()
        {
            this.InitializeComponent();

            _dbContext = new DB_Wittek();

        }

        private void LoadData()
        {
            loadingRing.IsActive = true;

            using (var db = new DB_Wittek())
            {
                var projectKey = aktuelle.ProjektID; // Hier gehe ich davon aus, dass AnfrageID der Schlüssel für die Anfragen ist. Bitte ersetze es entsprechend.
                var relatedServices = db.Services.Where(offer => offer.ProjektID == projectKey).ToList();

                ServiceDataGrid.ItemsSource = relatedServices;
            }

            loadingRing.IsActive = false;
        }

        private List<Service> GetSelectedRows()
        {
            List<Service> selectedRows = new List<Service>();

            foreach (var item in ServiceDataGrid.SelectedItems)
            {
                selectedRows.Add((Service)item);
            }

            return selectedRows;
        }

        private void DeleteSelectedRows(List<Service> selectedRows)
        {
            if (selectedRows.Count == 0)
            {
                // Keine ausgewählten Zeilen
                return;
            }

            foreach (var row in selectedRows)
            {
                _dbContext.Services.Remove(row);
            }

            _dbContext.SaveChanges();
            LoadData(); // Die Ansicht aktualisieren
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            loadingRing.IsActive = true;
            List<Service> selectedRows = GetSelectedRows();
            DeleteSelectedRows(selectedRows);
            loadingRing.IsActive = false;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Starten Sie die Ladeanimation
            loadingRing.IsActive = true;

            // Warten Sie eine kurze Zeitspanne (kann entfernt werden, je nach Bedarf)
            await Task.Delay(1000);

            // Rufen Sie die Methode zum erneuten Laden der Datenbank auf
            LoadData();

            // Stoppen Sie die Ladeanimation
            loadingRing.IsActive = false;
        }



        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedService != null)
            {
                // Übergeben Sie den ausgewählten Datensatz an die OfferDetailPage
                Frame.Navigate(typeof(ServiceDetailPage), selectedService);
            }
        }


        private void dataGrid_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (ServiceDataGrid.SelectedItems.Count == 1)
            {
                // Wenn genau ein Datensatz ausgewählt ist, aktivieren Sie den "Edit" Button
                selectedService = (Service)ServiceDataGrid.SelectedItem;
                EditButton.IsEnabled = true;
            }
            else
            {
                // Andernfalls deaktivieren Sie den "Edit" Button
                EditButton.IsEnabled = false;
                selectedService = null;
            }
        }

        // Override the OnNavigatedTo method to populate TextBox controls with data
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (e.Parameter is int angebotID)
            {
                // Now you have the personID, and you can use it as needed
                AngebotIDTextBox.Text = angebotID.ToString();

                // Additional logic if needed...
            }
            else if (e.Parameter is Projekt data)
            {
                aktuelle = data;

                LoadData();

                DatumInbetriebnahmeDatePicker.Date = data.DatumInbetriebnahme.Date;
                DatumInbetriebnahmeTimePicker.Time = data.DatumInbetriebnahme.TimeOfDay;
                DatumOfenzeremonieDatePicker.Date = data.DatumOfenzeremonie.Date;
                DatumOfenzeremonieTimePicker.Time = data.DatumOfenzeremonie.TimeOfDay; 
                AngebotIDTextBox.Text = data.AngebotID.ToString();

            }
            else
            {
                DatumInbetriebnahmeDatePicker.Date = new DateTime(2100, 1, 1);
                DatumInbetriebnahmeTimePicker.Time = new TimeSpan(12, 0, 0);

                DatumOfenzeremonieDatePicker.Date = new DateTime(2100, 1, 1);
                DatumOfenzeremonieTimePicker.Time = new TimeSpan(12, 0, 0);
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            loadingRing.IsActive = true;

            try
            {
                var inbetriebnameDate = DatumInbetriebnahmeDatePicker.Date;
                var inbetriebnahmeTime = DatumInbetriebnahmeTimePicker.Time;

                var zeremonieDate = DatumOfenzeremonieDatePicker.Date;
                var zeremonieTime = DatumOfenzeremonieTimePicker.Time;

                if (aktuelle is not Projekt data)
                {
                    // Erstelle einen neuen Datensatz
                    var newProjekt = new Projekt
                    {
                        DatumInbetriebnahme = new DateTime(
                            inbetriebnameDate.Year, inbetriebnameDate.Month, inbetriebnameDate.Day,
                            inbetriebnahmeTime.Hours, inbetriebnahmeTime.Minutes, inbetriebnahmeTime.Seconds, DateTimeKind.Utc
                        ),
                        DatumOfenzeremonie = new DateTime(
                            zeremonieDate.Year, zeremonieDate.Month, zeremonieDate.Day,
                            zeremonieTime.Hours, zeremonieTime.Minutes, zeremonieTime.Seconds, DateTimeKind.Utc
                        ),
                        AngebotID = int.Parse(AngebotIDTextBox.Text),
                    };

                    using (var db = new DB_Wittek())
                    {
                        db.Add(newProjekt);
                        await db.SaveChangesAsync();
                    }
                    loadingRing.IsActive = false;
                }
                else
                {
                    aktuelle.DatumInbetriebnahme = new DateTime(
                            inbetriebnameDate.Year, inbetriebnameDate.Month, inbetriebnameDate.Day,
                            inbetriebnahmeTime.Hours, inbetriebnahmeTime.Minutes, inbetriebnahmeTime.Seconds, DateTimeKind.Utc
                        );
                    aktuelle.DatumOfenzeremonie = new DateTime(
                        zeremonieDate.Year, zeremonieDate.Month, zeremonieDate.Day,
                        zeremonieTime.Hours, zeremonieTime.Minutes, zeremonieTime.Seconds, DateTimeKind.Utc
                    );
                    aktuelle.AngebotID = int.Parse(AngebotIDTextBox.Text);

                    using (var db = new DB_Wittek())
                    {
                        db.Update(aktuelle);
                        await db.SaveChangesAsync();
                    }
                    loadingRing.IsActive = false;
                }


                loadingRing.IsActive = false;

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







        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ServiceDetailPage), aktuelle.ProjektID);
        }



        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            loadingRing.IsActive = true;
            try
            {
                // Speichern Sie die Änderungen in der DataGrid-Ansicht in der Datenbank
                await _dbContext.SaveChangesAsync();
                // Aktualisieren Sie die Ansicht, um sicherzustellen, dass sie die neuesten Daten aus der Datenbank anzeigt
                LoadData();
                loadingRing.IsActive = false;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Fehler beim Speichern der Änderungen: " + ex.Message);

                loadingRing.IsActive = false;
            }
        }
    }
}
