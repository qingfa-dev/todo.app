using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Todo.Api.Domain.IAM.Constants;
using Todo.Api.Domain.IAM.Entities;

namespace Todo.Api.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(
        EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        // -----------------------------------------
        // Token
        // -----------------------------------------

        builder.Property(x => x.TokenHash)
            .HasMaxLength(RefreshTokenConstant.Constraints.TokenHashMaxLength)
            .IsRequired();

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        // -----------------------------------------
        // User
        // -----------------------------------------

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------------------------------
        // Dates
        // -----------------------------------------

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.RevokedAtUtc);

        builder.Property(x => x.ReplacedByTokenHash)
            .HasMaxLength(RefreshTokenConstant.Constraints.TokenHashMaxLength);
    }
}
