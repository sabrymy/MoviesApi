using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesApi.Services
{
    public class WriteToFileHostedService : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string fileName = "File1.txt";
        private Timer timer ;
        public WriteToFileHostedService(IWebHostEnvironment env )
        {
            this.env = env;
        }
        //executed when app starts
        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            WriteToFile("Process started");
            // throw new NotImplementedException();
            timer = new Timer(DoWork,null,TimeSpan.Zero,TimeSpan.FromSeconds(5) );
            return Task.CompletedTask;
        }
        //executed when app stops not guranted to execute after app starts
        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            WriteToFile("Process endded");
            return Task.CompletedTask;
            // throw new NotImplementedException();
        }
        private void DoWork(object state)
        {
            WriteToFile("Process on Going" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:"));
        }


        private void  WriteToFile(string message)
        {
            var path = $@"{env.ContentRootPath}\wwwroot\{fileName}";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
            }
        }


    }
}
