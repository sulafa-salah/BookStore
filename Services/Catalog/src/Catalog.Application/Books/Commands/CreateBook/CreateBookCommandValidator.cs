using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Books.Commands.CreateBook;
    public  class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator()
        {
        RuleFor(x => x.Title)
       .NotEmpty().WithMessage("Title is required.")
       .MaximumLength(250).WithMessage("Title must not exceed 250 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(4000).WithMessage("Description must not exceed 4000 characters.");

        RuleFor(x => x.Isbn)
            .NotEmpty().WithMessage("ISBN is required.")
            .MaximumLength(17).WithMessage("ISBN must not exceed 17 characters.");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(32).WithMessage("SKU must not exceed 32 characters.");

        RuleFor(x => x.PriceAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be greater than or equal to 0.");

        RuleFor(x => x.PriceCurrency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be a 3-letter ISO code (e.g., USD, SAR).");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");

        RuleForEach(x => x.AuthorIds)
            .NotEmpty().WithMessage("Author ID cannot be empty.");

        RuleFor(x => x.AuthorIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate authors are not allowed.");
    }
    }