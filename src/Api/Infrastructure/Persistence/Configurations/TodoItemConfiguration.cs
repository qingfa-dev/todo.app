using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Todo.Api.Domain.Todos.Constants;
using Todo.Api.Domain.Todos.Entities;

namespace Todo.Api.Infrastructure.Persistence.Configurations;

public sealed class TodoItemConfiguration
    : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(
        EntityTypeBuilder<TodoItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        // -----------------------------------------
        // Properties
        // -----------------------------------------

        builder.Property(x => x.ListId)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(TodoItemConstant.Constraints.TitleMaxLength);

        builder.Property(x => x.Note)
            .HasMaxLength(TodoItemConstant.Constraints.NoteMaxLength);

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.Done)
            .IsRequired();

        // -----------------------------------------
        // TodoItem -> TodoList
        // -----------------------------------------

        builder.HasOne(x => x.List)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------------------------------
        // Indexes
        // -----------------------------------------

        builder.HasIndex(x => x.ListId);

        builder.HasIndex(x => new
        {
            x.ListId,
            x.Title
        });
    }
}