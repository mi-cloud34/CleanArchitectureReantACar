using Application.Features.Cars.Dtos;
using Application.Features.Cars.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Core.Storage;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Cars.Queries.GetCarById;

public class GetByIdCarQuery : IRequest<CarDto>
{
    public int Id { get; set; }

    public class GetByIdCarQueryHandler : IRequestHandler<GetByIdCarQuery, CarDto>
    {
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;
        private readonly CarBusinessRules _carBusinessRules;
        private readonly IStorageService _storageService;

        public GetByIdCarQueryHandler(ICarRepository carRepository, CarBusinessRules carBusinessRules, IMapper mapper, IStorageService storageService)
        {
            _carRepository = carRepository;
            _carBusinessRules = carBusinessRules;
            _mapper = mapper;
            _storageService = storageService;
        }


        public async Task<CarDto> Handle(GetByIdCarQuery request, CancellationToken cancellationToken)
        {
            await _carBusinessRules.CarIdShouldExistWhenSelected(request.Id);

            Car? car = await _carRepository.GetAsync(c => c.Id == request.Id ,c=>c.Include(x=>x.CarFileImages));

            CarFileImage carFileImage = car.CarFileImages.FirstOrDefault();
            CarDto carDto = _mapper.Map<CarDto>(car);
            carDto.ImageUrl = await _storageService.GetByNameFileAsync(carFileImage.Path, carFileImage.Name);
            return carDto;
        }
    }
}