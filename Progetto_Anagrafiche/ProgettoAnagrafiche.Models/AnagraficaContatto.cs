using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using ProgettoAnagrafiche.Models.Enums;

namespace ProgettoAnagrafiche.Models;

[Table("AnagraficaContatto")]
public partial class AnagraficaContatto : BaseEntity
    {
    [Key]
    public int AnagraficaId { get; set; }

    [StringLength(100)]
    public string? Note { get; set; }


    // no str length needed
    // swap to an enum
    public required Provenienza Provenienza { get; set; }

    [ForeignKey("AnagraficaId")]
    [InverseProperty("AnagraficaContatto")]
    public virtual Anagrafiche Anagrafica { get; set; } = null!;

    //// keep temporarily, convert to enums later
    //private static readonly string[] AllowedProvenienzaValues = { "LinkedIn", "SitoWeb", "Pubblicita" };

 

    public override List<string> Validate()
    {
        List<string> errors = new List<string>();

        // does anagrafica even exist?
        if (Anagrafica == null)
        {
            errors.Add("Anagrafica non trovata.");
            return errors;
        }

        // Delegate to Anagrafica's Validation (email, p fisica etc)
        // add any errors to the list
        Anagrafica.Validate().ForEach(e => errors.Add(e));

        // Validate Provenienza: 0 is not a defined enum member and must be caught here
        if (!Enum.IsDefined(typeof(Provenienza), Provenienza))
        {
            errors.Add($"La provenienza è obbligatoria e deve essere una delle seguenti: {string.Join(", ", Enum.GetNames(typeof(Provenienza)))}.");
        }

        return errors;
    }

    public override void Normalize()
    {
        // Delegate to anagrafica
        Anagrafica?.Normalize();
        // likewise flip note to null if empty, null or full of white spaces, and trim if it's not null
        Note = string.IsNullOrWhiteSpace(Note) ? null : Note.Trim();
    }
}
