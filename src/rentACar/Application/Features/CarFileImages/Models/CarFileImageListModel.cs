
using Application.Features.CarFileImages.Dtos;
using Core.Persistence.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CarFileImages.Models
{
    public class CarFileImageListModel : BasePageableModel
    {
        public IList<CarFileImageListDto> Items { get; set; }

        //
    }
}
