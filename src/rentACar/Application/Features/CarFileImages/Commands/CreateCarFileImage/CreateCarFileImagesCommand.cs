using Application.Features.CarFileImages.Dtos;
using Application.Services.ImageService.StorageService;
using Application.Services.Repositories;
using AutoMapper;
using Core.Storage;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CarFileImages.Commands.CreateCarFileImage
{
    public class CreateCarFileImagesCommand : IRequest<IList<CreateCarFileImageDto>>
    {
      
        //public string Name { get; set; }
        //public int CarId { get; set; }
        public IFormFileCollection? Files { get; set; }

        public class CreateCarFileImageCommandHandler : IRequestHandler<CreateCarFileImagesCommand, IList<CreateCarFileImageDto>>
        {
            private readonly IStorageService _storageService;
            private readonly IMapper _mapper;
            ICarImageFileRepository _carImageFileRepository;
          
            public CreateCarFileImageCommandHandler(IStorageService storageService , IMapper mapper, ICarImageFileRepository carImageFileRepository)
            {
                _storageService = storageService;
                _mapper = mapper;
                _carImageFileRepository = carImageFileRepository;
              
               
            }

            public async Task<IList<CreateCarFileImageDto>> Handle(CreateCarFileImagesCommand request,
                CancellationToken cancellationToken)
            {
                List<(string fileName, string pathOrContainerName)> result =
            await _storageService.UploadsAsync("photo-images", request.Files);

                List<CarFileImage>  mappedCarFileImage = result.Select(r => new CarFileImage
                {
                    Name = r.fileName,
                    Path = r.pathOrContainerName,
                    Storage = _storageService.StorageName,
                   
                }).ToList();

                //await _storageService.SaveAsync();

               
           
                IList<CarFileImage> createdCarFileImages = await _carImageFileRepository.AddRangeAsync(mappedCarFileImage);
                IList<CreateCarFileImageDto> createdCarFileImageDto = _mapper.Map<List<CreateCarFileImageDto>>(createdCarFileImages);

         

               return createdCarFileImageDto;

            }
        }

    }
}
