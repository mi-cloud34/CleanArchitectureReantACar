
using Application.Features.Brands.Dtos;
using Application.Features.CarFileImages.Dtos;
using Application.Services.ImageService.StorageService;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Caching;
using Core.Storage;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.CarFileImages.Commands.DeleteCarFileImage;

public class DeleteCarFileImageCommand : IRequest<DeleteCarFileImageDto>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IFormFileCollection? Files { get; set; }
    public class DeleteCarFileImageCommandHandler : IRequestHandler<DeleteCarFileImageCommand, DeleteCarFileImageDto>
    {
        private readonly IStorageService _storageServices;
        private readonly IMapper _mapper;
        ICarImageFileRepository _carImageFileRepository;
        public DeleteCarFileImageCommandHandler(IStorageService storageServices, IMapper mapper,
         ICarImageFileRepository carImageFileRepository                           )
        {
            _storageServices = storageServices;
            _mapper = mapper;
            _carImageFileRepository=carImageFileRepository;
        }

        public async Task<DeleteCarFileImageDto> Handle(DeleteCarFileImageCommand request, CancellationToken cancellationToken)
        {
            // IStorage result = (IStorage)_storageServices.DeleteAsync(request.Id);
           IStorage result =
             (IStorage)_storageServices.DeleteAsync(request.Files.ToString(),"photo-images");

            //CarFileImage mappedCarFileImage = _mapper.Map<CarFileImage>((new CarFileImage
            //{
            //    Name = result.fileName,
            //    Path = result.pathOrContainerName,
            //    Storage = _storageServices.StorageName,

            //}));
             CarFileImage mappedCarFileImage = _mapper.Map<CarFileImage>(result);
            // result=>{ request.Id = result.id});
            CarFileImage deletedCarFileImage = await _carImageFileRepository.DeleteAsync(mappedCarFileImage);
            DeleteCarFileImageDto deletedCarFileImageDto = _mapper.Map<DeleteCarFileImageDto>(deletedCarFileImage);
            return deletedCarFileImageDto;
        }
    }
}