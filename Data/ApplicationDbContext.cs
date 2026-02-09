using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Projekt_mtg.Models;

namespace projekt_mtg.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
     //modellerna i databasen (Card) och colletion (Cards)
     public DbSet<Card> Cards { get; set; }


        public DbSet<Set> Sets { get; set; }


        public DbSet<Collection> Collections { get; set; }


        public DbSet<User> Users { get; set; }
    
}



