using Microsoft.EntityFrameworkCore;
using Projekt_mtg.Models;

namespace Projekt_mtg.Services
{
    public class MtgIndexDbContext : DbContext
    {
        //modellerna i databasen (Card) och colletion (Cards)
        public DbSet<Card> Cards { get; init; }


        public DbSet<Set> Sets { get; init; }


        public DbSet<Collection> Collections { get; init; }


        public DbSet<User> Users { get; init; }


        public MtgIndexDbContext(DbContextOptions options) : base(options)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Card>();
            modelBuilder.Entity<Set>();
            modelBuilder.Entity<Collection>();
            modelBuilder.Entity<User>();

        }
        
    }
}