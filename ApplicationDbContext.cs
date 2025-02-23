using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=camp_db;Username=postgres;Password=1503;Encoding=UTF8");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Child>()
            .HasKey(c => c.Child_Id);

        modelBuilder.Entity<Counselor>()
            .HasKey(c => c.Counselor_Id);

        modelBuilder.Entity<Event>()
            .HasKey(c => c.Event_Id);

        modelBuilder.Entity<EventTemplate>()
            .HasKey(c => c.EventTemplate_Id);

        modelBuilder.Entity<Group>()
            .HasKey(c => c.Group_Id);

        modelBuilder.Entity<Notification>()
            .HasKey(c => c.Notification_Id);

        modelBuilder.Entity<Session>()
            .HasKey(c => c.Session_Id);

        modelBuilder.Entity<SessionChild>()
            .HasKey(c => c.SessionChild_Id);

        modelBuilder.Entity<SessionCounselor>()
            .HasKey(c => c.SessionCounselor_Id);

        modelBuilder.Entity<User>()
            .HasKey(c => c.User_Id);
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
//  \c camp_db