using Catalog.Application.Authors.Commands.UpdateAuthor;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Authors.Commands.UpdateAuthor;
  
public sealed class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
    public UpdateAuthorCommandValidator()
    {
       
        RuleFor(x => x.Name)
          .NotEmpty().WithMessage("Name is required.")
           .MinimumLength(2).WithMessage("Name must not less than 2 characters.")
          .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        RuleFor(x => x.Bio)
                .NotEmpty().WithMessage("Bio is required.")
                .MaximumLength(1000).WithMessage("Bio must not exceed 1000 characters.");
    }
}