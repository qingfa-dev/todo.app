using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Todo.Api.Domain.IAM.Entities;

namespace Todo.Api.Infrastructure.Persistence.Configurations;

public sealed class ApplicationRoleConfiguration
    : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(
        EntityTypeBuilder<ApplicationRole> b)
    {
        // Each Role can have many entries in the UserRole join table
        b.HasMany(e => e.UserRoles)
            .WithOne(e => e.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();
    }
}
