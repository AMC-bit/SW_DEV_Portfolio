using System.Windows;
using ProgettoAnagrafiche.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPFClientProgettoAnagrafiche.Views.Windows
{ 
    public partial class ClienteEditWindow : Window
    {
        private readonly ClienteEditViewModel _viewModel;


        // I will feed to my constructor the new / empty cliente dto from the viewmodel
        public ClienteEditWindow(AnagraficaCliente cliente, bool isNew)
        {

            InitializeComponent();
            _viewModel = new ClienteEditViewModel(cliente, isNew);
            DataContext = _viewModel;

            Title = isNew ? "Aggiungi Cliente" : "Modifica Cliente";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public partial class ClienteEditViewModel : ObservableObject
    {
        [ObservableProperty]
        private int anagraficaId;

        [ObservableProperty]
        private bool? isPersonaFisica;

        [ObservableProperty]
        private string? nome;

        [ObservableProperty]
        private string? cognome;

        [ObservableProperty]
        private string? ragioneSociale;

        [ObservableProperty]
        private string? email;

        [ObservableProperty]
        private string? telefono;

        [ObservableProperty]
        private string? codiceFiscale;

        [ObservableProperty]
        private string? partitaIva;

        [ObservableProperty]
        private int codiceCliente;

        [ObservableProperty]
        private DateTime dataRegistrazione;

        [ObservableProperty]
        private string? note;

        [ObservableProperty]
        private bool isPersonaGiuridica;

        [ObservableProperty]
        private bool _isNewCliente;

        public ClienteEditViewModel(AnagraficaCliente cliente, bool isNew)
        {
            _isNewCliente = isNew;

            AnagraficaId = cliente.Anagrafica.Id;
            IsPersonaFisica = cliente.Anagrafica.IsPersonaFisica ?? false;
            Nome = cliente.Anagrafica.Nome;
            Cognome = cliente.Anagrafica.Cognome;
            RagioneSociale = cliente.Anagrafica.RagioneSociale;
            Email = cliente.Anagrafica.Email;
            Telefono = cliente.Anagrafica.Telefono;
            CodiceFiscale = cliente.Anagrafica.CodiceFiscale;
            PartitaIva = cliente.Anagrafica.PartitaIva;
            CodiceCliente = cliente.CodiceCliente;
            DataRegistrazione = cliente.DataRegistrazione;
            Note = cliente.Note;
            IsPersonaGiuridica = !(cliente.Anagrafica.IsPersonaFisica ?? false);
        }


        partial void OnIsPersonaGiuridicaChanged(bool value)
        {
            IsPersonaFisica = !value;
        }

        public AnagraficaCliente ToEntity()
            {
            // cleanup and normalize when converting
            return new AnagraficaCliente
                {
                CodiceCliente = CodiceCliente,
                DataRegistrazione = DataRegistrazione,
                Note = Note?.Trim(),

                Anagrafica = new Anagrafiche
                    {
                    Id = AnagraficaId,
                    IsPersonaFisica = IsPersonaFisica,
                    Nome = Nome?.Trim(),
                    Cognome = Cognome?.Trim(),
                    RagioneSociale = RagioneSociale?.Trim(),
                    Email = Email?.Trim(),
                    Telefono = Telefono?.Trim(),
                    CodiceFiscale = CodiceFiscale?.Trim().ToUpper(),
                    PartitaIva = PartitaIva?.Trim().ToUpper(),
                    }
                };
            }
        }
}