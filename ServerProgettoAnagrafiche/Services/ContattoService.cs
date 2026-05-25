using Azure;
using BlazorServerProgettoAnagrafiche.Components.Pages;
using BlazorServerProgettoAnagrafiche.Interfaces;
using BlazorServerProgettoAnagrafiche.Services.ResponseWrapper;
using BlazorServerProgettoAnagrafiche.ViewModels;
using Microsoft.EntityFrameworkCore;
using ProgettoAnagrafiche.Models;

namespace BlazorServerProgettoAnagrafiche.Services
    {
    public class ContattoService : IService<AnagraficaContatto>
        {

        private readonly AppDbContext _db;


        public ContattoService(AppDbContext dbContext)
            {
            _db = dbContext;
            }

        // grids

        public async Task<List<ContattoBackendViewModel>> GetContattiGrid()
            {
            return await _db.AnagraficaContatti
                .Select(cl => new ContattoBackendViewModel
                    {
                    Id = cl.Anagrafica.Id,
                    Nome = cl.Anagrafica.Nome,
                    Cognome = cl.Anagrafica.Cognome,
                    Telefono = cl.Anagrafica.Telefono,
                    Email = cl.Anagrafica.Email,
                    CodiceFiscale = cl.Anagrafica.CodiceFiscale,
                    PartitaIva = cl.Anagrafica.PartitaIva,
                    IsPersonaFisica = cl.Anagrafica.IsPersonaFisica,
                    RagioneSociale = cl.Anagrafica.RagioneSociale,


                    Provenienza = cl.Provenienza,
                    Note = cl.Note
                    })
                .ToListAsync();
            }

        public async Task<ContattoBackendViewModel> GetGridContattoByIdAsync(int anagraficaId)
            {
            var contatto = await _db.AnagraficaContatti
                .Where(c => c.AnagraficaId == anagraficaId)
                .Select(cl => new ContattoBackendViewModel
                    {
                    Id = cl.Anagrafica.Id,
                    Nome = cl.Anagrafica.Nome,
                    Cognome = cl.Anagrafica.Cognome,
                    Telefono = cl.Anagrafica.Telefono,
                    Email = cl.Anagrafica.Email,
                    CodiceFiscale = cl.Anagrafica.CodiceFiscale,
                    PartitaIva = cl.Anagrafica.PartitaIva,
                    IsPersonaFisica = cl.Anagrafica.IsPersonaFisica,
                    RagioneSociale = cl.Anagrafica.RagioneSociale,
                    Provenienza = cl.Provenienza,
                    Note = cl.Note
                    })
                .FirstOrDefaultAsync();

            if (contatto == null)
                return new ContattoBackendViewModel { Provenienza = 0 };
            else
                return contatto;

            }


        //// validate stuff, return list of errors
        //public List<string> CheckUpdate(AnagraficaContatto entity)
        //    {
        //    if (entity == null)
        //        return new List<string> { "Nessuna entità caricata." };

        //    try
        //        {
        //        // Normalize data first (trim, uppercase, etc.)
        //        entity.Normalize();

        //        // Then validate business rules
        //        return entity.Validate();
        //        }
        //    catch (Exception ex)
        //        {
        //        return new List<string> { $"Errore validazione: {ex.Message}" };
        //        }
        //    }

        // also check for duplicates -> contatto
        public async Task<List<string>> CheckUpdateAsync(AnagraficaContatto entity, int? excludeId = null)
            {
            if (entity == null)
                return new List<string> { "Nessuna entità caricata." };

            try
                {
                // Normalize data first (trim, uppercase, etc.)
                entity.Normalize();

                // Then validate business rules
                var errors = entity.Validate();

                // Dup check for Codice Fiscale or Partita IVA
                if (!string.IsNullOrEmpty(entity.Anagrafica.CodiceFiscale) ||
                    !string.IsNullOrEmpty(entity.Anagrafica.PartitaIva))
                    {
                    var duplicates = await ExistsByCodiceFiscaleOrPIVAAsync(
                        entity.Anagrafica.CodiceFiscale,
                        entity.Anagrafica.PartitaIva,
                        excludeId: excludeId);

                    if (duplicates)
                        {
                        errors.Add("Errore: Codice Fiscale o Partita IVA duplicati.");
                        }
                    }

                return errors;
                }
            catch (Exception ex)
                {
                return new List<string> { $"Errore validazione: {ex.Message}" };
                }
            }


        // need to fetch something: return wrapper+data or wrapper+errors. 
        // Errors: didn't find what I wanted to get, or something went wrong (catch exceptions and translate them to a list of errors)
        public async Task<Return<AnagraficaContatto>> GetByIdAsync(int pkValue)
            {
            // setup empty wrapper
            var response = new Return<AnagraficaContatto>();

            try
                {
                // try fetching the entity from db, with related anagrafica
                var entity = await _db.AnagraficaContatti
                    .Include(c => c.Anagrafica)
                    .FirstOrDefaultAsync(c => c.AnagraficaId == pkValue);

                // check first, did you find anything?

                // no: add error to wrapper, return immediately
                if (entity == null)
                    {
                    response.Errors.Add("Contatto non trovato.");
                    return response;
                    }

                // else set it as data and return the wrapper with data in it & no errors
                else
                    {
                    response.Data = entity;
                    return response;
                    }
                }

            // catch unpexpected errors
            catch (DbUpdateException ex) // try to save but DB rejects it, es constraint violation
                {
                // use inner because EF buries these, fallback to outer if there isn't any
                response.Errors.Add($"Errore: {ex.InnerException?.Message ?? ex.Message}");
                return response;
                }
            // you're trying to do something invalid/nonsensical
            catch (InvalidOperationException ex)
                {
                response.Errors.Add($"Errore: {ex.Message}");
                return response;
                }
            // misc.
            catch (Exception ex)
                {
                response.Errors.Add($"Errore: {ex.Message}");
                return response;
                }
            }


        // same as before, get several somethings
        // same error pattern
        public async Task<Return<List<AnagraficaContatto>>> GetAllAsync()
            {
            var response = new Return<List<AnagraficaContatto>>();

            // linq, include to get rel. anag + to list
            try
                {
                var entities = await _db.AnagraficaContatti
                    .Include(c => c.Anagrafica)
                    .ToListAsync();

                // no: add error to wrapper, return immediately
                // handle both, null (no list to begin with) and empty list as "not found"
                if (entities == null || !entities.Any())
                    {
                    response.Errors.Add("Nessun contatto trovato.");
                    return response;
                    }
                else
                    {
                    response.Data = entities;
                    return response;
                    }
                }
            // catch unpexpected errors
            catch (DbUpdateException ex) // try to save but DB rejects it, es constraint violation
                {
                // use inner because EF buries these, fallback to outer if there isn't any
                response.Errors.Add($"Errore: {ex.InnerException?.Message ?? ex.Message}");
                return response;
                }
            // you're trying to do something invalid/nonsensical
            catch (InvalidOperationException ex)
                {
                response.Errors.Add($"Errore: {ex.Message}");
                return response;
                }
            // misc.
            catch (Exception ex)
                {
                response.Errors.Add($"Errore: {ex.Message}");
                return response;
                }
            }


        // refactored, don't bother returning entities, just return wrapper with success or failure and details
        public async Task<Return<AnagraficaContatto>> CreateAsync(AnagraficaContatto contatto)
            {
            var response = new Return<AnagraficaContatto>();



            try
                {

                // thinned down, use checkupdateasync to just handle all duplications, validations etc

                var errors = await CheckUpdateAsync(contatto);

                if (errors.Any())
                    {
                    response.Errors = errors;
                    return response;
                    }


                // else proceed
                _db.AnagraficaContatti.Add(contatto);
                await _db.SaveChangesAsync();
                response.Data = contatto;
                return response;


                }

            // catch unpexpected errors
            catch (DbUpdateException ex) // try to save but DB rejects it, es constraint violation
                {
                // use inner because EF buries these, fallback to outer if there isn't any
                response.Errors.Add($"Errore: {ex.InnerException?.Message ?? ex.Message}");
                return response;
                }
            // you're trying to do something invalid/nonsensical
            catch (InvalidOperationException ex)
                {
                response.Errors.Add($"Errore: {ex.Message}");
                return response;
                }
            // misc.
            catch (Exception ex)
                {
                response.Errors.Add($"Errore: {ex.Message}");
                return response;
                }
            }

        public async Task<Return<AnagraficaContatto>> UpdateAsync(AnagraficaContatto contatto)
        {
            var response = new Return<AnagraficaContatto>();

            try
                {

                var errors = await CheckUpdateAsync(contatto, excludeId: contatto.AnagraficaId);

                if (errors.Any())
                    {
                    response.Errors = errors;
                    return response;
                    }

                // Fetch the tracked entity from DB instead of attaching a detached one
                var existing = await _db.AnagraficaContatti
                    .Include(c => c.Anagrafica)
                    .FirstOrDefaultAsync(c => c.AnagraficaId == contatto.AnagraficaId);

                if (existing == null)
                    {
                    response.Errors.Add("Contatto non trovato.");
                    return response;
                    }

                // Update only the properties you want to change
                existing.Note = contatto.Note;
                existing.Provenienza = contatto.Provenienza;
                existing.Anagrafica.Nome = contatto.Anagrafica.Nome;
                existing.Anagrafica.Cognome = contatto.Anagrafica.Cognome;
                existing.Anagrafica.RagioneSociale = contatto.Anagrafica.RagioneSociale;
                existing.Anagrafica.Email = contatto.Anagrafica.Email;
                existing.Anagrafica.Telefono = contatto.Anagrafica.Telefono;
                existing.Anagrafica.CodiceFiscale = contatto.Anagrafica.CodiceFiscale;
                existing.Anagrafica.PartitaIva = contatto.Anagrafica.PartitaIva;
                existing.Anagrafica.IsPersonaFisica = contatto.Anagrafica.IsPersonaFisica;

                await _db.SaveChangesAsync();

                response.Data = existing;
                return response;
                }
            catch (DbUpdateException ex)
                {
                response.Errors.Add($"Errore: {ex.InnerException?.Message ?? ex.Message}");
                return response;
                }
            catch (InvalidOperationException ex)
                {
                response.Errors.Add($"Errore: {ex.Message}");
                return response;
                }
            catch (Exception ex)
                {
                response.Errors.Add($"Errore: {ex.Message}");
                return response;
                }
            }


        public async Task<Result> DeleteAsync(int pkValue)
            {
            // setup answer
            var response = new Result();

            try
                {
                // try to find it
                var contatto = await _db.AnagraficaContatti
                    .Include(c => c.Anagrafica)
                    .FirstOrDefaultAsync(c => c.AnagraficaId == pkValue);

                // if not found, just throw a KeyNotFound
                if (contatto == null)
                    {
                    response.Errors.Add("Contatto non trovato.");
                    return response;
                    }

                // cascade delete
                _db.Anagrafiche.Remove(contatto.Anagrafica);

                // save
                await _db.SaveChangesAsync();
                return response;

                }
            catch (DbUpdateException ex)
                {
                response.Errors.Add($"Errore: {ex.InnerException?.Message ?? ex.Message}");
                return response;
                }
            catch (InvalidOperationException ex)
                {
                response.Errors.Add($"Errore: {ex.Message}");
                return response;
                }
            catch (Exception ex)
                {
                response.Errors.Add($"Errore: {ex.Message}");
                return response;
                }
            }

        public async Task<bool> ConvertContattoAsync(int anagraficaId)
            {
            try
                {
                // Check if a cliente already exists with this anagrafica id
                var existingCliente = await _db.AnagraficaClienti
                    .AnyAsync(c => c.AnagraficaId == anagraficaId);

                if (existingCliente)
                    return false;

                // Load the contatto to convert
                var contatto = await _db.AnagraficaContatti
                    .Include(c => c.Anagrafica)
                    .FirstOrDefaultAsync(c => c.AnagraficaId == anagraficaId);

                if (contatto?.Anagrafica == null)
                    return false;

                // Remove from contatti
                _db.AnagraficaContatti.Remove(contatto);

                // Add as cliente
                var cliente = new AnagraficaCliente
                    {
                    AnagraficaId = contatto.AnagraficaId,
                    DataRegistrazione = DateTime.UtcNow,
                    Note = contatto.Note,
                    Anagrafica = contatto.Anagrafica
                    };

                _db.AnagraficaClienti.Add(cliente);
                await _db.SaveChangesAsync();
                return true;
                }
            catch (Exception ex)
                {
                throw new InvalidOperationException("Errore durante la conversione del contatto.", ex);
                }
            }



        // Check for duplicate Codice Fiscale or Partita IVA (excluding current record)
        public async Task<bool> ExistsByCodiceFiscaleOrPIVAAsync(string? codiceFiscale, string? partitaIva, int? excludeId = null)
            {
            try
                {
                // Normalize
                codiceFiscale = codiceFiscale?.Trim().ToUpper();
                partitaIva = partitaIva?.Trim().ToUpper();

                if (string.IsNullOrEmpty(codiceFiscale) && string.IsNullOrEmpty(partitaIva))
                    return false; // nothing to check

                return await _db.Anagrafiche
                    .Where(a =>
                        (excludeId == null || a.Id != excludeId) &&
                        ((!string.IsNullOrEmpty(codiceFiscale) && !string.IsNullOrEmpty(a.CodiceFiscale) && a.CodiceFiscale.ToUpper() == codiceFiscale) ||
                         (!string.IsNullOrEmpty(partitaIva) && !string.IsNullOrEmpty(a.PartitaIva) && a.PartitaIva.ToUpper() == partitaIva))
                    )
                    .AnyAsync();
                }
            catch (Exception ex)
                {
                throw new InvalidOperationException("Errore durante la verifica dei duplicati.", ex);
                }
            }
        }
    }
