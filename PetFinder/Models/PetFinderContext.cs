﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetFinder.Areas.Identity;

#nullable disable

namespace PetFinder.Models
{
    public class PetFinderContext : IdentityDbContext<ApplicationUser>
    {
        public PetFinderContext()
        {
        }

        public PetFinderContext(DbContextOptions<PetFinderContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> AspNetUsers { get; set; }
        public DbSet<AnimalType> AnimalTypes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Comment> Comments { get; set; }

        // Creo los generos por defecto ya que no disponen de un CRUD
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var gender1 = new Gender {Id = 1, Name = "Macho", SerializedName = "MACHO"};
            var gender2 = new Gender {Id = 2, Name = "Hembra", SerializedName = "HEMBRA"};
            modelBuilder.Entity<Gender>().HasData(gender1, gender2);
            base.OnModelCreating(modelBuilder);
        }
    }
}