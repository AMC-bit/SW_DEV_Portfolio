using BlazorServerProgettoAnagrafiche.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgettoAnagrafiche.Models;
using ProgettoAnagrafiche.Models.DTO;


namespace BlazorServerProgettoAnagrafiche.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientiController : ControllerBase
    {
        private readonly ClienteService _clienteService;

        public ClientiController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }


        // return entities now
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AnagraficaCliente>>>> Get()
        {
            try
            {

                // split the wrapper, check for errors
                var result = await _clienteService.GetAllAsync();


                // is there any data at all? if not return not found with message
                if (result.Data == null || !result.Data.Any())
                {
                    return NotFound(ApiResponse<List<AnagraficaCliente>>.NotFound("Nessun cliente trovato."));
                }

                // any errors carried over in the wrapper?
                // return the list of errors + bad request
                if (result.Errors.Any())
                {
                    return BadRequest(ApiResponse<List<AnagraficaCliente>>.BadRequest(result.Errors, "Errore di validazione in uno o più clienti."));
                    }

                // return with 200 and apiresponse+list of clienti  
                return Ok(ApiResponse<List<AnagraficaCliente>>.Ok(result.Data));
            }

            // catch any other issue
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<AnagraficaCliente>>.InternalError($"Errore: {ex.Message}"));
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AnagraficaCliente>>> GeteByIdAsync(int id)
        {
            try
            {
                var result = await _clienteService.GetByIdAsync(id);

                if (result.Data == null)
                {
                    return NotFound(ApiResponse<AnagraficaCliente>.NotFound("Cliente non trovato."));
                }

                if (result.Errors.Any())
                    {
                    return BadRequest(ApiResponse<AnagraficaCliente>.BadRequest(result.Errors, "Errore di validazione del cliente."));
                    }


                // no issues? send data over with ok
                return Ok(ApiResponse<AnagraficaCliente>.Ok(result.Data));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AnagraficaCliente>.InternalError($"Errore: {ex.Message}"));
            }
        }

        // create
        // don't return data here if you succeed, just that it worked
        // return errors/validations if it failed

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AnagraficaCliente>>> CreateAsync([FromBody] AnagraficaCliente entity)
        {
            try
            {
                if (entity == null)
                    return BadRequest(ApiResponse<AnagraficaCliente>.BadRequest("Dati cliente null."));

                var fullErrorList = await _clienteService.CheckUpdateAsync(entity);


                if (fullErrorList.Any())
                    return BadRequest(ApiResponse<AnagraficaCliente>.BadRequest(fullErrorList));

                var result = await _clienteService.CreateAsync(entity);
                
                if (result.Errors.Any())
                    return BadRequest(ApiResponse<AnagraficaCliente>.BadRequest(result.Errors));

                if (result.Data == null)
                    return StatusCode(500, ApiResponse<AnagraficaCliente>.InternalError("Errore interno: dati cliente non disponibili dopo la creazione."));

                return StatusCode(201, ApiResponse<AnagraficaCliente>.Created(result.Data, "Cliente aggiunto con successo!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AnagraficaCliente>.InternalError($"Errore: {ex.Message}"));
            }
        }


        // edit
        // same thing, don't return data anymore
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<AnagraficaCliente>>> Update(int id, [FromBody] AnagraficaCliente entity)
        {
            try
            {
                // is incoming entity null?
                if (entity == null)
                    return BadRequest(ApiResponse<AnagraficaCliente>.BadRequest("Dati cliente null."));

                // look for the entity you should update, did you find it?
                var existing = await _clienteService.GetByIdAsync(id);
                if (existing.Data == null)
                    return NotFound(ApiResponse<AnagraficaCliente>.NotFound("Cliente non trovato."));

                // start an error list, run validate, service now does everything
                var fullErrorList = await _clienteService.CheckUpdateAsync(entity, excludeId: id);

                // if there's errors return bad request
                if (fullErrorList.Any())
                    return BadRequest(ApiResponse<AnagraficaCliente>.BadRequest(fullErrorList));

                // otherwise, update the existing entity with incoming data
                var result = await _clienteService.UpdateAsync(entity);

                // check result too when it comes back
                if (result.Errors.Any())
                    return BadRequest(ApiResponse<AnagraficaCliente>.BadRequest(result.Errors));

                if (result.Data == null)
                    return StatusCode(500, ApiResponse<AnagraficaCliente>.InternalError("Errore interno: dati cliente non disponibili dopo l'aggiornamento."));

                // if it passes, jsut ok it
                return Ok(ApiResponse<AnagraficaCliente>.Ok(result.Data!, "Cliente aggiornato con successo!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AnagraficaCliente>.InternalError($"Errore: {ex.Message}"));
            }
        }


        // delete
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            try
            {
                // check if it exists
                var existing = await _clienteService.GetByIdAsync(id);

                // if not return not found
                if (existing == null)
                {
                    return NotFound(ApiResponse.NotFound("Cliente non trovato."));
                }

                // otherwise go ahead and delete it
                var deleted = await _clienteService.DeleteAsync(id);
                // if there's an error during deletion, return that
                if (deleted.Errors.Any())
                {
                    return StatusCode(500, ApiResponse.BadRequest(deleted.Errors));
                }

                // otherwise return ok and true
                return Ok(ApiResponse.Ok("Cliente eliminato con successo!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.InternalError($"Errore: {ex.Message}"));
            }
        }
    }
}
