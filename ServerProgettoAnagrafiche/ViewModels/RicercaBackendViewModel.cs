using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorServerProgettoAnagrafiche.ViewModels
{
    public class RicercaBackendViewModel
    {

        // from Anagrafiche
        public string? Tipo { get; set; } // "Cliente" or "Contatto"
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
        // nullable here because of joined dto
        public string? Provenienza { get; set; }
        public string? NoteContatto { get; set; }

        // from AnagraficaCliente
        public int? CodiceCliente { get; set; }
        public DateTime? DataRegistrazione { get; set; } = DateTime.UtcNow;
        public string? NoteCliente { get; set; }

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

        // switch notes based on type
        public string DisplayNote
        {
            get
            {
                if (Tipo == "Cliente")
                    return NoteCliente ?? string.Empty;
                else if (Tipo == "Contatto")
                    return NoteContatto ?? string.Empty;
                else
                    return string.Empty;
            }
        }
    }
}
