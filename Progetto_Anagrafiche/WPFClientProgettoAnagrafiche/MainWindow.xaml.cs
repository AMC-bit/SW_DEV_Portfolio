using System.Windows;
using WPFClientProgettoAnagrafiche.Views.Pages;
using WPFClientProgettoAnagrafiche.Services;
using WPFClientProgettoAnagrafiche.Services.Api;

namespace WPFClientProgettoAnagrafiche
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentFrame.Navigate(new HomePage());
        }

        private void NavigateHome(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new HomePage());
        }

        private void NavigateContatti(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ContattiPage(new ContattoFrontService(new ApiService(App.ApiClient))));
        }

        private void NavigateClienti(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ClientiPage(new ClienteFrontService(new ApiService(App.ApiClient))));
        }

        private void NavigateRicerca(object sender, RoutedEventArgs e)
        {
            // get both contatto and cliente and pass them to ricerca
            ContentFrame.Navigate(new RicercaPage(new ContattoFrontService(new ApiService(App.ApiClient)), new ClienteFrontService(new ApiService(App.ApiClient))));
        }
    }
}