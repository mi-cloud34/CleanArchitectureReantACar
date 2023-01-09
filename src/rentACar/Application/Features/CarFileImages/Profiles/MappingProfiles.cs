
using Application.Features.CarFileImages.Dtos;
using Application.Features.CarFileImages.Commands.CreateCarFileImage;
using Application.Features.CarFileImages.Commands.DeleteCarFileImage;
using AutoMapper;
using Core.Persistence.Paging;
using Domain.Entities;
using Application.Features.CarFileImages.Models;

namespace Application.Features.CarFileImages.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CarFileImage, CreateCarFileImagesCommand>().ReverseMap();
        CreateMap<CarFileImage, CreateCarFileImageDto>().ReverseMap();
       CreateMap<CarFileImage, CreateCarFileImageCommand>().ReverseMap();
        CreateMap<CarFileImage, UpdateCarFileImageDto>().ReverseMap();
        CreateMap<CarFileImage, DeleteCarFileImageCommand>().ReverseMap();
        CreateMap<CarFileImage, DeleteCarFileImageDto>().ReverseMap();
        CreateMap<CarFileImage, CarFileImageDto>().ReverseMap();
        CreateMap<CarFileImage, CarFileImageListDto>().ForMember(c=>c.CarName,opt=>opt.MapFrom(c=>c.Car.Model.Brand.Name)).ReverseMap();
        CreateMap<IPaginate<CarFileImage>, CarFileImageListModel>().ReverseMap();
    }
}