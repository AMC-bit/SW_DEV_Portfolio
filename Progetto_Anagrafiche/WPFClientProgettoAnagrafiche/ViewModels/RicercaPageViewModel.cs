using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgettoAnagrafiche.Models;
using ProgettoAnagrafiche.Models.DTO;
using ProgettoAnagrafiche.Models.Enums;
using System.Collections.ObjectModel;
using System.Windows;
using WPFClientProgettoAnagrafiche.Services;
using WPFClientProgettoAnagrafiche.Utilities;
using WPFClientProgettoAnagrafiche.ViewModels;
using WPFClientProgettoAnagrafiche.Views.Windows;

namespace WPFClientProgettoAnagrafiche.ViewModels
{
    public partial class RicercaPageViewModel : ObservableObject
        {
        // I get both clienti and contatti stuff 
        private ContattoFrontService _contattoService;
        private ClienteFrontService _clienteService;

        [ObservableProperty]
        private ObservableCollection<AnagraficaContatto> contatti = [];

        [ObservableProperty]
        private ObservableCollection<AnagraficaCliente> clienti = [];

        [ObservableProperty]
        private string statusText = string.Empty;

        [ObservableProperty]
        private RicercaMergedViewModel? selectedAnagrafica;

        [ObservableProperty]
        private AnagraficaCliente? selectedClient;

        [ObservableProperty]
        private AnagraficaContatto? selectedContact;

        [ObservableProperty]
        private ObservableCollection<RicercaMergedViewModel> anagrafiche = [];

        [ObservableProperty]
        private ObservableCollection<RicercaMergedViewModel> filteredAnagrafiche = [];

        [ObservableProperty]
        private string searchNome = string.Empty;

        [ObservableProperty]
        private string searchCognome = string.Empty;

        [ObservableProperty]
        private string searchEmail = string.Empty;

        [ObservableProperty]
        private string searchCodiceFiscale = string.Empty;

        [ObservableProperty]
        private string searchPartitaIva = string.Empty;

        [ObservableProperty]
        private string searchRagioneSociale = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private string successMessage = string.Empty;

        public RicercaPageViewModel(ContattoFrontService contattoService, ClienteFrontService clienteService)
            {
            // load both services
            _contattoService = contattoService;
            _clienteService = clienteService;
        }

