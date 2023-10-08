#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace WeedingPlanner.Models;

public class Invitation
{
    [Key]
    public int InvitationId{get;set;}
    [Required]
    public int UserId{get;set;}
    [Required]
    public int WeddingId {get;set;}
    public User? User {get;set;}
    public Wedding? Wedding {get;set;}
}   