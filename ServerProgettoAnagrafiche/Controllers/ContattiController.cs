using BlazorServerProgettoAnagrafiche.Services; 

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;    
using ProgettoAnagrafiche.Models;
using ProgettoAnagrafiche.Models.DTO;

namespace BlazorServerProgettoAnagrafiche.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContattiController : ControllerBase
    {
        private readonly ContattoService _contattoService;
        public ContattiController(ContattoService contattoService)
        {
            _contattoService = contattoService;
        }


            [HttpGet]
            public async Task<ActionResult<ApiResponse<List<AnagraficaContatto>>>> Get()
            {
                try
                {
                    var result = await _contattoService.GetAllAsync();

                // is there any data at all? if not return not found with message
                if (result.Data == null || !result.Data.Any())
                    {
                    return NotFound(ApiResponse<List<AnagraficaContatto>>.NotFound("Nessun contatto trovato."));
                    }
                // any errors carried over in the wrapper?
                // return the list of errors + bad request
                if (result.Errors.Any())
                    {
                    return BadRequest(ApiResponse<List<AnagraficaContatto>>.BadRequest(result.Errors, "Errore di validazione in uno o più contatti."));
                    }

                // return with 200 and apiresponse+list of contatti  
                return Ok(ApiResponse<List<AnagraficaContatto>>.Ok(result.Data));
                }

            // catch any other issue
            catch (Exception ex)
                {
                return StatusCode(500, ApiResponse<List<AnagraficaContatto>>.InternalError($"Errore: {ex.Message}"));
                }
            }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AnagraficaContatto>>> GetByIdAsync(int id)
            {
            try
                {
                var result = await _contattoService.GetByIdAsync(id);

                if (result.Data == null)
                    {
                    return NotFound(ApiResponse<AnagraficaContatto>.NotFound("Contatto non trovato."));
                    }

                if (result.Errors.Any())
                    {
                    return BadRequest(ApiResponse<AnagraficaContatto>.BadRequest(result.Errors, "Errore di validazione del contatto."));
                    }


                // no issues? send data over with ok
                return Ok(ApiResponse<AnagraficaContatto>.Ok(result.Data));

                }
            catch (Exception ex)
                {
                return StatusCode(500, ApiResponse<AnagraficaContatto>.InternalError($"Errore: {ex.Message}"));
                }
            }

        // create
        [HttpPost]
        public async Task<ActionResult<ApiResponse<AnagraficaContatto>>> CreateAsync([FromBody] AnagraficaContatto entity)
            {
            try
                {
                if (entity == null)
                    return BadRequest(ApiResponse<AnagraficaContatto>.BadRequest("Dati contatto null."));

                var result = await _contattoService.CreateAsync(entity);

                if (result.Errors.Any())
                    return BadRequest(ApiResponse<AnagraficaContatto>.BadRequest(result.Errors));

                // also check data
                if (result.Data == null)
                    return StatusCode(500, ApiResponse<AnagraficaContatto>.InternalError("Errore interno: il contatto non è stato creato."));

                

                return StatusCode(201, ApiResponse<AnagraficaContatto>.Created(result.Data, "Contatto aggiunto con successo!"));
                }
            catch (Exception ex)
                {
                return StatusCode(500, ApiResponse<AnagraficaContatto>.InternalError($"Errore: {ex.Message}"));
                }
            }


        // edit
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<AnagraficaContatto>>> Update(int id, [FromBody] AnagraficaContatto entity)
            {
            try
                {
                if (entity == null)
                    return BadRequest(ApiResponse<AnagraficaContatto>.BadRequest("Dati contatto null."));

                var existing = await _contattoService.GetByIdAsync(id);
                if (existing.Data == null)
                    return NotFound(ApiResponse<AnagraficaContatto>.NotFound("Contatto non trovato."));

                var fullErrorList = await _contattoService.CheckUpdateAsync(entity, excludeId: id);

                if (fullErrorList.Any())
                    return BadRequest(ApiResponse<AnagraficaContatto>.BadRequest(fullErrorList));

                var result = await _contattoService.UpdateAsync(entity);
                
                if (result.Errors.Any())
                    return BadRequest(ApiResponse<AnagraficaContatto>.BadRequest(result.Errors));

                // check if no data
                if (result.Data == null)
                    return StatusCode(500, ApiResponse<AnagraficaContatto>.InternalError("Errore interno: il contatto non è stato aggiornato."));


                return Ok(ApiResponse<AnagraficaContatto>.Ok(result.Data, "Contatto aggiornato con successo!"));
                }
            catch (Exception ex)
                {
                return StatusCode(500, ApiResponse<AnagraficaContatto>.InternalError($"Errore: {ex.Message}"));
                }
            }


        // delete
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
            {
            try
                {
                var existing = await _contattoService.GetByIdAsync(id);

                if (existing.Data == null)
                    {
                    return NotFound(ApiResponse.NotFound("Contatto non trovato."));
                    }

                var deleted = await _contattoService.DeleteAsync(id);
                if (deleted.Errors.Any())
                    {
                    return StatusCode(500, ApiResponse.BadRequest(deleted.Errors));
                    }

                return Ok(ApiResponse.Ok("Contatto eliminato con successo!"));
                }
            catch (Exception ex)
                {
                return StatusCode(500, ApiResponse.InternalError($"Errore: {ex.Message}"));
                }
            }

        [HttpPost("{id}/convert")]
        public async Task<ActionResult<ApiResponse<bool>>> Convert(int id)
        {
            try
            {
                var existing = await _contattoService.GetByIdAsync(id);

                if (existing.Data == null)
                {
                    return NotFound(ApiResponse<bool>.NotFound("Contatto non trovato."));
                }

                var converted = await _contattoService.ConvertContattoAsync(id);
                if (!converted)
                {
                    return BadRequest(ApiResponse<bool>.BadRequest(
                        "Impossibile convertire: un cliente con questo ID esiste già."));
                }

                return Ok(ApiResponse<bool>.Ok(true));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.InternalError($"Errore: {ex.Message}"));
            }
        }

    }
}