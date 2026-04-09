using Microsoft.EntityFrameworkCore;
using APICatalogo.DTOs;
using APICatalogo.Mappings;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;


namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(IUnitOfWork uof, ILogger<CategoriasController> logger)
        {
            _uof = uof;
            _logger = logger;
        }

        // ✅ CORRIGIDO
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategorias()
        {
            _logger.LogInformation("Obtendo categorias");

            var categorias = await _uof.CategoriaRepository.GetAllAsync();

            var categoriasDto = categorias.ToCategoriaDTOList();

            return Ok(categoriasDto);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get(
            [FromQuery] CategoriasParameters categoriasParameters)
        {
            var categorias = await _uof.CategoriaRepository
                .GetCategoriasAsync(categoriasParameters);

            return ObterCategorias(categorias);
        }

        [HttpGet("Filter/nome/pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltradas(
            [FromQuery] CategoriasFiltroNome categoriasFiltro)
        {
            var categoriasFiltradas = await _uof.CategoriaRepository
                .GetCategoriaFiltroNomeAsync(categoriasFiltro);

            return ObterCategorias(categoriasFiltradas);
        }

        // ✅ CORRIGIDO (IPagedList)
        private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(PagedList<Categoria> categorias)
        {
            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };

            Response.Headers.Append("x-pagination",
                JsonConvert.SerializeObject(metadata));

            var categoriasDto = categorias.ToCategoriaDTOList();

            return Ok(categoriasDto);
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            var categoria = await _uof.CategoriaRepository
                .GetAsync(c => c.CategoriaId == id);

            if (categoria is null)
            {
                _logger.LogWarning("Categoria com id {Id} não encontrada", id);
                return NotFound("Categoria não encontrada");
            }

            return Ok(categoria.ToCategoriaDTO());
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
        {
            if (categoriaDto is null)
            {
                _logger.LogWarning("Objeto categoria enviado é nulo");
                return BadRequest();
            }

            var categoria = categoriaDto.ToCategoria();

            var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
            await _uof.CommitAsync();

            var categoriaCriadaDto = categoriaCriada.ToCategoriaDTO();

            // ✅ CORRIGIDO (retorna DTO)
            return CreatedAtRoute(
                "ObterCategoria",
                new { id = categoriaCriadaDto.CategoriaId },
                categoriaCriadaDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            {
                _logger.LogWarning("Id inconsistente {Id}", id);
                return BadRequest("Categoria inválida");
            }

            var categoria = categoriaDto.ToCategoria();

            _uof.CategoriaRepository.Update(categoria);
            await _uof.CommitAsync();

            return Ok(categoria.ToCategoriaDTO());
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            var categoria = await _uof.CategoriaRepository
                .GetAsync(c => c.CategoriaId == id);

            if (categoria == null)
            {
                _logger.LogWarning("Categoria com id {Id} não encontrada", id);
                return NotFound("Categoria não encontrada");
            }

            _uof.CategoriaRepository.Delete(categoria);
            await _uof.CommitAsync();

            return Ok(categoria.ToCategoriaDTO());
        }
    }
}