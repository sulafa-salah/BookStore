using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;


namespace Catalog.Application.Books.Commands.UpdateBookCover;
public sealed record UpdateBookCoverCommand(
    Guid BookId,
    Stream Content,
    string ContentType,
    string FileExtension,
    string ContainerName
) : IRequest<ErrorOr<(Guid BookId, string BlobName)>>;
