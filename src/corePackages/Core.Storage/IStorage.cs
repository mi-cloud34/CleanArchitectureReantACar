
using Microsoft.AspNetCore.Http;

namespace Core.Storage;

public interface IStorage
{
    Task<List<(string fileName, string pathOrContainerName)>> UploadsAsync(string pathOrContainerName,IFormFileCollection files);
    Task<(string fileName, string pathOrContainerName)> UploadAsync(string pathOrContainerName, IFormFile file);

    Task DeleteAsync(string pathOrContainerName, string fileName);
    List<string> GetByNameFilesAsync(string pathOrContainerName);
    Task<string> GetByNameFileAsync(string pathOrContainerName, string fileName);
    //Task<(string fileName, string pathOrContainerName)> GetByNameFilesAsync(string pathOrContainerName, string fileName);
    //Task<List<T>> GetListAsync(string fileName);
    bool HasFile(string pathOrContainerName, string fileName);
    //hocam ilki 
    //mesela burda file neden hata veriyor olabilir sizce çünkü mimariye aykırı c# da böyle birşey yok yani değiken ismi verm
    // vermeniz gereksiz tek tip dönğceksiniz üste tuple nesnesi dönülmüş
    // eğer bir metod birden fazla şey dönecekse ya dönen değerler classlar ile taşıacak veya primitve olarak tuple ile

    //Task<string> getName(string fileNmae); // bu yeterli eğer 2 şey dönecekseniz
    //(string filename , string extension) GetFiles(string filename)
    //{
    //    return filename; // bakın bu hata veriyor çünkğ tek obje dönüyom
    //    return (filename, filename); // burda vermedi gördüğünüz gibi eyvallah hocam rica ederim iyi çalışmalar
    //    //hocam bide size kısa bi yer sormak istiyorum 
    //}
}
