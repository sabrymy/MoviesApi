using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//class where we can add configuration providers
namespace MoviesApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            //add optional custom json file
           //  .ConfigureAppConfiguration((env,config)=> {
            //       config.AddJsonFile("custom json", true, reloadOnChange: true);
             //  }
               //   )

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //add provider for logging
              //      webBuilder.ConfigureLogging(logginBuilder =>
              //      {
              //          logginBuilder.AddProvider()
              //      })



                    webBuilder.UseStartup<Startup>();
                });
    }
}
