using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.BookAggregate;
using Catalog.Domain.CategoryAggreate;
using Catalog.Domain.Common.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Persistence;
    public static class CatalogDbContextSeeder
    {
        public static async Task SeedAsync(CatalogDbContext db)
        {
            // --- Categories ---
            if (!await db.Categories.AnyAsync())
            {
            db.Categories.AddRange(
                new Category(name: "Software Development", description: "Books related to software development processes and methodologies")
               ,
                new Category(name: "DevOps", description: "Books about DevOps practices and tools"),
                new Category(name: "Cloud Computing", description: "Books on cloud platforms and services")
            
            );
                await db.SaveChangesAsync();
            }

            // --- Authors ---
            if (!await db.Authors.AnyAsync())
            {
                db.Authors.AddRange(
                    new Author(name: "Martin Fowler", bio: "Software developer, author, and speaker specializing in enterprise software design."),
                    new Author(name: "Robert C. Martin", bio: "Software engineer and author. Known as 'Uncle Bob'."),
                    new Author(name: "Vaughn Vernon", bio: "Author and expert in Domain-Driven Design and distributed systems.")
                   
                );
                await db.SaveChangesAsync();
            }

           
        }
    }