namespace The_Post.Models.VM
{
    public class AdminDashboardVM
    {
        public int TotalArticles { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalSubscribers { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int ExpiredSubscriptions { get; set; }
        public int RecentActivities { get; set; }
    }
}
