using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.RatingDTO.Request
{
    public class CreateRatingRequestDTO
    {
        [Required]
        public Guid OrderDetailId { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Score must be between 1 and 5.")]
        public int Score { get; set; } // 1-5
        public string? Comment { get; set; }
    }
}
