namespace WebProjekt.Models
{
    public class OrderArticle
    {
        public int OrderArticleId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public Order Order { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
