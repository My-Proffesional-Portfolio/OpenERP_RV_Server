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
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CorporateOffice> CorporateOffices { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<SalesConcept> SalesConcepts { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseSqlServer("Server=open-erp.database.windows.net;Database=OpenERP_RV;User Id=open-erp-admin;password=op3n3rp-070421;Trusted_Connection=False;MultipleActiveResultSets=true;");
                optionsBuilder.UseSqlServer("Server=DESKTOP-VL2FT7Q\\SQLEXPRESS;Database=OpenERP_RV;Trusted_Connection=True;");
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

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CompanyName).IsRequired();

                entity.Property(e => e.ContactName).IsRequired();

                entity.Property(e => e.FiscalIdentifier)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.BusinessCategory)
                    .WithMany(p => p.Clients)
                    .HasForeignKey(d => d.BusinessCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Clients__Busines__3B75D760");

                entity.HasOne(d => d.CorporateOffice)
                    .WithMany(p => p.Clients)
                    .HasForeignKey(d => d.CorporateOfficeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Clients__Corpora__3A81B327");
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
                    .HasConstraintName("FK__Companies__Busin__2A4B4B5E");

                entity.HasOne(d => d.CorporateOffice)
                    .WithMany(p => p.Companies)
                    .HasForeignKey(d => d.CorporateOfficeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Companies__Corpo__276EDEB3");
            });

            modelBuilder.Entity<CorporateOffice>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).IsRequired();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BarCode).IsRequired();

                entity.Property(e => e.Cost).HasColumnType("money");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.ProductName).IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.CorporateOffice)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CorporateOfficeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Products__Corpor__4E88ABD4");
            });

            modelBuilder.Entity<SalesConcept>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Cost).HasColumnType("money");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.ServiceName).IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.CorporateOffice)
                    .WithMany(p => p.SalesConcepts)
                    .HasForeignKey(d => d.CorporateOfficeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SalesConc__Corpo__52593CB8");
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
