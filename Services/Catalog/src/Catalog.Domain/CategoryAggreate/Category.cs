using Catalog.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.CategoryAggreate;

    public  class Category : AggregateRoot
    {
        public string Name { get;  } = null!;
    public string Description { get; } = null!;
    public bool IsActive { get; private set; }


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

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}


