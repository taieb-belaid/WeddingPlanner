#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Transactions;
namespace WeedingPlanner.Models;

public class User
{
    [Key]
    public int UserId { get; set; }
    [Required]
    [MinLength(3)]
    public string FirstName { get; set; }

    [Required]
    [MinLength(3)]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [MinLength(6)]
    public string Password { get; set; }
    [NotMapped]
    [Compare("Password",ErrorMessage ="The password do not match.")]
    
    public string ConfirmPassword { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public List <Wedding> MyWeddings {get;set;} = new List<Wedding>();
    public List<Invitation> MyInvitaion {get;set;} = new List<Invitation>();
    
}
