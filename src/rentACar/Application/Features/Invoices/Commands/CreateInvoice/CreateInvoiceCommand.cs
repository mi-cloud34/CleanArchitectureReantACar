using Application.Features.Invoices.Constants;
using Application.Features.Invoices.Dtos;
using Application.Features.Invoices.Dtos.EventInvoiceBus;
using Application.Features.Invoices.Rules;
using Application.Services.AzureBus;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Mailing;
using Core.Security.Entities;
using Domain.Entities;
using MediatR;
using static Application.Features.Invoices.Constants.OperationClaims;
using static Domain.Constants.OperationClaims;

namespace Application.Features.Invoices.Commands.CreateInvoice;

public class CreateInvoiceCommand : IRequest<CreatedInvoiceDto>, ISecuredRequest
{
    public int CustomerId { get; set; }
    public string No { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime RentalStartDate { get; set; }
    public DateTime RentalEndDate { get; set; }
    public short TotalRentalDate { get; set; }
    public decimal RentalPrice { get; set; }

    public string[] Roles => new[] { Admin, InvoiceAdd };

    public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, CreatedInvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly InvoiceBusinessRules _invoiceBusinessRules;
        private readonly IAzureBusService<Invoice> _azureBusService;
        private readonly IMailService _mailService;

        public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IMapper mapper, IMailService mailService,
                                           InvoiceBusinessRules invoiceBusinessRules,IAzureBusService<Invoice> azureBusService)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _invoiceBusinessRules = invoiceBusinessRules;
            _azureBusService= azureBusService;
            _mailService= mailService;
        }

        public async Task<CreatedInvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            User mappedUser = _mapper.Map<User>(request);
            Invoice mappedInvoice = _mapper.Map<Invoice>(request);
            Invoice createdInvoice = await _invoiceRepository.AddAsync(mappedInvoice);
            CreatedInvoiceDto createdInvoiceDto = _mapper.Map<CreatedInvoiceDto>(createdInvoice);
            Invoice listInvoice = await _invoiceRepository.AddAsync(mappedInvoice);
            InvoiceListDto invoiceListDto = _mapper.Map<InvoiceListDto>(listInvoice);

            Mail mail = new Mail()
            {
                Subject = InvoiceMessages.InvoiceMailSubject,
                TextBody = InvoiceMessages.InvoiceMailBody,
                //HtmlBody="",
                //Attachments=
                ToFullName = mappedUser.FirstName,
                ToEmail = mappedUser.Email,

            };
            var invoiceCreatedEvent = new CreatedEventInvoice()
            {
                CreatedAt = DateTime.Now,
                Id = createdInvoiceDto.Id,
                CustomerName = invoiceListDto.CustomerName,
                No = invoiceListDto.No,
                RentalStartDate = invoiceListDto.RentalStartDate,
                RentalEndDate = invoiceListDto.RentalEndDate,
                RentalPrice = invoiceListDto.RentalPrice,
                
            };
            if (invoiceListDto != null)
            {
                await _azureBusService.CreateQueeIfNotExists(InvoiceMessages.InvoiceCreatedQuee);
                await _azureBusService.sendMessageToQuee(invoiceCreatedEvent, InvoiceMessages.InvoiceCreatedQuee);

            }

            await _azureBusService.ConsumerQuee<CreatedEventInvoice>(InvoiceMessages.InvoiceCreatedQuee, i =>
            {
                invoiceCreatedEvent.Id = i.Id;
                invoiceCreatedEvent.CustomerName = i.CustomerName;
                invoiceCreatedEvent.No = i.No;
                invoiceCreatedEvent.RentalStartDate = i.RentalStartDate;
                invoiceCreatedEvent.CreatedDate = DateTime.Now;
                invoiceCreatedEvent.RentalEndDate = i.RentalEndDate;
                invoiceCreatedEvent.RentalPrice = i.RentalPrice;
                _mailService.SendMail(mail);
            });
            return createdInvoiceDto;
        }
    }
}