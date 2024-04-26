using ApiExamenCubos.Models;
using ApiExamenCubos.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiExamenCubos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CubosController : ControllerBase
    {
        private RepositoryCubos repo;

        public CubosController(RepositoryCubos repo)
        {
            this.repo = repo;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<Cubo>>> GetPeliculas()
        {
            return await this.repo.GetCubosAsync();
        }
        [HttpGet("[action]/{marca}")]
        public async Task<ActionResult<List<Cubo>>> GetCubosMarca(string marca)
        {
            return await this.repo.GetCubosMarcaAsync(marca);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Registro(Usuario user)
        {
            await this.repo.InsertarUsuarioAsync(user);
            return Ok();
        }
        [Authorize]
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<Usuario>> GetPerfilUsuario(int id)
        {
            return await this.repo.FindUsuarioAsync(id);
        }
        [Authorize]
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<List<Compra>>> GetComprasUsuario(int id)
        {
            return await this.repo.GetComprasAsync(id);
        }
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> Comprar(Compra compra)
        {
            string jsonUsuario = HttpContext.User.FindFirst(x => x.Type == "UserData").Value;
            Usuario usuarioLogeado = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            compra.IdUsuario = usuarioLogeado.IdUsuario;
            await this.repo.InsertarCompraAsync(compra);
            return Ok();
        }
    }
}
