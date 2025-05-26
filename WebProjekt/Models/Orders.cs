namespace WebProjekt.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Warenkorb;
        public List<OrderArticle> OrderArticles { get; set; }
    }
}
