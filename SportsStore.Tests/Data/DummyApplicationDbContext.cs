using SportsStore.Models.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SportsStore.Tests.Data
{
    public class DummyApplicationDbContext
    {
        private readonly City _gent;
        private readonly City _antwerpen;

        private readonly Category _watersports;
        private readonly Category _chess;
        private readonly IList<Product> _products;

        public DummyApplicationDbContext()
        {
            _products = new List<Product>();
            _gent = new City { Name = "Gent", Postalcode = "9000" };
            _antwerpen = new City { Name = "Antwerpen", Postalcode = "3000" };
            _watersports = new Category("WaterSports");
            Soccer = new Category("Soccer")
            {
                CategoryId = 1
            };
            _chess = new Category("Chess");
            Soccer.AddProduct("Football", 25, "WK colors");
            Football.ProductId = 1;
            Soccer.AddProduct("Corner flags", 34, "Give your playing field that professional touch");
            Soccer.AddProduct("Stadium", 75, "Flat-packed 35000-seat stadium miniature");
            Soccer.AddProduct("Running shoes", 95, "Protective and fashionable");
            RunningShoes.ProductId = 4;
            _watersports.AddProduct("Surf board", 275, "A boat for one person");
            _watersports.AddProduct("Kayak", 170, "High quality");
            _watersports.FindProduct("Kayak").Availability = Availability.ShopOnly;
            _watersports.AddProduct("Lifejacket", 49, "Protective and fashionable");
            _chess.AddProduct("Thinking cap", 16, "Improve your brain efficiency by 75%");
            _chess.AddProduct("Unsteady chair", 30, "Secretly give your opponent a disadvantage");
            _chess.AddProduct("Human chess board", 75, "A fun game for the whole extended family!");
            _chess.AddProduct("Bling-bling King", 1200, "Gold plated, diamond-studded king");
            foreach (Category c in Categories)
            {
                foreach (Product p in c.Products)
                {
                    _products.Add(p);
                }
            }
        }

        public IEnumerable<Category> Categories => new List<Category>
        {
            _watersports,
            Soccer,
            _chess
        };

        public IEnumerable<City> Cities => new List<City> { _gent, _antwerpen };

        public Customer Customer => new Customer("jan", "Janneman", "Jan", "Nieuwstraat 100", _gent);

        public IEnumerable<Product> Products => _products;

        public IEnumerable<Product> ProductsOnline => _products.Where(p => p.Availability == Availability.OnlineOnly || p.Availability == Availability.ShopAndOnline);

        public Product Football => Soccer.FindProduct("Football");

        public Product RunningShoes => Soccer.FindProduct("Running shoes");

        public Category Soccer { get; }
    }
}
