using She___He_Store.Models;

namespace She___He_Store.Models
{
    public class JoinTables
    {
        public Category Category { get; set; }
        public Customer Customer { get; set; }
        public Product Product { get; set; }
        public OrderDetail OrderDetail { get; set; }
    }
}
