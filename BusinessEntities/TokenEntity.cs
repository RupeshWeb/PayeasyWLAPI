using System.ComponentModel.DataAnnotations;

namespace BusinessEntities
{
    public class TokenGenerateResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class TokenValidateEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public int UserID { get; set; }
    }

    public class TokenRequestEntity
    {
        [MaxLength(80)]
        [Required(ErrorMessage = "Token is Required.")]
        public string Token { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Retailer ID is Required.")]
        public string RetailerID { get; set; }

        [MaxLength(80)]
        [Required(ErrorMessage = "Logo url is Required.")]
        public string LogoUrl { get; set; }

        [MaxLength(80)]
        [Required(ErrorMessage = "Copyright is Required.")]
        public string Copyright { get; set; }

        //[MaxLength(10)]
        //[Required(ErrorMessage = "Merchant ID is Required.")]
        //[RegularExpression("[0-9]{10}", ErrorMessage = "Merchant ID should be 10 digits.")]
        public string MerchantId { get; set; }

        [Required(ErrorMessage = "Firm Name is Required.")]
        [RegularExpression("[a-zA-Z0-9. ]{3,80}$", ErrorMessage = "Valid characters: Alphabets, numbers, dots, min 3 and max 80.")]
        public string FirmName { get; set; }

        //[MaxLength(3)]
        //[Required(ErrorMessage = "Service ID is Required.")]
        //[RegularExpression("[1-5]{3}", ErrorMessage = "Service ID should be digits.")]
        public string ServiceID { get; set; }
    }

    public class TokenResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public TokenResponseDataEntity Data { get; set; }
    }
    public class TokenResponseDataEntity
    {
        public string Token { get; set; }
    }
    public class TokenRequestResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Copyright { get; set; }
        public string Logourl { get; set; }
        public string Value { get; set; }
        public string FirmName { get; set; }
        public string ServiceId { get; set; }
        public int Status { get; set; }
    }
}
