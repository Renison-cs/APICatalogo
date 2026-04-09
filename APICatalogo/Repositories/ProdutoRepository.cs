using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParams)
        {
            var query = GetAll();

            return await PagedList<Produto>.ToPagedListAsync(
                query.OrderBy(p => p.ProdutoId),
                produtosParams.PageNumber,
                produtosParams.PageSize);
        }

        public async Task<PagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroParams)
        {
            var query = GetAll();

            if (produtosFiltroParams.Preco.HasValue &&
                !string.IsNullOrEmpty(produtosFiltroParams.PrecoCriterio))
            {
                if (produtosFiltroParams.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.Preco > produtosFiltroParams.Preco.Value);
                }
                else if (produtosFiltroParams.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.Preco < produtosFiltroParams.Preco.Value);
                }
                else if (produtosFiltroParams.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.Preco == produtosFiltroParams.Preco.Value);
                }
            }

            return await PagedList<Produto>.ToPagedListAsync(
                query.OrderBy(p => p.Preco),
                produtosFiltroParams.PageNumber,
                produtosFiltroParams.PageSize);
        }

        public async Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int id)
        {
            return await GetAll()
                .Where(p => p.CategoriaId == id)
                .ToListAsync();
        }
    }
}