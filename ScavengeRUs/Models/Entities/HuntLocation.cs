namespace ScavengeRUs.Models.Entities
{
    // This class represents a link in a many-to-many relationship between Hunts and Locations.
    // Each instance of HuntLocation links a single Hunt to a single Location, enabling
    //The modeling of Hunts that involve multiple Locations and Locations that can be
    // part of multiple Hunts.
    public class HuntLocation
    {
        // Unique identifier for the HuntLocation record. This serves as the primary key in the database.
        public int Id { get; set; }
        
        // Foreign key referencing the Hunt entity. This is part of what establishes the many-to-many relationship.
        // It identifies which Hunt this HuntLocation is part of.
        public int HuntId { get; set; } 
        
        // Navigation property for the Hunt entity. This allows for direct access to the Hunt object that
        // this HuntLocation is associated with, facilitating operations like loading Hunt details
        // from the HuntLocation context.
        public Hunt? Hunt { get; set; }
        
        // Foreign key referencing the Location entity. This is the other half of establishing the
        // many-to-many relationship, identifying which Location is involved in this HuntLocation.
        public int LocationId { get; set; }
        
        // Navigation property for the Location entity. Similar to the Hunt navigation property,
        // this provides direct access to the Location object associated with this HuntLocation,
        // making it easier to load Location details in the context of this HuntLocation.
        public Location? Location { get; set; }
    }
}
