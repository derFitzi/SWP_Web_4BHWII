namespace WebProjekt.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Warenkorb;
        public ICollection<OrderArticle> OrderArticles { get; set; }
    }
}
