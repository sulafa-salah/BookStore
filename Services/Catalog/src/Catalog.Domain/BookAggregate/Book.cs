using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.BookAggregate.Events;
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
    public string? CoverBlobName { get; private set; }
    public string? ThumbBlobName { get; private set; }
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
        Title = title.Trim(); ;
        Description = description.Trim(); ;
        CategoryId = categoryId;
        IsPublished = true;
        CreatedAt = DateTime.UtcNow;

      

    }
    /// <summary>
    /// Factory method to create a new Book aggregate.
    /// Validates inputs, creates value objects, enforces invariants,
    /// and raises a domain event upon successful creation.
    /// </summary>
    public static ErrorOr<Book> Create(string isbn, string sku, decimal amount, string currency,
        string title, string description, Guid categoryId, IEnumerable<Guid> authorIds)
    {
        // Collect validation errors from value objects.
        var errors = new List<Error>();
        // Create and validate ISBN value object.
        var isbnResult = ISBN.Create(isbn);        
        if (isbnResult.IsError) errors.AddRange(isbnResult.Errors);
        // Create and validate SKU value object.
        var skuResult = Sku.Create(sku);
        if (skuResult.IsError) errors.AddRange(skuResult.Errors);
        // Create and validate Money value object.
        var moneyResult = Money.Create(amount, currency);
        if (moneyResult.IsError) errors.AddRange(moneyResult.Errors);

        // If any value object validation failed, return the collected errors.
        if (errors.Count > 0)
            return errors;

        // Validate core book properties (invariants).
        if (string.IsNullOrWhiteSpace(title))
          return  BookErrors.InvalidTitle;

        if (string.IsNullOrWhiteSpace(description))
            return BookErrors.InvalidDescription;

        if (categoryId == Guid.Empty)
           return BookErrors.InvalidCategory;

        // Ensure there is at least one valid, distinct author ID.
        var distinctAuthors = authorIds.Where(id => id != Guid.Empty)
                              .Distinct()
                              .ToList();

        if (distinctAuthors.Count == 0)
            return BookErrors.AuthorsEmpty;

        // Create the Book aggregate root using validated value objects and primitives.
        var book = new Book(
          isbnResult.Value,
          skuResult.Value,
          moneyResult.Value,
          title.Trim(),
          description.Trim(),
          categoryId);

        // Associate authors with the book, enforcing invariant (no duplicates).
        foreach (var authorId in authorIds )
            book.AddAuthor(authorId);


        // Raise a domain event indicating the book was created.
        // This event can be used to trigger actions in other bounded contexts
        // (e.g., initializing stock in Inventory Service).
        book._domainEvents.Add(new BookCreatedEvent(
            BookId : book.Id,
          Isbn: book.Isbn.Value,
            Sku: book.Sku.Value,
            Price: book.Price.Amount,
            Currency: book.Price.Currency,
            Title: book.Title,
            CreatedAtUtc: book.CreatedAt));

        // Return the successfully created book aggregate.
        return book;

    }
    public ErrorOr<Success> SetCover(string blobName, string? thumbBlobName = null)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            return Error.Validation("Book.Cover.Empty", "Cover blob is required.");

        CoverBlobName = blobName.Trim();
        ThumbBlobName = thumbBlobName?.Trim();
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }
    public void SetThumbBlobName(string blobName)
    {
        ThumbBlobName = blobName;
        UpdatedAt = DateTime.UtcNow;
    }
    public void AddAuthor(Guid authorId)
    {
        if (_bookAuthors.Any(x => x.AuthorId == authorId)) return;
        _bookAuthors.Add(new BookAuthor(Id, authorId));
       
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