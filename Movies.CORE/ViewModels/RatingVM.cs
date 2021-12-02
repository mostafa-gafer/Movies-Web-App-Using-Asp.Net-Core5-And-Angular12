﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.CORE.ViewModels
{
    public class RatingVM
    {

        [Range(1, 5)]
        public int Rating { get; set; }
        public int MovieId { get; set; }
       
    }
}
