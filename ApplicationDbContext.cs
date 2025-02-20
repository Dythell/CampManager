using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=camp_db;Username=postgres;Password=1503");
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Counselor> Counselors { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Child> Children { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<EventTemplate> EventTemplates { get; set; }
    public DbSet<SessionCounselor> SessionCounselors { get; set; }
    public DbSet<SessionChild> SessionChildren { get; set; }
}
