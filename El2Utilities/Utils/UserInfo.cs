﻿using El2Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace El2Core.Utils
{
    public readonly struct UserInfo
    {
        public static string? PC => _PC ?? string.Empty;
        public static User User => _User;

        private static string? _PC;
        private static User _User;

        public void Initialize(string PC, User Usr)
        {
            _PC = PC;
            _User = Usr;
        }
    }
}
