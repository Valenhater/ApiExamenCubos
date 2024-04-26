using ApiExamenCubos.Helpers;
using ApiExamenCubos.Models;
using ApiExamenCubos.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiExamenCubos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryCubos repo;
        private HelperActionServicesOAuth  helper;

        public AuthController(RepositoryCubos repo, HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            Usuario user = await this.repo.LoginAsync(model.Email, model.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            else
            {
                //DEBEMOS CREAR UNAS CREDENCIALES PARA 
                //INCLUIRLAS DENTRO DEL TOKEN Y QUE ESTARAN 
                //COMPUESTAS POR EL SECRET KEY CIFRADO Y EL TIPO
                //DE CIFRADO QUE DESEEMOS INCLUIR EN EL TOKEN
                SigningCredentials credentials =
                    new SigningCredentials(
                        this.helper.GetKeyToken()
                        , SecurityAlgorithms.HmacSha256);

                //CONVERTIMOS EL EMPLEADO A FORMATO JSON
                string jsonUsuario = JsonConvert.SerializeObject(user);
                //ESTO DEBE IR CIFRADO POR NOSOTROS

                //CREAMOS UN ARRAY DE CLAIMS CON TODA 
                //LA INFORMACION QUE DESEAMOS GUARDAR EN EL TOKEN
                Claim[] informacion = new[]
                {
                    new Claim("UserData", jsonUsuario)
                };

                //EL TOKEN SE GENERA CON UNA CLASE Y 
                //DEBEMOS INDICAR LOS ELEMENTOS QUE ALMACENARA 
                //DENTRO DE DICHO TOKEN, POR EJEMPLO, ISSUER,
                //AUDIENCE O EL TIEMPO DE VALIDACION DEL TOKEN
                JwtSecurityToken token =
                    new JwtSecurityToken(
                        claims: informacion,
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        notBefore: DateTime.UtcNow
                        );
                //POR ULTIMO, DEVOLVEMOS UNA RESPUESTA AFIRMATIVA
                //CON UN OBJETO ANONIMO EN FORMATO JSON
                return Ok(
                    new
                    {
                        response = new JwtSecurityTokenHandler().WriteToken(token)
                    });
            }
        }
    }
}
