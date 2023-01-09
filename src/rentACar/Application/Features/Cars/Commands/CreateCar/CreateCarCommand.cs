using Application.Features.Cars.Dtos;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Storage;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Collections.ObjectModel;
using static Application.Features.Cars.Constants.OperationClaims;
using static Domain.Constants.OperationClaims;

namespace Application.Features.Cars.Commands.CreateCar;

public class CreateCarCommand : IRequest<CreatedCarDto>, ISecuredRequest
{
    public int ColorId { get; set; }
    public int ModelId { get; set; }
    public int RentalBranchId { get; set; }
    public CarState CarState { get; set; }
    public int Kilometer { get; set; }
    public short ModelYear { get; set; }
    public string Plate { get; set; }
    public short MinFindeksCreditRate { get; set; }
    public IFormFile? Files { get; set; }
    //public string Name { get; set; }
    //public string Storage { get; set; }
    //public string Path { get; set; }

    public string[] Roles => new[] { Admin, CarAdd };


    public class CreateCarCommandHandler : IRequestHandler<CreateCarCommand, CreatedCarDto>
    {
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageServices;

        public CreateCarCommandHandler(ICarRepository carRepository, IMapper mapper,IStorageService storageService)
        {
            _carRepository = carRepository;
            _mapper = mapper;
            _storageServices = storageService;
        }

        public async Task<CreatedCarDto> Handle(CreateCarCommand request, CancellationToken cancellationToken)
        {
            Car mappedCar = _mapper.Map<Car>(request);
            (string fileName, string pathOrContainerName) result = await _storageServices.UploadAsync("car-files", request.Files);
            mappedCar.CarFileImages = new Collection<CarFileImage>()
            {
                new CarFileImage(){
                Name = result.fileName,
                Path = result.pathOrContainerName,
                Storage = _storageServices.StorageName,
                CarId=mappedCar.Id
                }
            };
            Car createdCar = await _carRepository.AddAsync(mappedCar);
            CreatedCarDto createdCarDto = _mapper.Map<CreatedCarDto>(createdCar);
            return createdCarDto;
        }
    }
}