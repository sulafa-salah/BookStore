using Catalog.Domain.BookAggregate;
using Catalog.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.AuthorAggregate;

    public class Author: AggregateRoot
{
   
    public string Name { get; private set; } = null!;
    public string Biography { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public ICollection<BookAuthor> BookAuthors { get; private set; } = new List<BookAuthor>();
    private Author() { }
    public Author(string name, string bio, bool isActive = true, Guid? id = null) : base(id ?? Guid.NewGuid())
    { Name = name; Biography = bio; IsActive = isActive; }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

   


