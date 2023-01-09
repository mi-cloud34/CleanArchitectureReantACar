using Application.Features.CarFileImages.Commands.CreateCarFileImage;
using Application.Features.CarFileImages.Commands.DeleteCarFileImage;

using Application.Features.CarFileImages.Dtos;
using Application.Features.CarFileImages.Models;
using Application.Features.CarFileImages.Querise.GetCarImage;
using Core.Application.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarFileImagesController : BaseController
{
    IFormCollection formCollection;

    [HttpGet(/*"{Id}"*/)]
    public async Task<IActionResult> GetById([FromQuery] GetCarImageQuery getByIdCarFileImageQuery)
    {
        CarFileImageDto result = await Mediator.Send(getByIdCarFileImageQuery);
        return Ok(result);
    }

    //[HttpGet]
    //public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
    //{
    //    GetListCarFileImageQuery getListCarFileImageQuery = new() { PageRequest = pageRequest };
    //    CarFileImageListModel result = await Mediator.Send(getListCarFileImageQuery);
    //    return Ok(result);
    //}

    [HttpPost]
    public async Task<IActionResult> AddImages([FromForm] CreateCarFileImagesCommand createCarFileImageCommand)
    {
        createCarFileImageCommand.Files= Request.Form.Files;
        IList<CreateCarFileImageDto> result = await Mediator.Send(createCarFileImageCommand);
        return Created("", result);
    }

    [HttpPost]
    public async Task<IActionResult> AddImage([FromBody] CreateCarFileImageCommand createCarFileImageCommand)
    {
        createCarFileImageCommand.Files =(IFormFile) Request.Form.Files;
        CreateCarFileImageDto result = await Mediator.Send(createCarFileImageCommand);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteCarFileImageCommand deleteCarFileImageCommand)
    {
        deleteCarFileImageCommand.Files = Request.Form.Files;
        DeleteCarFileImageDto result = await Mediator.Send(deleteCarFileImageCommand);
        return Ok(result);
    }
}