using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace OpenERP_RV_Server.DataAccess
{
    public partial class OpenERP_RVContext : DbContext
    {
        public OpenERP_RVContext()
        {
        }

        public OpenERP_RVContext(DbContextOptions<OpenERP_RVContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BusinessCategory> BusinessCategories { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CorporateOffice> CorporateOffices { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-ONROABL\\SQLEXPRESS02;Database=OpenERP_RV;Trusted_Connection=True;");
              //optionsBuilder.UseSqlServer("Server=open-erp.database.windows.net;Database=OpenERP_RV;User Id=open-erp-admin;password=op3n3rp-070421;Trusted_Connection=False;MultipleActiveResultSets=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");

            modelBuilder.Entity<BusinessCategory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).IsRequired();
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).IsRequired();

                entity.Property(e => e.CommercialName).IsRequired();

                entity.Property(e => e.FiscalIdentifier)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.LegalName).IsRequired();

                entity.Property(e => e.OfficeNumberId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Phone).HasMaxLength(70);

                entity.HasOne(d => d.BusinessCategory)
                    .WithMany(p => p.Companies)
                    .HasForeignKey(d => d.BusinessCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Companies__Busin__6383C8BA");

                entity.HasOne(d => d.CorporateOffice)
                    .WithMany(p => p.Companies)
                    .HasForeignKey(d => d.CorporateOfficeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Companies__Corpo__628FA481");
            });

            modelBuilder.Entity<CorporateOffice>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).IsRequired();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.HashedPassword).IsRequired();

                entity.Property(e => e.Salt).IsRequired();

                entity.Property(e => e.UserName).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
