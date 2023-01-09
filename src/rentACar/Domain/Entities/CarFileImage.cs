
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

    public class CarFileImage : File
    
     {
        public string Name { get; set; }
        public int? CarId { get; set; }
        public virtual Car Car { get; set; }
       public string Path { get; set; }
    public CarFileImage()
        {
           
        }

        public CarFileImage(int id, string name,int carId, string path,string storage) : this()
        {
            Id = id;
            Name = name;
           // CarId = carId;
            Path = path;
            Storage = storage;
        }
    }
