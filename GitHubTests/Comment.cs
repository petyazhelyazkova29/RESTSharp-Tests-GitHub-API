﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubTests
{
    public class Comment
    {
        public int id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string body { get; set; }   
    }
    
}
