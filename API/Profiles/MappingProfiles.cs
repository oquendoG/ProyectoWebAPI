using API.Dtos;
using AutoMapper;
using CORE.Entities;

namespace API.Profiles;
public class MappingProfiles : Profile
{
	public MappingProfiles()
	{
		CreateMap<Producto, ProductoDTO>().ReverseMap();
		CreateMap<Categoria, CategoriaDTO>().ReverseMap();
		CreateMap<Marca, MarcaDTO>().ReverseMap();

		CreateMap<Producto, ProductoListDTO>()
			.ForMember(destino => destino.Marca,
				origen => origen.MapFrom(origen => origen.Marca.Nombre))
			.ForMember(destino => destino.Categoria,
				origen => origen.MapFrom(origen => origen.Categoria.Nombre))
			.ReverseMap()
			.ForMember(destino => destino.Categoria, origen => origen.Ignore())
			.ForMember(destino => destino.Marca, origen => origen.Ignore());

		CreateMap<Producto, ProductoAddUpdateDTO>()
			.ReverseMap()
			.ForMember(origen => origen.Categoria, destino => destino.Ignore())
			.ForMember(origen => origen.Marca, destino => destino.Ignore());
	}
}
