using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Authors.Commands.CreateAuthor;

    public class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
    {
        public CreateAuthorCommandValidator()
        {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
             .MinimumLength(8).WithMessage("Name must not less than 8 characters.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        RuleFor(x => x.Bio)
                .NotEmpty().WithMessage("Bio is required.")
                .MaximumLength(1000).WithMessage("Bio must not exceed 1000 characters.");
    }
    }

