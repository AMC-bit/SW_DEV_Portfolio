using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Net.Mail;
using System.Text.Json.Serialization;

namespace ProgettoAnagrafiche.Models;

[Table("Anagrafiche")]

// Anagrafiche now inherits from BaseEntity, must implement Validate and Normalize
public partial class Anagrafiche : BaseEntity
    {
    [Key]
    public int Id { get; set; }

    [StringLength(30)]
    public string? Nome { get; set; }

    [StringLength(30)]
    public string? Cognome { get; set; }

    [StringLength(30)]
    public string? Telefono { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(25)]
    public string? CodiceFiscale { get; set; }

    [Column("PartitaIVA")]
    [StringLength(25)]
    public string? PartitaIva { get; set; }

    public bool? IsPersonaFisica { get; set; }

    [StringLength(100)]
    public string? RagioneSociale { get; set; }

    [InverseProperty("Anagrafica")]
    [JsonIgnore]
    public virtual AnagraficaCliente? AnagraficaCliente { get; set; }

    [InverseProperty("Anagrafica")]
    [JsonIgnore]
    public virtual AnagraficaContatto? AnagraficaContatto { get; set; }



    // new: Validate

    // standard email validation w/ System.Net.Mail
    public static bool IsValidEmail(string email)
        {
        try
            {
            var addr = new MailAddress(email);
            return addr.Address == email;
            }
        catch
            {
            return false;
            }
        }

    public override List<string> Validate()
        {
        List<string> errors = new List<string>();

        // IsPersonaFisica is required
        if (IsPersonaFisica == null)
            {
            errors.Add("Specificare se persona fisica!");
            }

        // If true, then nome and cognome are required
        if (IsPersonaFisica == true)
            {
            if (string.IsNullOrWhiteSpace(Nome))
                {
                errors.Add("Il Nome è obbligatorio per le persone fisiche.");
                }
            if (string.IsNullOrWhiteSpace(Cognome))
                {
                errors.Add("Il Cognome è obbligatorio per le persone fisiche.");
                }
            }

        // If false, then ragione sociale is required, could also write as && both conditions
        if (IsPersonaFisica == false)
            {
            if (string.IsNullOrWhiteSpace(RagioneSociale))
                {
                errors.Add("La Ragione Sociale è obbligatoria per le persone giuridiche.");
                }
            }

        // If email is provided, validate format
        // Validate Email format
        if (!string.IsNullOrWhiteSpace(Email) && !IsValidEmail(Email))
            {
            errors.Add("Email non valida.");
            }

        return errors;
        }

    // input cleanup
    // make sure in addition that partitaiva and codice fiscale are never just whitespace, but null instead, to avoid unique index issues

    // OLD implementation: ignores empty, ignores full of white spaces, doesn't convert to null,

    /*
    public override void Normalize()
    {
        if (!string.IsNullOrEmpty(CodiceFiscale))
            CodiceFiscale = CodiceFiscale.ToUpper().Trim();
        if (!string.IsNullOrEmpty(PartitaIva))
            PartitaIva = PartitaIva.ToUpper().Trim();
        if (!string.IsNullOrEmpty(Nome))
            Nome = Nome.Trim();
        if (!string.IsNullOrEmpty(Cognome))
            Cognome = Cognome.Trim();
        if (!string.IsNullOrEmpty(RagioneSociale))
            RagioneSociale = RagioneSociale.Trim();
        if (!string.IsNullOrEmpty(Email))
            Email = Email.Trim();
        if (!string.IsNullOrEmpty(Telefono))
            Telefono = Telefono.Trim();
    }
    */

    // NEW: null stays null, empty or whitespace becomes null
    public override void Normalize()
        {
        // "is codicefiscale null, empty, or full of spaces? If so set it to null. Otherwise if it has values, trim and uppercase it."
        CodiceFiscale = string.IsNullOrWhiteSpace(CodiceFiscale) ? null : CodiceFiscale.Trim().ToUpper();

        PartitaIva = string.IsNullOrWhiteSpace(PartitaIva) ? null : PartitaIva.Trim().ToUpper();

        Nome = string.IsNullOrWhiteSpace(Nome) ? null : Nome.Trim();

        Cognome = string.IsNullOrWhiteSpace(Cognome) ? null : Cognome.Trim();

        RagioneSociale = string.IsNullOrWhiteSpace(RagioneSociale) ? null : RagioneSociale.Trim();

        Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim();

        Telefono = string.IsNullOrWhiteSpace(Telefono) ? null : Telefono.Trim();
        }

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
