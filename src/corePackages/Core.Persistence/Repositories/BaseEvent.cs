using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistence.Repositories
{
    public class BaseEvent
    {
        public int Id { get; set; }
        public DateTime onCreate { get; set; }
        public BaseEvent()
        {
        }

        public BaseEvent(int id) : this()
        {
            Id = id;
        }
    }
    
}
