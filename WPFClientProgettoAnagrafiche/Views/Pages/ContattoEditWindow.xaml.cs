using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgettoAnagrafiche.Models;
using ProgettoAnagrafiche.Models.DTO;
using ProgettoAnagrafiche.Models.Enums;
using System.Windows;


namespace WPFClientProgettoAnagrafiche.Views.Windows
{
    public partial class ContattoEditWindow : Window
    {
        private readonly ContattoEditViewModel _viewModel;

        public ContattoEditWindow(AnagraficaContatto contatto, bool isNew)
        {
            InitializeComponent();

            _viewModel = new ContattoEditViewModel(contatto, isNew);
            DataContext = _viewModel;

            Title = isNew ? "Aggiungi Contatto" : "Modifica Contatto";
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

    public partial class ContattoEditViewModel : ObservableObject
    {
        [ObservableProperty]
        private int anagraficaId;

        [ObservableProperty]
        private int anagraficaTableId;

        [ObservableProperty]
        private bool isPersonaFisica;

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
        private string provenienza;

        private static readonly List<string> ProvenienzaOptions = new()
        {
            "LinkedIn",
            "SitoWeb",
            "Pubblicita"
        };

        public IReadOnlyList<string> ProvenienzaOptionsList => ProvenienzaOptions;

        [ObservableProperty]
        private string? note;

        [ObservableProperty]
        private bool isPersonaGiuridica;

        [ObservableProperty]
        private bool isNewContatto;

        public ContattoEditViewModel(AnagraficaContatto contatto, bool isNew)
        {
            isNewContatto = isNew;

            AnagraficaId = contatto.Anagrafica.Id;
            IsPersonaFisica = contatto.Anagrafica.IsPersonaFisica ?? false;
            Nome = contatto.Anagrafica.Nome;
            Cognome = contatto.Anagrafica.Cognome;
            RagioneSociale = contatto.Anagrafica.RagioneSociale;
            Email = contatto.Anagrafica.Email;
            Telefono = contatto.Anagrafica.Telefono;
            CodiceFiscale = contatto.Anagrafica.CodiceFiscale;
            PartitaIva = contatto.Anagrafica.PartitaIva;
            Provenienza = contatto.Provenienza.ToString();
            Note = contatto.Note;
            IsPersonaGiuridica = !(contatto.Anagrafica.IsPersonaFisica ?? false);
        }

        partial void OnIsPersonaFisicaChanged(bool value)
        {
            IsPersonaGiuridica = !value;
        }

        partial void OnIsPersonaGiuridicaChanged(bool value)
        {
            IsPersonaFisica = !value;
        }


        public AnagraficaContatto ToEntity()
            {
            // cleanup and normalize when converting
            return new AnagraficaContatto
                {

                // looked up/had help
                Provenienza = Enum.TryParse<Provenienza>(Provenienza.Replace(" ", ""), out var provenienzaEnum) ? provenienzaEnum : throw new InvalidOperationException($"Valore Provenienza non valido: {Provenienza}"),

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
