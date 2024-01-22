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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CRM.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Dashboard : Page
    {
        private DB_Wittek _dbContext;

        public Dashboard()
        {
            this.InitializeComponent();
            _dbContext = new DB_Wittek();
            databasePathTextBox.Text = _dbContext.DbPath;
            LoadData();
        }

        private void LoadData()
        {
            loadingRing.IsActive = true;
            // Retrieve data from the database
            var service = _dbContext.Services.OrderBy(x => x.Datum).Where(x => x.Aktiv == false).ToList();

            // Set the DataGrid's ItemsSource to the retrieved data
            dataGrid.ItemsSource = service;
            loadingRing.IsActive = false;
        }
    }
}
