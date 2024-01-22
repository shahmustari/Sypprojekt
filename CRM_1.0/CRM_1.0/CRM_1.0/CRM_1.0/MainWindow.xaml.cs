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

namespace CRM_1._0
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.InitializeComponent();
            contentFrame.Navigate(typeof(CRM.Pages.Dashboard));
        }


        private void NvSample_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                // Hier können Sie die Logik für die Einstellungen implementieren, wenn erforderlich.
            }
            else
            {
                string selectedTag = args.InvokedItemContainer.Tag.ToString();

                // Hier können Sie die Navigation basierend auf dem ausgewählten Tag durchführen.
                switch (selectedTag)
                {
                    case "Person":
                        contentFrame.Navigate(typeof(CRM.Pages.PersonPage));
                        break;
                    case "Anfragen":
                        contentFrame.Navigate(typeof(CRM.Pages.RequestPage));
                        break;
                    case "Angebote":
                        contentFrame.Navigate(typeof(CRM.Pages.OfferPage));
                        break;
                    case "Projekte":
                        contentFrame.Navigate(typeof(CRM.Pages.ProjectPage));
                        break;
                    case "Service":
                        contentFrame.Navigate(typeof(CRM.Pages.ServicePage));
                        break;
                    case "Dashboard":
                        contentFrame.Navigate(typeof(CRM.Pages.Dashboard));
                        break;

                }
            }
        }
    }
}
