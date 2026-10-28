using System.ComponentModel.DataAnnotations;

namespace CloudCompare.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int CloudServiceId { get; set; }
        public CloudService CloudService { get; set; }

        [Required(ErrorMessage = "Your name is required")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "Name must be 2 to 60 characters")]
        [Display(Name = "Your Name")]
        public string ReviewerName { get; set; }

        [Range(1, 5, ErrorMessage = "Please select a rating")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Please write a comment")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Comment must be 10 to 500 characters")]
        public string Comment { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
