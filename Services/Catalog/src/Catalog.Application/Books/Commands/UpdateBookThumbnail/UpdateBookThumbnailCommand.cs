using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Books.Commands.UpdateBookThumbnail;
    public  record UpdateBookThumbnailCommand(Guid BookId, string ThumbBlobName) : IRequest<ErrorOr<Success>>;