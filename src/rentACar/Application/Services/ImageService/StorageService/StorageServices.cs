using Core.Storage;
using Microsoft.AspNetCore.Http;

namespace Application.Services.ImageService.StorageService;

public class StorageServices : IStorageService
{
    readonly IStorage _storage;

    public StorageServices(IStorage storage) => _storage = storage;

    public string StorageName { get => _storage.GetType().Name; }

    public async Task DeleteAsync(string pathOrContainerName, string fileName)
           => await _storage.DeleteAsync(pathOrContainerName, fileName);


    public List<string> GetByNameFilesAsync(string pathOrContainerName)
        => _storage.GetByNameFilesAsync(pathOrContainerName);

    public bool HasFile(string pathOrContainerName, string fileName)
        => _storage.HasFile(pathOrContainerName, fileName);

    public Task<List<(string fileName, string pathOrContainerName)>> UploadsAsync(string pathOrContainerName, IFormFileCollection files)
        => _storage.UploadsAsync(pathOrContainerName, files);

    public Task<(string fileName, string pathOrContainerName)> UploadAsync(string pathOrContainerName, IFormFile file) => _storage.UploadAsync(pathOrContainerName, file);

    //public Task<string> GetAsync(string fileName)
    //{
    //    throw new NotImplementedException();
    //}

    public async Task<string> GetByNameFileAsync(string pathOrContainerName,string fileName)
    {
        return await _storage.GetByNameFileAsync(pathOrContainerName, fileName);
    } 
    
}