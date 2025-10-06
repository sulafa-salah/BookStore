using Catalog.Domain.BookAggregate;
using Catalog.Domain.CategoryAggreate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Persistence.Configuration;

    public  class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> b)
        {
            b.ToTable("Books");
            b.HasKey(x => x.Id);

            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description).IsRequired().HasMaxLength(2000);
            b.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired();
            b.Property(x => x.StockQuantity).IsRequired().HasDefaultValue(0);
        b.Property(x => x.IsPublished).IsRequired().HasDefaultValue(false);
        b.Property(x => x.Isbn).HasColumnName("ISBN").HasMaxLength(13).IsRequired();

        b.HasIndex(x => x.Isbn).IsUnique();                        // unique ISBN
        b.HasIndex(x => x.Title);                                  // search-by-title

        b.ToTable(tb =>
        {
            tb.HasCheckConstraint("CK_Books_Price_NonNegative", "[Price] >= 0");
            tb.HasCheckConstraint("CK_Books_Stock_NonNegative", "[StockQuantity] >= 0");
        });


        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt);

        b.Property(x => x.CategoryId).IsRequired();
            b.HasIndex(x => x.CategoryId);
            b.HasOne<Category>()
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            
         
        }
    }