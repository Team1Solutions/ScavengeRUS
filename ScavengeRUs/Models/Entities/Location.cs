// Import necessary namespaces for data annotations and handling.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Define the namespace for the models of ScavengeRUs application.
namespace ScavengeRUs.Models.Entities
{
    // Represents a Location entity within the application.
    public class Location
    {
        // Unique identifier for the Location entity.
        [Key]
        public int Id { get; set; }
        
        // The name of the place, required field.
        [Required]
        public string Place { get; set; } = string.Empty;
        
        // Latitude of the location, required field. Displayed as "Latitude" in UI.
        [Required]
        [Display(Name = "Latitude")]
        public double? Lat { get; set; }
        
        // Longitude of the location, required field. Displayed as "Longitude" in UI.
        [Required]
        [Display(Name = "Longitude")]
        public double? Lon { get; set; }
        
        // A task or question associated with the location, required field.
        [Required]
        public string Task { get; set; } = string.Empty;

        // An optional access code for entering or participating in the location's activity.
        [Display(Name = "Access Code")]
        public string? AccessCode { get; set; }

        // An optional QR code associated

