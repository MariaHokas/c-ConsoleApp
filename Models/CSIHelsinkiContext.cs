using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ConsoleAppCSI.Models
{
    public partial class CSIHelsinkiContext : DbContext
    {
        public CSIHelsinkiContext()
        {
        }

        public CSIHelsinkiContext(DbContextOptions<CSIHelsinkiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Asiakas> Asiakas { get; set; }
        public virtual DbSet<Lasku> Lasku { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-35CADGH\\SQLEMA;Database=CSIHelsinki;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asiakas>(entity =>
            {
                entity.HasKey(e => e.AsiakasNro)
                    .HasName("PK__Tiedot__3214EC07C6017CCB");

                entity.Property(e => e.AsiakasNro)
                    .HasColumnName("Asiakas_nro")
                    .ValueGeneratedNever();

                entity.Property(e => e.Etunimi)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.Property(e => e.Osoite)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.Property(e => e.Postitoimipaikka)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Sukunimi)
                    .HasMaxLength(50)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Lasku>(entity =>
            {
                entity.HasKey(e => e.IdLasku)
                    .HasName("PK_Lasku_1");

                entity.Property(e => e.IdLasku).HasColumnName("idLasku");

                entity.Property(e => e.AsiakasNro).HasColumnName("Asiakas_nro");

                entity.Property(e => e.LaskuNro).HasColumnName("lasku_nro");

                entity.Property(e => e.Selite)
                    .HasColumnName("selite")
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.Property(e => e.Summa)
                    .HasColumnName("summa")
                    .HasColumnType("decimal(18, 0)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
