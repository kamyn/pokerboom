﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBoom.Shared.Models
{
    public class LoginResultViewModel
    {
        public bool Success { get;set; }
        public string Error { get; set; } 
        public string Token { get; set; }
    }
}
