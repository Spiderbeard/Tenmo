using System.ComponentModel.DataAnnotations;

namespace TenmoClient.Data
{
    public class Transfer
    {
        public int transfer_ID { get; set; }
        public int type_ID { get; set; }
        public int status_ID { get; set; }
        public string account_From_UserName { get; set; }
        public string account_To_UserName { get; set; }

        [Required(ErrorMessage = "The field 'Account_From_ID' should not be blank.")]
        public int account_From_ID { get; set; }

        [Required(ErrorMessage = "The field 'Account_To_ID' should not be blank.")]
        public int account_To_ID { get; set; }

        [Range(1, double.PositiveInfinity, ErrorMessage = "The field 'AmountToTransfer' should be greater than 0.")]
        public decimal AmountToTransfer { get; set; }
        
    }
}
