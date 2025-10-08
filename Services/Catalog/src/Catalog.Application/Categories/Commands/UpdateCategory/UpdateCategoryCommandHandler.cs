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

namespace Catalog.Application.Categories.Commands.UpdateCategory;
    public  class UpdateCategoryCommandHandler
     : IRequestHandler<UpdateCategoryCommand, ErrorOr<Category>>
    {
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryCommandHandler(ICategoriesRepository categoriesRepository, IUnitOfWork unitOfWork)
            => (_categoriesRepository, _unitOfWork) = (categoriesRepository, unitOfWork);

        public async Task<ErrorOr<Category>> Handle(UpdateCategoryCommand r, CancellationToken ct)
        {
            var category = await _categoriesRepository.GetByIdAsync(r.Id, ct);
            if (category is null) return CategoryErrors.NotFound;

            var newName = r.Name.Trim();
            var newDesc = r.Description.Trim();

            var exists = await _categoriesRepository.ExistsByNameExcludingIdAsync(newName, r.Id, ct);
            if (exists) return CategoryErrors.DuplicateName;

            category.Update(newName, newDesc, r.IsActive);
        await _categoriesRepository.UpdateAsync(category);
        await _unitOfWork.CommitChangesAsync();

            return category;
        }
    }