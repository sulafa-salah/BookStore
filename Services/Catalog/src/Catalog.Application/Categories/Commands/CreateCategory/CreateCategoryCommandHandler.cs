using Catalog.Application.Common.Interfaces;
using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using MediatR;


namespace Catalog.Application.Categories.Commands.CreateCategory
{
    
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ErrorOr<Category>>
    {
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryCommandHandler(ICategoriesRepository categoriesRepository, IUnitOfWork unitOfWork)
        {
            _categoriesRepository = categoriesRepository;
             _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Category>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            // domain invariant: unique name 
            var exists = await _categoriesRepository.ExistsByNameAsync(request.Name, cancellationToken);
            if (exists) return CategoryErrors.DuplicateName;
            // Create a Category
            var category = new Category
            (
              
                name: request.Name,
                description : request.Description
               
            );

            // Add it to the database
            await _categoriesRepository.AddCategoryAsync(category,cancellationToken);
             await _unitOfWork.CommitChangesAsync();

            // Return Category
            return category;
        }
    }

}
