using API.Dtos;
using API.Helpers.Errors;
using Asp.Versioning;
using AutoMapper;
using CORE.Entities;
using CORE.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
[Route("api/v{v:apiVersion}/datosproductos")]
public class DatosProductosController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DatosProductosController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this._unitOfWork = unitOfWork;
        this._mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProductoListDTO>>> Get()
    {
        IEnumerable<Producto> productos = await _unitOfWork.Productos.GetAllAsync();
        if (productos is null)
        {
            return NotFound(new ApiResponse(404, "Los productos solicitados no existen"));
        }

        return Ok(_mapper.Map<List<ProductoListDTO>>(productos));
    }

    [HttpGet]
    [MapToApiVersion("1.1")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProductoDTO>>> Get11()
    {
        IEnumerable<Producto> productos = await _unitOfWork.Productos.GetAllAsync();
        if (productos is null)
        {
            return NotFound(new ApiResponse(404, "Los productos solicitados no existen"));
        }

        return Ok(_mapper.Map<List<ProductoDTO>>(productos));
    }
}
