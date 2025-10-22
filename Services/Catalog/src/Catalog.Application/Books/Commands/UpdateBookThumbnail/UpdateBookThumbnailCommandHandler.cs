using Catalog.Application.Common.Interfaces;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Books.Commands.UpdateBookThumbnail;
    public sealed class UpdateBookThumbnailCommandHandler
     : IRequestHandler<UpdateBookThumbnailCommand, ErrorOr<Success>>
    {
        private readonly IBooksRepository _booksRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBookThumbnailCommandHandler(IBooksRepository booksRepository,IUnitOfWork unitOfWork)
    {
        _booksRepository= booksRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateBookThumbnailCommand cmd, CancellationToken ct)
        {
            var book = await _booksRepository.GetByIdAsync(cmd.BookId, ct);
            if (book is null) return Error.NotFound(description: "Book not found");

            book.SetThumbBlobName($"thumbs/{cmd.BookId}.jpg"); 
        _booksRepository.Update(book);
     await _unitOfWork.SaveChangesAsync();
        return Result.Success;
        }
    }