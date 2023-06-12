using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Services
{
    public class InAppStorageService : IFileStorageService
{
        private readonly string connectionString;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IWebHostEnvironment env;
            
        public InAppStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.env = env;
        }

        public async Task DeleteFile(string fileRoute, string containerName)
        {
            string savingPath = Path.Combine(containerName, containerName);
            var deletionTask = Task.Run(() => File.Delete(savingPath));
            await deletionTask.ContinueWith(task => task.IsCompletedSuccessfully ) ;
        }

        public async Task<string> EditFile(byte[] content, string extension, string containerName, string fileRoute, string contentType)
        {
            await DeleteFile(fileRoute, containerName);
            return await SaveFile(content, extension, containerName, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string containerName, string contentType)
        {
            //retrieve a storage account

            //set permissions to anonymous to read  Blob only not the rest of container data 
            var filename = $"{Guid.NewGuid()}.{extension}";
            string folder = Path.Combine(env.WebRootPath, containerName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string savingPath = Path.Combine(folder, filename);
            await File.WriteAllBytesAsync(savingPath, content);
            var currentUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var pathForDatabase = Path.Combine(currentUrl, containerName, filename).Replace("\\", "/");
            return pathForDatabase;


        }

    }
}
