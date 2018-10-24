
namespace KRF.Core.Entities.AccessControl
{
    public class Pages
    {
        public int PageID { get; set; }

        /// <summary>
        /// Holds the name of a page.
        /// </summary>
        public string PageName { get; set; }

        public bool Active { get; set; }
    }
}
