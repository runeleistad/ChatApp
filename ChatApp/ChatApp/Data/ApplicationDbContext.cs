using ChatApp.Data.Models;
using ChatApp.Data.ValueGenerators;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Room> Room { get; set; }
        public DbSet<RoomUser> RoomUser { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Contact> Contact { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            OnCreateRoom(builder);
            OnCreateRoomUser(builder);
            OnCreateMessage(builder);
            OnCreateApplicationUser(builder);
            OnCreateContact(builder);
        }

        private void OnCreateRoom(ModelBuilder builder)
        {
            // pk
            builder.Entity<Room>()
                .HasKey(r => r.Id);
            builder.Entity<Room>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<LongValueGenerator>();

            // Ignore
            builder.Entity<Room>()
                .Ignore(r => r.RoomUsers);
        }

        private void OnCreateRoomUser(ModelBuilder builder) 
        {
            // pk
            builder.Entity<RoomUser>()
                .HasKey(ru => ru.Id);
            builder.Entity<RoomUser>()
                .Property(ru => ru.Id)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<LongValueGenerator>();
            
            // foreign keys
            builder.Entity<RoomUser>()
                .HasOne(ru => ru.Room)
                .WithMany(r => r.RoomUsers)
                .HasForeignKey(ru => ru.RoomId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<RoomUser>()
                .HasOne(ru => ru.User)
                .WithMany(au => au.Rooms)
                .HasForeignKey(ru => ru.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        private void OnCreateMessage(ModelBuilder builder)
        {
            // pk
            builder.Entity<Message>()
                .HasKey(m => m.Id);
            builder.Entity<Message>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<LongValueGenerator>();

            // Foreign keys
            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(au => au.Messages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Message>()
                .HasOne(m => m.Room)
                .WithMany(r => r.Messages)
                .HasForeignKey(m => m.RoomId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        private void OnCreateApplicationUser(ModelBuilder builder)
        {
            // Don't need to save these
            builder.Entity<ApplicationUser>().Ignore(au => au.Messages);
            builder.Entity<ApplicationUser>().Ignore(au => au.Rooms);
            builder.Entity<ApplicationUser>().Ignore(au => au.Contacts);
        }
        
        private void OnCreateContact(ModelBuilder builder)
        {
            builder.Entity<Contact>()
                .HasKey(c => c.Id);
            builder.Entity<Contact>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<LongValueGenerator>();

            // Unique constraint on Owner -> Contact
            builder.Entity<Contact>()
                .HasIndex(c => new { c.OwnerUserId, c.ContactUserId })
                .IsDescending();

            // Foreign keys
            builder.Entity<Contact>()
                .HasOne(c => c.OwnerUser)
                .WithMany(au => au.Owners)
                .HasForeignKey(c => c.OwnerUserId);
            builder.Entity<Contact>()
                .HasOne(c => c.ContactUser)
                .WithMany(au => au.Contacts)
                .HasForeignKey(c => c.ContactUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
