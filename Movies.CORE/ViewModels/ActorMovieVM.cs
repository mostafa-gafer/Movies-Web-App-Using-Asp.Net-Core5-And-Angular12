﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.CORE.ViewModels
{
    public class ActorMovieVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Character { get; set; }
        public string Picture { get; set; }

        public int Order { get; set; }

    }
}
