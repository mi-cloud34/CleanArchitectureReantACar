using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services.ImageService.StorageService;
using Core.Storage.Local;
using Core.Storage.Azure;

namespace Application.Services.ImageService;

    public class AzureStorage : Storage, IAzureStorage
    {
        readonly BlobServiceClient _blobServiceClient;
        BlobContainerClient _blobContainerClient;
        public AzureStorage(IConfiguration configuration)
        {
            _blobServiceClient = new(configuration["Storage:Azure"]);
        }
        public async Task DeleteAsync(string containerName, string fileName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
            await blobClient.DeleteAsync();
        }

    public  async Task<string?> GetByNameFileAsync(string pathOrContainerName, string fileName)
    {
        _blobContainerClient = _blobServiceClient.GetBlobContainerClient(pathOrContainerName);
       // boş gelme durumunu mutlaka bu fonksiyonu açğırdınız yerde kontrol edin yada ? ile işsratletyin nu  değerin boş olabileceği manasına geliyor 

        //anladım hocam teşekür ederim 
        // başka bir soru var yok ben kaçayım :D hocam sormak istedigim iki durum vr tabi
        //ses problemi olmasaydı daha iyi olurdu 
        return await Task.FromResult(_blobContainerClient.GetBlobs().FirstOrDefault(b => b.Name == fileName).Name);
    }

    public List<string> GetByNameFilesAsync(string containerName)
        {
        _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        return _blobContainerClient.GetBlobs().Select(b => b.Name).ToList();
    }


    public bool HasFile(string containerName, string fileName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            return _blobContainerClient.GetBlobs().Any(b => b.Name == fileName);
        }

    public async Task<(string fileName, string pathOrContainerName)> UploadAsync(string containerName, IFormFile file)
    {
        _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
       
        await _blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

        (string fileName, string pathOrContainerName) data = new();
      
            string fileNewName = await FileRenameAsync(containerName, file.Name, HasFile);

            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileNewName);
            await blobClient.UploadAsync(file.OpenReadStream());
            data=((fileNewName, $"{containerName}/{fileNewName}"));

        return data;   }

    public async Task<List<(string fileName, string pathOrContainerName)>> UploadsAsync(string containerName, IFormFileCollection files)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await _blobContainerClient.CreateIfNotExistsAsync();
            await _blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

            List<(string fileName, string pathOrContainerName)> datas = new();
            foreach (IFormFile file in files)
            {
                string fileNewName = await FileRenameAsync(containerName, file.Name, HasFile);

                BlobClient blobClient = _blobContainerClient.GetBlobClient(fileNewName);
                await blobClient.UploadAsync(file.OpenReadStream());
                datas.Add((fileNewName, $"{containerName}/{fileNewName}"));
            }
            return datas;
        }
    }
