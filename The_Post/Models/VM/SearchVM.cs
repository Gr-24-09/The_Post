namespace The_Post.Models.VM
{
    public class SearchVM
    {
        public List<Article> Articles { get; set; } = new List<Article>();
        public string SearchTerm { get; set; }
    }
}
