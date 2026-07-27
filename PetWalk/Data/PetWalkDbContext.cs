using Microsoft.EntityFrameworkCore;
using PetWalk.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetWalk.Data
{
    public class PetWalkDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Walker> Walkers { get; set; }
        public DbSet<Dog> Dogs { get; set; }
        public DbSet<Walk> Walks { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<AvailabilitySlot> AvailabilitySlots { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=PetWalk.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TPH - Owner and Walker stored in Users table
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Owner>("Owner")
                .HasValue<Walker>("Walker");

            // Owner -> Dogs (composition - cascade delete)
            modelBuilder.Entity<Dog>()
                .HasOne(d => d.Owner)
                .WithMany(o => o.Dogs)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Owner -> Walks
            modelBuilder.Entity<Walk>()
                .HasOne(w => w.Owner)
                .WithMany(o => o.ScheduledWalks)
                .HasForeignKey(w => w.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Walker -> Walks
            modelBuilder.Entity<Walk>()
                .HasOne(w => w.Walker)
                .WithMany(wk => wk.AssignedWalks)
                .HasForeignKey(w => w.WalkerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Dog -> Walks
            modelBuilder.Entity<Walk>()
                .HasOne(w => w.Dog)
                .WithMany()
                .HasForeignKey(w => w.DogId)
                .OnDelete(DeleteBehavior.Restrict);

            // Walk -> Review (one-to-one)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Walk)
                .WithOne(w => w.Review)
                .HasForeignKey<Review>(r => r.WalkId)
                .OnDelete(DeleteBehavior.Cascade);

            // Owner -> Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Owner)
                .WithMany(o => o.WrittenReviews)
                .HasForeignKey(r => r.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Walker -> Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Walker)
                .WithMany(wk => wk.Reviews)
                .HasForeignKey(r => r.WalkerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Price precision
            modelBuilder.Entity<Walk>()
                .Property(w => w.Price)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Walker>()
                .Property(w => w.HourlyRate)
                .HasColumnType("decimal(10,2)");

            // Walker -> AvailabilitySlots
            modelBuilder.Entity<AvailabilitySlot>()
                .HasOne(a => a.Walker)
                .WithMany(w => w.AvailabilitySlots)
                .HasForeignKey(a => a.WalkerId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
