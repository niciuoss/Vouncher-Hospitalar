using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVoucher.DTOs;
using SistemaVoucher.Services;

namespace SistemaVoucher.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilasController : ControllerBase
    {
        private readonly IFilaService _filaService;

        public FilasController(IFilaService filaService)
        {
            _filaService = filaService;
        }

        /// <summary>
        /// Obtém fila de uma sala específica
        /// </summary>
        [HttpGet("sala/{salaId}")]
        public async Task<ActionResult<IEnumerable<FilaDto>>> GetFilasPorSala(Guid salaId)
        {
            try
            {
                var filas = await _filaService.GetFilasPorSalaAsync(salaId);
                return Ok(filas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todas as filas aguardando atendimento
        /// </summary>
        [HttpGet("aguardando")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FilaDto>>> GetFilasAguardando()
        {
            try
            {
                var filas = await _filaService.GetFilasAguardandoAsync();
                return Ok(filas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém fila por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<FilaDto>> GetFilaPorId(Guid id)
        {
            try
            {
                var fila = await _filaService.GetFilaPorIdAsync(id);

                if (fila == null)
                {
                    return NotFound(new { message = "Fila não encontrada" });
                }

                return Ok(fila);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Cria nova entrada na fila (paciente entra na fila)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<FilaDto>> CriarFila([FromBody] CriarFilaDto criarFilaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var fila = await _filaService.CriarFilaAsync(criarFilaDto);

                return CreatedAtAction(
                    nameof(GetFilaPorId),
                    new { id = fila.Id },
                    fila
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza status da fila
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<FilaDto>> AtualizarFila(Guid id, [FromBody] AtualizarFilaDto atualizarFilaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var fila = await _filaService.AtualizarFilaAsync(id, atualizarFilaDto);

                if (fila == null)
                {
                    return NotFound(new { message = "Fila não encontrada" });
                }

                return Ok(fila);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove paciente da fila
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeletarFila(Guid id)
        {
            try
            {
                var sucesso = await _filaService.DeletarFilaAsync(id);

                if (!sucesso)
                {
                    return NotFound(new { message = "Fila não encontrada" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Chama próximo paciente da fila
        /// </summary>
        [HttpPost("sala/{salaId}/chamar-proximo")]
        [Authorize]
        public async Task<ActionResult<FilaDto>> ChamarProximoPaciente(Guid salaId)
        {
            try
            {
                var fila = await _filaService.ChamarProximoPacienteAsync(salaId);

                if (fila == null)
                {
                    return NotFound(new { message = "Não há pacientes aguardando nesta sala" });
                }

                return Ok(fila);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém posição do paciente na fila
        /// </summary>
        [HttpGet("{id}/posicao")]
        public async Task<ActionResult<int>> GetPosicaoNaFila(Guid id)
        {
            try
            {
                var posicao = await _filaService.GetPosicaoNaFilaAsync(id);

                if (posicao == 0)
                {
                    return NotFound(new { message = "Fila não encontrada" });
                }

                return Ok(new { posicao });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém tempo estimado de espera para uma sala
        /// </summary>
        [HttpGet("sala/{salaId}/tempo-estimado")]
        public async Task<ActionResult<int>> GetTempoEstimado(Guid salaId)
        {
            try
            {
                var tempoEstimado = await _filaService.CalcularTempoEstimadoAsync(salaId);
                return Ok(new { tempoEstimadoMinutos = tempoEstimado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém próximo número de voucher para uma sala
        /// </summary>
        [HttpGet("sala/{salaId}/proximo-numero")]
        public async Task<ActionResult<int>> GetProximoNumeroVoucher(Guid salaId)
        {
            try
            {
                var proximoNumero = await _filaService.GetProximoNumeroVoucherAsync(salaId);
                return Ok(new { proximoNumero });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Marcar paciente como atendido
        /// </summary>
        [HttpPatch("{id}/atendido")]
        [Authorize]
        public async Task<ActionResult<FilaDto>> MarcarComoAtendido(Guid id)
        {
            try
            {
                var atualizarDto = new AtualizarFilaDto
                {
                    Status = "Atendido"
                };

                var fila = await _filaService.AtualizarFilaAsync(id, atualizarDto);

                if (fila == null)
                {
                    return NotFound(new { message = "Fila não encontrada" });
                }

                return Ok(fila);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Cancelar chamada do paciente
        /// </summary>
        [HttpPatch("{id}/cancelar")]
        [Authorize]
        public async Task<ActionResult<FilaDto>> CancelarChamada(Guid id)
        {
            try
            {
                var atualizarDto = new AtualizarFilaDto
                {
                    Status = "Cancelado"
                };

                var fila = await _filaService.AtualizarFilaAsync(id, atualizarDto);

                if (fila == null)
                {
                    return NotFound(new { message = "Fila não encontrada" });
                }

                return Ok(fila);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
    }
}