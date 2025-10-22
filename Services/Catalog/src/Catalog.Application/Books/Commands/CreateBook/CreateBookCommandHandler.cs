using Catalog.Application.Common.Interfaces;
using Catalog.Domain.BookAggregate;
using Catalog.Domain.CategoryAggreate;
using Catalog.Domain.Common.ValueObjects;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Books.Commands.CreateBook;
   
   public sealed class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, ErrorOr<Book>>
    {
        private readonly IBooksRepository _booksRepository;
        private readonly IAuthorsRepository _authorsRepository;
        private readonly ICategoriesRepository _categoriesRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookCommandHandler(
            IBooksRepository books,
            IAuthorsRepository authors,
            ICategoriesRepository categories,
            IUnitOfWork unitOfWork
         )
        {
        _booksRepository = books;
        _authorsRepository = authors;
        _categoriesRepository = categories;
        _unitOfWork = unitOfWork;
    }

        public async Task<ErrorOr<Book>> Handle(CreateBookCommand cmd, CancellationToken ct)
        {
        // 1) Category existence
        if (!await _categoriesRepository.ExistsByIdAsync(cmd.CategoryId, ct))
            return CategoryErrors.NotFound;

        // 2) Uniqueness (fast-fail)
        if (await _booksRepository.IsIsbnTakenAsync(cmd.Isbn, ct))
            return BookErrors.DuplicateISBN;

        if (await _booksRepository.IsSkuTakenAsync(cmd.Sku, ct))
            return BookErrors.DuplicateSKU;

        // 3) Authors existence 
        var hasAnyAuthors =
     cmd.AuthorIds is ICollection<Guid> coll ? coll.Count > 0
   : cmd.AuthorIds?.Any() == true;

        if (hasAnyAuthors)
        {
            var missing = await _authorsRepository.GetMissingIdsAsync(cmd.AuthorIds, ct);
            if (missing.Count > 0)
                return Error.Validation("Author.NotFound", $"Missing authors: {string.Join(",", missing)}");
        }

        // 4) Build value objects (collect validation)
        var errors = new List<Error>();

        var isbnRes = ISBN.Create(cmd.Isbn);
        if (isbnRes.IsError) errors.AddRange(isbnRes.Errors);

        var skuRes = Sku.Create(cmd.Sku);
        if (skuRes.IsError) errors.AddRange(skuRes.Errors);

        var moneyRes = Money.Create(cmd.PriceAmount, cmd.PriceCurrency);
        if (moneyRes.IsError) errors.AddRange(moneyRes.Errors);

      

        if (errors.Count > 0) return errors;

        // 5) Create aggregate
        var created =Book.Create(
            isbnRes.Value.ToString(),
            skuRes.Value.ToString(),
            moneyRes.Value.Amount,
            cmd.PriceCurrency,
            cmd.Title!.Trim(),
            cmd.Description!.Trim(),
            cmd.CategoryId,
            cmd.AuthorIds
           );
        if (created.IsError) return created.Errors;

        var book = created.Value;

        // 6) Persist
        await _booksRepository.AddBookAsync(book, ct);
      await  _unitOfWork.SaveChangesAsync();

        return book;
    
}
    }