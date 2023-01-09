
namespace Application.Features.CarFileImages.Dtos;

public class UpdateCarFileImageDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CarId { get; set; }
    public string Path { get; set; }
    public string Storage { get; set; }
}
