using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.BookAggregate;

public class BookAuthor 
{
    public Guid BookId { get; private set; }
    public Guid AuthorId { get; private set; }

    // Navigation
    public Book Book { get; private set; } = null!;
    public Author Author { get; private set; } = null!;
    private BookAuthor() { } 

    public BookAuthor(Guid bookId, Guid authorId)
    {
        BookId = bookId;
        AuthorId = authorId;
    }
}