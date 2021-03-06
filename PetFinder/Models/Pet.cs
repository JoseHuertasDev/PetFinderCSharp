﻿using System;
using PetFinder.Areas.Identity;

#nullable disable

namespace PetFinder.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AnimalTypeId { get; set; }
        public int CityId { get; set; }
        public int GenderId { get; set; }
        public DateTime Date { get; set; }
        public string PhoneNumber { get; set; }
        public string Photo { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public byte Found { get; set; }

        public virtual AnimalType AnimalType { get; set; }
        public virtual City City { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}