        [RelayCommand]
        public async Task LoadRicerca()
        {
            try
            {
                StatusText = "Loading...";

                var contattiTask = _contattoService.GetAllContattiAsync();
                var clientiTask = _clienteService.GetAllClientiAsync();

                await Task.WhenAll(contattiTask, clientiTask);


                // Check for null or unsuccessful responses if needed
                var contattiItems = contattiTask.Result.Data ?? new List<AnagraficaContatto>();
                var clientiItems = clientiTask.Result.Data ?? new List<AnagraficaCliente>();

                // Merge contatti and clienti into a single collection
                var mergedList = new ObservableCollection<RicercaMergedViewModel>();

                // Add contatti
                foreach (var contatto in contattiItems)
                {
                    mergedList.Add(new RicercaMergedViewModel
                    {
                        Id = contatto.Anagrafica.Id,
                        Tipo = "Contatto",
                        Nome = contatto.Anagrafica.Nome,
                        Cognome = contatto.Anagrafica.Cognome,
                        Telefono = contatto.Anagrafica.Telefono,
                        Email = contatto.Anagrafica.Email,
                        CodiceFiscale = contatto.Anagrafica.CodiceFiscale,
                        PartitaIva = contatto.Anagrafica.PartitaIva,
                        IsPersonaFisica = contatto.Anagrafica.IsPersonaFisica,
                        RagioneSociale = contatto.Anagrafica.RagioneSociale,
                        DisplayName = contatto.Anagrafica.DisplayName,
                        Provenienza = contatto.Provenienza.ToString(),
                        NoteContatto = contatto.Note ?? string.Empty,
                        NoteCliente = string.Empty,
                        // no dataregistraz for contatti
                        });
                }

                foreach (var cliente in clientiItems)
                {
                    mergedList.Add(new RicercaMergedViewModel
                    {
                        Id = cliente.Anagrafica.Id,
                        Tipo = "Cliente",
                        Nome = cliente.Anagrafica.Nome,
                        Cognome = cliente.Anagrafica.Cognome,
                        Telefono = cliente.Anagrafica.Telefono,
                        Email = cliente.Anagrafica.Email,
                        CodiceFiscale = cliente.Anagrafica.CodiceFiscale,
                        PartitaIva = cliente.Anagrafica.PartitaIva,
                        IsPersonaFisica = cliente.Anagrafica.IsPersonaFisica,
                        RagioneSociale = cliente.Anagrafica.RagioneSociale,
                        DisplayName = cliente.Anagrafica.DisplayName,
                        Provenienza = string.Empty,
                        NoteContatto = string.Empty,
                        NoteCliente = cliente.Note ?? string.Empty,
                        DataRegistrazione = cliente.DataRegistrazione
                    });
                }

                Anagrafiche = mergedList;
                FilteredAnagrafiche = new ObservableCollection<RicercaMergedViewModel>(mergedList);

                StatusText = string.Empty;
            }
            catch (Exception ex)
            {
                string finalmessage = ExceptionParser.ParseApiException(ex.Message);

                MessageBox.Show(
                        finalmessage,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        public void ApplyFilter()
        {
            if (Anagrafiche == null || Anagrafiche.Count == 0)
            {
                FilteredAnagrafiche = new ObservableCollection<RicercaMergedViewModel>();
                return;
            }

            var filtered = Anagrafiche.Where(a =>
                (string.IsNullOrWhiteSpace(SearchNome) || (a.Nome?.Contains(SearchNome, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                (string.IsNullOrWhiteSpace(SearchCognome) || (a.Cognome?.Contains(SearchCognome, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                (string.IsNullOrWhiteSpace(SearchEmail) || (a.Email?.Contains(SearchEmail, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                (string.IsNullOrWhiteSpace(SearchCodiceFiscale) || (a.CodiceFiscale?.Contains(SearchCodiceFiscale, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                (string.IsNullOrWhiteSpace(SearchPartitaIva) || (a.PartitaIva?.Contains(SearchPartitaIva, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                (string.IsNullOrWhiteSpace(SearchRagioneSociale) || (a.RagioneSociale?.Contains(SearchRagioneSociale, StringComparison.OrdinalIgnoreCase) ?? false))
            ).ToList();

            FilteredAnagrafiche = new ObservableCollection<RicercaMergedViewModel>(filtered);
        }

        [RelayCommand]
        public void ClearFilters()
        {
            SearchNome = string.Empty;
            SearchCognome = string.Empty;
            SearchEmail = string.Empty;
            SearchCodiceFiscale = string.Empty;
            SearchPartitaIva = string.Empty;
            SearchRagioneSociale = string.Empty;
            ApplyFilter();
        }

        [RelayCommand]
        public async Task EditAnagrafica()
        {
            if (SelectedAnagrafica == null)
            {
                StatusText = "Per favore seleziona qualcosa da modificare";
            }
            else if (SelectedAnagrafica.Tipo == "Contatto")
            {
                await EditContact();
            }
            else if (SelectedAnagrafica.Tipo == "Cliente")
            {
                await EditClient();
            }
        }

        public async Task EditClient()
        {
            try
            {
                if (SelectedAnagrafica == null)
                {
                    StatusText = "Per favore seleziona qualcosa da modificare";
                    return;
                }

                // new: make a copy
                var clienteEntityCopy = new AnagraficaCliente
                {
                    AnagraficaId = SelectedAnagrafica.Id,
                    // standard datareg in case it's null, all to utcnow
                    DataRegistrazione = SelectedAnagrafica.DataRegistrazione ?? DateTime.UtcNow,
                    Note = SelectedAnagrafica.NoteCliente,
                    Anagrafica = new Anagrafiche
                    {
                        Id = SelectedAnagrafica.Id,
                        IsPersonaFisica = SelectedAnagrafica.IsPersonaFisica,
                        Nome = SelectedAnagrafica.Nome,
                        Cognome = SelectedAnagrafica.Cognome,
                        RagioneSociale = SelectedAnagrafica.RagioneSociale,
                        Email = SelectedAnagrafica.Email,
                        Telefono = SelectedAnagrafica.Telefono,
                        CodiceFiscale = SelectedAnagrafica.CodiceFiscale,
                        PartitaIva = SelectedAnagrafica.PartitaIva,
                    }
                };


                //// account for null
                //if (SAnagToClienteEntity == null)
                //    {
                //    StatusText = "Impossibile mappare l'elemento selezionato verso Cliente Entity.";
                //    return;
                //    }


                var editWindow = new ClienteEditWindow(clienteEntityCopy, isNew: false);
                var result = editWindow.ShowDialog();

                if (result == true)
                {
                    var clienteEditVM = editWindow.DataContext as ClienteEditViewModel;
                    if (clienteEditVM != null)
                    {
                        var updatedEntity = clienteEditVM.ToEntity();

                        // backend changes here
                        updatedEntity.AnagraficaId = SelectedAnagrafica.Id;
                        var updateResponse = await _clienteService.UpdateClienteAsync(updatedEntity.Anagrafica.Id, updatedEntity);

                        // if no errors and data not null
                        if (updateResponse?.Errors?.Any() != true && updateResponse?.Data != null)
                        {
                            // make new viewmodel, with response data
                            var updated = new RicercaMergedViewModel
                            {
                                Id = updateResponse.Data.Anagrafica.Id,
                                Tipo = "Cliente",
                                Nome = updateResponse.Data.Anagrafica.Nome,
                                Cognome = updateResponse.Data.Anagrafica.Cognome,
                                Telefono = updateResponse.Data.Anagrafica.Telefono,
                                Email = updateResponse.Data.Anagrafica.Email,
                                CodiceFiscale = updateResponse.Data.Anagrafica.CodiceFiscale,
                                PartitaIva = updateResponse.Data.Anagrafica.PartitaIva,
                                IsPersonaFisica = updateResponse.Data.Anagrafica.IsPersonaFisica,
                                RagioneSociale = updateResponse.Data.Anagrafica.RagioneSociale,
                                DisplayName = updateResponse.Data.Anagrafica.DisplayName,
                                Provenienza = string.Empty,
                                NoteContatto = string.Empty,
                                NoteCliente = updateResponse.Data.Note ?? string.Empty,
                                DataRegistrazione = updateResponse.Data.DataRegistrazione
                            };

                            // update at index
                            var index = Anagrafiche.IndexOf(SelectedAnagrafica);
                            if (index >= 0)
                                Anagrafiche[index] = updated;

                            StatusText = "Cliente aggiornato con successo";

                            // reapply filters if any to update viesw
                            ApplyFilter();
                        }
                        else
                        {
                            StatusText = updateResponse?.Message ?? "Errore durante l'aggiornamento del cliente";
                        }
                    }
                }
                else
                {
                    StatusText = "Operazione annullata";
                }
            }
            catch (Exception ex)
            {
                string finalmessage = ExceptionParser.ParseApiException(ex.Message);

                MessageBox.Show(
                        finalmessage,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
        }

        public async Task EditContact()
        {
            try
            {
                if (SelectedAnagrafica == null)
                {
                    StatusText = "Per favore seleziona qualcosa da modificare";
                    return;
                }

                // map like you would for other entities, non dto edt
                var contattoEntityCopy = new AnagraficaContatto
                {
                    AnagraficaId = SelectedAnagrafica.Id,
                    // looked up/had help
                    Provenienza = Enum.TryParse<Provenienza>(SelectedAnagrafica.Provenienza, out var provenienzaValue) ? provenienzaValue : default,
                    Note = SelectedAnagrafica.NoteContatto,
                    Anagrafica = new Anagrafiche
                    {
                        Id = SelectedAnagrafica.Id,
                        IsPersonaFisica = SelectedAnagrafica.IsPersonaFisica,
                        Nome = SelectedAnagrafica.Nome,
                        Cognome = SelectedAnagrafica.Cognome,
                        RagioneSociale = SelectedAnagrafica.RagioneSociale,
                        Email = SelectedAnagrafica.Email,
                        Telefono = SelectedAnagrafica.Telefono,
                        CodiceFiscale = SelectedAnagrafica.CodiceFiscale,
                        PartitaIva = SelectedAnagrafica.PartitaIva,
                    }
                };

                //// account for null
                //if (SAnagToContattoEntity == null)
                //    {
                //    StatusText = "Impossibile mappare l'elemento verso ContattoEntity.";
                //    return;
                //    }


                var editWindow = new ContattoEditWindow(contattoEntityCopy, isNew: false);
                var result = editWindow.ShowDialog();

                if (result == true)
                {
                    var contattoEditVM = editWindow.DataContext as ContattoEditViewModel;
                    if (contattoEditVM != null)
                    {
                        var updatedEntity = contattoEditVM.ToEntity();

                        // backend changes here
                        updatedEntity.AnagraficaId = SelectedAnagrafica.Id;
                        var updateResponse = await _contattoService.UpdateContattoAsync(updatedEntity.Anagrafica.Id, updatedEntity);

                        // as before if no errors and data not null
                        if (updateResponse?.Errors?.Any() != true && updateResponse?.Data != null)
                        {
                            // copy/map returned data to viewmodel
                            var updated = new RicercaMergedViewModel
                            {
                                Id = updateResponse.Data.Anagrafica.Id,
                                Tipo = "Contatto",
                                Nome = updateResponse.Data.Anagrafica.Nome,
                                Cognome = updateResponse.Data.Anagrafica.Cognome,
                                Telefono = updateResponse.Data.Anagrafica.Telefono,
                                Email = updateResponse.Data.Anagrafica.Email,
                                CodiceFiscale = updateResponse.Data.Anagrafica.CodiceFiscale,
                                PartitaIva = updateResponse.Data.Anagrafica.PartitaIva,
                                IsPersonaFisica = updateResponse.Data.Anagrafica.IsPersonaFisica,
                                RagioneSociale = updateResponse.Data.Anagrafica.RagioneSociale,
                                DisplayName = updateResponse.Data.Anagrafica.DisplayName,
                                Provenienza = updateResponse.Data.Provenienza.ToString(),
                                NoteContatto = updateResponse.Data.Note ?? string.Empty,
                                NoteCliente = string.Empty,
                            };

                            // update at index
                            var index = Anagrafiche.IndexOf(SelectedAnagrafica);
                            if (index >= 0)
                                Anagrafiche[index] = updated;

                            StatusText = "Contatto aggiornato con successo";
                            ApplyFilter();
                        }
                        else
                        {
                            StatusText = updateResponse?.Message ?? "Errore durante l'aggiornamento del contatto";
                        }
                    }
                }
                else
                {
                    StatusText = "Operazione annullata";
                }
            }
            catch (Exception ex)
            {
                string finalmessage = ExceptionParser.ParseApiException(ex.Message);

                MessageBox.Show(
                        finalmessage,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
        }

        // this stuff is (partially) autogenerated. Onsearch etc etc. Apply filter (method) every time this happens
        partial void OnSearchNomeChanged(string value) => ApplyFilter();
        partial void OnSearchCognomeChanged(string value) => ApplyFilter();
        partial void OnSearchEmailChanged(string value) => ApplyFilter();
        partial void OnSearchCodiceFiscaleChanged(string value) => ApplyFilter();
        partial void OnSearchPartitaIvaChanged(string value) => ApplyFilter();
        partial void OnSearchRagioneSocialeChanged(string value) => ApplyFilter();
    }
}




