using SportsStore.Models.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsStore.Data
{
    public class SportsStoreDataInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        public SportsStoreDataInitializer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InitializeData()
        {
            _dbContext.Database.EnsureDeleted();
            if (_dbContext.Database.EnsureCreated())
            {
                Category watersports = new Category("WaterSports");
                Category soccer = new Category("Soccer");
                Category chess = new Category("Chess");
                var categories = new List<Category>
                                     {
                                         watersports,
                                         soccer,
                                         chess
                                     };

                _dbContext.Categories.AddRange(categories);

                soccer.AddProduct("Football", 25, "WK colors");
                soccer.AddProduct("Corner flags", 34, "Give your playing field that professional touch");
                soccer.AddProduct("Stadium", 75, "Flat-packed 35000-seat stadium miniature");
                soccer.AddProduct("Running shoes", 95, "Protective and fashionable");
                watersports.AddProduct("Surf board", 275, "A boat for one person");
                watersports.AddProduct("Kayak", 170, "High quality");
                watersports.AddProduct("Lifejacket", 49, "Protective and fashionable");
                watersports.FindProduct("Lifejacket").Availability = Availability.ShopOnly;
                chess.AddProduct("Thinking cap", 16, "Improve your brain efficiency by 75%");
                chess.AddProduct("Unsteady chair", 30, "Secretly give your opponent a disadvantage");
                chess.AddProduct("Human chess board", 75, "A fun game for the whole extended family!");
                chess.AddProduct("Bling-bling King", 1200, "Gold plated, diamond-studded king");
                chess.FindProduct("Bling-bling King").InStock = false;

                City gent = new City { Name = "Gent", Postalcode = "9000" };
                City antwerpen = new City { Name = "Antwerpen", Postalcode = "3000" };
                City[] cities = new City[] { gent, antwerpen };
                _dbContext.Cities.AddRange(cities);

                Random r = new Random();
                for (int i = 1; i < 10; i++)
                {
                    Customer klant = new Customer("student" + i, "Student" + i, "Jan", "Nieuwstraat 10", cities[r.Next(2)]);

                    if (i <= 5)
                    {
                        Cart cart = new Cart();
                        cart.AddLine(soccer.FindProduct("Football"), 1);
                        cart.AddLine(soccer.FindProduct("Corner flags"), 2);
                        klant.PlaceOrder(cart, DateTime.Today.AddDays(10), false, klant.Street, klant.City);
                    }
                    _dbContext.Customers.Add(klant);
                }
                _dbContext.SaveChanges();
            }
        }
    }
}
