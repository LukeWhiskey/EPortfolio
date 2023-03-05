using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eportfolio.Models
{
    public class LayoutViewModel
    {
        public int ProjectId { get; set; }

        public string ProjectName { 
            get { 
                return ProjectName; 
            } 
            set { 
                if (value == null)
                {
                    ProjectName = "Unk";
                }
            } 
        }
    }
}

