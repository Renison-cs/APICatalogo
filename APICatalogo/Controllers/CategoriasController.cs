using APICatalogo.Context;
using Microsoft.AspNetCore.Mvc;
using APICatalogo.Models;
using APICatalogo.Repositories;
using APICatalogo.DTOs;
using APICatalogo.Mappings;
using APICatalogo.Filters;

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

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            _logger.LogInformation("Obtendo categorias com seus produtos relacionados");
            var categorias =  _uof.CategoriaRepository.GetAll();
            return Ok(categorias);
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<CategoriaDTO>> Get()
        {
           var categorias = _uof.CategoriaRepository.GetAll();
            if(categorias is null)
                return NotFound("Categorias não encontradas");

            var categoriasDto = categorias.ToCategoriaDTOList();
            return Ok(categoriasDto);
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<CategoriaDTO> Get(int id)
        {
            var categoria = _uof.CategoriaRepository.Get(c => c.CategoriaId == id);
            if (categoria is null)
            {
                _logger.LogWarning("Categoria com id {Id} não encontrada", id);
                return NotFound("Categoria não encontrada");
            }

            var categoriaDTO = categoria.ToCategoriaDTO();

            return Ok(categoriaDTO);
        }
        [HttpPost]
        public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDto)
        {
            if (categoriaDto is null)
            {
                _logger.LogWarning("Objeto categoria enviado é nulo");
                return BadRequest();
            }

            var categoria = categoriaDto.ToCategoria();

            var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
            _uof.Commit();

            var categoriaCriadaDto = categoriaCriada.ToCategoriaDTO();

            return CreatedAtRoute("ObterCategoria", new { id = categoriaCriada.CategoriaId }, categoriaCriada);
        }

        [HttpPut("{id:int}")]
        public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            { 
                _logger.LogWarning("O id da categoria {Id} não corresponde ao id do objeto enviado", id);
                return BadRequest("Categoria Invalida");
            }
           
            var categoria = categoriaDto.ToCategoria();

            _uof.CategoriaRepository.Update(categoria);
            _uof.Commit();
           
            var categoriaAtualizada = categoria.ToCategoriaDTO();

            return Ok(categoriaAtualizada);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<CategoriaDTO> Delete(int id)
        {
            var categoria = _uof.CategoriaRepository.Get(c => c.CategoriaId == id);
            if (categoria == null)
            {
                _logger.LogWarning("Categoria com id {Id} não encontrada para exclusão", id);
                return NotFound("Categoria não encontrada");
            }
           var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);
            _uof.Commit();
                       
            var categoriaExcluidaDto = categoria.ToCategoriaDTO();

            return Ok(categoriaExcluidaDto);

        }
    }
}
