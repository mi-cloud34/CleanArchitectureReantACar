using Core.Persistence.Repositories;
using Core.Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Dtos.EventUserBus
{
    public class DeleteEventUser:User
    {
        public DateTime CreatedAt { get; set; }
    }
}
