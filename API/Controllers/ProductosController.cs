using API.Dtos;
using AutoMapper;
using CORE.Entities;
using CORE.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
public class ProductosController : BaseApiController
{
	private readonly IUnitOfWork unitOfWork;
	private readonly IMapper mapper;

	public ProductosController(IUnitOfWork unitOfWork, IMapper mapper)
	{
		this.unitOfWork = unitOfWork;
		this.mapper = mapper;
	}

	[HttpGet]
	[MapToApiVersion("1.0")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<IEnumerable<ProductoListDTO>>> Get()
	{
		IEnumerable<Producto> productos = await unitOfWork.Productos.GetAllAsync();
		if (productos is null)
		{
			return NotFound();
		}

		return mapper.Map<List<ProductoListDTO>>(productos);
	}

    [HttpGet]
    [MapToApiVersion("1.1")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ProductoDTO>>> Get11()
    {
        IEnumerable<Producto> productos = await unitOfWork.Productos.GetAllAsync();
        if (productos is null)
        {
            return NotFound();
        }

        return mapper.Map<List<ProductoDTO>>(productos);
    }

    [HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<ProductoDTO>> Get(int id)
	{
		Producto producto = await unitOfWork.Productos.GetByIdAsync(id);
		if (producto is null)
		{
			return NotFound();
		}

		return mapper.Map<ProductoDTO>(producto);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<Producto>> Post(ProductoAddUpdateDTO productoDTO)
	{
		Producto producto = mapper.Map<Producto>(productoDTO);

		unitOfWork.Productos.Add(producto);
		await unitOfWork.SaveAsync();
		if (producto is null)
		{
			return BadRequest();
		}

		productoDTO.Id = producto.Id;
		return CreatedAtAction(nameof(Post), new { id = productoDTO.Id, productoDTO });
	}

	[HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductoAddUpdateDTO>>
		Put(int id, [FromBody]ProductoAddUpdateDTO productoDTO)
	{
        if (productoDTO is null)
		{
			return NotFound();
		}

        Producto producto = mapper.Map<Producto>(productoDTO);
		unitOfWork.Productos.Update(producto);
		await unitOfWork.SaveAsync();

		return productoDTO;
	}

	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Delete(int id)
	{
		Producto producto = await unitOfWork.Productos.GetByIdAsync(id);
		if (producto is null)
			return NotFound();

		unitOfWork.Productos.Remove(producto);
		await unitOfWork.SaveAsync();

		return NoContent();
	}
}
