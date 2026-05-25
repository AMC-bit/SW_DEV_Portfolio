using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProgettoAnagrafiche.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnagraficaCliente> AnagraficaClienti { get; set; }

    public virtual DbSet<AnagraficaContatto> AnagraficaContatti { get; set; }

    public virtual DbSet<Anagrafiche> Anagrafiche { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnagraficaCliente>(entity =>
        {
            entity.HasKey(e => e.AnagraficaId).HasName("PK__Anagrafi__80B7C3F2B25E7E5C");

            entity.Property(e => e.AnagraficaId).ValueGeneratedNever();
            entity.Property(e => e.CodiceCliente).ValueGeneratedOnAdd();
            entity.Property(e => e.DataRegistrazione).HasDefaultValue(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            entity.HasOne(d => d.Anagrafica)
                .WithOne(p => p.AnagraficaCliente)
                .HasConstraintName("FK_AnagraficaIdCliente")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AnagraficaContatto>(entity =>
        {
            entity.HasKey(e => e.AnagraficaId).HasName("PK__Anagrafi__80B7C3F22B7BB5C3");

            entity.Property(e => e.AnagraficaId).ValueGeneratedNever();

            entity.HasOne(d => d.Anagrafica)
                .WithOne(p => p.AnagraficaContatto)
                .HasConstraintName("FK_AnagraficaIdContatto")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Anagrafiche>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Anagrafi__3214EC071A87A9FC");

            entity.HasIndex(e => e.CodiceFiscale, "UK_CodiceFiscale")
                .IsUnique()
                .HasFilter("([CodiceFiscale] IS NOT NULL)");

            entity.HasIndex(e => e.PartitaIva, "UK_PartitaIVA")
                .IsUnique()
                .HasFilter("([PartitaIVA] IS NOT NULL)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
