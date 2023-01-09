using Application.Features.Invoices.Constants;
using Application.Features.Invoices.Dtos;
using Application.Features.Invoices.Dtos.EventInvoiceBus;
using Application.Features.Invoices.Rules;
using Application.Services.AzureBus;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Domain.Entities;
using MediatR;
using static Application.Features.Invoices.Constants.OperationClaims;
using static Domain.Constants.OperationClaims;

namespace Application.Features.Invoices.Commands.DeleteInvoice;

public class DeleteInvoiceCommand : IRequest<DeletedInvoiceDto>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => new[] { Admin, InvoiceDelete };

    public class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand, DeletedInvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly InvoiceBusinessRules _invoiceBusinessRules;
        private readonly IAzureBusService<Invoice> _azureBusService;
        public DeleteInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IMapper mapper,
                                           InvoiceBusinessRules invoiceBusinessRules,IAzureBusService<Invoice> azureBusService)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _invoiceBusinessRules = invoiceBusinessRules;
            _azureBusService = azureBusService;
        }

        public async Task<DeletedInvoiceDto> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
        {
            await _invoiceBusinessRules.InvoiceIdShouldExistWhenSelected(request.Id);

            Invoice mappedInvoice = _mapper.Map<Invoice>(request);
            Invoice deletedInvoice = await _invoiceRepository.DeleteAsync(mappedInvoice);
            DeletedInvoiceDto deletedInvoiceDto = _mapper.Map<DeletedInvoiceDto>(deletedInvoice);
            //var invoiceDeleteEvent = new DeleteEventInvoice()
            //{
            //    CreatedAt = DateTime.Now,
            //    Id = deletedInvoiceDto.Id,
              
            //};
            //if (deletedInvoiceDto != null)
            //{
            //    await _azureBusService.CreateQueeIfNotExists(InvoiceMessages.InvoiceDeletedQuee);
            //    await _azureBusService.sendMessageToQuee(invoiceDeleteEvent, InvoiceMessages.InvoiceDeletedQuee);

            //}
            return deletedInvoiceDto;
        }
    }
}