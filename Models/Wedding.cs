#pragma warning disable CS8618
#pragma warning disable CS8605

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Transactions;
namespace WeedingPlanner.Models;

public class Wedding
{
    [Key]
    public int WeddingId {get;set;}
    [Required]
    public string WedderOne {get;set;}
    [Required]
    public string WedderTwo {get;set;}
    [Required]
    [FutureDate]
    public DateTime Date{get;set;}
    [Required]
    public string Adress {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
    [Required]
    public int UserId{get;set;}
    public User Creator{get;set;}
    public List<Invitation> Guest {get;set;}

}

public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if((DateTime)value <= DateTime.Today)
        {
            return new ValidationResult("Only Date in the future allowed");
        }
        return ValidationResult.Success;
    }
}