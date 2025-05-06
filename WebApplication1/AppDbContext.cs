using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1
{
    public class AppDbContext : DbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<ParentType> ParentTypes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Event>()
                .ToTable(tb => tb.HasCheckConstraint("CK_Event_Duration", "[DurationInMinutes] > 0"));


            modelBuilder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Restrict); 


            modelBuilder.Entity<Student>()
                .HasOne(s => s.Member)
                .WithOne(m => m.Student)
                .HasForeignKey<Student>(s => s.Id)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Parent>()
                .HasOne(p => p.Member)
                .WithOne(m => m.Parent)
                .HasForeignKey<Parent>(p => p.Id)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Member)
                .WithOne(m => m.Teacher)
                .HasForeignKey<Teacher>(t => t.Id)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Student>()
                .HasMany(s => s.Parents)
                .WithMany(p => p.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "ParentStudent",
                    j => j
                        .HasOne<Parent>()
                        .WithMany()
                        .HasForeignKey("ParentsId")
                        .OnDelete(DeleteBehavior.Restrict),
                    j => j
                        .HasOne<Student>()
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Restrict)
                );


            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventType)
                .WithMany()
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Event>()
                .HasMany(e => e.Members)
                .WithMany(m => m.Events)
                .UsingEntity(j => j.ToTable("EventMember")); 


            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.Event)
                .WithOne(e => e.Task)
                .HasForeignKey<Models.Task>(t => t.Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ParentType>().HasData(
                new ParentType { Id = 1, ParentTypeName = "Батько" },
                new ParentType { Id = 2, ParentTypeName = "Мати" }
            );

            modelBuilder.Entity<EventType>().HasData(
                new EventType { Id = 1, EventTypeName = "Екзамен" },
                new EventType { Id = 2, EventTypeName = "Контрольна робота" },
                new EventType { Id = 3, EventTypeName = "Шкільні заходи" },
                new EventType { Id = 4, EventTypeName = "Батьківські збори" },
                new EventType { Id = 5, EventTypeName = "Особисті події" }
            );

            base.OnModelCreating(modelBuilder);
        }



    }
}
