using Core.Persistence.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Rentals.Dtos.EventCarBus
{
    public class CreatedEventRental :Rental
    {

        public string CarModelName { get; set; }
        public string CustomerMail { get; set; }
        public DateTime RentStartDate { get; set; }
        public DateTime RentEndDate { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
