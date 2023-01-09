using Application.Services.Repositories;
using Core.Persistence.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class CarImageFileRepository : EfRepositoryBase<CarFileImage, BaseDbContext>, ICarImageFileRepository
{
    public CarImageFileRepository(BaseDbContext context) : base(context)
    {
    }
}