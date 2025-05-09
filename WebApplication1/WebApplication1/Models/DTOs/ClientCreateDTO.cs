using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Controllers.DTOs;

public class ClientCreateDTO
{
    [Required(ErrorMessage = "First Name is required")]
    [StringLength(120, ErrorMessage = "First Name cant be longer than 120 characters")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(120, ErrorMessage = "Last name cant be longer than 120 characters")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Incorrect email format")]
    [StringLength(120, ErrorMessage = "Email cant be longer than 120 characters")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\+?[0-9\s\-()]{9,}$", 
        ErrorMessage = "Incorrect phone number format")]
    [StringLength(120, ErrorMessage = "Phone number cant be longer than 120 characters")]
    public string Telephone { get; set; }
    
    [Required(ErrorMessage = "PESEL is required")]
    [RegularExpression(@"^\d{11}$", 
        ErrorMessage = "PESEL must be 11 digit number")]
    public string Pesel { get; set; }
}