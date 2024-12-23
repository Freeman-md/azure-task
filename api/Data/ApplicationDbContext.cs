using Microsoft.EntityFrameworkCore;
using api.Models;
using api.Data.Configurations;

public class ApplicationDbContext : DbContext {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        TaskItems = Set<TaskItem>();
    }

    public DbSet<TaskItem> TaskItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration<TaskItem>(new TaskItemConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}