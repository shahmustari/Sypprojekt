using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace CRM.Pages
{
    public sealed partial class PersonPage : Page
    {
        private DB_Wittek _dbContext;
        private Person selectedAngebot; // Klassenvariable, um den ausgewählten Datensatz zu speichern


        public PersonPage()
        {
            this.InitializeComponent();

            _dbContext = new DB_Wittek();
            LoadData();
        }

        private void LoadData()
        {
            loadingRing.IsActive = true;
            // Retrieve data from the database
            var people = _dbContext.Person.ToList();


            // Set the DataGrid's ItemsSource to the retrieved data
            dataGrid.ItemsSource = people;
            loadingRing.IsActive = false;
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

        private List<Person> GetSelectedRows()
        {
            List<Person> selectedRows = new List<Person>();

            foreach (var item in dataGrid.SelectedItems)
            {
                selectedRows.Add((Person)item);
            }

            return selectedRows;
        }

        private void DeleteSelectedRows(List<Person> selectedRows)
        {
            if (selectedRows.Count == 0)
            {
                // Keine ausgewählten Zeilen
                return;
            }

            foreach (var row in selectedRows)
            {
                _dbContext.Person.Remove(row);
            }

            _dbContext.SaveChanges();
            LoadData(); // Die Ansicht aktualisieren
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            loadingRing.IsActive = true;
            List<Person> selectedRows = GetSelectedRows();
            DeleteSelectedRows(selectedRows);
            loadingRing.IsActive = false;
        }





        private void ExportSelectedRowsToCsv(List<Person> selectedRows)
        {
            if (selectedRows.Count == 0)
            {
                // Keine ausgewählten Zeilen
                return;
            }

            StringBuilder csvData = new StringBuilder();
            // Erstellen Sie die Headerzeile
            csvData.AppendLine("PersonID;Anrede1;Titel1;Nachname1;Vorname1;TelNr1;MobilNr1;Email1;Geburtstag1;Sternzeichen1;Anrede2;Titel2;Nachname2;Vorname2;TelNr2;MobilNr2;Email2;Geburtstag2;Sternzeichen2;PlzBst;OrtBest;StraßeBst;NrBest;Art;Leistung;PlzWhng;OrtWhng;StraßeWhng;NrWhng;wiezuuns;sonstiges;Erstkontakt;Termin;AKTIV");

            foreach (var row in selectedRows)
            {
                // Fügen Sie die Daten für jede Zeile hinzu und trennen Sie sie mit Semikolon
                csvData.AppendLine($"{row.PersonID};{row.Anrede1};{row.Titel1};{row.Nachname1};{row.Vorname1};{row.TelNr1};{row.MobilNr1};{row.Email1};{row.Geburtstag1};{row.Sternzeichen1};{row.Anrede2};{row.Titel2};{row.Nachname2};{row.Vorname2};{row.TelNr2};{row.MobilNr2};{row.Email2};{row.Geburtstag2};{row.Sternzeichen2};{row.PlzBst};{row.OrtBest};{row.StraßeBst};{row.NrBest};{row.Art};{row.Leistung};{row.PlzWhng};{row.OrtWhng};{row.StraßeWhng};{row.NrWhng};{row.Wiezuuns};{row.Sonstiges};{row.Erstkontakt};{row.Termin};{row.AKTIV}");
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string csvFilePath = Path.Combine(desktopPath, "selected_data_person.csv");

            File.WriteAllText(csvFilePath, csvData.ToString());

        }



        private async void ToCsvButton_Click(object sender, RoutedEventArgs e)
        {
            // Aktivieren Sie den Ladevorgang
            loadingRing.IsActive = true;

            await Task.Run(() =>
            {
                List<Person> selectedRows = GetSelectedRows();
                ExportSelectedRowsToCsv(selectedRows);
            });

            // Deaktivieren Sie den Ladevorgang, nachdem die CSV-Datei erstellt wurde
            loadingRing.IsActive = false;

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PersonDetailPage));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAngebot != null)
            {
                // Übergeben Sie den ausgewählten Datensatz an die OfferDetailPage
                Frame.Navigate(typeof(PersonDetailPage), selectedAngebot);
            }
        }



        private void dataGrid_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItems.Count == 1)
            {
                // Wenn genau ein Datensatz ausgewählt ist, aktivieren Sie den "Edit" Button
                selectedAngebot = (Person)dataGrid.SelectedItem;
                EditButton.IsEnabled = true;
            }
            else
            {
                // Andernfalls deaktivieren Sie den "Edit" Button
                EditButton.IsEnabled = false;
                selectedAngebot = null;
            }
        }




        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                _dbContext.SaveChanges();

                // Filter the data based on the entered text in the search bar
                string searchText = sender.Text.ToLower();
                var filteredPersons = _dbContext.Person
                    .Where(p =>
                        p.Nachname1.ToLower().Contains(searchText) ||
                        p.Nachname2.ToLower().Contains(searchText)
                    )
                    .ToList();

                // Update the DataGrid with the filtered data
                dataGrid.ItemsSource = filteredPersons;
            }
        }










    }



}
