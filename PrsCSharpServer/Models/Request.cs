using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrsCSharpServer.Models;

public class Request {

    public int Id { get; set; } = 0;
    [StringLength(80)]
    public string Description { get; set; } = string.Empty;
    [StringLength(80)]
    public string Justification { get; set; } = string.Empty;
    [StringLength(80)]
    public string? RejectionReason { get; set; } = null;
    [StringLength(20)]
    public string DeliveryMode { get; set; } = "Pickup";
    [StringLength(10)]
    public string Status { get; set; } = "NEW";
    [Column(TypeName = "decimal(11,2)")]
    public decimal Total { get; set; } = decimal.Zero;

    public int UserId { get; set; } = 0;
    public virtual User? User { get; set; } = null;

    public virtual ICollection<Requestline>? Requestlines { get; set; } = default!;
}
