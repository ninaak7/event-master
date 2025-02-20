using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Models
{
    public class Ticket : BaseEntity
    {
        [Required]
        public String? Type { get; set; }
        [Required]
        [Range(1.0, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public double Price { get; set; }
    }
}
