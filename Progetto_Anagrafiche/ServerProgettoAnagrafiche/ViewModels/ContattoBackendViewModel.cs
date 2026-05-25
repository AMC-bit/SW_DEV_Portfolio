using BlazorServerProgettoAnagrafiche.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgettoAnagrafiche.Models.Enums;

namespace BlazorServerProgettoAnagrafiche.ViewModels
{
    public class ContattoBackendViewModel
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

        // from AnagraficaContatto
        // provenienza will never be null, because it's required in the db

        // it's an enum now
        public required Provenienza Provenienza { get; set; }
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
        public ContattoBackendViewModel Clone()
        {
            return new ContattoBackendViewModel
            {
                Id = Id,
                Nome = Nome,
                Cognome = Cognome,
                Telefono = Telefono,
                Email = Email,
                CodiceFiscale = CodiceFiscale,
                PartitaIva = PartitaIva,
                IsPersonaFisica = IsPersonaFisica,
                RagioneSociale = RagioneSociale,
                Provenienza = Provenienza,
                Note = Note

            };
        }

        //Update the current instance with values from another instance / mutate the current instance of the object instead of creating a new one

        public void UpdateFrom(ContattoBackendViewModel other)
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
            this.Provenienza = other.Provenienza;
        }

        // Convert to entity, then validate from entity and return errors as a list of string
        public List<string> Validate()
        {
            var entity = this.ContattoToEntity();
            return entity.Validate();
        }

    }
}
