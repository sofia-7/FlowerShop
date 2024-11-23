using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Order
{
    [Key]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100)]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [StringLength(15)]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(250)]
    public string Address { get; set; }

    [Required(ErrorMessage = "BouqetId is required.")]
    public int BouqetId { get; set; }
}



