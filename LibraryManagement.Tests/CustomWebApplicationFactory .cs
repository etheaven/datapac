using LibraryManagement.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Find and remove existing local db connection for testing
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory db
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                    db.Database.EnsureCreated();
                    InitializeDbForTests(db);
                }
            });
        }
        public static void InitializeDbForTests(ApplicationDbContext db)
        {
            var book = new Book { Id = 1, Author = "Simon Soka", Title = "How I said hello to Datapac team" };
            db.Books.Add(book);
            var book2 = new Book { Id = 2, Author = "Simon Soka", Title = "Very creative name" };
            db.Books.Add(book2);
            var book3 = new Book { Id = 3, Author = "Simon Soka", Title = "Very boring name" };
            db.Books.Add(book3);
            db.SaveChanges();
        }
    }
}
