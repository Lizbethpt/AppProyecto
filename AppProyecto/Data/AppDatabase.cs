using AppProyecto.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AppProyecto.Data
{
    public class AppDatabase
    {
        readonly SQLiteAsyncConnection _database;
        public AppDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<TipoUsuario>().Wait();
            _database.CreateTableAsync<Usuario>().Wait();
            _database.CreateTableAsync<TipoProducto>().Wait();
            _database.CreateTableAsync<Producto>().Wait();
            _database.CreateTableAsync<Existencia>().Wait();
            _database.CreateTableAsync<Venta>().Wait();
            _database.CreateTableAsync<DetalleVenta>().Wait();
            _database.CreateTableAsync<Cliente>().Wait();
        }
        // Guardar (insertar o actualizar)
        public Task<int> SaveAsync<T>(T item) where T : new()
        {
            // Obtengo el valor de la propiedad "ID" (que es tu PK autoincrement)
            var prop = typeof(T).GetProperty("ID");
            if (prop == null)
                throw new InvalidOperationException("No se encontró propiedad ID en el tipo " + typeof(T));

            var id = (int)prop.GetValue(item);

            if (id == 0)
            {
                // Si es nuevo (ID=0) → inserto
                return _database.InsertAsync(item);
            }
            else
            {
                // Si ya existe (ID>0) → actualizo
                return _database.UpdateAsync(item);
            }
        }

        // Obtener todos los registros
        public Task<List<T>> GetAllAsync<T>() where T : new()
        {
            return _database.Table<T>().ToListAsync();
        }
        // Obtener por ID
        public Task<T> GetByIdAsync<T>(int id) where T : class, new()
        {
            return _database.FindAsync<T>(id);
        }
        // Eliminar por objeto
        public Task<int> DeleteAsync<T>(T item) where T : new()
        {
            return _database.DeleteAsync(item);
        }
        //Productos
        public Task<int> SaveProductoAsync(Producto producto)
        {
            return _database.InsertAsync(producto);
        }
        public Task<List<Producto>> GetProductosAsync()
        {
            return _database.Table<Producto>().ToListAsync();
        }
        //existencia
        public Task<List<Existencia>> GetExistenciasAsync()
        {
            return _database.Table<Existencia>().ToListAsync();
        }
        public async Task<int> SaveExistenciaAsync(Existencia existencia)
        {
            var existing = await _database.FindAsync<Existencia>(existencia.ID);
            if (existing != null)
            {
                return await _database.UpdateAsync(existencia);
            }
            else
            {
                return await _database.InsertAsync(existencia);
            }
        }
        //Tipo de usuarios
        public Task<List<TipoUsuario>> GetTiposUsuarioAsync()
        {
            return _database.Table<TipoUsuario>().ToListAsync();
        }
        public Task<int> SaveTipoUsuarioAsync(TipoUsuario tipoUsuario)
        {
            return _database.InsertAsync(tipoUsuario);
        }
        //cliente
        public Task<List<Cliente>> GetClientesAsync()
        {
            return _database.Table<Cliente>().ToListAsync();
        }
        public Task<int> SaveClientesAsync(Cliente cliente)
        {
            return _database.InsertAsync(cliente);
        }
        //Usuario
        public Task<int> SaveUsuarioAsync(Usuario usuario)
        {
            return _database.InsertOrReplaceAsync(usuario);
        }
        public Task<List<Usuario>> GetUsuariosAsync()
        {
            return _database.Table<Usuario>().ToListAsync();
        }
        // TipoProducto
        public Task<List<TipoProducto>> GetTiposProductoAsync()
        {
            return _database.Table<TipoProducto>().ToListAsync();
        }

        // Método para obtener los detalles de una venta por su ID
        public Task<List<DetalleVenta>> GetDetallesVentaByVentaId(int ventaId)
        {
            return _database.Table<DetalleVenta>()
                            .Where(d => d.ID_Venta == ventaId)
                            .ToListAsync();
        }
        // Método para actualizar el estado de una venta
        public async Task<int> UpdateVentaAsync(Venta venta)
        {
            return await _database.UpdateAsync(venta);
        }
        // Método para obtener un producto por su código
        public Task<Producto> GetProductoByCodigoAsync(string codigo)
        {
            return _database.Table<Producto>().Where(p => p.Codigo_Barras == codigo).FirstOrDefaultAsync();
        }

        // Método para obtener una venta por su código
        public Task<Venta> GetVentaByCodigoAsync(string codigo)
        {
            return _database.Table<Venta>().Where(v => v.Codigo == codigo).FirstOrDefaultAsync();
        }

    }
}
