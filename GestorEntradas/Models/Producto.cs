using Microsoft.EntityFrameworkCore.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorEntradas.Models;

public class Producto
{
    [Key]
    public int ProductoId { get; set; }

    [Required(ErrorMessage = "La descripcion del producto es obligatoria.")]
    public string Descripcion { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor que 0.")]
    [Required(ErrorMessage = "El costo es obligatorio.")]
    public decimal Costo { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0.")]
    [Required(ErrorMessage = "El precio es obligatorio.")]
    public decimal Precio { get; set; }

    public int Existencia { get; set; }

    [InverseProperty("Producto")]
    public virtual ICollection<EntradaDetalle> EntradaDetalles { get; set; } = new List<EntradaDetalle>();
}
