// Controllers/SalasController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVoucher.DTOs;
using SistemaVoucher.Services;

namespace SistemaVoucher.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalasController : ControllerBase
    {
        private readonly ISalaService _salaService;

        public SalasController(ISalaService salaService)
        {
            _salaService = salaService;
        }

        /// <summary>
        /// Obtém todas as salas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalaDto>>> GetTodasSalas()
        {
            try
            {
                var salas = await _salaService.GetTodasSalasAsync();
                return Ok(salas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém apenas salas ativas
        /// </summary>
        [HttpGet("ativas")]
        public async Task<ActionResult<IEnumerable<SalaDto>>> GetSalasAtivas()
        {
            try
            {
                var salas = await _salaService.GetSalasAtivasAsync();
                return Ok(salas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém sala por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SalaDto>> GetSalaPorId(Guid id)
        {
            try
            {
                var sala = await _salaService.GetSalaPorIdAsync(id);

                if (sala == null)
                {
                    return NotFound(new { message = "Sala não encontrada" });
                }

                return Ok(sala);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Cria nova sala
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SalaDto>> CriarSala([FromBody] CriarSalaDto criarSalaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar se sala com esse nome já existe
                var salaExiste = await _salaService.SalaExisteAsync(criarSalaDto.Nome);
                if (salaExiste)
                {
                    return Conflict(new { message = "Sala com este nome já existe" });
                }

                var sala = await _salaService.CriarSalaAsync(criarSalaDto);

                return CreatedAtAction(
                    nameof(GetSalaPorId),
                    new { id = sala.Id },
                    sala
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza sala existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SalaDto>> AtualizarSala(Guid id, [FromBody] AtualizarSalaDto atualizarSalaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sala = await _salaService.AtualizarSalaAsync(id, atualizarSalaDto);

                if (sala == null)
                {
                    return NotFound(new { message = "Sala não encontrada" });
                }

                return Ok(sala);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Deleta sala
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeletarSala(Guid id)
        {
            try
            {
                var sucesso = await _salaService.DeletarSalaAsync(id);

                if (!sucesso)
                {
                    return NotFound(new { message = "Sala não encontrada" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Ativa/Desativa sala
        /// </summary>
        [HttpPatch("{id}/toggle")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SalaDto>> ToggleSala(Guid id)
        {
            try
            {
                var salaAtual = await _salaService.GetSalaPorIdAsync(id);

                if (salaAtual == null)
                {
                    return NotFound(new { message = "Sala não encontrada" });
                }

                var atualizarDto = new AtualizarSalaDto
                {
                    Ativa = !salaAtual.Ativa
                };

                var sala = await _salaService.AtualizarSalaAsync(id, atualizarDto);

                return Ok(sala);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Verifica se sala existe por nome
        /// </summary>
        [HttpGet("existe/{nome}")]
        [Authorize]
        public async Task<ActionResult<bool>> SalaExiste(string nome)
        {
            try
            {
                var existe = await _salaService.SalaExisteAsync(nome);
                return Ok(new { existe });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
    }
}