#nullable disable

using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Eportfolio.Models
{
    public class ProjectModel
    {
        [Required(ErrorMessage = "Project Name is required.")]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Project Description")]
        public string ProjectDesc { get; set; }
    }
}
