
namespace SportsStore.Models.Domain
{
    public class OrderLine : CartLine
    {
        public int OrderId { get; private set; }
        public int ProductId { get; private set; }
        public decimal Price { get; set; }
    }
}