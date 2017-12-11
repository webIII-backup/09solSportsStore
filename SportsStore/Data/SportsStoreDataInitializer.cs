﻿using Microsoft.AspNetCore.Identity;
using SportsStore.Models.Domain;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportsStore.Data
{
    public class SportsStoreDataInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public SportsStoreDataInitializer(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
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
                        var userName = klant.CustomerName + "@hogent.be";
                        await CreateUser(userName, userName, "P@ssword1!", "Customer");
                    }
                    _dbContext.Customers.Add(klant);
                }
                await CreateUser("admin@sportsstore.be", "admin@sportsstore.be", "P@ssword1!", "Admin");
                _dbContext.SaveChanges();
            }
        }

        private async Task CreateUser(string userName, string email, string password, string role)
        {
            var user = new ApplicationUser { UserName = userName, Email = email };
            await _userManager.CreateAsync(user, password);
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role));
        }
    }
}
