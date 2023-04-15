namespace Ussd.App.BLL.Models
{
    public class SubmitGeneralEnquiryRequestDto
    {
        public required string CustomerName { get; set; } 
        public required string PhoneNumber { get; set; } 
        public required string GeneralEnquiryQuestion { get; set; } 
    }
}
