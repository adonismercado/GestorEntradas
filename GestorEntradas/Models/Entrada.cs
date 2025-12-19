using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorEntradas.Models;

public class Entrada
{
    [Key]
    public int EntradaId { get; set; }

    [Required(ErrorMessage = "La fecha de entrada es obligatoria.")]
    public DateTime FechaEntrada { get; set; }

    [Required(ErrorMessage = "El concepto es obligatorio.")]
    public string Concepto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El total es obligatorio.")]
    public decimal Total { get; set; }

    [InverseProperty("Entrada")]
    public virtual ICollection<EntradaDetalle> Detalles { get; set; } = new List<EntradaDetalle>();
}
