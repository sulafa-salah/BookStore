using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.CategoryAggreate;
using Catalog.Domain.Common;
using Catalog.Domain.Common.ValueObjects;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.BookAggregate;
public class Book : AggregateRoot, IAuditable
{

    private readonly List<BookAuthor> _bookAuthors = new();

    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Money Price { get; private set; }
    public Sku Sku { get; private set; }
    public ISBN Isbn { get; private set; }
    public bool IsPublished { get; private set; }
    public Guid CategoryId { get; private set; }
    public IReadOnlyCollection<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private Book() { } // EF

    public  Book(ISBN isbn, Sku sku, Money price, string title, string description, Guid categoryId, Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
      
        Isbn = isbn;
        Sku = sku;
        Price = price;
        Title = title;
        Description = description;
        CategoryId = categoryId;
        IsPublished = false;
        CreatedAt = DateTime.UtcNow;
    }

   
    public static ErrorOr<Book> Create(string isbn, string sku, decimal amount, string currency,
        string title, string description, Guid categoryId)
    {
        var isbnResult = ISBN.Create(isbn);
        var skuResult = Sku.Create(sku);
        var moneyRes = Money.Create(amount, currency);

        if (isbnResult.IsError || skuResult.IsError || moneyRes.IsError)
            return Error.Validation("Book.Invalid", "Invalid data for Book.");

        return new Book(isbnResult.Value, skuResult.Value, moneyRes.Value, title, description, categoryId);
    }

    public void AddAuthor(Guid authorId)
    {
        if (_bookAuthors.Any(x => x.AuthorId == authorId)) return;
        _bookAuthors.Add(new BookAuthor(Id, authorId));
        UpdatedAt = DateTime.UtcNow;
    }

    public ErrorOr<Success> ChangeCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty) return BookErrors.InvalidCategory;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

    public ErrorOr<Success> ChangePrice(decimal newAmount, string currency)
    {
        var moneyRes = Money.Create(newAmount, currency);
        if (moneyRes.IsError) return moneyRes.FirstError;
        Price = moneyRes.Value;
        UpdatedAt = DateTime.UtcNow;
       
        return Result.Success;
    }

    public ErrorOr<Success> Publish()
    {
        if (IsPublished) return Result.Success;
        IsPublished = true;
        UpdatedAt = DateTime.UtcNow;
      
        return Result.Success;
    }
}