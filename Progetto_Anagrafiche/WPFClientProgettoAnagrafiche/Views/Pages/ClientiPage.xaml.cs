using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFClientProgettoAnagrafiche.ViewModels;
using WPFClientProgettoAnagrafiche.Services;

namespace WPFClientProgettoAnagrafiche.Views.Pages
{
    /// <summary>
    /// Interaction logic for ClientiPage.xaml
    /// </summary>
    public partial class ClientiPage : Page
    {
        public ClientiPage(ClienteFrontService clienteService)
        {
            InitializeComponent();

            var viewModel = new ClientiPageViewModel(clienteService);
            DataContext = viewModel;

            Loaded += async (s, e) => await viewModel.LoadClientiCommand.ExecuteAsync(null);
        }
    }
}
