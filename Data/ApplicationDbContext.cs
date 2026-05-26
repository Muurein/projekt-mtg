using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Projekt_mtg.Models;

namespace projekt_mtg.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
        //modellerna i databasen (Card) och colletion (Cards)
        public DbSet<Card> Cards { get; set; }


       // public DbSet<Set> Sets { get; set; }


        public DbSet<Collection> Collections { get; set; }


        public DbSet<User> CollUsers { get; set; }

        
        //definiera relationer för ökad tydlighet
        protected override void OnModelCreating(ModelBuilder builder)
        {
                base.OnModelCreating(builder);

                //makes sure one card cannot be added twice
                builder.Entity<Card>()
                        .HasIndex(c => c.Name)
                        .IsUnique();

                //defines card/collection-realtion
                builder.Entity<Collection>()
                        .HasOne(c => c.Card)
                        .WithMany(c => c.Collections)
                        .HasForeignKey(c => c.CardId);

                //same user casn't have two rows with the same card
                builder.Entity<Collection>()
                        .HasIndex(c => new { c.CardId, c.UserId })
                        .IsUnique();

                        
        }
        
}



