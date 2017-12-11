using SportsStore.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Models.ProductViewModels
{
    public class EditViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        [Display(Name = "In stock")]
        public bool InStock { get; set; }
        public Availability Availability { get; set; }
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
            CategoryId = p.Category?.CategoryId ?? 0;
        }
    }
}
