using MetaBond.Domain;
using MetaBond.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Extensions;

public static class RoleSeed
{
    public static void SeedRoles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Roles>().HasData(
            new Roles
            {
                Id = Guid.Parse("d290f1ee-6c54-4b01-90e6-d701748f0851"),
                Name = UserRoles.User.ToString(),
                Description = "Default role for standard users"
            },
            new Roles
            {
                Id = Guid.Parse("5a4b8c2f-3d1e-4b6f-a1c2-9f3e2d7a6c1b"),
                Name = UserRoles.Admin.ToString(),
                Description = "Administrator with full permissions"
            }
        );
    }
}