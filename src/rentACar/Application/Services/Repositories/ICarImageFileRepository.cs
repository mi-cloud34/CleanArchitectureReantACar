using Core.Persistence.Repositories;
using Domain.Entities;

namespace Application.Services.Repositories;

public interface ICarImageFileRepository : IAsyncRepository<CarFileImage>, IRepository<CarFileImage>
{
    
}