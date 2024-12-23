using System;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Data.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.Property(t => t.DueDate)
            .HasDefaultValueSql("getdate()");
        
        builder.Property(t => t.Status)
            .HasDefaultValue(TaskItemStatus.Pending);

        builder.Property(t => t.Images)
            .HasDefaultValue(new List<string>());
    }
}
