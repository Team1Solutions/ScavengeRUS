﻿namespace ScavengeRUs.Models.Entities
{
	/// <summary>
	/// This is the weak entity for the many to many relationship between Hunt and 
	/// Location(tasks)
	/// </summary>
    public class HuntLocation
    {
        public int Id { get; set; }
        public int HuntId { get; set; } //Foreign key
        public Hunt? Hunt { get; set; } //Navigation property
        public int LocationId { get; set; } //Foreign key
        public Location? Location { get; set; } //Navigation property
    }
}
