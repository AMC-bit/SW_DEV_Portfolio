using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace ProgettoAnagrafiche.Models.DTO
{
    public class ClienteDTO
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
        public required DateTime DataRegistrazione { get; set; } = DateTime.UtcNow;
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

    }
}

