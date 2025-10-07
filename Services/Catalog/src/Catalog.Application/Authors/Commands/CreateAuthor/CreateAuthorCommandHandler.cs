using Catalog.Application.Common.Interfaces;
using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using MediatR;


namespace Catalog.Application.Authors.Commands.CreateAuthor
{
    
    public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, ErrorOr<Author>>
    {
        private readonly IAuthorsRepository _authorsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAuthorCommandHandler(IAuthorsRepository authorsRepository, IUnitOfWork unitOfWork)
        {
            _authorsRepository = authorsRepository;
             _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Author>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
        {
            if (await _authorsRepository.ExistsByNameAsync(request.Name, cancellationToken))
                return AuthorErrors.DuplicateName;

            // Create an Author
            var author = new Author
            (

                name: request.Name,
                bio : request.Bio

            );

            // Add it to the database
            await _authorsRepository.AddAuthorAsync(author,cancellationToken);
             await _unitOfWork.CommitChangesAsync();

            // Return author
            return author;
        }
    }

}
