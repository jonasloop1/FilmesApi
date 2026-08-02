using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.Dtos;
using FilmesApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesApi.Controllers;

[ApiController]
[Route("[controller]")]

public class FilmeController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;
    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um filme ao banco de dados
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionarFilme([FromBody] CreateFilmeDto filmeDto)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);
        _context.SaveChanges(); 
        return CreatedAtAction(nameof(BuscarFilmePorId), new { id = filme.Id }, filme); 
    }

    /// <summary>
    /// Retorna todos os filmes do banco de dados em um range de skip=0 e take=100
    /// </summary>
    /// <param name="skip">Campo necessário para paginação, ignorar um número especificado de elementos no início de uma sequência</param>
    /// <param name="take">Campo necessário para paginação, retornar um número especificado de elementos contíguos a partir do início de uma sequência</param>
    /// <returns>IEnumerable</returns>
    /// <response code="200">Caso a busca seja feita com sucesso</response>
    [HttpGet]  
    public IEnumerable<ReadFilmeDto> BuscarTodosFilmes([FromQuery] int skip = 0, [FromQuery] int take = 100)
    {
        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take));
    }

    /// <summary>
    /// Retorna um filme do banco de dados passando um id específico
    /// </summary>
    /// <param name="id">Parametro necessário para retornar um filme especifico</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso a busca seja feita com sucesso</response>
    [HttpGet("{id}")]  
    public IActionResult BuscarFilmePorId(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
        return Ok(filmeDto);
    }

    /// <summary>
    /// Atualiza um filme do banco de dados passando um id específico e um objeto com as novas informações 
    /// </summary>
    /// <param name="id">Campo necessário para a atualização de um filme</param>
    /// <param name="filmeDto">Objeto com os campos necessários para a atualização de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso a atulização seja feita com sucesso</response>
    [HttpPut("{id}")] 
    public IActionResult AtualizarFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id); 
        if (filme == null) return NotFound();
        _mapper.Map(filmeDto, filme); 
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Atualiza um filme do banco de dados passando informações parciais de um objeto com as novas informações 
    /// </summary>
    /// <param name="id">Campo necessário para a atualização de um filme</param>
    /// <param name="pacth">Objeto com os campos necessários para a atualização de um filme de forma parcial</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso a atulização seja feita com sucesso</response>
    [HttpPatch("{id}")] 
    public IActionResult AtualizarFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> pacth)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();

        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme); 
        pacth.ApplyTo(filmeParaAtualizar, ModelState); 
        if (!TryValidateModel(filmeParaAtualizar)) 
        {
            return ValidationProblem(ModelState); 
        }
        _mapper.Map(filmeParaAtualizar, filme);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Deleta um filme do banco de dados passando um id específico
    /// </summary>
    /// <param name="id">Campo necessário para a exclusão de um filme do banco de dados</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso a exclusão seja feita com sucesso</response>
    [HttpDelete("{id}")] //deleta um filme pelo id 
    public IActionResult DeletarFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        _context.Filmes.Remove(filme);
        _context.SaveChanges();
        return NoContent();
    }
}