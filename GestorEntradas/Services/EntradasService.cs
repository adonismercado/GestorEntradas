using GestorEntradas.DAL;
using GestorEntradas.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestorEntradas.Services;

public class EntradasService(IDbContextFactory<Contexto> DbFactory)
{
    public async Task<bool> Existe(int entradaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Entradas
            .AnyAsync(e => e.EntradaId == entradaId);
    }

    private async Task<bool> Insertar(Entrada entrada)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Entradas.Add(entrada);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task AfectarExistenciaProducto(EntradaDetalle[] detalle, TipoOperacion operacion)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        foreach (var item in detalle)
        {
            var producto = await contexto.Productos
                .SingleAsync(p => p.ProductoId == item.ProductoId);

            if (operacion == TipoOperacion.Suma)
            {
                producto.Existencia += item.Cantidad;
            }
            else if (operacion == TipoOperacion.Resta)
            {
                producto.Existencia -= item.Cantidad;
            }

            await contexto.SaveChangesAsync();
        }
    }
    
    private async Task<bool> Modificar(Entrada entrada)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var entradaAnterior = await contexto.Entradas
            .Include(e => e.Detalles)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EntradaId == entrada.EntradaId);

        if (entradaAnterior == null)
        {
            return false;
        }

        // Restar cantidad original a la existencia
        await AfectarExistenciaProducto(entradaAnterior.Detalles.ToArray(), TipoOperacion.Resta);

        // Sumar nueva cantidad a la existencia
        await AfectarExistenciaProducto(entrada.Detalles.ToArray(), TipoOperacion.Suma);

        contexto.Entradas.Update(entrada);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Entrada entrada)
    {
        if (!await Existe(entrada.EntradaId))
        {
            return await Insertar(entrada);
        }
        else
        {
            return await Modificar(entrada);
        }
    }

    public async Task<Entrada?> Buscar(int entradaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Entradas
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(e => e.EntradaId == entradaId);
    }

    public async Task<List<Entrada>> Listar(Expression<Func<Entrada, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Entradas
            .Include(e => e.Detalles)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> Eliminar(int entradaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var entrada = await contexto.Entradas
            .Include(e => e.Detalles)
            .FirstOrDefaultAsync(e => e.EntradaId == entradaId);
        
        if (entrada == null)
        {
            return false;
        }

        await AfectarExistenciaProducto(entrada.Detalles.ToArray(), TipoOperacion.Resta);

        contexto.EntradaDetalles.RemoveRange(entrada.Detalles);
        contexto.Entradas.Remove(entrada);

        return await contexto.SaveChangesAsync() > 0;
    }
}

public enum TipoOperacion
{
    Suma = 1,
    Resta = 2
}