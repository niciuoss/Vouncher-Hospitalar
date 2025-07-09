using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVoucher.DTOs;
using SistemaVoucher.Services;

namespace SistemaVoucher.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;

        public PacientesController(IPacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        /// <summary>
        /// Obtém todos os pacientes
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PacienteDto>>> GetTodosPacientes()
        {
            try
            {
                var pacientes = await _pacienteService.GetTodosPacientesAsync();
                return Ok(pacientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém paciente por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PacienteDto>> GetPacientePorId(Guid id)
        {
            try
            {
                var paciente = await _pacienteService.GetPacientePorIdAsync(id);

                if (paciente == null)
                {
                    return NotFound(new { message = "Paciente não encontrado" });
                }

                return Ok(paciente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém paciente por CPF
        /// </summary>
        [HttpGet("cpf/{cpf}")]
        public async Task<ActionResult<PacienteDto>> GetPacientePorCpf(string cpf)
        {
            try
            {
                var paciente = await _pacienteService.GetPacientePorCpfAsync(cpf);

                if (paciente == null)
                {
                    return NotFound(new { message = "Paciente não encontrado" });
                }

                return Ok(paciente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Cria novo paciente
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PacienteDto>> CriarPaciente([FromBody] CriarPacienteDto criarPacienteDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar se CPF já existe
                var pacienteExiste = await _pacienteService.PacienteExisteAsync(criarPacienteDto.Cpf);
                if (pacienteExiste)
                {
                    return Conflict(new { message = "Paciente com este CPF já existe" });
                }

                var paciente = await _pacienteService.CriarPacienteAsync(criarPacienteDto);

                return CreatedAtAction(
                    nameof(GetPacientePorId),
                    new { id = paciente.Id },
                    paciente
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza paciente existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<PacienteDto>> AtualizarPaciente(Guid id, [FromBody] AtualizarPacienteDto atualizarPacienteDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var paciente = await _pacienteService.AtualizarPacienteAsync(id, atualizarPacienteDto);

                if (paciente == null)
                {
                    return NotFound(new { message = "Paciente não encontrado" });
                }

                return Ok(paciente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Deleta paciente
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeletarPaciente(Guid id)
        {
            try
            {
                var sucesso = await _pacienteService.DeletarPacienteAsync(id);

                if (!sucesso)
                {
                    return NotFound(new { message = "Paciente não encontrado" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Verifica se paciente existe por CPF
        /// </summary>
        [HttpGet("existe/{cpf}")]
        public async Task<ActionResult<bool>> PacienteExiste(string cpf)
        {
            try
            {
                var existe = await _pacienteService.PacienteExisteAsync(cpf);
                return Ok(new { existe });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
    }
}