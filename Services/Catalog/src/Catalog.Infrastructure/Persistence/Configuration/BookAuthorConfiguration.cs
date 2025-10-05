using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.BookAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Persistence.Configuration;

    public  class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
    {
        public void Configure(EntityTypeBuilder<BookAuthor> b)
        {
            b.ToTable("BookAuthors");
            b.HasKey(x => new { x.BookId, x.AuthorId });
        

        b.HasOne(b => b.Book)
             .WithMany(b => b.BookAuthors)
             .HasForeignKey(x => x.BookId)
          .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(a => a.Author)
        .WithMany(a => a.BookAuthors)
             .HasForeignKey(x => x.AuthorId)
             .OnDelete(DeleteBehavior.Cascade);

       
    }
    }