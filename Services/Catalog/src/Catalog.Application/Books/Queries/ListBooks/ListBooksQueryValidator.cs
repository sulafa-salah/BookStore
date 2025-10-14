
using FluentValidation;


namespace Catalog.Application.Books.Queries.ListBooks;
  
   public class ListBooksQueryValidator : AbstractValidator<ListBooksQuery>
    {
        public ListBooksQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.SortDir).Must(d => d is "asc" or "desc")
                .WithMessage("SortDir must be 'asc' or 'desc'.");
        }
    }
