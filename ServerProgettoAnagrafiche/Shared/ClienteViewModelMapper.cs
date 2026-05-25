using ProgettoAnagrafiche.Models;
using BlazorServerProgettoAnagrafiche.ViewModels;

namespace BlazorServerProgettoAnagrafiche.Shared
{
    public static class ClienteViewModelMapper
    {
        // ViewModel to Entity
        public static AnagraficaCliente ClienteToEntity(this ClienteBackendViewModel viewModel)
        {
            return new AnagraficaCliente
            {
                AnagraficaId = viewModel.Id,
                CodiceCliente = viewModel.CodiceCliente,
                DataRegistrazione = viewModel.DataRegistrazione,
                Note = viewModel.Note,
                Anagrafica = new Anagrafiche
                {
                    Id = viewModel.Id,
                    IsPersonaFisica = viewModel.IsPersonaFisica,
                    Nome = viewModel.Nome,
                    Cognome = viewModel.Cognome,
                    RagioneSociale = viewModel.RagioneSociale,
                    Email = viewModel.Email,
                    Telefono = viewModel.Telefono,
                    CodiceFiscale = viewModel.CodiceFiscale,
                    PartitaIva = viewModel.PartitaIva
                }
            };
        }

        // Entity to ViewModel
        public static ClienteBackendViewModel ClienteToViewModel(this AnagraficaCliente entity)
        {
            return new ClienteBackendViewModel
            {
                Id = entity.AnagraficaId,
                CodiceCliente = entity.CodiceCliente,
                DataRegistrazione = entity.DataRegistrazione,
                Note = entity.Note,
                IsPersonaFisica = entity.Anagrafica.IsPersonaFisica,
                Nome = entity.Anagrafica.Nome,
                Cognome = entity.Anagrafica.Cognome,
                RagioneSociale = entity.Anagrafica.RagioneSociale,
                Email = entity.Anagrafica.Email,
                Telefono = entity.Anagrafica.Telefono,
                CodiceFiscale = entity.Anagrafica.CodiceFiscale,
                PartitaIva = entity.Anagrafica.PartitaIva
            };
        }
    }
}
