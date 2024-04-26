using ApiExamenCubos.Data;
using ApiExamenCubos.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiExamenCubos.Repositories
{
    public class RepositoryCubos
    {
        private CubosContext context;
        public RepositoryCubos(CubosContext context)
        {
            this.context = context;
        }
        public async Task<List<Cubo>> GetCubosAsync()
        {
            return await this.context.Cubos.ToListAsync();
        }
        public async Task<List<Cubo>> GetCubosMarcaAsync(string marca)
        {
            return await this.context.Cubos.Where(x => x.Marca == marca).ToListAsync();
        }
        public async Task<Usuario> LoginAsync(string email, string password)
        {
            return await this.context.Usuarios.Where(x => x.Email == email && x.Password == password).FirstOrDefaultAsync();
        }
        public async Task InsertarUsuarioAsync(Usuario user)
        {
            this.context.Usuarios.Add(user);
            await this.context.SaveChangesAsync();
        }
        public async Task<Usuario> FindUsuarioAsync(int idUsuario)
        {
            return await this.context.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);
        }
        public async Task<List<Compra>> GetComprasAsync(int idUsuario)
        {
            return await this.context.Compras.Where(x => x.IdUsuario == idUsuario).ToListAsync();
        }
        public async Task InsertarCompraAsync(Compra compra)
        {
            this.context.Compras.Add(compra);
            await this.context.SaveChangesAsync();
        }

    }
}
