using ProgettoAnagrafiche.Models;
using ProgettoAnagrafiche.Models.DTO;

namespace BlazorServerProgettoAnagrafiche.Shared
{
    public static class ClienteMapper
    {
        // Dto to Entity
        public static AnagraficaCliente ClienteToEntity(this ClienteDTO dto)
        {
            return new AnagraficaCliente
            {
                AnagraficaId = dto.Id,
                DataRegistrazione = dto.DataRegistrazione,
                Note = dto.Note,
                Anagrafica = new Anagrafiche
                {
                    Id = dto.Id,
                    IsPersonaFisica = dto.IsPersonaFisica,
                    Nome = dto.Nome,
                    Cognome = dto.Cognome,
                    RagioneSociale = dto.RagioneSociale,
                    Email = dto.Email,
                    Telefono = dto.Telefono,
                    CodiceFiscale = dto.CodiceFiscale,
                    PartitaIva = dto.PartitaIva
                }
            };
        }

        // Entity to DTO
        public static ClienteDTO ClienteToDTO(this AnagraficaCliente entity)
        {
            return new ClienteDTO
            {

                // removed: codcliente, because it's auto generated

                Id = entity.AnagraficaId,
                IsPersonaFisica = entity.Anagrafica.IsPersonaFisica,
                Nome = entity.Anagrafica.Nome,
                Cognome = entity.Anagrafica.Cognome,
                RagioneSociale = entity.Anagrafica.RagioneSociale,
                Email = entity.Anagrafica.Email,
                Telefono = entity.Anagrafica.Telefono,
                CodiceFiscale = entity.Anagrafica.CodiceFiscale,
                PartitaIva = entity.Anagrafica.PartitaIva,
                DataRegistrazione = entity.DataRegistrazione,
                Note = entity.Note
            };
        }
    }
}
