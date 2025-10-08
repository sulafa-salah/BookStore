using Catalog.Application.Authors.Queries.GetAuthor;
using Catalog.Application.Categories.Queries.GetCategory;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Authors.Queries.GetAuthor;
   
        public class GetAuthorQueryHandler : IRequestHandler<GetAuthorQuery, ErrorOr<Author>>
        {
            private readonly IAuthorsRepository _authorsRepository;

            public GetAuthorQueryHandler(IAuthorsRepository authorsRepository)
            {
        _authorsRepository = authorsRepository;
            }

         
           public async Task<ErrorOr<Author>> Handle(GetAuthorQuery req, CancellationToken ct)
                => await _authorsRepository.GetByIdAsync(req.AuthorId, ct) is { } a ? a : AuthorErrors.NotFound;
}

    

   