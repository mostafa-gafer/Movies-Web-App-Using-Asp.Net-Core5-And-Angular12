using Movies.CORE.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.CORE.Entities
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Name Is Required Please")]
        [StringLength(10)] 
        [FirstLetterUpperCaseAttribute]  //to validate firstletter by custom atrribute
        public string Name { get; set; }

        //[Range(18, 120)]
        //public int Age { get; set; }

        //[CreditCard]
        //public string CreditCard { get; set; }

        //[Url]
        //public string Url { get; set; }
    }
}
