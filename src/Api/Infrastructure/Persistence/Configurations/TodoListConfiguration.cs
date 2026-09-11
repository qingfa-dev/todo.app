using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Todo.Api.Domain.Todos.Constants;
using Todo.Api.Domain.Todos.Entities;

namespace Todo.Api.Infrastructure.Persistence.Configurations;

public sealed class TodoListConfiguration
    : IEntityTypeConfiguration<TodoList>
{
    public void Configure(
        EntityTypeBuilder<TodoList> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Title)
            .HasMaxLength(TodoListConstant.Constraints.TitleMaxLength);

        // -----------------------------------------
        // Colour value object
        // -----------------------------------------

        builder.OwnsOne(
            x => x.Colour,
            colour =>
            {
                colour.Property(x => x.Code)
                    .HasMaxLength(TodoListConstant.Constraints.ColorMaxLength)
                    .IsRequired();
            });

        // -----------------------------------------
        // TodoList -> TodoItems
        // -----------------------------------------

        builder.HasMany(x => x.Items)
            .WithOne(x => x.List)
            .HasForeignKey(x => x.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------------------------------
        // Indexes
        // -----------------------------------------

        builder.HasIndex(x => x.Title);
    }
}

