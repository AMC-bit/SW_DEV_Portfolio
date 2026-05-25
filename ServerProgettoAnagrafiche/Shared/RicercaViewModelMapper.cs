using ProgettoAnagrafiche.Models;
using BlazorServerProgettoAnagrafiche.ViewModels;

namespace BlazorServerProgettoAnagrafiche.Shared
{
    public static class RicercaViewModelMapper
    {
        // Cliente Entity to Ricerca ViewModel
        public static RicercaBackendViewModel ToRicercaViewModel(this AnagraficaCliente cliente)
        {
            return new RicercaBackendViewModel
            {
                Tipo = "Cliente",
                Id = cliente.AnagraficaId,
                IsPersonaFisica = cliente.Anagrafica.IsPersonaFisica,
                Nome = cliente.Anagrafica.Nome,
                Cognome = cliente.Anagrafica.Cognome,
                RagioneSociale = cliente.Anagrafica.RagioneSociale,
                Email = cliente.Anagrafica.Email,
                Telefono = cliente.Anagrafica.Telefono,
                CodiceFiscale = cliente.Anagrafica.CodiceFiscale,
                PartitaIva = cliente.Anagrafica.PartitaIva,
                CodiceCliente = cliente.CodiceCliente,
                DataRegistrazione = cliente.DataRegistrazione,
                NoteCliente = cliente.Note,
                Provenienza = null,
                NoteContatto = null
            };
        }

        // Contatto Entity to Ricerca ViewModel
        public static RicercaBackendViewModel ToRicercaViewModel(this AnagraficaContatto contatto)
        {
            return new RicercaBackendViewModel
            {
                Tipo = "Contatto",
                Id = contatto.AnagraficaId,
                IsPersonaFisica = contatto.Anagrafica.IsPersonaFisica,
                Nome = contatto.Anagrafica.Nome,
                Cognome = contatto.Anagrafica.Cognome,
                RagioneSociale = contatto.Anagrafica.RagioneSociale,
                Email = contatto.Anagrafica.Email,
                Telefono = contatto.Anagrafica.Telefono,
                CodiceFiscale = contatto.Anagrafica.CodiceFiscale,
                PartitaIva = contatto.Anagrafica.PartitaIva,
                Provenienza = contatto.Provenienza.ToString(),
                NoteContatto = contatto.Note,
                CodiceCliente = null,
                DataRegistrazione = null,
                NoteCliente = null
            };
        }
    }
}
