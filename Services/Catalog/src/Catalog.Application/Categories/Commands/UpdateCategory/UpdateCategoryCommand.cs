using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Categories.Commands.UpdateCategory;

    public  record UpdateCategoryCommand(
        Guid Id,
        string Name,
        string Description,
        bool IsActive
    ) : IRequest<ErrorOr<Category>>;