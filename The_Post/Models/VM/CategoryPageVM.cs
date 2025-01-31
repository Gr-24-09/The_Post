namespace The_Post.Models.VM
{
    public class CategoryPageVM
    {
        public List<Article> Articles { get; set; } = new List<Article>();
        public string Category { get; set; }

    }
}
