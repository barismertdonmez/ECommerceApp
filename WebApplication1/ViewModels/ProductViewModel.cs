using Entity;
namespace WebUI.ViewModels
{
    public class PageInfo
    {
        public int TotalItems { get; set; }
        public int ItemPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string CurrentCategory { get; set; }
        public int TotalPAges()
        {
            return (int)Math.Ceiling((decimal)TotalItems/ItemPerPage);
        }
    }
    public class ProductViewModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories{ get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
