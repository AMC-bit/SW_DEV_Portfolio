using ProgettoAnagrafiche.Models;
using BlazorServerProgettoAnagrafiche.ViewModels;

namespace BlazorServerProgettoAnagrafiche.Shared
{
    public static class ContattoViewModelMapper
    {
        // ViewModel to Entity
        public static AnagraficaContatto ContattoToEntity(this ContattoBackendViewModel viewModel)
        {
            return new AnagraficaContatto
            {
                AnagraficaId = viewModel.Id,
                Provenienza = viewModel.Provenienza,
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
        public static ContattoBackendViewModel ContattoToViewModel(this AnagraficaContatto entity)
        {
            return new ContattoBackendViewModel
            {
                Id = entity.AnagraficaId,
                Provenienza = entity.Provenienza,
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
