using Microsoft.EntityFrameworkCore;
using Project_Dotnet_4_V2.Models;

namespace Project_Dotnet_4_V2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
            /*modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Seats)
                .WithMany();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Reservations)  
                .WithOne(r => r.User)     
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<User>()
                    .HasIndex(u => u.Username)
                    .IsUnique();*/
        //}
    }
}