using Catalog.Domain.BookAggregate;
using Catalog.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.AuthorAggregate;

    public class Author : AggregateRoot, IAuditable
{
    private readonly List<BookAuthor> _bookAuthors = new();
    public string Name { get; private set; } = null!;
    public string Biography { get; private set; } = null!;
    public bool IsActive { get; private set; }

    // IAuditable props
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyCollection<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();
    private Author() { }
    public Author(string name, string bio, bool isActive = true, Guid? id = null) : base(id ?? Guid.NewGuid())
    { Name = name; Biography = bio; IsActive = isActive; }

    public void Update(string name, string bio, bool isActive =true)
    { Name = name; Biography = bio; IsActive = isActive; }
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

   


