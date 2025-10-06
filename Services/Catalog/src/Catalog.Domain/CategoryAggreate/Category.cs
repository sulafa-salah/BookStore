using Catalog.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.CategoryAggreate;

    public  class Category : AggregateRoot, IAuditable
{
        public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool IsActive { get; private set; }
    // IAuditable props
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Category(  
            string name,
             string description,
             bool isActive = true,
               Guid? id = null) : base(id ?? Guid.NewGuid())
        {
            Name = name;
            Description = description;
        IsActive = isActive;

    }
    private Category() { }
    public void Update(string name, string description, bool isActive)
    { Name = name; Description = description; IsActive = isActive; }
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}


