using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgettoAnagrafiche.Models;
using ProgettoAnagrafiche.Models.DTO;
using System.Collections.ObjectModel;
using System.Windows;
using WPFClientProgettoAnagrafiche.Services;
using WPFClientProgettoAnagrafiche.Utilities;
using WPFClientProgettoAnagrafiche.Views.Windows;

namespace WPFClientProgettoAnagrafiche.ViewModels
{
    public partial class ClientiPageViewModel : ObservableObject
    {
        private ClienteFrontService _clienteService;

        [ObservableProperty]
        private ObservableCollection<AnagraficaCliente> clienti = [];

        [ObservableProperty]
        private string statusText = string.Empty;

        [ObservableProperty]
        private AnagraficaCliente? selectedClient;

        public ClientiPageViewModel(ClienteFrontService clienteService)
        {
            _clienteService = clienteService;
        }

        [RelayCommand]
        public async Task LoadClienti()
        {
            try
            {
                // display loading while loading
                StatusText = "Loading...";

                var items = await _clienteService.GetAllClientiAsync();
                Clienti = new ObservableCollection<AnagraficaCliente>(items.Data ?? new List<AnagraficaCliente>());

                // clear text when you're done
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
        public async Task AddClient()
        {
            try
            {

                var nuovoCliente = new AnagraficaCliente 
                { 
                    DataRegistrazione = DateTime.UtcNow,
                    Anagrafica = new Anagrafiche()
                };




                var editWindow = new ClienteEditWindow(nuovoCliente, isNew: true);
                var result = editWindow.ShowDialog();

                if (result == true)
                {
                    // await the async call
                    var clienteEditVM = ((ClienteEditWindow)editWindow).DataContext as ClienteEditViewModel;
                    if (clienteEditVM != null)
                    {
                        var newEntity = clienteEditVM.ToEntity();
                        var createdResponse = await _clienteService.CreateClienteAsync(newEntity);

                        if (createdResponse?.Errors?.Any() != true)
                        {
                            Clienti.Add(createdResponse?.Data ?? newEntity);

                            // no need to refresh ui because we're adding the created entity directly to the collection
                            // before: await LoadClienti(); 

                            StatusText = "Cliente creato con successo";
                        }
                        else
                        {
                            // no display switch on p fisica/giur
                            StatusText = createdResponse?.Message ?? "Errore durante la creazione del cliente";
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

        [RelayCommand]
        public async Task EditClient()
        {
            try
            {
                if (SelectedClient == null)
                {
                    StatusText = "Per favore seleziona un cliente da modificare";
                    return;
                }

                // make a copy! entity, not DTO
                var clienteEntityCopy = new AnagraficaCliente
                {
                    AnagraficaId = SelectedClient.AnagraficaId,
                    CodiceCliente = SelectedClient.CodiceCliente,
                    DataRegistrazione = SelectedClient.DataRegistrazione,
                    Note = SelectedClient.Note ?? string.Empty,
                    Anagrafica = new Anagrafiche
                    {
                        Id = SelectedClient.Anagrafica.Id,
                        IsPersonaFisica = SelectedClient.Anagrafica.IsPersonaFisica,
                        Nome = SelectedClient.Anagrafica.Nome,
                        Cognome = SelectedClient.Anagrafica.Cognome,
                        RagioneSociale = SelectedClient.Anagrafica.RagioneSociale,
                        Email = SelectedClient.Anagrafica.Email,
                        Telefono = SelectedClient.Anagrafica.Telefono,
                        CodiceFiscale = SelectedClient.Anagrafica.CodiceFiscale,
                        PartitaIva = SelectedClient.Anagrafica.PartitaIva,
                    }
                };


                var editWindow = new ClienteEditWindow(clienteEntityCopy, isNew: false);
                var result = editWindow.ShowDialog();

                if (result == true)
                {
                    var clienteEditVM = editWindow.DataContext as ClienteEditViewModel;
                    if (clienteEditVM != null)
                    {
                        var updatedEntity = clienteEditVM.ToEntity();
                        updatedEntity.AnagraficaId = SelectedClient.AnagraficaId;

                        var updateResponse = await _clienteService.UpdateClienteAsync(updatedEntity.Anagrafica.Id, updatedEntity);

                        // update index so we can avoid refreshing entire coll
                        // no more LoadClienti()

                        if (updateResponse?.Errors?.Any() != true && updateResponse?.Data != null)
                        {
                            var index = Clienti.IndexOf(SelectedClient);
                            if (index >= 0)
                            {
                                Clienti[index] = updateResponse.Data;
                            }



                            StatusText = "Cliente aggiornato con successo";
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

        [RelayCommand]
        public async Task DeleteClient()
        {
            try
            {
                if (SelectedClient == null)
                {
                    StatusText = "Per favore seleziona un cliente da cancellare";
                    return;
                }

                var clientName = SelectedClient.Anagrafica?.IsPersonaFisica == true
                    ? $"{SelectedClient.Anagrafica.Nome} {SelectedClient.Anagrafica.Cognome}"
                    : SelectedClient.Anagrafica?.RagioneSociale ?? "Cliente";

                var messageBoxResult = System.Windows.MessageBox.Show(
                    $"Sei sicuro di voler cancellare '{clientName}'?",
                    "Confirm Delete",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (messageBoxResult == System.Windows.MessageBoxResult.Yes)
                {
                    var anagraficaId = SelectedClient.Anagrafica?.Id;
                    if (anagraficaId != null)
                    {
                        await _clienteService.DeleteClienteAsync(anagraficaId.Value);
                        Clienti.Remove(SelectedClient);
                        SelectedClient = null;
                        StatusText = $"Cliente '{clientName}' cancellato con successo";
                    }
                    else
                    {
                        StatusText = "Errore: dati anagrafici del cliente non disponibili";
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
    }
}