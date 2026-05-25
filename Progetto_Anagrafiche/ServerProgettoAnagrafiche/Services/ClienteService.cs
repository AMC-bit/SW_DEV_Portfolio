using Azure;
using BlazorServerProgettoAnagrafiche.Components.Pages;
using BlazorServerProgettoAnagrafiche.Interfaces;
using BlazorServerProgettoAnagrafiche.Services.ResponseWrapper;
using BlazorServerProgettoAnagrafiche.ViewModels;
using Microsoft.EntityFrameworkCore;
using ProgettoAnagrafiche.Models;

namespace BlazorServerProgettoAnagrafiche.Services
    {
    public class ClienteService : IService<AnagraficaCliente>
        {

        private readonly AppDbContext _db;


        public ClienteService(AppDbContext dbContext)
            {
            _db = dbContext;
            }

        // grid handling


        public async Task<List<ClienteBackendViewModel>> GetClientiGrid()
            {
            return await _db.AnagraficaClienti
                .Select(cl => new ClienteBackendViewModel
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
                    DataRegistrazione = cl.DataRegistrazione,
                    Note = cl.Note
                    })
                .ToListAsync();
            }

        public async Task<ClienteBackendViewModel> GetGridClienteByIdAsync(int anagraficaId)
            {
            var cliente = await _db.AnagraficaClienti
                .Where(c => c.AnagraficaId == anagraficaId)
                .Select(cl => new ClienteBackendViewModel
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
                    DataRegistrazione = cl.DataRegistrazione,
                    Note = cl.Note
                    })
                .FirstOrDefaultAsync();

            if (cliente == null)
                return new ClienteBackendViewModel { DataRegistrazione = DateTime.UtcNow };
            else
                return cliente;
            }


        //// validate stuff, return list of errors
        //public List<string> CheckUpdate(AnagraficaCliente entity)
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

        public async Task<List<string>> CheckUpdateAsync(AnagraficaCliente entity, int? excludeId = null)
            {
            if (entity == null)
                return new List<string> { "Nessuna entità caricata." };

            try
                {
                // Normalize
                entity.Normalize();

                // Then validate
                var errors = entity.Validate();

                // Dup check now here
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
        public async Task<Return<AnagraficaCliente>> GetByIdAsync(int pkValue)
            {
            // setup empty wrapper
            var response = new Return<AnagraficaCliente>();

            try
                {
                // try fetching the entity from db, with related anagrafica
                var entity = await _db.AnagraficaClienti
                    .Include(c => c.Anagrafica)
                    .FirstOrDefaultAsync(c => c.AnagraficaId == pkValue);

                // check first, did you find anything?

                // no: add error to wrapper, return immediately
                if (entity == null)
                    {
                    response.Errors.Add("Cliente non trovato.");
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
        public async Task<Return<List<AnagraficaCliente>>> GetAllAsync()
            {
            var response = new Return<List<AnagraficaCliente>>();

            // linq, include to get rel. anag + to list
            try
                {
                var entities = await _db.AnagraficaClienti
                    .Include(c => c.Anagrafica)
                    .ToListAsync();

                // no: add error to wrapper, return immediately
                // handle both, null (no list to begin with) and empty list as "not found"
                if (entities == null || !entities.Any())
                    {
                    response.Errors.Add("Nessun cliente trovato.");
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


        // return the entity to update ui
        public async Task<Return<AnagraficaCliente>> CreateAsync(AnagraficaCliente cliente)
            {
            var response = new Return<AnagraficaCliente>();

            try
                {

                // simplified
                var errors = await CheckUpdateAsync(cliente);

                if (errors.Any())
                    {
                    response.Errors = errors;
                    return response;
                    }


                // else proceed
                _db.AnagraficaClienti.Add(cliente);
                await _db.SaveChangesAsync();
                response.Data = cliente;
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


        // also return the entity to update ui
        public async Task<Return<AnagraficaCliente>> UpdateAsync(AnagraficaCliente cliente)
            {

            var response = new Return<AnagraficaCliente>();

            try
                {

                var errors = await CheckUpdateAsync(cliente, excludeId: cliente.AnagraficaId);

                if (errors.Any())
                    {
                    response.Errors = errors;
                    return response;
                    }

                // Fetch the tracked entity from DB instead of attaching a detached one
                var existing = await _db.AnagraficaClienti
                    .Include(c => c.Anagrafica)
                    .FirstOrDefaultAsync(c => c.AnagraficaId == cliente.AnagraficaId);

                if (existing == null)
                    {
                    response.Errors.Add("Cliente non trovato.");
                    return response;
                    }

                // Update only the properties you want to change
                existing.Note = cliente.Note;
                existing.DataRegistrazione = cliente.DataRegistrazione;
                existing.Anagrafica.Nome = cliente.Anagrafica.Nome;
                existing.Anagrafica.Cognome = cliente.Anagrafica.Cognome;
                existing.Anagrafica.RagioneSociale = cliente.Anagrafica.RagioneSociale;
                existing.Anagrafica.Email = cliente.Anagrafica.Email;
                existing.Anagrafica.Telefono = cliente.Anagrafica.Telefono;
                existing.Anagrafica.CodiceFiscale = cliente.Anagrafica.CodiceFiscale;
                existing.Anagrafica.PartitaIva = cliente.Anagrafica.PartitaIva;
                existing.Anagrafica.IsPersonaFisica = cliente.Anagrafica.IsPersonaFisica;

                await _db.SaveChangesAsync();

                response.Data = existing;
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


        public async Task<Result> DeleteAsync(int pkValue)
            {
            // setup answer
            var response = new Result();

            try
                {
                // try to find it
                var cliente = await _db.AnagraficaClienti
                    .Include(c => c.Anagrafica)
                    .FirstOrDefaultAsync(c => c.AnagraficaId == pkValue);

                // if not found, just throw a KeyNotFound
                if (cliente == null)
                    {
                    response.Errors.Add("Cliente non trovato.");
                    return response;
                    }

                // cascade delete
                _db.Anagrafiche.Remove(cliente.Anagrafica); 

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
                        (!string.IsNullOrEmpty(codiceFiscale) && !string.IsNullOrEmpty(a.CodiceFiscale) && a.CodiceFiscale.ToUpper() == codiceFiscale ||
                         !string.IsNullOrEmpty(partitaIva) && !string.IsNullOrEmpty(a.PartitaIva) && a.PartitaIva.ToUpper() == partitaIva)
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
