using April_17_homework.Data;

namespace April_17_homework.Web.Models
{
    public class ViewImageViewModel
    {
        public Image Image { get; set; }
        public bool ShouldShow { get; set; }
        public int Id { get; set; }
        public int Views { get; set; }
        public bool NotCorrect { get; set; }
        public List<int> Ids { get; set; }
    }
}
