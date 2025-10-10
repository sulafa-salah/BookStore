using Catalog.Application.Common.Interfaces;
using Catalog.Domain.BookAggregate;
using Catalog.Domain.CategoryAggreate;
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
            IUnitOfWork uow)
        {
        _booksRepository = books;
        _authorsRepository = authors;
        _categoriesRepository = categories;
        _unitOfWork = uow;
        }

        public async Task<ErrorOr<Book>> Handle(CreateBookCommand cmd, CancellationToken ct)
        {
            // 1) Cross-aggregate existence checks
            if (!await _categoriesRepository.ExistsByIdAsync(cmd.CategoryId, ct))
            return CategoryErrors.NotFound;
       

            // 2) Uniqueness checks (fast-fail)
            if (await _booksRepository.IsIsbnTakenAsync(cmd.Isbn, ct))
            return BookErrors.DuplicateISBN;
            if (await _booksRepository.IsSkuTakenAsync(cmd.Sku, ct))
            return BookErrors.DuplicateSKU;

        // 3) Validate authors existence 
        if (cmd.AuthorIds?.Count > 0)
            {
                var missing = await _authorsRepository.GetMissingIdsAsync(cmd.AuthorIds, ct);
                if (missing.Count > 0)
                    return Error.Validation("Author.NotFound", $"Missing authors: {string.Join(",", missing)}");
            }

            // 4) Create book aggregate
          
            var created = Book.Create(
                cmd.Isbn,
                cmd.Sku,
                cmd.PriceAmount,
                 cmd.PriceCurrency,  
                cmd.Title,
                cmd.Description,
                cmd.CategoryId
            );

            if (created.IsError) return created.Errors;

            var book = created.Value;

          

            // 5) Attach authors
            foreach (var authorId in cmd.AuthorIds ?? Array.Empty<Guid>())
                book.AddAuthor(authorId);

            //  todo : raise event

            // 6) Persist
            await _booksRepository.AddBookAsync(book, ct);
            await _unitOfWork.CommitChangesAsync();

            return book;
        }
    }