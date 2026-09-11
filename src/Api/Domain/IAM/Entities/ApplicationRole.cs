using Microsoft.AspNetCore.Identity;

namespace Todo.Api.Domain.IAM.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = default!;
}