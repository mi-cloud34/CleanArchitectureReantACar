using Core.Persistence.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Dtos.EventInvoiceBus
{
    public class CreatedEventInvoice:Invoice
    {
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; }
        public string No { get; set; }
        public DateTime RentalStartDate { get; set; }
        public DateTime RentalEndDate { get; set; }
        public decimal RentalPrice { get; set; }

    }
}
