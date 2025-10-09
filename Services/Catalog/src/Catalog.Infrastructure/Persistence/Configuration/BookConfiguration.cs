using Catalog.Domain.BookAggregate;
using Catalog.Domain.CategoryAggreate;
using Catalog.Domain.Common.ValueObjects;
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

        b.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(250);

        b.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(4000);

        // ---- Money  ----
        b.OwnsOne(x => x.Price, money =>
        {
            money.Property(m => m.Amount)
                 .HasColumnName("PriceAmount")
                 .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                 .HasColumnName("PriceCurrency")
                 .HasMaxLength(3)
                 .IsRequired();

            money.WithOwner();
        });

        // ---- SKU (value object) ----
        b.Property(x => x.Sku)
            .HasConversion(
                sku => sku.Value,
                value => Sku.Create(value).Value)
            .HasMaxLength(32)
            .IsRequired();

        // ---- ISBN (value object) ----
        b.Property(x => x.Isbn)
            .HasConversion(
                isbn => isbn.Value,
                value => ISBN.Create(value).Value)
            .HasColumnName("ISBN")
            .HasMaxLength(13) 
            .IsRequired();

        b.Property(x => x.IsPublished)
            .IsRequired()
            .HasDefaultValue(false);

        b.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        b.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("NULL");

        b.Property(x => x.CategoryId)
            .IsRequired();

        b.HasOne<Category>()
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---- Indexes ----
        b.HasIndex(x => x.CategoryId);
        b.HasIndex(x => x.IsPublished);
        b.HasIndex(x => x.Title);
        b.HasIndex(x => x.Isbn).IsUnique();
        b.HasIndex(x => x.Sku).IsUnique();

        // ---- Check constraints ----
        b.ToTable(tb =>
        {
            tb.HasCheckConstraint("CK_Books_Price_NonNegative", "[PriceAmount] >= 0");
            
        });
    }
}