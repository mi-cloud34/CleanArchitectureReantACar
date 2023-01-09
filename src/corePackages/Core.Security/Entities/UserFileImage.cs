using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Security.Entities;

public class UserFileImage : File

{
    public string Name { get; set; }
    public int? UserId { get; set; }
    public virtual User Car { get; set; }
    public string Path { get; set; }
    public UserFileImage()
    {
     
    }

    public UserFileImage(int id, string name, int carId, string path, string storage) : this()
    {
        Id = id;
        Name = name;
        UserId = carId;
        Path = path;
        Storage = storage;
    }

    public static implicit operator UserFileImage(Collection<UserFileImage> v)
    {
        throw new NotImplementedException();
    }
}
