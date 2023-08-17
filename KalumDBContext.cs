using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KalumManagement.Entities;
using Microsoft.EntityFrameworkCore;

namespace KalumManagement
{
    public class KalumDBContext : DbContext
    {
        public KalumDBContext(DbContextOptions options) 
            : base(options)
        {
        }
        public DbSet<CarreraTecnica> CarrerasTecnicas {get; set;}
        public DbSet<Jornada> Jornadas {get; set;}
        public DbSet<ExamenAdmision> ExamenesAdmision { get; set; }
        public DbSet<Aspirante> Aspirantes { get; set; }
        public DbSet<ResultadoExamenAdmision> ResultadosExamenAdmision { get; set; }
        public DbSet<InscripcionPago> InscripcionPagos { get; set; }
        public DbSet<InversionCarreraTecnica> InversionCarreraTecnicas { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<CuentaCobrar> CuentasCobrar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarreraTecnica>().ToTable("carreratecnica").HasKey(ct => new {ct.CarreraId});
            modelBuilder.Entity<Jornada>().ToTable("jornada").HasKey(j => new {j.JornadaId});
            modelBuilder.Entity<ExamenAdmision>().ToTable("examenadmision").HasKey(ea => new {ea.ExamenId});
            modelBuilder.Entity<Aspirante>().ToTable("aspirante").HasKey(a => new {a.NoExpediente});

            modelBuilder.Entity<Aspirante>()
                .HasOne<CarreraTecnica>(a => a.CarrerasTecnica)
                .WithMany(ct => ct.Aspirantes)
                .HasForeignKey(a => a.CarreraId);

            modelBuilder.Entity<Aspirante>()
                .HasOne<Jornada>(a => a.Jornada)
                .WithMany(j => j.Aspirantes)
                .HasForeignKey( j => j.JornadaId); 

            modelBuilder.Entity<Aspirante>()
                .HasOne<ExamenAdmision>(a => a.ExamenAdmision)
                .WithMany(e => e.Aspirante)
                .HasForeignKey( e => e.ExamenId); 

            modelBuilder.Entity<ResultadoExamenAdmision>().ToTable("resultadoexamenadmision").HasKey(rea => new {rea.NoExpediente, rea.Anio});
            modelBuilder.Entity<InscripcionPago>().ToTable("inscripcionpago").HasKey(ip => new {ip.BoletaPago, ip.NoExpediente, ip.Anio});
            modelBuilder.Entity<InversionCarreraTecnica>().ToTable("inversioncarreratecnica").HasKey(ict => new {ict.InversionId});
            
            modelBuilder.Entity<InversionCarreraTecnica>()
                .HasOne<CarreraTecnica>(ict => ict.CarrerasTecnicas)
                .WithMany(ct => ct.InversionCarreraTecnicas)
                .HasForeignKey(ict => ict.CarreraId);

            modelBuilder.Entity<Cargo>().ToTable("cargo").HasKey(c => new {c.CargoId});
            modelBuilder.Entity<CuentaCobrar>().ToTable("cuentaporcobrar").HasKey(cc => new {cc.Carne});
        }
    }
}