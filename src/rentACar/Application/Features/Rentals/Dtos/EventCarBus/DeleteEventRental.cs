using Core.Persistence.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Rentals.Dtos.EventCarBus
{
    public class DeleteEventRental : Rental
    {
        public DateTime CreatedAt { get; set; }
    }
}
