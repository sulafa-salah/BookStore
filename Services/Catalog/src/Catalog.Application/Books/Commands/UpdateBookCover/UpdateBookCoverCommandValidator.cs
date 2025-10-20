using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Books.Commands.UpdateBookCover;
    public  class UpdateBookCoverValidator : AbstractValidator<UpdateBookCoverCommand>
    {
        public UpdateBookCoverValidator()
        {
            RuleFor(x => x.BookId).NotEmpty();
          
        }
    }