using CORE.Entities;
using CORE.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class ProductosController : BaseApiController
{
	private readonly IUnitOfWork unitOfWork;

	public ProductosController(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<IEnumerable<Producto>>> Get()
	{
		IEnumerable<Producto> productos = await unitOfWork.Productos.GetAllAsync();
		if (productos is null)
		{
			return NotFound();
		}

		return Ok(productos);
	}

	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Get(int id)
	{
		Producto producto = await unitOfWork.Productos.GetByIdAsync(id);
		if (producto is null)
		{
			return NotFound();
		}

		return Ok(producto);
	}


}
