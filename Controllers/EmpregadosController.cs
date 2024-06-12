using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace api.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class EmpregadosController : ControllerBase
    {        
        private readonly DataContext dc;
        private readonly IConfiguration _configuration;

        public EmpregadosController(DataContext dc, IConfiguration configuration)
        {
            this.dc = dc;
            _configuration = configuration;
        }

        private bool ValidarToken()
        {
            var secretToken = _configuration.GetValue<string>("Token:SecretKey");
            var accessToken = Request.Headers["access_token"].ToString();
            return accessToken == secretToken;
        }
        [HttpGet("list")]
        public async Task<ActionResult<List<Empregado>>> List(Guid? id)
        {
            if (!ValidarToken())
            {
                return Unauthorized("Token inválido.");
            }

            if (id.HasValue)
            {
                var empregado = await dc.Empregados.FindAsync(id.Value);
                return Ok(empregado);
            }
            else
            {
                return await dc.Empregados.ToListAsync();

            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<Empregado>> Create([FromBody] Empregado empregado)
        {
            if (!ValidarToken())
            {
                return Unauthorized("Token inválido.");
            }
            dc.Empregados.Add(empregado);
            await dc.SaveChangesAsync();

            return CreatedAtAction(nameof(Create), new { id = empregado.Id }, empregado);
        }

        [HttpPut("update")]
        public async Task<ActionResult<Empregado>> Update([FromQuery] Guid id, [FromBody] Empregado empregado)
        {
            if (!ValidarToken())
            {
                return Unauthorized("Token inválido.");
            }
            var novoEmpregado = await dc.Empregados.FindAsync(id);

            if (novoEmpregado == null)
            {
                return NotFound();
            }

            novoEmpregado.FirstName = empregado.FirstName;
            novoEmpregado.LastName = empregado.LastName;
            novoEmpregado.Email = empregado.Email;
            novoEmpregado.CPF = empregado.CPF;
            novoEmpregado.DateOfBirth = empregado.DateOfBirth;
            novoEmpregado.Address = empregado.Address;
            novoEmpregado.CurrentlyEmployed = empregado.CurrentlyEmployed;

            await dc.SaveChangesAsync();

            return Ok(novoEmpregado);
        }
    }
}