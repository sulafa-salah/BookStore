using Catalog.Application.Common.Interfaces;
using ErrorOr;
using MediatR;


namespace Catalog.Application.Books.Commands.UpdateBookCover;
    public  class UpdateBookCoverHandler(IBooksRepository books,
    IBlobStorage blobStorage
) : IRequestHandler<UpdateBookCoverCommand, ErrorOr<(Guid BookId, string BlobName)>>
{
    public async Task<ErrorOr<(Guid BookId, string BlobName)>> Handle(UpdateBookCoverCommand req, CancellationToken ct)
    {
        var book = await books.GetByIdAsync(req.BookId, ct);
        if (book is null)
            return Error.NotFound("Book.NotFound", "Book not found.");

        var ext = string.IsNullOrWhiteSpace(req.FileExtension) ? ".jpg" : req.FileExtension.ToLowerInvariant();
        if (!ext.StartsWith(".")) ext = "." + ext;

        var blobName = $"{req.BookId}{ext}";

        var savedBlobName = await blobStorage.UploadAsync(
            req.Content,
            req.ContentType,
            req.ContainerName,
            blobName,
            ct);

        var setRes = book.SetCover(savedBlobName);
        if (setRes.IsError) return setRes.Errors;

        books.Update(book);
       

        return (book.Id, savedBlobName);
    }
}