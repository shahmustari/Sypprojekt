using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CRM.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OfferDetailPage : Page
    {
        private DB_Wittek _dbContext;
        private Projekt selectedProject;

        public Angebot aktuelle;
        public OfferDetailPage()
        {
            this.InitializeComponent();

            _dbContext = new DB_Wittek();

        }

        private List<Projekt> GetSelectedRows()
        {
            List<Projekt> selectedRows = new List<Projekt>();

            foreach (var item in ProjectDataGrid.SelectedItems)
            {
                selectedRows.Add((Projekt)item);
            }

            return selectedRows;
        }

        private void DeleteSelectedRows(List<Projekt> selectedRows)
        {
            if (selectedRows.Count == 0)
            {
                // Keine ausgewählten Zeilen
                return;
            }

            foreach (var row in selectedRows)
            {
                _dbContext.Projekte.Remove(row);
            }

            _dbContext.SaveChanges();
            LoadData(); // Die Ansicht aktualisieren
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            loadingRing.IsActive = true;
            List<Projekt> selectedRows = GetSelectedRows();
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

        private void LoadData()
        {
            loadingRing.IsActive = true;

            using (var db = new DB_Wittek())
            {
                var offerKey = aktuelle.AngebotID; // Hier gehe ich davon aus, dass AnfrageID der Schlüssel für die Anfragen ist. Bitte ersetze es entsprechend.
                var relatedProjects = db.Projekte.Where(offer => offer.AngebotID == offerKey).ToList();

                ProjectDataGrid.ItemsSource = relatedProjects;
            }

            loadingRing.IsActive = false;
        }


        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProject != null)
            {
                // Übergeben Sie den ausgewählten Datensatz an die OfferDetailPage
                Frame.Navigate(typeof(ProjectDetailPage), selectedProject);
            }
        }


        private void dataGrid_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (ProjectDataGrid.SelectedItems.Count == 1)
            {
                // Wenn genau ein Datensatz ausgewählt ist, aktivieren Sie den "Edit" Button
                selectedProject = (Projekt)ProjectDataGrid.SelectedItem;
                EditButton.IsEnabled = true;
            }
            else
            {
                // Andernfalls deaktivieren Sie den "Edit" Button
                EditButton.IsEnabled = false;
                selectedProject = null;
            }
        }




        // Override the OnNavigatedTo method to populate TextBox controls with data
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is int anfrageID)
            {
                // Now you have the personID, and you can use it as needed
                AnfrageIDTextBox.Text = anfrageID.ToString();

                // Additional logic if needed...
            }
            else if (e.Parameter is Angebot data)
            {

                aktuelle = data;

                LoadData();

                LeistungTextBox.Text = data.Leistung;
                HeizgaszulaengeTextBox.Text = data.Heizgaszulaenge;
                WirkungsgradTextBox.Text = data.Wirkungsgrad.ToString();
                HeizintervallTextBox.Text = data.Heizintervall;
                HolzMinTextBox.Text = data.HolzMin.ToString();

                KacheltypTextBox.Text = data.Kacheltyp;
                Glasur1TextBox.Text = data.Glasur1;
                Glasur2TextBox.Text = data.Glasur2;
                FugenmasseTextBox.Text = data.Fugenmasse;
                HeiztuerTextBox.Text = data.Heiztuer;

                HolzMaxTextBox.Text = data.HolzMax.ToString();
                SteuerungTextBox.Text = data.Steuerung;
                VerkabelungTextBox.Text = data.Verkabelung;
                ZuluftTextBox.Text = data.Zuluft;
                NotizTextBox.Text = data.Notiz;

                AktivCheckBox.IsChecked = data.Aktiv;
                AngebotsstatusTextBox.Text = data.Angebotsstatus;
                AnfrageIDTextBox.Text = data.AnfrageID.ToString();
            }
        }


        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            loadingRing.IsActive = true;

            try
            {

                if (aktuelle is not Angebot data)
                {
                    var newAngebot = new Angebot 
                    {
                        Leistung = LeistungTextBox.Text,
                        Heizgaszulaenge = HeizgaszulaengeTextBox.Text,
                        Wirkungsgrad = string.IsNullOrWhiteSpace(WirkungsgradTextBox.Text) ? 0 : int.Parse(WirkungsgradTextBox.Text),
                        Heizintervall = HeizintervallTextBox.Text,
                        HolzMax = string.IsNullOrWhiteSpace(HolzMaxTextBox.Text) ? 0 : int.Parse(HolzMaxTextBox.Text),

                        Kacheltyp = KacheltypTextBox.Text,
                        Glasur1 = Glasur1TextBox.Text,
                        Glasur2 = Glasur2TextBox.Text,
                        Fugenmasse = FugenmasseTextBox.Text,
                        Heiztuer = HeiztuerTextBox.Text,

                        HolzMin = string.IsNullOrWhiteSpace(HolzMinTextBox.Text) ? 0 : int.Parse(HolzMinTextBox.Text),
                        Steuerung = SteuerungTextBox.Text,
                        Verkabelung = VerkabelungTextBox.Text,
                        Zuluft = ZuluftTextBox.Text,
                        Notiz = NotizTextBox.Text,

                        Aktiv = (bool)AktivCheckBox.IsChecked,
                        Angebotsstatus = AngebotsstatusTextBox.Text,
                        AnfrageID = int.Parse(AnfrageIDTextBox.Text),
                    };
                    

                    using (var db = new DB_Wittek())
                    {
                        db.Add(newAngebot);
                        await db.SaveChangesAsync();
                    }
                    loadingRing.IsActive = false;
                }
                else
                {

                    aktuelle.Leistung = LeistungTextBox.Text;
                    aktuelle.Heizgaszulaenge = HeizgaszulaengeTextBox.Text;
                    aktuelle.Wirkungsgrad = string.IsNullOrWhiteSpace(WirkungsgradTextBox.Text) ? 0 : int.Parse(WirkungsgradTextBox.Text);
                    aktuelle.Heizintervall = HeizintervallTextBox.Text;
                    aktuelle.HolzMax = string.IsNullOrWhiteSpace(HolzMaxTextBox.Text) ? 0 : int.Parse(HolzMaxTextBox.Text);

                    aktuelle.Kacheltyp = KacheltypTextBox.Text;
                    aktuelle.Glasur1 = Glasur1TextBox.Text;
                    aktuelle.Glasur2 = Glasur2TextBox.Text;
                    aktuelle.Fugenmasse = FugenmasseTextBox.Text;
                    aktuelle.Heiztuer = HeiztuerTextBox.Text;

                    aktuelle.HolzMin = string.IsNullOrWhiteSpace(HolzMinTextBox.Text) ? 0 : int.Parse(HolzMinTextBox.Text);
                    aktuelle.Steuerung = SteuerungTextBox.Text;
                    aktuelle.Verkabelung = VerkabelungTextBox.Text;
                    aktuelle.Zuluft = ZuluftTextBox.Text;
                    aktuelle.Notiz = NotizTextBox.Text;

                    aktuelle.Aktiv = (bool)AktivCheckBox.IsChecked;
                    aktuelle.Angebotsstatus = AngebotsstatusTextBox.Text;
                    aktuelle.AnfrageID = int.Parse(AnfrageIDTextBox.Text);

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














        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProjectDetailPage), aktuelle.AngebotID);
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
