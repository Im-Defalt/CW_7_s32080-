using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Controllers.DTOs;

public class ClientCreateDTO
{
    [StringLength(120, ErrorMessage = "First Name cant be longer than 120 characters")]
    public required string FirstName { get; set; }
    
    [StringLength(120, ErrorMessage = "Last name cant be longer than 120 characters")]
    public required string LastName { get; set; }
    
    [EmailAddress(ErrorMessage = "Incorrect email format")]
    [StringLength(120, ErrorMessage = "Email cant be longer than 120 characters")]
    public required string Email { get; set; }
    
    [RegularExpression(@"^\+?[0-9\s\-()]{9,}$", 
        ErrorMessage = "Incorrect phone number format")]
    [StringLength(120, ErrorMessage = "Phone number cant be longer than 120 characters")]
    public required string Telephone { get; set; }
    
    [RegularExpression(@"^\d{11}$", 
        ErrorMessage = "PESEL must be 11 digit number")]
    public required string Pesel { get; set; }
}