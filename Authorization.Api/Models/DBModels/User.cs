﻿using System;
using System.Collections.Generic;

namespace Authorization.Api.Models.DBModels
{
    public partial class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }
}
