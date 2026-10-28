using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCompare.Models
{
    public class CloudService
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Service name is required")]
        [StringLength(100)]
        [Display(Name = "Service Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        [StringLength(50)]
        public string Category { get; set; }

        [Display(Name = "Provider")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a provider")]
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(300)]
        public string Description { get; set; }

        [Display(Name = "Starting Price (USD / month)")]
        [Range(0, 100000, ErrorMessage = "Price must be between 0 and 100000")]
        public decimal StartingPrice { get; set; }

        [Display(Name = "Free Tier Available")]
        public bool FreeTier { get; set; }

        [Display(Name = "Uptime SLA (%)")]
        [Range(90, 100, ErrorMessage = "SLA must be between 90 and 100")]
        public decimal UptimeSla { get; set; }

        [Display(Name = "Regions")]
        [Range(1, 200, ErrorMessage = "Regions must be between 1 and 200")]
        public int RegionCount { get; set; }

        public ICollection<Review> Reviews { get; set; }

        [NotMapped]
        public double AvgRating =>
            Reviews != null && Reviews.Any() ? Math.Round(Reviews.Average(r => r.Rating), 1) : 0;
    }
}
