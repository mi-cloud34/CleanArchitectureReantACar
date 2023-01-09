using Application.Features.Cars.Dtos;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Storage;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Collections.ObjectModel;
using static Application.Features.Cars.Constants.OperationClaims;
using static Domain.Constants.OperationClaims;

namespace Application.Features.Cars.Commands.DeleteCar;

public class DeleteCarCommand : IRequest<DeletedCarDto>, ISecuredRequest
{
    public int Id { get; set; }
    public IFormFileCollection? Files { get; set; }
    public string[] Roles => new[] { Admin, CarDelete };

    public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand, DeletedCarDto>
    {
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageServices;

        public DeleteCarCommandHandler(ICarRepository carRepository, IMapper mapper, IStorageService storageServices)
        {
            _carRepository = carRepository;
            _mapper = mapper;
            _storageServices = storageServices;
        }

        public async Task<DeletedCarDto> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
        {
            IStorage result =(IStorage)_storageServices.DeleteAsync(request.Files.ToString(), "photo-images");
            Car mappedCar = _mapper.Map<Car>(request);
            mappedCar.CarFileImages = new Collection<CarFileImage>()
            {
                new CarFileImage(){
                Storage = _storageServices.StorageName,
                CarId=mappedCar.Id
                }
            };
            Car deletedCar = await _carRepository.DeleteAsync(mappedCar);
            DeletedCarDto deletedCarDto = _mapper.Map<DeletedCarDto>(deletedCar);
            return deletedCarDto;
        }
    }
}