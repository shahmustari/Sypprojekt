using CommunityToolkit.WinUI.UI.Controls;
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
    public sealed partial class RequestDetailPage : Page
    {
        private DB_Wittek _dbContext;
        private Angebot selectedAngebot;

        public Anfrage aktuelle;

        public RequestDetailPage()
        {
            this.InitializeComponent();

            _dbContext = new DB_Wittek();

        }

        private void LoadData()
        {
            loadingRing.IsActive = true;

            using (var db = new DB_Wittek())
            {
                var requestKey = aktuelle.AnfrageID; // Hier gehe ich davon aus, dass AnfrageID der Schlüssel für die Anfragen ist. Bitte ersetze es entsprechend.
                var relatedOffers = db.Angebote.Where(offer => offer.AnfrageID == requestKey).ToList();

                OfferDataGrid.ItemsSource = relatedOffers;
            }

            loadingRing.IsActive = false;
        }

        private List<Angebot> GetSelectedRows()
        {
            List<Angebot> selectedRows = new List<Angebot>();

            foreach (var item in OfferDataGrid.SelectedItems)
            {
                selectedRows.Add((Angebot)item);
            }

            return selectedRows;
        }

        private void DeleteSelectedRows(List<Angebot> selectedRows)
        {
            if (selectedRows.Count == 0)
            {
                // Keine ausgewählten Zeilen
                return;
            }

            foreach (var row in selectedRows)
            {
                _dbContext.Angebote.Remove(row);
            }

            _dbContext.SaveChanges();
            LoadData(); // Die Ansicht aktualisieren
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            loadingRing.IsActive = true;
            List<Angebot> selectedRows = GetSelectedRows();
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
            if (selectedAngebot != null)
            {
                // Übergeben Sie den ausgewählten Datensatz an die OfferDetailPage
                Frame.Navigate(typeof(OfferDetailPage), selectedAngebot);
            }
        }

        private void dataGrid_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (OfferDataGrid.SelectedItems.Count == 1)
            {
                // Wenn genau ein Datensatz ausgewählt ist, aktivieren Sie den "Edit" Button
                selectedAngebot = (Angebot)OfferDataGrid.SelectedItem;
                EditButton.IsEnabled = true;
            }
            else
            {
                // Andernfalls deaktivieren Sie den "Edit" Button
                EditButton.IsEnabled = false;
                selectedAngebot = null;
            }
        }

        // Override the OnNavigatedTo method to populate TextBox controls with data
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is int personID)
            {
                // Now you have the personID, and you can use it as needed
                PersonIDTextBox.Text = personID.ToString();

                // Additional logic if needed...
            }
            else if (e.Parameter is Anfrage data)
            {
                aktuelle = data;

                LoadData();

                AnfrageDatumDatePicker.Date = data.AnfrageDatum.Date;
                AnfrageDatumTimePicker.Time = data.AnfrageDatum.TimeOfDay;
                KostenintervallTextBox.Text = data.Kostenintervall;
                ZeitintervallTextBox.Text = data.Zeitintervall;
                AnfragestatusTextBox.Text = data.Anfragestatus;
                PersonIDTextBox.Text = data.PersonID.ToString();
            }
            else
            {
                AnfrageDatumDatePicker.Date = new DateTime(2100, 1, 1);
                AnfrageDatumTimePicker.Time = new TimeSpan(12, 0, 0);
            }
        }


        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            loadingRing.IsActive = true;

            try
            {
                var anfrageDate = AnfrageDatumDatePicker.Date;
                var anfrageTime = AnfrageDatumTimePicker.Time;

                if (aktuelle is not Anfrage data)
                {
                    // Erstelle einen neuen Datensatz
                    var newAnfrage = new Anfrage
                    {
                        AnfrageDatum = new DateTime(
                            anfrageDate.Year, anfrageDate.Month, anfrageDate.Day,
                            anfrageTime.Hours, anfrageTime.Minutes, anfrageTime.Seconds, DateTimeKind.Utc
                        ),
                        Kostenintervall = KostenintervallTextBox.Text,
                        Zeitintervall = ZeitintervallTextBox.Text,
                        Anfragestatus = AnfragestatusTextBox.Text,
                        PersonID = int.Parse(PersonIDTextBox.Text),
                };

                    using (var db = new DB_Wittek())
                    {
                        db.Add(newAnfrage);
                        await db.SaveChangesAsync();
                    }
                    loadingRing.IsActive = false;
                }
                else
                {
                    aktuelle.AnfrageDatum = new DateTime(
                            anfrageDate.Year, anfrageDate.Month, anfrageDate.Day,
                            anfrageTime.Hours, anfrageTime.Minutes, anfrageTime.Seconds, DateTimeKind.Utc
                        );
                    aktuelle.Kostenintervall = KostenintervallTextBox.Text;
                    aktuelle.Zeitintervall = ZeitintervallTextBox.Text;
                    aktuelle.Anfragestatus = AnfragestatusTextBox.Text;
                    aktuelle.PersonID = int.Parse(PersonIDTextBox.Text);

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
            Frame.Navigate(typeof(OfferDetailPage), aktuelle.AnfrageID);
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
