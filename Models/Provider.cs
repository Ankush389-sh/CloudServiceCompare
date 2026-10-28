using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCompare.Models
{
    public class Provider
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Provider name is required")]
        [StringLength(80)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Headquarters is required")]
        [StringLength(100)]
        public string Headquarters { get; set; }

        [Required(ErrorMessage = "Website is required")]
        [Url(ErrorMessage = "Enter a valid URL starting with http:// or https://")]
        [StringLength(200)]
        public string Website { get; set; }

        [Display(Name = "Founded Year")]
        [Range(1950, 2100, ErrorMessage = "Enter a valid year")]
        public int FoundedYear { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(300)]
        public string Description { get; set; }

        public ICollection<CloudService> Services { get; set; }

        [NotMapped]
        public int ReviewCount =>
            Services == null ? 0 : Services.Where(s => s.Reviews != null).Sum(s => s.Reviews.Count);

        [NotMapped]
        public double AvgRating
        {
            get
            {
                if (Services == null) return 0;
                var all = Services.Where(s => s.Reviews != null).SelectMany(s => s.Reviews).ToList();
                return all.Any() ? Math.Round(all.Average(r => r.Rating), 1) : 0;
            }
        }
    }
}
