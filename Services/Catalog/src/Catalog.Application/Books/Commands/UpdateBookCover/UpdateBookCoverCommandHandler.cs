using Catalog.Application.Common.Interfaces;
using ErrorOr;
using MediatR;


namespace Catalog.Application.Books.Commands.UpdateBookCover;
    public  class UpdateBookCoverHandler : IRequestHandler<UpdateBookCoverCommand, ErrorOr<(Guid BookId, string BlobName)>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBooksRepository books;
    private readonly IBlobStorage blobStorage;

    public UpdateBookCoverHandler(IUnitOfWork unitOfWork, IBooksRepository books, IBlobStorage blobStorage)
    {
        _unitOfWork = unitOfWork;
        this.books = books;
        this.blobStorage = blobStorage;
    }
    public async Task<ErrorOr<(Guid BookId, string BlobName)>> Handle(UpdateBookCoverCommand req, CancellationToken ct)
    {
        var book = await books.GetByIdAsync(req.BookId, ct);
        if (book is null)
            return Error.NotFound("Book.NotFound", "Book not found.");

        // Accept the real file extension but normalize a few variants
        var ext = string.IsNullOrWhiteSpace(req.FileExtension)
            ? ".jpg" // fallback only if unknown
            : req.FileExtension.ToLowerInvariant();

        if (!ext.StartsWith(".")) ext = "." + ext;

        // Normalize confusing or uncommon extensions
        if (ext is ".jpeg" or ".jfif") ext = ".jpg";

        // Only allow supported formats
        var allowed = new[] { ".jpg", ".png", ".webp" };
        if (!allowed.Contains(ext))
            return Error.Validation("File.Extension", $"Unsupported image format: {ext}");

        // store under "covers/"
        var blobName = $"covers/{req.BookId}{ext}";

        var savedBlobName = await blobStorage.UploadAsync(
            req.Content,
            req.ContentType,
            req.ContainerName, // must be "media"
            blobName,
            ct);

        var setRes = book.SetCover(savedBlobName);
        if (setRes.IsError) return setRes.Errors;

        books.Update(book);

        await _unitOfWork.SaveChangesAsync();
        return (book.Id, savedBlobName);// e.g. "covers/{id}.jpg"
    }
}