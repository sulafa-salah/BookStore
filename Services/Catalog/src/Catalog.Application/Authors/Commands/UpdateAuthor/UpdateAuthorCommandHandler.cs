using Catalog.Application.Authors.Commands.CreateAuthor;
using Catalog.Application.Categories.Commands.UpdateCategory;
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

namespace Catalog.Application.Authors.Commands.UpdateAuthor;
    public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, ErrorOr<Author>>
    {
    private readonly IAuthorsRepository _authorsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAuthorCommandHandler(IAuthorsRepository authorsRepository, IUnitOfWork unitOfWork) => (_authorsRepository, _unitOfWork) = (authorsRepository, unitOfWork);
 
    public async Task<ErrorOr<Author>> Handle(UpdateAuthorCommand cmd, CancellationToken ct)
    {
        var author = await _authorsRepository.GetByIdAsync(cmd.AuthorId, ct);
            if (author is null) return AuthorErrors.NotFound;
        var newName = cmd.Name.Trim();
        var newBio = cmd.Bio.Trim();

        var exists = await _authorsRepository.ExistsByNameExcludingIdAsync(newName, cmd.AuthorId, ct);
        if (exists) return AuthorErrors.DuplicateName;
        author.Update(newName, newBio,cmd.IsActive);
        await _authorsRepository.UpdateAsync(author);

        await _unitOfWork.CommitChangesAsync();
            return author;
        }
    }