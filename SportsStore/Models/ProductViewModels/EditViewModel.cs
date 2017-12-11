using SportsStore.Models.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Models.ProductViewModels
{
    public class EditViewModel
    {
        [Required(ErrorMessage="{0} is required")]
        [StringLength(100, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 5)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [Range(1, 3000, ErrorMessage = "{0} must be in the range {1} - {2}")]
        public int Price { get; set; }
        [Display(Name = "In stock")]
        public bool InStock { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public Availability Availability { get; set; }
        [Display(Name = "Available till")]
        [DataType(DataType.Date)]
        public DateTime? AvailableTill { get; set; }
        [Display(Name = "Category")]
        [Required(ErrorMessage = "{0} is required")]
        public int CategoryId { get; set; }

        public EditViewModel()
        {

        }

        public EditViewModel(Product p)
        {
            Name = p.Name;
            Description = p.Description;
            Price = p.Price;
            InStock = p.InStock;
            Availability = p.Availability;
            AvailableTill = p.AvailableTill;
            CategoryId = p.Category?.CategoryId ?? 0;
        }
    }
}
