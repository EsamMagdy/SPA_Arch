using System.ComponentModel.DataAnnotations;

namespace OA_Service.DTOs
{
    public class RegisterDto
    {
        //  [Required]
        [EmailAddress]
        //  [DataType(DataType.Password)]
        public string Email { get; set; }
        //[Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
 
        //public string FullName { get; set; }
    }
}