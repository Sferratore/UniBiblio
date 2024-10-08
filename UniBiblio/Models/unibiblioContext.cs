﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace UniBiblio.Models
{
    public partial class UniBiblioContext : DbContext
    {
        public UniBiblioContext()
        {
        }

        public UniBiblioContext(DbContextOptions<UniBiblioContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Biblioteche> Biblioteches { get; set; } = null!;
        public virtual DbSet<Categorie> Categories { get; set; } = null!;
        public virtual DbSet<Libri> Libris { get; set; } = null!;
        public virtual DbSet<Prenotalibriview> Prenotalibriviews { get; set; } = null!;
        public virtual DbSet<PrenotazioniLibri> PrenotazioniLibris { get; set; } = null!;
        public virtual DbSet<PrenotazioniSale> PrenotazioniSales { get; set; } = null!;
        public virtual DbSet<Prenotazionieffettuatelibriview> Prenotazionieffettuatelibriviews { get; set; } = null!;
        public virtual DbSet<Prenotazionieffettuatesaleview> Prenotazionieffettuatesaleviews { get; set; } = null!;
        public virtual DbSet<SaleStudio> SaleStudios { get; set; } = null!;
        public virtual DbSet<StatistichePrenotazioniMensili> StatistichePrenotazioniMensilis { get; set; } = null!;
        public virtual DbSet<Utenti> Utentis { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;database=UniBiblio;user=root;password=DB09Gennaio", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.31-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Biblioteche>(entity =>
            {
                entity.HasKey(e => e.IdBiblioteca)
                    .HasName("PRIMARY");

                entity.ToTable("biblioteche");

                entity.Property(e => e.IdBiblioteca).HasColumnName("id_biblioteca");

                entity.Property(e => e.Indirizzo)
                    .HasMaxLength(255)
                    .HasColumnName("indirizzo");

                entity.Property(e => e.NomeBiblioteca)
                    .HasMaxLength(100)
                    .HasColumnName("nome_biblioteca");

                entity.Property(e => e.OrariApertura)
                    .HasMaxLength(50)
                    .HasColumnName("orari_apertura");

                entity.Property(e => e.Telefono)
                    .HasMaxLength(15)
                    .HasColumnName("telefono");
            });

            modelBuilder.Entity<Categorie>(entity =>
            {
                entity.HasKey(e => e.IdCategoria)
                    .HasName("PRIMARY");

                entity.ToTable("categorie");

                entity.HasIndex(e => e.NomeCategoria, "nome_categoria")
                    .IsUnique();

                entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");

                entity.Property(e => e.NomeCategoria)
                    .HasMaxLength(100)
                    .HasColumnName("nome_categoria");
            });

            modelBuilder.Entity<Libri>(entity =>
            {
                entity.HasKey(e => e.IdLibro)
                    .HasName("PRIMARY");

                entity.ToTable("libri");

                entity.HasIndex(e => e.IdBiblioteca, "fk_libri_biblioteca");

                entity.HasIndex(e => e.Isbn, "isbn")
                    .IsUnique();

                entity.Property(e => e.IdLibro).HasColumnName("id_libro");

                entity.Property(e => e.AnnoPubblicazione).HasColumnName("anno_pubblicazione");

                entity.Property(e => e.Autore)
                    .HasMaxLength(255)
                    .HasColumnName("autore");

                entity.Property(e => e.Categoria)
                    .HasMaxLength(100)
                    .HasColumnName("categoria");

                entity.Property(e => e.IdBiblioteca).HasColumnName("id_biblioteca");

                entity.Property(e => e.Isbn)
                    .HasMaxLength(13)
                    .HasColumnName("isbn");

                entity.Property(e => e.QuantitaDisponibile)
                    .HasColumnName("quantita_disponibile")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Titolo)
                    .HasMaxLength(255)
                    .HasColumnName("titolo");

                entity.HasOne(d => d.IdBibliotecaNavigation)
                    .WithMany(p => p.Libris)
                    .HasForeignKey(d => d.IdBiblioteca)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_libri_biblioteca");

                entity.HasMany(d => d.IdCategoria)
                    .WithMany(p => p.IdLibros)
                    .UsingEntity<Dictionary<string, object>>(
                        "LibriCategorie",
                        l => l.HasOne<Categorie>().WithMany().HasForeignKey("IdCategoria").HasConstraintName("fk_libri_categorie_categoria"),
                        r => r.HasOne<Libri>().WithMany().HasForeignKey("IdLibro").HasConstraintName("fk_libri_categorie_libro"),
                        j =>
                        {
                            j.HasKey("IdLibro", "IdCategoria").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                            j.ToTable("libri_categorie");

                            j.HasIndex(new[] { "IdCategoria" }, "fk_libri_categorie_categoria");

                            j.IndexerProperty<ulong>("IdLibro").HasColumnName("id_libro");

                            j.IndexerProperty<ulong>("IdCategoria").HasColumnName("id_categoria");
                        });
            });

            modelBuilder.Entity<Prenotalibriview>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("prenotalibriview");

                entity.Property(e => e.AnnoPubblicazione).HasColumnName("anno_pubblicazione");

                entity.Property(e => e.Autore)
                    .HasMaxLength(255)
                    .HasColumnName("autore");

                entity.Property(e => e.Biblioteca)
                    .HasMaxLength(100)
                    .HasColumnName("biblioteca");

                entity.Property(e => e.Categoria)
                    .HasMaxLength(100)
                    .HasColumnName("categoria");

                entity.Property(e => e.IdLibro).HasColumnName("id_libro");

                entity.Property(e => e.Isbn)
                    .HasMaxLength(13)
                    .HasColumnName("isbn");

                entity.Property(e => e.QuantitaDisponibile)
                    .HasColumnName("quantita_disponibile")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Titolo)
                    .HasMaxLength(255)
                    .HasColumnName("titolo");
            });

            modelBuilder.Entity<PrenotazioniLibri>(entity =>
            {
                entity.HasKey(e => e.IdPrenotazione)
                    .HasName("PRIMARY");

                entity.ToTable("prenotazioni_libri");

                entity.HasIndex(e => e.IdLibro, "fk_prenotazioni_libri_libro");

                entity.HasIndex(e => e.IdUtente, "fk_prenotazioni_libri_utente");

                entity.Property(e => e.IdPrenotazione).HasColumnName("id_prenotazione");

                entity.Property(e => e.DataPrenotazione).HasColumnName("data_prenotazione");

                entity.Property(e => e.DataRestituzione).HasColumnName("data_restituzione");

                entity.Property(e => e.DataRitiro).HasColumnName("data_ritiro");

                entity.Property(e => e.IdLibro).HasColumnName("id_libro");

                entity.Property(e => e.IdUtente).HasColumnName("id_utente");

                entity.Property(e => e.Stato)
                    .HasColumnType("enum('Prenotato','Ritirato','Restituito','Cancellato')")
                    .HasColumnName("stato");

                entity.HasOne(d => d.IdLibroNavigation)
                    .WithMany(p => p.PrenotazioniLibris)
                    .HasForeignKey(d => d.IdLibro)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_prenotazioni_libri_libro");

                entity.HasOne(d => d.IdUtenteNavigation)
                    .WithMany(p => p.PrenotazioniLibris)
                    .HasForeignKey(d => d.IdUtente)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_prenotazioni_libri_utente");
            });

            modelBuilder.Entity<PrenotazioniSale>(entity =>
            {
                entity.HasKey(e => e.IdPrenotazione)
                    .HasName("PRIMARY");

                entity.ToTable("prenotazioni_sale");

                entity.HasIndex(e => e.IdSala, "fk_prenotazioni_sale_sala");

                entity.HasIndex(e => e.IdUtente, "fk_prenotazioni_sale_utente");

                entity.Property(e => e.IdPrenotazione).HasColumnName("id_prenotazione");

                entity.Property(e => e.DataPrenotazione).HasColumnName("data_prenotazione");

                entity.Property(e => e.GiornoPrenotato).HasColumnName("giorno_prenotato");

                entity.Property(e => e.IdSala).HasColumnName("id_sala");

                entity.Property(e => e.IdUtente).HasColumnName("id_utente");

                entity.Property(e => e.OraFine)
                    .HasColumnType("time")
                    .HasColumnName("ora_fine");

                entity.Property(e => e.OraInizio)
                    .HasColumnType("time")
                    .HasColumnName("ora_inizio");

                entity.Property(e => e.Stato)
                    .HasColumnType("enum('Prenotato','Usufruito','Cancellato')")
                    .HasColumnName("stato");

                entity.HasOne(d => d.IdSalaNavigation)
                    .WithMany(p => p.PrenotazioniSales)
                    .HasForeignKey(d => d.IdSala)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_prenotazioni_sale_sala");

                entity.HasOne(d => d.IdUtenteNavigation)
                    .WithMany(p => p.PrenotazioniSales)
                    .HasForeignKey(d => d.IdUtente)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_prenotazioni_sale_utente");
            });

            modelBuilder.Entity<Prenotazionieffettuatelibriview>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("prenotazionieffettuatelibriview");

                entity.Property(e => e.DataPrenotazione).HasColumnName("data_prenotazione");

                entity.Property(e => e.DataRitiro).HasColumnName("data_ritiro");

                entity.Property(e => e.EmailUtente)
                    .HasMaxLength(150)
                    .HasColumnName("email_utente");

                entity.Property(e => e.IdLibro).HasColumnName("id_libro");

                entity.Property(e => e.IdPrenotazione).HasColumnName("id_prenotazione");

                entity.Property(e => e.IdUtente).HasColumnName("id_utente");

                entity.Property(e => e.Isbn)
                    .HasMaxLength(13)
                    .HasColumnName("isbn");

                entity.Property(e => e.NomeBiblioteca)
                    .HasMaxLength(100)
                    .HasColumnName("nome_biblioteca");

                entity.Property(e => e.Stato)
                    .HasColumnType("enum('Prenotato','Ritirato','Restituito','Cancellato')")
                    .HasColumnName("stato");

                entity.Property(e => e.TitoloLibro)
                    .HasMaxLength(255)
                    .HasColumnName("titolo_libro");
            });

            modelBuilder.Entity<Prenotazionieffettuatesaleview>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("prenotazionieffettuatesaleview");

                entity.Property(e => e.Biblioteca)
                    .HasMaxLength(100)
                    .HasColumnName("biblioteca");

                entity.Property(e => e.DataPrenotazione).HasColumnName("data_prenotazione");

                entity.Property(e => e.EmailUtente)
                    .HasMaxLength(150)
                    .HasColumnName("email_utente");

                entity.Property(e => e.GiornoPrenotato).HasColumnName("giorno_prenotato");

                entity.Property(e => e.IdPrenotazione).HasColumnName("id_prenotazione");

                entity.Property(e => e.IdUtente).HasColumnName("id_utente");

                entity.Property(e => e.NomeSala)
                    .HasMaxLength(100)
                    .HasColumnName("nome_sala");

                entity.Property(e => e.OraFine)
                    .HasColumnType("time")
                    .HasColumnName("ora_fine");

                entity.Property(e => e.OraInizio)
                    .HasColumnType("time")
                    .HasColumnName("ora_inizio");

                entity.Property(e => e.Stato)
                    .HasColumnType("enum('Prenotato','Usufruito','Cancellato')")
                    .HasColumnName("stato");
            });

            modelBuilder.Entity<SaleStudio>(entity =>
            {
                entity.HasKey(e => e.IdSala)
                    .HasName("PRIMARY");

                entity.ToTable("sale_studio");

                entity.HasIndex(e => e.IdBiblioteca, "fk_sala_biblioteca");

                entity.Property(e => e.IdSala).HasColumnName("id_sala");

                entity.Property(e => e.Capienza).HasColumnName("capienza");

                entity.Property(e => e.Disponibilita)
                    .HasColumnName("disponibilita")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.IdBiblioteca).HasColumnName("id_biblioteca");

                entity.Property(e => e.NomeSala)
                    .HasMaxLength(100)
                    .HasColumnName("nome_sala");

                entity.HasOne(d => d.IdBibliotecaNavigation)
                    .WithMany(p => p.SaleStudios)
                    .HasForeignKey(d => d.IdBiblioteca)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_sala_biblioteca");
            });

            modelBuilder.Entity<StatistichePrenotazioniMensili>(entity =>
            {
                entity.HasKey(e => e.IdStatistica)
                    .HasName("PRIMARY");

                entity.ToTable("statistiche_prenotazioni_mensili");

                entity.HasIndex(e => e.IdStatistica, "id_statistica")
                    .IsUnique();

                entity.Property(e => e.IdStatistica).HasColumnName("id_statistica");

                entity.Property(e => e.Anno).HasColumnName("anno");

                entity.Property(e => e.CreatoIl)
                    .HasColumnType("timestamp")
                    .HasColumnName("creato_il")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LibroPiuPrenotato)
                    .HasMaxLength(255)
                    .HasColumnName("libro_piu_prenotato");

                entity.Property(e => e.Mese).HasColumnName("mese");

                entity.Property(e => e.PrenotazioniLibriCancellati).HasColumnName("prenotazioni_libri_cancellati");

                entity.Property(e => e.PrenotazioniLibriPrenotati).HasColumnName("prenotazioni_libri_prenotati");

                entity.Property(e => e.PrenotazioniLibriRestituiti).HasColumnName("prenotazioni_libri_restituiti");

                entity.Property(e => e.PrenotazioniLibriRitirati).HasColumnName("prenotazioni_libri_ritirati");

                entity.Property(e => e.PrenotazioniSaleCancellate).HasColumnName("prenotazioni_sale_cancellate");

                entity.Property(e => e.PrenotazioniSalePrenotate).HasColumnName("prenotazioni_sale_prenotate");

                entity.Property(e => e.PrenotazioniSaleUsufruite).HasColumnName("prenotazioni_sale_usufruite");

                entity.Property(e => e.SalaPiuPrenotata)
                    .HasMaxLength(255)
                    .HasColumnName("sala_piu_prenotata");

                entity.Property(e => e.TotalePrenotazioniLibri).HasColumnName("totale_prenotazioni_libri");

                entity.Property(e => e.TotalePrenotazioniSale).HasColumnName("totale_prenotazioni_sale");
            });

            modelBuilder.Entity<Utenti>(entity =>
            {
                entity.HasKey(e => e.IdUtente)
                    .HasName("PRIMARY");

                entity.ToTable("utenti");

                entity.HasIndex(e => e.Email, "email")
                    .IsUnique();

                entity.Property(e => e.IdUtente).HasColumnName("id_utente");

                entity.Property(e => e.Cognome)
                    .HasMaxLength(100)
                    .HasColumnName("cognome");

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .HasColumnName("email");

                entity.Property(e => e.IsAmministratore).HasColumnName("is_amministratore");

                entity.Property(e => e.Nome)
                    .HasMaxLength(100)
                    .HasColumnName("nome");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .HasColumnName("password_hash");

                entity.Property(e => e.Telefono)
                    .HasMaxLength(15)
                    .HasColumnName("telefono");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
