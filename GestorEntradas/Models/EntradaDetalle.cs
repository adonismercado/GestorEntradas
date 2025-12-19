using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorEntradas.Models;

public class EntradaDetalle
{
    [Key]
    public int EntradaDetalleId { get; set; }

    public int EntradaId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal Costo { get; set; }

    [ForeignKey("EntradaId")]
    [InverseProperty("Detalles")]
    public virtual Entrada Entrada { get; set; }

    [ForeignKey("ProductoId")]
    [InverseProperty("EntradaDetalles")]
    public virtual Producto Producto { get; set; }
}
