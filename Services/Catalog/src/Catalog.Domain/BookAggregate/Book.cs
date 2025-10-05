using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.CategoryAggreate;
using Catalog.Domain.Common;

using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.BookAggregate;
    public class Book : AggregateRoot
    {
 

    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    public string Isbn { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsPublished { get; private set; }

   
    public Guid CategoryId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public ICollection<BookAuthor> BookAuthors { get; private set; } = new List<BookAuthor>();

    private Book() { } // EF

    public Book(
        string title,
        string description,
        decimal price,
        string isbn,
        Guid categoryId,
        IEnumerable<Guid>? authorIds = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Title = title;
        Description = description;
        Price = price;
        Isbn = isbn;
        CategoryId = categoryId;
        StockQuantity = 0;
        IsPublished = false;
        CreatedAt = DateTime.UtcNow;

        BookAuthors = (authorIds ?? Enumerable.Empty<Guid>())
       .Distinct()
       .Select(aid => new BookAuthor(Id, aid))
       .ToList();


    }
    public ErrorOr<Success> UpdateStock(int quantity)
    {
        if (quantity < 0) return BookErrors.StockCannotBeNegative;
        StockQuantity = quantity;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

  

    public ErrorOr<Success> ChangeCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty) return BookErrors.InvalidCategory;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

   
  
}
