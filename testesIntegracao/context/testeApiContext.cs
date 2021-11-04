using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace testesIntegracao.context
{
    public partial class testeApiContext : DbContext
    {
        public testeApiContext()
        {
        }

        public testeApiContext(DbContextOptions<testeApiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Curso> Cursos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=PC-RAMON;Database=testeApi;Trusted_Connection=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<Curso>(entity =>
            {
                entity.HasKey(e => e.IdCurso)
                    .HasName("PK__cursos__8551ED05EBFE4D76");

                entity.ToTable("cursos");

                entity.Property(e => e.IdCurso).HasColumnName("idCurso");

                entity.Property(e => e.DescricaoCurso)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("descricaoCurso");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
