using BlazorServerProgettoAnagrafiche.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorServerProgettoAnagrafiche.ViewModels
{
    public class ClienteBackendViewModel
    {

        // from Anagrafiche
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Cognome { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? CodiceFiscale { get; set; }
        public string? PartitaIva { get; set; }
        public bool? IsPersonaFisica { get; set; }
        public string? RagioneSociale { get; set; }

        // from AnagraficaCliente
        public int CodiceCliente { get; set; }
        public DateTime DataRegistrazione { get; set; } = DateTime.UtcNow;
        public string? Note { get; set; }


        // custom display name
        public string DisplayName
        {
            get
            {
                if (IsPersonaFisica ?? false)
                    return $"{Nome} {Cognome}".Trim();
                else
                    return RagioneSociale ?? string.Empty;
            }
        }

        // I don't want a shallow copy (memberwise clone)
        public ClienteBackendViewModel Clone()
        {
            return new ClienteBackendViewModel
            {
                Id = this.Id,
                Nome = this.Nome,
                Cognome = this.Cognome,
                Telefono = this.Telefono,
                Email = this.Email,
                CodiceFiscale = this.CodiceFiscale,
                PartitaIva = this.PartitaIva,
                IsPersonaFisica = this.IsPersonaFisica,
                RagioneSociale = this.RagioneSociale,
                CodiceCliente = this.CodiceCliente,
                DataRegistrazione = this.DataRegistrazione,
                Note = this.Note

            };
        }


        //Update the current instance with values from another instance / mutate the current instance of the object instead of creating a new one

        public void UpdateFrom(ClienteBackendViewModel other)
        {
            this.Id = other.Id;
            this.Nome = other.Nome;
            this.Cognome = other.Cognome;
            this.Telefono = other.Telefono;
            this.Email = other.Email;
            this.CodiceFiscale = other.CodiceFiscale;
            this.PartitaIva = other.PartitaIva;
            this.IsPersonaFisica = other.IsPersonaFisica;
            this.RagioneSociale = other.RagioneSociale;
            this.Note = other.Note;
            this.DataRegistrazione = other.DataRegistrazione;
        }




        // Validation method
        // auto applies validation in original entities!
        public List<string> Validate()
        {
            var entity = this.ClienteToEntity();
            return entity.Validate();
        }


    }
}
