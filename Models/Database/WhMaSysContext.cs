using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WhMaSysApi.Models.Database
{
    public partial class WhMaSysContext : DbContext
    {
        public WhMaSysContext()
        {
        }

        public WhMaSysContext(DbContextOptions<WhMaSysContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BatteryAfterSale> BatteryAfterSales { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<NewProduct> NewProducts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<PartAfterSale> PartAfterSales { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCate> ProductCates { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<SupplierCategory> SupplierCategories { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=WhMaSys;Integrated Security=SSPI");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_PRC_CI_AS");

            modelBuilder.Entity<BatteryAfterSale>(entity =>
            {
                entity.ToTable("BatteryAfterSale");

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ItemName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ItemPrice).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Remark).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.CustomerAddress)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CustomerPhone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.DateAdded).HasColumnType("date");
            });

            modelBuilder.Entity<NewProduct>(entity =>
            {
                entity.ToTable("NewProduct");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Categoryid)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.NewProductPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductName).HasMaxLength(255);

                entity.Property(e => e.ProductPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Supplierid)
                    .HasMaxLength(10)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.CustomerName).HasMaxLength(50);

                entity.Property(e => e.ItemName).HasMaxLength(50);

                entity.Property(e => e.ItemPrice).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ItemQuantity).HasMaxLength(50);

                entity.Property(e => e.Remark).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.Property(e => e.UserName).HasMaxLength(50);
            });

            modelBuilder.Entity<PartAfterSale>(entity =>
            {
                entity.ToTable("PartAfterSale");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ItemName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ItemPrice).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ItemQuantity)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Remark).HasMaxLength(100);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Categoryid).HasMaxLength(50);

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ProductPrice).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ProductStock).IsRequired();

                entity.Property(e => e.Supplierid).HasMaxLength(50);
            });

            modelBuilder.Entity<ProductCate>(entity =>
            {
                entity.ToTable("ProductCate");

                entity.Property(e => e.CateName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Cateid)
                    .HasMaxLength(10)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Supplier");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.SupId)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.SupplierAddress)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierPhone)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SupplierCategory>(entity =>
            {
                entity.ToTable("SupplierCategory");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SupplierName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Address).HasColumnName("address");

                entity.Property(e => e.Birthday)
                    .HasColumnType("date")
                    .HasColumnName("birthday");

                entity.Property(e => e.Education).HasMaxLength(50);

                entity.Property(e => e.Idnumber)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("IDnumber");

                entity.Property(e => e.Nation)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Role).HasMaxLength(50);

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Sex)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
