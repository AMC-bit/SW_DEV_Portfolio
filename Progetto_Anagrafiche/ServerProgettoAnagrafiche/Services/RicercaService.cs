using BlazorServerProgettoAnagrafiche.Services.ResponseWrapper;
using Microsoft.EntityFrameworkCore;
using ProgettoAnagrafiche.Models;
using BlazorServerProgettoAnagrafiche.ViewModels;

namespace BlazorServerProgettoAnagrafiche.Services
{
    public class RicercaService
    {
        private readonly AppDbContext _db;

        public RicercaService(AppDbContext dbContext)
        {
            _db = dbContext;
        }

        // Get search results combining both clienti and contatti
        // display only
        public async Task<List<RicercaBackendViewModel>> GetRicercaGrid()
        {
            var clienti = await _db.AnagraficaClienti
                .Select(cl => new RicercaBackendViewModel
                {
                    Tipo = "Cliente",
                    Id = cl.Anagrafica.Id,
                    Nome = cl.Anagrafica.Nome,
                    Cognome = cl.Anagrafica.Cognome,
                    Telefono = cl.Anagrafica.Telefono,
                    Email = cl.Anagrafica.Email,
                    CodiceFiscale = cl.Anagrafica.CodiceFiscale,
                    PartitaIva = cl.Anagrafica.PartitaIva,
                    IsPersonaFisica = cl.Anagrafica.IsPersonaFisica,
                    RagioneSociale = cl.Anagrafica.RagioneSociale,
                    CodiceCliente = cl.CodiceCliente,
                    DataRegistrazione = cl.DataRegistrazione,
                    NoteCliente = cl.Note,
                    Provenienza = null,
                    NoteContatto = null
                })
                .ToListAsync();

            var contatti = await _db.AnagraficaContatti
                .Select(cl => new RicercaBackendViewModel
                {
                    Tipo = "Contatto",
                    Id = cl.Anagrafica.Id,
                    Nome = cl.Anagrafica.Nome,
                    Cognome = cl.Anagrafica.Cognome,
                    Telefono = cl.Anagrafica.Telefono,
                    Email = cl.Anagrafica.Email,
                    CodiceFiscale = cl.Anagrafica.CodiceFiscale,
                    PartitaIva = cl.Anagrafica.PartitaIva,
                    IsPersonaFisica = cl.Anagrafica.IsPersonaFisica,
                    RagioneSociale = cl.Anagrafica.RagioneSociale,
                    CodiceCliente = null,
                    DataRegistrazione = null,
                    NoteCliente = null,
                    Provenienza = cl.Provenienza.ToString(),
                    NoteContatto = cl.Note
                })
                .ToListAsync();

            return clienti.Union(contatti).ToList();
        }
    }
}

