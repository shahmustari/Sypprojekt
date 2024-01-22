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
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CRM.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RequestPage : Page
    {
        private DB_Wittek _dbContext;
        private Anfrage selectedAngebot; // Klassenvariable, um den ausgewählten Datensatz zu speichern


        public RequestPage()
        {
            this.InitializeComponent();
            _dbContext = new DB_Wittek();
            LoadData();
        }

        private void LoadData()
        {
            loadingRing.IsActive = true;
            // Retrieve data from the database
            var request = _dbContext.Anfragen.ToList();

            // Set the DataGrid's ItemsSource to the retrieved data
            dataGrid.ItemsSource = request;
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


        private List<Anfrage> GetSelectedRows()
        {
            List<Anfrage> selectedRows = new List<Anfrage>();

            foreach (var item in dataGrid.SelectedItems)
            {
                selectedRows.Add((Anfrage)item);
            }

            return selectedRows;
        }

        private void DeleteSelectedRows(List<Anfrage> selectedRows)
        {
            if (selectedRows.Count == 0)
            {
                // Keine ausgewählten Zeilen
                return;
            }

            foreach (var row in selectedRows)
            {
                _dbContext.Anfragen.Remove(row);
            }

            _dbContext.SaveChanges();
            LoadData(); // Die Ansicht aktualisieren
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            loadingRing.IsActive = true;
            List<Anfrage> selectedRows = GetSelectedRows();
            DeleteSelectedRows(selectedRows);
            loadingRing.IsActive = false;
        }


        private void ExportSelectedRequestsToCsv(List<Anfrage> selectedRequests)
        {
            if (selectedRequests.Count == 0)
            {
                // Keine ausgewählten Anfragen
                return;
            }

            StringBuilder csvData = new StringBuilder();
            // Erstellen Sie die Headerzeile
            csvData.AppendLine("AnfrageID;AnfrageDatum;Kostenintervall;Zeitintervall;Anfragestatus;PersonID");

            foreach (var request in selectedRequests)
            {
                // Fügen Sie die Daten für jede Anfrage hinzu und trennen Sie sie mit Semikolon
                csvData.AppendLine($"{request.AnfrageID};{request.AnfrageDatum};{request.Kostenintervall};{request.Zeitintervall};{request.Anfragestatus};{request.PersonID}");
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string csvFilePath = Path.Combine(desktopPath, "selected_data_request.csv");

            File.WriteAllText(csvFilePath, csvData.ToString());
        }



        private async void ToCsvButton_Click(object sender, RoutedEventArgs e)
        {
            // Aktivieren Sie den Ladevorgang
            loadingRing.IsActive = true;

            await Task.Run(() =>
            {
                List<Anfrage> selectedRequests = GetSelectedRows(); // Ersetzen Sie GetSelectedRequests durch Ihre Methode zum Abrufen ausgewählter Anfragen
                ExportSelectedRequestsToCsv(selectedRequests);
            });

            // Deaktivieren Sie den Ladevorgang, nachdem die CSV-Datei erstellt wurde
            loadingRing.IsActive = false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RequestDetailPage));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAngebot != null)
            {
                // Übergeben Sie den ausgewählten Datensatz an die OfferDetailPage
                Frame.Navigate(typeof(RequestDetailPage), selectedAngebot);
            }
        }



        private void dataGrid_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItems.Count == 1)
            {
                // Wenn genau ein Datensatz ausgewählt ist, aktivieren Sie den "Edit" Button
                selectedAngebot = (Anfrage)dataGrid.SelectedItem;
                EditButton.IsEnabled = true;
            }
            else
            {
                // Andernfalls deaktivieren Sie den "Edit" Button
                EditButton.IsEnabled = false;
                selectedAngebot = null;
            }
        }
    }
}
