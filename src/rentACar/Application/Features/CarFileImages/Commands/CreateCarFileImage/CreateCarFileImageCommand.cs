
using Application.Features.CarFileImages.Dtos;
using Application.Services.ImageService.StorageService;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Caching;
using Core.Storage;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Linq;
using static Domain.Constants.OperationClaims;

namespace Application.Features.CarFileImages.Commands.CreateCarFileImage;

public class CreateCarFileImageCommand : IRequest<CreateCarFileImageDto>, ICacheRemoverRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Storage { get; set; }
    public IFormFile? Files { get; set; }
    public bool BypassCache { get; }
   

    public string CacheKey => "CarFileImages-list";


    public class CreateCarFileImageCommandHandler : IRequestHandler<CreateCarFileImageCommand, CreateCarFileImageDto>
    {
        private readonly IStorageService _storageServices;
        private readonly IMapper _mapper;
        ICarImageFileRepository _carImageFileRepository;
        public CreateCarFileImageCommandHandler(IStorageService storageServices, IMapper mapper,
         ICarImageFileRepository carImageFileRepository)
        {
            _storageServices = storageServices;
            _mapper = mapper;
            _carImageFileRepository = carImageFileRepository;

        }

        public async Task<CreateCarFileImageDto> Handle(CreateCarFileImageCommand request, CancellationToken cancellationToken)
        {
            //IStorage result = (IStorage)_storageServices.CreateAsync(request.Id);
            (string fileName, string pathOrContainerName) result =
             await _storageServices.UploadAsync("photo-images", request.Files);

            CarFileImage mappedCarFileImage = _mapper.Map<CarFileImage>((new CarFileImage
            {
                Name = result.fileName,
                Path = result.pathOrContainerName,
                Storage = _storageServices.StorageName,
               
            }));
            // CarFileImage mappedCarFileImage = _mapper.Map<CarFileImage>(request);
            CarFileImage createdCarFileImage = await _carImageFileRepository.AddAsync(mappedCarFileImage);
            CreateCarFileImageDto createdCarFileImageDto = _mapper.Map<CreateCarFileImageDto>(createdCarFileImage);
            return createdCarFileImageDto;
        }
    }
}