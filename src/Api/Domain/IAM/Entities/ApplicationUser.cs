using Microsoft.AspNetCore.Identity;

using Todo.Api.Domain.Common;

namespace Todo.Api.Domain.IAM.Entities;

public class ApplicationUser : IdentityUser<Guid>, IAuditable
{
    public DateTimeOffset Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    public ICollection<IdentityUserClaim<Guid>> Claims { get; set; } = default!;
    public ICollection<IdentityUserLogin<Guid>> Logins { get; set; } = default!;
    public ICollection<IdentityUserToken<Guid>> Tokens { get; set; } = default!;
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = default!;
}
