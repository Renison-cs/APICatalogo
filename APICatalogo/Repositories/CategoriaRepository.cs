using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedList<Categoria>> GetCategoriasAsync(CategoriasParameters parametros)
        {
            var query = GetAll();

            var resultado = await PagedList<Categoria>.ToPagedListAsync(
                query.OrderBy(c => c.CategoriaId),
                parametros.PageNumber,
                parametros.PageSize);

            return resultado;
        }

        public async Task<PagedList<Categoria>> GetCategoriaFiltroNomeAsync(
            CategoriasFiltroNome categoriasParams)
        {
            var categorias = GetAll(); // IQueryable

            if (!string.IsNullOrEmpty(categoriasParams.Nome))
            {
                categorias = categorias
                    .Where(c => c.Nome.Contains(categoriasParams.Nome));
            }

            return await PagedList<Categoria>.ToPagedListAsync(
                categorias.OrderBy(c => c.Nome),
                categoriasParams.PageNumber,
                categoriasParams.PageSize);
        }
    }
}