using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

namespace ProgettoAnagrafiche.Models;

[Table("AnagraficaCliente")]
[Index("CodiceCliente", Name = "UK_CodiceCliente", IsUnique = true)]
public partial class AnagraficaCliente : BaseEntity
    {
    [Key]
    public int AnagraficaId { get; set; }

    public int CodiceCliente { get; set; }

    [Column(TypeName = "datetime")]
    public required DateTime DataRegistrazione { get; set; }

    [StringLength(100)]
    public string? Note { get; set; }

    [ForeignKey("AnagraficaId")]
    [InverseProperty("AnagraficaCliente")]
    public virtual Anagrafiche Anagrafica { get; set; } = null!;


    public override List<string> Validate()
    {
        List<string> errors = new List<string>();

        // Anagrafica null, exit right away
        if (Anagrafica == null)
        {
            errors.Add("Anagrafica non trovata.");
            return errors;
        }

        // Validate

        // Delegate to Anagrafica's Validation (email, p fisica etc)
        // add any errors to the list
        Anagrafica.Validate().ForEach(e => errors.Add(e));

        if (DataRegistrazione == default)
        {
            errors.Add("La Data di Registrazione è obbligatoria.");
        }
        else
        {
            if (DataRegistrazione < new DateTime(1900, 1, 1))
            {
                errors.Add("La Data di Registrazione non può essere precedente al 1 gennaio 1900.");
            }
            if (DataRegistrazione > DateTime.UtcNow)
            {
                errors.Add("La Data di Registrazione non può essere nel futuro.");
            }
        }

        return errors;
    }

    public override void Normalize()
    {
        // Deleg. to anag
        Anagrafica?.Normalize();
        // trim note
        Note = string.IsNullOrWhiteSpace(Note) ? null : Note.Trim();
        }
}
