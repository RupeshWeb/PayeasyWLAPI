using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessEntities
{
    public class MerchantActivationEntity
    {
        public string Token { get; set; }

        [Required(ErrorMessage = "Mobile No is required.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Contact no should be 10 digits.")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Merchant first name is required.")]
        [RegularExpression("[a-zA-Z]{3,50}$", ErrorMessage = "Valid characters: Alphabets and min 3.")]
        public string FirstName { get; set; }

        [RegularExpression("[a-zA-Z]{3,50}$", ErrorMessage = "Valid characters: Alphabets and min 3.")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Merchant last name is required.")]
        [RegularExpression("[a-zA-Z]{3,50}$", ErrorMessage = "Valid characters: Alphabets and min 3.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Pan Number is required.")]
        [RegularExpression(@"[A-Z]{5}\d{4}[A-Z]{1}", ErrorMessage = "Invalid pancard format.")]
        public string PanNumber { get; set; }

        [MaxLength(12)]
        [Required(ErrorMessage = "Aadhar No is required.")]
        [RegularExpression("[0-9]{12}", ErrorMessage = "Aadhar no should be 12 digits.")]
        public string AadharNo { get; set; }

        [MaxLength(6)]
        [Required(ErrorMessage = "Pincode is required.")]
        [DataType(DataType.PostalCode)]
        [RegularExpression("[0-9]{6}", ErrorMessage = "Pincode should be 6 digits.")]
        public string PinCode { get; set; }

        [Required(ErrorMessage = "State Name is required.")]
        public string StateName { get; set; }

        [Required(ErrorMessage = "District is required.")]
        public string District { get; set; }

        [Display(Name = "Proof of ID")]
        [Required(ErrorMessage = "Document1 is required.")]
        public string Document1 { get; set; }

        [Display(Name = "Proof of Address Front")]
        [Required(ErrorMessage = "Document2 is required.")]
        public string Document2 { get; set; }

        [Display(Name = "Proof of Address Back")]
        [Required(ErrorMessage = "Document3 is required.")]
        public string Document3 { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
    public class MerchantActivationListEntity
    {
        public int ID { get; set; }
        public string MobileNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PanNumber { get; set; }
        public string AadharNo { get; set; }
        public int PinCode { get; set; }
        public string StateName { get; set; }
        public string District { get; set; }
        public string Document1 { get; set; }
        public string Document2 { get; set; }
        public string Document3 { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime EditDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }

    public class MerchantActivationResponseEntity
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public MerchantActivationListEntity Data { get; set; }
    }
}
