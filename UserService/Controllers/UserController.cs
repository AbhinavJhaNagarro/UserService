using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using UserService.DatabaseContext;


namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        public UserController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        [HttpGet("{id}")]
        public Object Get(int id)
        {
            var connectionString = Configuration["ConnectionStrings:DatabaseConnection"];
            if (!String.IsNullOrWhiteSpace(connectionString))
            {
                // Creating context builder object
                var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
                optionsBuilder.UseSqlServer(connectionString, providerOptions => providerOptions.CommandTimeout(60))
                  .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                // Using context to create instance
                using (var dbContext = new UserDbContext(optionsBuilder.Options))
                {
                    // Verifying Database creation
                    dbContext.Database.EnsureCreated();
                    // Verifying if the added user in there in the database
                    var testUser = dbContext.Users.FirstOrDefault(b => b.Id == 1);
                    if (testUser == null)
                    {
                        // Creating dummy user with id 1
                        dbContext.Users.Add(new User { Name = "Joe Dark", Age = 20, Email = "joe@micro.com" });
                        dbContext.SaveChanges();
                    }
                    return dbContext.Users.FirstOrDefault(user => user.Id == id);
                }
            }
            return "Error";
        }
    }
}