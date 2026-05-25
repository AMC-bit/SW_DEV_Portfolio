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
    public partial class ContattiPageViewModel : ObservableObject
    {
        private ContattoFrontService _contattoService;

        [ObservableProperty]
        private ObservableCollection<AnagraficaContatto> contatti = [];

        [ObservableProperty]
        private string statusText = string.Empty;

        // this is bound from UI, check the value in datagrid
        // auto passed to this page when you click on a row in the datagrid, and then you can use it to edit or delete the contact
        [ObservableProperty]
        private AnagraficaContatto? selectedContact;

        public ContattiPageViewModel(ContattoFrontService contattoService)
        {
            _contattoService = contattoService;
        }

        // adds command to the method name and runs it when the ui btn is clicked
        [RelayCommand]
        public async Task LoadContatti()
        {
            try
            {
                // display loading while loading
                StatusText = "Loading...";

                var response = await _contattoService.GetAllContattiAsync();
                if (response != null && response.Success && response.Data != null)
                {
                    Contatti = new ObservableCollection<AnagraficaContatto>(response.Data);

                    // clear text when you're done
                    StatusText = string.Empty;
                }
                else
                {
                    StatusText = response?.Message ?? "Errore nel caricamento dei contatti";
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
        public async Task AddContact()
        {
            try
            {
                var nuovoContatto = new AnagraficaContatto 
                { 
                    Provenienza = 0,
                    Anagrafica = new Anagrafiche()
                };

                var editWindow = new ContattoEditWindow(nuovoContatto, isNew: true);
                var result = editWindow.ShowDialog();

                if (result == true)
                {
                    // await the async call
                    var contattoEditVM = ((ContattoEditWindow)editWindow).DataContext as ContattoEditViewModel;
                    if (contattoEditVM != null)
                    {
                        var newEntity = contattoEditVM.ToEntity();
                        var createdResponse = await _contattoService.CreateContattoAsync(newEntity);
                        
                        if (createdResponse?.Errors?.Any() != true)
                        {
                            Contatti.Add(createdResponse?.Data ?? newEntity);
                            StatusText = "Contatto aggiunto con successo";
                        }
                        else
                        {
                            // no display switch on p fisica/giur
                            StatusText = createdResponse?.Message ?? "Errore nell'aggiunta del contatto";
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
        public async Task EditContact()
        {
            try
            {
                if (SelectedContact == null)
                {
                    StatusText = "Per favore seleziona un contatto da modificare";
                    return;
                }

                // copy for contatto instead
                var contactEntityCopy = new AnagraficaContatto
                {
                    AnagraficaId = SelectedContact.AnagraficaId,
                    Provenienza = SelectedContact.Provenienza,
                    Note = SelectedContact.Note,
                    Anagrafica = new Anagrafiche()
                    {
                        Id = SelectedContact.Anagrafica.Id,
                        IsPersonaFisica = SelectedContact.Anagrafica.IsPersonaFisica,
                        Nome = SelectedContact.Anagrafica.Nome,
                        Cognome = SelectedContact.Anagrafica.Cognome,
                        RagioneSociale = SelectedContact.Anagrafica.RagioneSociale,
                        Email = SelectedContact.Anagrafica.Email,
                        Telefono = SelectedContact.Anagrafica.Telefono,
                        CodiceFiscale = SelectedContact.Anagrafica.CodiceFiscale,
                        PartitaIva = SelectedContact.Anagrafica.PartitaIva,
                    }
                };

                var editWindow = new ContattoEditWindow(contactEntityCopy, isNew: false);
                var result = editWindow.ShowDialog();

                if (result == true)
                {
                    var contattoEditVM = editWindow.DataContext as ContattoEditViewModel;
                    if (contattoEditVM != null)
                    {
                        var updatedEntity = contattoEditVM.ToEntity();
                        updatedEntity.AnagraficaId = SelectedContact.AnagraficaId;

                        var updateResponse = await _contattoService.UpdateContattoAsync(updatedEntity.Anagrafica.Id, updatedEntity);

                        // update index so we can avoid refreshing entire coll
                        // no more LoadContatti()

                        if (updateResponse?.Errors?.Any() != true && updateResponse?.Data != null)
                        {
                            var index = Contatti.IndexOf(SelectedContact);
                            if (index >= 0)
                            {
                                Contatti[index] = updateResponse.Data;
                            }

                            StatusText = "Contatto aggiornato con successo";
                        }
                        else
                        {
                            StatusText = updateResponse?.Message ?? "Errore nell'aggiornamento del contatto";
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
        public async Task DeleteContact()
        {
            try
            {
                if (SelectedContact == null)
                {
                    StatusText = "Per favore seleziona un contatto da cancellare";
                    return;
                }

                var messageBoxResult = System.Windows.MessageBox.Show(
                    $"Sei sicuro di voler cancellare il contatto?",
                    "Confirm Delete",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (messageBoxResult == System.Windows.MessageBoxResult.Yes)
                {
                    await _contattoService.DeleteContattoAsync(SelectedContact.Anagrafica.Id);
                    Contatti.Remove(SelectedContact);
                    SelectedContact = null;
                    StatusText = "Contatto cancellato con successo";
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
        public async Task ConvertContact()
        {
            try
            {
                if (SelectedContact == null)
                {
                    StatusText = "Per favore seleziona un contatto da convertire";
                    return;
                }

                var messageBoxResult = System.Windows.MessageBox.Show(
                    $"Sei sicuro di voler convertire il contatto? L'azione è irreversible.",
                    "Confirm Convert",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (messageBoxResult == System.Windows.MessageBoxResult.Yes)
                {
                    await _contattoService.ConvertContattoAsync(SelectedContact.Anagrafica.Id);
                    Contatti.Remove(SelectedContact);
                    SelectedContact = null;
                    StatusText = "Contatto convertito con successo";
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