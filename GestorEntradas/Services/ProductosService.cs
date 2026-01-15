using GestorEntradas.DAL;
using GestorEntradas.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace GestorEntradas.Services;

public class ProductosService(IDbContextFactory<Contexto> DbFactory)
{
    public async Task<bool> Existe(int productoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Productos
            .AnyAsync(p => p.ProductoId == productoId);
    }

    private async Task<bool> Insertar(Producto producto)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Productos.Add(producto);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Producto producto)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Productos.Update(producto);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Producto producto)
    {
        if (!await Existe(producto.ProductoId))
        {
            return await Insertar(producto);
        }
        else
        {
            return await Modificar(producto);
        }
    }

    public async Task<Producto?> Buscar(int productoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Productos
            .Include(p => p.EntradaDetalles)
            .FirstOrDefaultAsync(p => p.ProductoId == productoId);
    }

    public async Task<List<Producto>> ListarProductos(Expression<Func<Producto, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Productos
            .Include(p => p.EntradaDetalles)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> Eliminar(int productoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var producto = await contexto.Productos
            .FirstOrDefaultAsync(p => p.ProductoId == productoId);

        if (producto == null)
        {
            return false;
        }

        contexto.Productos.Remove(producto);
        return await contexto.SaveChangesAsync() > 0;
    }
}
