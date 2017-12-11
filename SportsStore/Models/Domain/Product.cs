using Newtonsoft.Json;
using System;

namespace SportsStore.Models.Domain
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Product
    {
        #region Fields and Properties
        private string _name;
        private int _price;
        private Category _category;

        [JsonProperty]
        public int ProductId { get; set; }
        public string Name {
            get { return _name; }
            set {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("You must give a name");
                if (value.Length < 5 || value.Length > 100)
                    throw new ArgumentException("Name must contain between 5 and 100 characters");
                _name = value;
            }
        }

        public string Description { get; set; }
        public int Price {
            get { return _price; }
            set {
                if (value < 1 || value > 3000)
                    throw new ArgumentException("Price must be in the range 1 - 3000");
                _price = value;
            }
        }

        public bool InStock { get; set; }
        public Availability Availability { get; set; }
        public DateTime? AvailableTill { get; set; }
        public Category Category {
            get { return _category; }
            set {
                _category = value ?? throw new ArgumentException("Category is compulsory");
            }
        }
        #endregion

        #region Constructors and methods
        public Product()
        {
            InStock = true;
            Availability = Availability.ShopAndOnline;
        }

        public Product(string name, int price, Category category, string description = null) : this()
        {
            Name = name;
            Price = price;
            Description = description;
            Category = category;
        }

        public Product(int productId, string name, int price, Category category) : this(name, price, category)
        {
            ProductId = productId;
        }

        public override bool Equals(object obj)
        {
            Product p = obj as Product;
            return p != null && p.ProductId == ProductId;
        }

        public override int GetHashCode()
        {
            return ProductId;
        }
        #endregion
    }
}