
using Application.Services.ImageService.StorageService;
using Core.Storage.Local;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;



namespace Application.Services.ImageService;

public class LocalStorage : Storage,ILocalStorage
{
    private readonly string rootpath;

    public LocalStorage(IWebHostEnvironment webHostEnvironment)
    {
        rootpath = webHostEnvironment.WebRootPath;
    }


    private async Task<bool> CopyFileAsync(string path, IFormFile file)
    {
        try
        {
            await using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None,
                1024 * 1024, useAsync: false);

            await file.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
            return true;
        }
        catch (Exception ex)
        {
            //todo log!
            throw ex;
        }
    }

    public async Task<List<(string fileName, string pathOrContainerName)>> UploadsAsync(string path,
        IFormFileCollection files)
    {
        string uploadPath = Path.Combine(rootpath, path);
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);
        List<(string fileName, string path)> datas = new();
        foreach (IFormFile file in files)
        {
            string fileNewName = await FileRenameAsync(path, file.FileName, HasFile);
            await CopyFileAsync($"{uploadPath}\\{fileNewName}", file);
            datas.Add((fileNewName, $"{path}\\{fileNewName}"));
        }
        return datas;
    }
    public async Task DeleteAsync(string path, string fileName) => File.Delete($"{rootpath}\\{path}\\{fileName}");
    public async Task<(string fileName, string pathOrContainerName)> UploadAsync(string path, IFormFile file)
    {
        string uploadPath = Path.Combine(rootpath, path);
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);
       // (string fileName, string path)
         (string fileName, string path) data = new();
       
            string fileNewName = await FileRenameAsync(path, file.FileName, HasFile);
            await CopyFileAsync($"{uploadPath}\\{fileNewName}", file);
            data=(file.FileName, $"{path}\\{fileNewName}");
            return data;
            //File.Create(data);
    }
    public List<string> GetByNameFilesAsync(string path)
    {
        DirectoryInfo directoryInfo = new(Path.Combine(rootpath, path));
        return directoryInfo.GetFiles().Select(c => c.Name).ToList();
    }

    public bool HasFile(string path, string fileName) => File.Exists($"{rootpath}\\{path}\\{fileName}");



  


    public async Task<string?> GetByNameFileAsync(string pathOrContainerName,string fileName)
    {
        DirectoryInfo directoryInfo = new(Path.Combine(rootpath, pathOrContainerName));
        var file = directoryInfo.GetFiles().Where(x=>x.Name ==fileName);
        return  await Task.FromResult(file.FirstOrDefault().FullName);
    }
}
//iamges/car/audi.jpg
// hocam metod asenkron açılmıyor bu şkile çalışır ama asekron olmasını isterseniz illa böyle bişey yapılabiliri ama null da gelebilir o yüzden