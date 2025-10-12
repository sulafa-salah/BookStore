using Catalog.Domain.Common.ValueObjects;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.BookAggregate;
    public static class BookErrors
    {
    public static readonly Error InvalidTitle = Error.Validation("Book.InvalidTitle", "Title required.");
    public static readonly Error InvalidDescription = Error.Validation("Book.InvalidDescription", "Description required.");
    public static readonly Error InvalidPrice = Error.Validation("Book.InvalidPrice", "Price must be positive.");
   
    public static readonly Error InvalidCategory = Error.Validation("Book.InvalidCategory", "Category is invalid.");
    public static readonly Error NotFound = Error.NotFound("Book.NotFound", "Book not found.");
    public static readonly Error DuplicateTitle = Error.Conflict("Book.DuplicateTitle", "Duplicate title.");
    public static readonly Error InvalidAuthors = Error.Validation("Book.InvalidAuthors", "One or more author IDs are invalid.");
    public static readonly Error DuplicateISBN = Error.Validation("Book.ISBN.Exists", "ISBN already exists.");
    public static readonly Error DuplicateSKU = Error.Validation("Book.SKU.Exists", "SKU already exists.");
    public static readonly Error AuthorsEmpty = Error.Validation("Book.Authors.Empty", "At least one author is required");



}