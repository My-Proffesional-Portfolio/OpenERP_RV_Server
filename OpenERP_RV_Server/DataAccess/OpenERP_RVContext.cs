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
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<ExpenseItem> ExpenseItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<SalesConcept> SalesConcepts { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=open-rv-erp.database.windows.net;Database=OpenERP_RV;User Id=open-erp-admin;password=op3n3rp-070421;Trusted_Connection=False;MultipleActiveResultSets=true;");
                //optionsBuilder.UseSqlServer("Server=DESKTOP-VL2FT7Q\\SQLEXPRESS;Database=OpenERP_RV;Trusted_Connection=True;");
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
                    .HasConstraintName("FK__Clients__Busines__693CA210");

                entity.HasOne(d => d.CorporateOffice)
                    .WithMany(p => p.Clients)
                    .HasForeignKey(d => d.CorporateOfficeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Clients__Corpora__68487DD7");
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

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AnotherTaxes).HasColumnType("money");

                entity.Property(e => e.Cfdiuse)
                    .HasMaxLength(128)
                    .HasColumnName("CFDIUse");

                entity.Property(e => e.Cfdiversion)
                    .HasMaxLength(128)
                    .HasColumnName("CFDIVersion");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.ExchangeRate)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ExpenseDate).HasColumnType("datetime");

                entity.Property(e => e.Number).IsRequired();

                entity.Property(e => e.PaymentMethod).HasMaxLength(128);

                entity.Property(e => e.PaymentTerm).HasMaxLength(128);

                entity.Property(e => e.ReceiverRfc).HasColumnName("ReceiverRFC");

                entity.Property(e => e.Subtotal).HasColumnType("money");

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.SupplierRfc).HasColumnName("SupplierRFC");

                entity.Property(e => e.Tax)
                    .HasColumnType("money")
                    .HasColumnName("TAX");

                entity.Property(e => e.Total).HasColumnType("money");

                entity.Property(e => e.Uuid).HasColumnName("UUID");

                entity.Property(e => e.Xml).HasColumnName("XML");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Expenses)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Expenses__Compan__1BC821DD");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Expenses)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Expenses__Suppli__1AD3FDA4");
            });

            modelBuilder.Entity<ExpenseItem>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Discount).HasColumnType("money");

                entity.Property(e => e.FullFilled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Importe).HasColumnType("money");

                entity.Property(e => e.Quantity).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.TotalTaxes).HasColumnType("money");

                entity.Property(e => e.Unidad).HasMaxLength(128);

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.Expense)
                    .WithMany(p => p.ExpenseItems)
                    .HasForeignKey(d => d.ExpenseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExpenseIt__Expen__1DB06A4F");
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
                    .HasConstraintName("FK__Products__Corpor__70DDC3D8");
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
                    .HasConstraintName("FK__SalesConc__Corpo__74AE54BC");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CompanyName).IsRequired();

                entity.Property(e => e.ContactName).IsRequired();

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.Phone).IsRequired();

                entity.Property(e => e.Rfc)
                    .IsRequired()
                    .HasColumnName("RFC");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Suppliers__Compa__19DFD96B");
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
