using Movies.CORE.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.CORE.ViewModels
{
    public class GenreVM
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Name Is Required Please")]
        [StringLength(10)] 
        [FirstLetterUpperCaseAttribute]  //to validate firstletter by custom atrribute
        public string Name { get; set; }
        

    }
}
