namespace WebProjekt.Models
{
    public class Article
    {
        public int ArticleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Brand { get; set; }
        public ArticleType Type { get; set; }
        public List<OrderArticle>? orderArticles { get; set; }
    }

    public enum ArticleType
    {
        Electronics,
        Clothing,
        Food,
        Furniture,
        Toys
    }
}
