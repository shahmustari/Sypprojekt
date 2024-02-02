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


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CRM.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonDetailPage : Page
    {

        private DB_Wittek _dbContext;
        private Anfrage selectedAnfrage; // Klassenvariable, um den ausgew‰hlten Datensatz zu speichern

        public Person aktuelle;

        //public NavigationEventArgs ne;
        public PersonDetailPage()
        {
            this.InitializeComponent();

            _dbContext = new DB_Wittek();


        }


        private List<Anfrage> GetSelectedRows()
        {
            List<Anfrage> selectedRows = new List<Anfrage>();

            foreach (var item in RequestDataGrid.SelectedItems)
            {
                selectedRows.Add((Anfrage)item);
            }

            return selectedRows;
        }

        private void DeleteSelectedRows(List<Anfrage> selectedRows)
        {
            if (selectedRows.Count == 0)
            {
                // Keine ausgew‰hlten Zeilen
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

        private /*async*/ void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Starten Sie die Ladeanimation
            loadingRing.IsActive = true;

            // Warten Sie eine kurze Zeitspanne (kann entfernt werden, je nach Bedarf)
            //await Task.Delay(1000);

            // Rufen Sie die Methode zum erneuten Laden der Datenbank auf
            LoadData();

            // Stoppen Sie die Ladeanimation
            loadingRing.IsActive = false;
        }



        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAnfrage != null)
            {
                // ‹bergeben Sie den ausgew‰hlten Datensatz an die OfferDetailPage
                Frame.Navigate(typeof(RequestDetailPage), selectedAnfrage);
            }
        }


        private void dataGrid_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (RequestDataGrid.SelectedItems.Count == 1)
            {
                // Wenn genau ein Datensatz ausgew‰hlt ist, aktivieren Sie den "Edit" Button
                selectedAnfrage = (Anfrage)RequestDataGrid.SelectedItem;
                EditButton.IsEnabled = true;
            }
            else
            {
                // Andernfalls deaktivieren Sie den "Edit" Button
                EditButton.IsEnabled = false;
                selectedAnfrage = null;
            }
        }



        private void LoadData()
        {
            loadingRing.IsActive = true;

            using (var db = new DB_Wittek())
            {
                var personKey = aktuelle.PersonID; // Replace with the actual property name
                var relatedRequests = db.Anfragen.Where(r => r.PersonID == personKey).ToList();

                RequestDataGrid.ItemsSource = relatedRequests;
            }

            loadingRing.IsActive = false;
        }

        // Override the OnNavigatedTo method to populate TextBox controls with person data
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Person personData)
            {
                aktuelle = personData;

                LoadData();


                Anrede1TextBox.Text = personData.Anrede1;
                Titel1TextBox.Text = personData.Titel1;
                Nachname1TextBox.Text = personData.Nachname1;
                Vorname1TextBox.Text = personData.Vorname1;
                TelNr1TextBox.Text = personData.TelNr1;
                MobilNr1TextBox.Text = personData.MobilNr1;
                Email1TextBox.Text = personData.Email1;
                Geburtstag1DatePicker.Date = personData.Geburtstag1.Date;
                Geburtstag1TimePicker.Time = personData.Geburtstag1.TimeOfDay; // Convert to string as needed
                Sternzeichen1TextBox.Text = personData.Sternzeichen1;


                Anrede2TextBox.Text = personData.Anrede2;
                Titel2TextBox.Text = personData.Titel2;
                Nachname2TextBox.Text = personData.Nachname2;
                Vorname2TextBox.Text = personData.Vorname2;
                TelNr2TextBox.Text = personData.TelNr2;
                MobilNr2TextBox.Text = personData.MobilNr2;
                Email2TextBox.Text = personData.Email2;
                Geburtstag1DatePicker.Date = personData.Geburtstag1.Date;
                Geburtstag1TimePicker.Time = personData.Geburtstag1.TimeOfDay; // Convert to string as needed ---------------------------------------------------------
                Sternzeichen2TextBox.Text = personData.Sternzeichen2;


                PlzBstTextBox.Text = personData.PlzBst.ToString(); // Convert to string as needed
                OrtBestTextBox.Text = personData.OrtBest;
                StraﬂeBstTextBox.Text = personData.StraﬂeBst;
                NrBestTextBox.Text = personData.NrBest.ToString(); // Convert to string as needed
                ArtTextBox.Text = personData.Art;
                LeistungTextBox.Text = personData.Leistung;
                PlzWhngTextBox.Text = personData.PlzWhng.ToString(); // Convert to string as needed
                OrtWhngTextBox.Text = personData.OrtWhng;
                StraﬂeWhngTextBox.Text = personData.StraﬂeWhng;
                NrWhngTextBox.Text = personData.NrWhng;
                wiezuunsTextBox.Text = personData.Wiezuuns;
                sonstigesTextBox.Text = personData.Sonstiges;
                ErstkontaktDatePicker.Date = personData.Erstkontakt.Date;
                ErstkontaktTimePicker.Time = personData.Erstkontakt.TimeOfDay;
                TerminDatePicker.Date = personData.Termin.Date;
                TerminTimePicker.Time = personData.Termin.TimeOfDay;
                AktivCheckBox.IsChecked = personData.AKTIV;
                
            }
            else
            {
                Geburtstag1DatePicker.Date = new DateTime(2100, 1, 1);
                Geburtstag1TimePicker.Time = new TimeSpan(12, 0, 0);

                Geburtstag2DatePicker.Date = new DateTime(2100, 1, 1);
                Geburtstag2TimePicker.Time = new TimeSpan(12, 0, 0);

                ErstkontaktDatePicker.Date = new DateTime(2100, 1, 1);
                ErstkontaktTimePicker.Time = new TimeSpan(12, 0, 0);

                TerminDatePicker.Date = new DateTime(2100, 1, 1);
                TerminTimePicker.Time = new TimeSpan(12, 0, 0);
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            loadingRing.IsActive = true;
            try
            {
                var geburtstag1Date = Geburtstag1DatePicker.Date;
                var geburtstag1Time = Geburtstag1TimePicker.Time;

                var geburtstag2Date = Geburtstag2DatePicker.Date;
                var geburtstag2Time = Geburtstag2TimePicker.Time;

                var erstkontaktDate = ErstkontaktDatePicker.Date;
                var erstkontaktTime = ErstkontaktTimePicker.Time;

                var terminDate = TerminDatePicker.Date;
                var terminTime = TerminTimePicker.Time;





                if (aktuelle is not Person personData)
                {
                    // Erstelle einen neuen Datensatz
                    var newPerson = new Person
                    {
                        Anrede1 = Anrede1TextBox.Text,
                        Titel1 = Titel1TextBox.Text,
                        Nachname1 = Nachname1TextBox.Text,
                        Vorname1 = Vorname1TextBox.Text,
                        TelNr1 = TelNr1TextBox.Text,
                        MobilNr1 = MobilNr1TextBox.Text,
                        Email1 = Email1TextBox.Text,

                        Geburtstag1 = DateTime.SpecifyKind(new DateTime(geburtstag1Date.Year, geburtstag1Date.Month, geburtstag1Date.Day), DateTimeKind.Utc),

                        Sternzeichen1 = Sternzeichen1TextBox.Text,
                        Anrede2 = Anrede2TextBox.Text,
                        Titel2 = Titel2TextBox.Text,
                        Nachname2 = Nachname2TextBox.Text,
                        Vorname2 = Vorname2TextBox.Text,
                        TelNr2 = TelNr2TextBox.Text,
                        MobilNr2 = MobilNr2TextBox.Text,
                        Email2 = Email2TextBox.Text,
                        Geburtstag2 = DateTime.SpecifyKind(new DateTime(geburtstag2Date.Year, geburtstag2Date.Month, geburtstag2Date.Day), DateTimeKind.Utc),
                        Sternzeichen2 = Sternzeichen2TextBox.Text,
                        PlzBst = string.IsNullOrWhiteSpace(PlzBstTextBox.Text) ? 0 : int.Parse(PlzBstTextBox.Text),
                        OrtBest = OrtBestTextBox.Text,
                        StraﬂeBst = StraﬂeBstTextBox.Text,
                        NrBest = string.IsNullOrWhiteSpace(NrBestTextBox.Text) ? 0 : int.Parse(NrBestTextBox.Text),
                        Art = ArtTextBox.Text,
                        Leistung = LeistungTextBox.Text,
                        PlzWhng = string.IsNullOrWhiteSpace(PlzBstTextBox.Text) ? 0 : int.Parse(PlzBstTextBox.Text),
                        OrtWhng = OrtWhngTextBox.Text,
                        StraﬂeWhng = StraﬂeWhngTextBox.Text,
                        NrWhng = NrWhngTextBox.Text,
                        Wiezuuns = wiezuunsTextBox.Text,
                        Sonstiges = sonstigesTextBox.Text,
                        Erstkontakt = new DateTime(
                            erstkontaktDate.Year, erstkontaktDate.Month, erstkontaktDate.Day,
                            erstkontaktTime.Hours, erstkontaktTime.Minutes, erstkontaktTime.Seconds, DateTimeKind.Utc
                        ),
                        Termin = new DateTime(
                            terminDate.Year, terminDate.Month, terminDate.Day,
                            terminTime.Hours, terminTime.Minutes, terminTime.Seconds, DateTimeKind.Utc
                        ),
                        AKTIV = (bool)AktivCheckBox.IsChecked,
                    };

                    using (var db = new DB_Wittek())
                    {
                        db.Add(newPerson);
                        await db.SaveChangesAsync();
                    }
                    loadingRing.IsActive = false;
                }
                else
                {
                    aktuelle.Anrede1 = Anrede1TextBox.Text;
                    aktuelle.Titel1 = Titel1TextBox.Text;
                    aktuelle.Nachname1 = Nachname1TextBox.Text;
                    aktuelle.Vorname1 = Vorname1TextBox.Text;
                    aktuelle.TelNr1 = TelNr1TextBox.Text;
                    aktuelle.MobilNr1 = MobilNr1TextBox.Text;
                    aktuelle.Email1 = Email1TextBox.Text;

                    aktuelle.Geburtstag1 = DateTime.SpecifyKind(new DateTime(geburtstag1Date.Year, geburtstag1Date.Month, geburtstag1Date.Day), DateTimeKind.Utc);

                    aktuelle.Sternzeichen1 = Sternzeichen1TextBox.Text;
                    aktuelle.Anrede2 = Anrede2TextBox.Text;
                    aktuelle.Titel2 = Titel2TextBox.Text;
                    aktuelle.Nachname2 = Nachname2TextBox.Text;
                    aktuelle.Vorname2 = Vorname2TextBox.Text;
                    aktuelle.TelNr2 = TelNr2TextBox.Text;
                    aktuelle.MobilNr2 = MobilNr2TextBox.Text;
                    aktuelle.Email2 = Email2TextBox.Text;
                    aktuelle.Geburtstag2 = DateTime.SpecifyKind(new DateTime(geburtstag2Date.Year, geburtstag2Date.Month, geburtstag2Date.Day), DateTimeKind.Utc);
                    aktuelle.Sternzeichen2 = Sternzeichen2TextBox.Text;
                    aktuelle.PlzBst = string.IsNullOrWhiteSpace(PlzBstTextBox.Text) ? 0 : int.Parse(PlzBstTextBox.Text);
                    aktuelle.OrtBest = OrtBestTextBox.Text;
                    aktuelle.StraﬂeBst = StraﬂeBstTextBox.Text;
                    aktuelle.NrBest = string.IsNullOrWhiteSpace(NrBestTextBox.Text) ? 0 : int.Parse(NrBestTextBox.Text);
                    aktuelle.Art = ArtTextBox.Text;
                    aktuelle.Leistung = LeistungTextBox.Text;
                    aktuelle.PlzWhng = string.IsNullOrWhiteSpace(PlzBstTextBox.Text) ? 0 : int.Parse(PlzBstTextBox.Text);
                    aktuelle.OrtWhng = OrtWhngTextBox.Text;
                    aktuelle.StraﬂeWhng = StraﬂeWhngTextBox.Text;
                    aktuelle.NrWhng = NrWhngTextBox.Text;
                    aktuelle.Wiezuuns = wiezuunsTextBox.Text;
                    aktuelle.Sonstiges = sonstigesTextBox.Text;
                    aktuelle.Erstkontakt = new DateTime(
                        erstkontaktDate.Year, erstkontaktDate.Month, erstkontaktDate.Day,
                        erstkontaktTime.Hours, erstkontaktTime.Minutes, erstkontaktTime.Seconds, DateTimeKind.Utc
                    );
                    aktuelle.Termin = new DateTime(
                        terminDate.Year, terminDate.Month, terminDate.Day,
                        terminTime.Hours, terminTime.Minutes, terminTime.Seconds, DateTimeKind.Utc
                    );
                    aktuelle.AKTIV = (bool)AktivCheckBox.IsChecked;

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

                Console.WriteLine(ex.Message);

                // Zeige eine Fehlermeldung an
                ContentDialog dialog = new ContentDialog();

                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                dialog.XamlRoot = this.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "Datensatz wurde NICHT gespeichert ";
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
            // Navigate to RequestDetailPage and pass the PersonID as a parameter
            Frame.Navigate(typeof(RequestDetailPage), aktuelle.PersonID);
        }




        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            loadingRing.IsActive = true;
            try
            {
                // Speichern Sie die ƒnderungen in der DataGrid-Ansicht in der Datenbank
                await _dbContext.SaveChangesAsync();

                // Aktualisieren Sie die Ansicht, um sicherzustellen, dass sie die neuesten Daten aus der Datenbank anzeigt
                LoadData();
                loadingRing.IsActive = false;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Fehler beim Speichern der ƒnderungen: " + ex.Message);

                loadingRing.IsActive = false;
            }
        }

        



    }
}
