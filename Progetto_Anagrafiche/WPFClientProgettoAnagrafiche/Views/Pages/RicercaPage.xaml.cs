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
using WPFClientProgettoAnagrafiche.Services;
using WPFClientProgettoAnagrafiche.ViewModels;

namespace WPFClientProgettoAnagrafiche.Views.Pages
{
    /// <summary>
    /// Interaction logic for RicercaPage.xaml
    /// </summary>
    public partial class RicercaPage : Page
    {
        public RicercaPage(ContattoFrontService contattoService, ClienteFrontService clienteService)
        {
            InitializeComponent();

            var viewModel = new RicercaPageViewModel(contattoService, clienteService);
            DataContext = viewModel;

            // on load execute command on viewmodel
            Loaded += async (s, e) => await viewModel.LoadRicercaCommand.ExecuteAsync(null);
        }
    }
}
