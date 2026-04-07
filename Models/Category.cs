using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Models
{
    public class Category
    {
        public int Id { get; set; }

        // Name is required and can't be longer than 50 characters
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
