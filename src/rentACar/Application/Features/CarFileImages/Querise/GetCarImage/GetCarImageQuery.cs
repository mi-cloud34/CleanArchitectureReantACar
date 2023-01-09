using Application.Features.CarFileImages.Dtos;
using Application.Services.Repositories;
using AutoMapper;
using Core.Storage;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CarFileImages.Querise.GetCarImage
{
    public class GetCarImageQuery : IRequest<CarFileImageDto>
    {
        public string fileName { get; set; }


        public class GetCarImageQueryHandler : IRequestHandler<GetCarImageQuery, CarFileImageDto>
        {
            private readonly ICarImageFileRepository _carImageFileRepository;
            private readonly IStorageService _storageService;
            private readonly IMapper _mapper;

            public GetCarImageQueryHandler(ICarImageFileRepository carImageFileRepository,IStorageService storageService,IMapper mapper)
            {
                _carImageFileRepository = carImageFileRepository;
                _storageService=storageService;
                _mapper = mapper;
            }

            public async Task<CarFileImageDto> Handle(GetCarImageQuery request, CancellationToken cancellationToken)
            {

                
              CarFileImage? carFileImage =  await _carImageFileRepository.GetAsync(x => x.Name == request.fileName);
                return new()
                {
                    Name = carFileImage.Name,
                    Id = carFileImage.Id,
                    Path = await _storageService.GetByNameFileAsync(carFileImage.Path , carFileImage.Name) ,
                    CarId = carFileImage.CarId
                };

                //hocam burda localstoragede hazırladıgım getfile metoduu cagırmak istiyorum ugraştım
                ////sürekli bi yerlerde hata verdi bu şekilde de olur ama azure için galiba hata verecek
            }
        }
    }
}
