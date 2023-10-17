using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using El2Utilities;
using System.Windows;
using El2Utilities.Models;
using Microsoft.EntityFrameworkCore;
using Lieferliste_WPF.Properties;

namespace Lieferliste_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            string defaultConnection = Settings.Default.ConnectionBosch;
            services.AddDbContextFactory<DB_COS_LIEFERLISTE_SQLContext>(
                options =>
                    options.UseSqlServer(defaultConnection));
           
        }
       
    }
}
