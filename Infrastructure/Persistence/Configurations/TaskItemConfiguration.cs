using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration: IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        ConfigureTaskItemsTable(builder);
    }

    private void ConfigureTaskItemsTable(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable(name: "TaskItems");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description);

        builder.Property(t => t.Status)
            .IsRequired();

        builder.Property(t => t.Deadline)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        builder.HasOne(t => t.CreatedBy)
            .WithMany(u => u.CreatedTasks)
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.UpdatedBy)
            .WithMany()
            .HasForeignKey(t => t.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}