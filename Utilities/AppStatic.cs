﻿using Lieferliste_WPF.Data;
using Lieferliste_WPF.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lieferliste_WPF.Utilities
{
    public static class AppStatic
    {
        public static User User { get; private set; }
        private static readonly DataContext _db = new();

        static AppStatic()
        {
            LoadData();
            User ??= new User();
        }

        private static void LoadData()
        {
            try
            {
                string us = Environment.UserName;
                var q = _db.Users
                    .Include(x => x.UserRoles)
                    .Include(x => x.UserWorkAreas)
                    .Include(x => x.UserCosts)
                    .Where(x => x.UserIdent == us);

                User = q.First();
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show("User wurde nicht erkannt!/n" + e.Message,"USER ERROR",MessageBoxButton.OK,MessageBoxImage.Error);
                throw;
            }

        }
    }
}