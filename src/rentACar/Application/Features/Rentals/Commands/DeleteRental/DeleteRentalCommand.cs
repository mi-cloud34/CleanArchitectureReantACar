using Application.Features.Rentals.Constants;
using Application.Features.Rentals.Dtos;
using Application.Features.Rentals.Dtos.EventCarBus;
using Application.Services.AzureBus;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Domain.Entities;
using MediatR;
using static Application.Features.Rentals.Constants.OperationClaims;
using static Domain.Constants.OperationClaims;

namespace Application.Features.Rentals.Commands.DeleteRental;

public class DeleteRentalCommand : IRequest<DeletedRentalDto>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => new[] { Admin, RentalDelete };

    public class DeleteRentalCommandHandler : IRequestHandler<DeleteRentalCommand, DeletedRentalDto>
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IMapper _mapper;
        private readonly IAzureBusService<Rental> _azureBusService;

        public DeleteRentalCommandHandler(IRentalRepository rentalRepository, IMapper mapper, IAzureBusService<Rental> azureBusService)
        {
            _rentalRepository = rentalRepository;
            _mapper = mapper;
            _azureBusService = azureBusService;
        }

        public async Task<DeletedRentalDto> Handle(DeleteRentalCommand request, CancellationToken cancellationToken)
        {
            Rental mappedRental = _mapper.Map<Rental>(request);
            Rental deletedRental = await _rentalRepository.DeleteAsync(mappedRental);
            DeletedRentalDto deletedRentalDto = _mapper.Map<DeletedRentalDto>(deletedRental);
            //var rentalDeletedEvent = new DeleteEventRental()
            //{
            //    CreatedAt = DateTime.Now,
            //    Id = deletedRentalDto.Id,


            //};
            //if (deletedRentalDto != null)
            //{
            //    await _azureBusService.CreateQueeIfNotExists(RentalMessages.RentalDeletedQuee);
            //    await _azureBusService.sendMessageToQuee(rentalDeletedEvent, RentalMessages.RentalDeletedQuee);

            //}
            return deletedRentalDto;
        }
    }
}