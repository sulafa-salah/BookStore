using Catalog.Application.Common.Constants;
using Catalog.Application.Common.Interfaces;
using ErrorOr;
using MediatR;


namespace Catalog.Application.Books.Commands.UpdateBookCover;
    public  class UpdateBookCoverHandler : IRequestHandler<UpdateBookCoverCommand, ErrorOr<(Guid BookId, string BlobName)>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBooksRepository books;
    private readonly IBlobStorage blobStorage;
    private readonly ICacheService _cache;

    public UpdateBookCoverHandler(IUnitOfWork unitOfWork, IBooksRepository books, IBlobStorage blobStorage, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        this.books = books;
        this.blobStorage = blobStorage;
        _cache = cache;
    }
    public async Task<ErrorOr<(Guid BookId, string BlobName)>> Handle(UpdateBookCoverCommand cmd, CancellationToken ct)
    {
        var book = await books.GetByIdAsync(cmd.BookId, ct);
        if (book is null)
            return Error.NotFound("Book.NotFound", "Book not found.");

        // Accept the real file extension but normalize a few variants
        var ext = string.IsNullOrWhiteSpace(cmd.FileExtension)
            ? ".jpg" // fallback only if unknown
            : cmd.FileExtension.ToLowerInvariant();

        if (!ext.StartsWith(".")) ext = "." + ext;

        // Normalize confusing or uncommon extensions
        if (ext is ".jpeg" or ".jfif") ext = ".jpg";

        // Only allow supported formats
        var allowed = new[] { ".jpg", ".png", ".webp" };
        if (!allowed.Contains(ext))
            return Error.Validation("File.Extension", $"Unsupported image format: {ext}");

        // store under "covers/"
        var blobName = $"covers/{cmd.BookId}{ext}";

        var savedBlobName = await blobStorage.UploadAsync(
            cmd.Content,
            cmd.ContentType,
            cmd.ContainerName, // must be "media"
            blobName,
            ct);

        var setRes = book.SetCover(savedBlobName);
        if (setRes.IsError) return setRes.Errors;

        books.Update(book);

        await _unitOfWork.SaveChangesAsync();
        // cache invalidation
        await _cache.RemoveAsync(CacheKeys.Book(cmd.BookId), ct);
        return (book.Id, savedBlobName);// e.g. "covers/{id}.jpg"
    }
}