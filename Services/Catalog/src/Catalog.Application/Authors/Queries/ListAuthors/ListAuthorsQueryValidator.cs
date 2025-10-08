using Catalog.Application.Authors.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Categories.Queries.ListCategories;

    public class ListAuthorsQueryValidator : AbstractValidator<ListAuthorsQuery>
    {
        public ListAuthorsQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.SortDir).Must(d => d is "asc" or "desc")
                .WithMessage("SortDir must be 'asc' or 'desc'.");
        }
    }
