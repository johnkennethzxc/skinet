using System.Linq;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Ken",
                    Email = "ken@gmail.com",
                    UserName = "ken@gmail.com",
                    Address = new Address
                    {
                        FirstName = "Ken",
                        LastName = "Kenny",
                        Street = "10th Bonifacio Global Street",
                        City = "Taguig City",
                        State = "Philippines",
                        Zipcode = "4027"
                    }
                };

                await userManager.CreateAsync(user, "4557021zxcX1*");
            }
        }
    }
}