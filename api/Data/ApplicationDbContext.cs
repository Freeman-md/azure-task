using Microsoft.EntityFrameworkCore;
using api.Models;

public class ApplicationDbContext : DbContext {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        TaskItems = Set<TaskItem>();
    }

    public DbSet<TaskItem> TaskItems { get; set; }
}