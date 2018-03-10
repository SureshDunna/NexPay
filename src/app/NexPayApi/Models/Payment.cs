using System.ComponentModel.DataAnnotations;

namespace NexPayApi.Models
{
    public class Payment
    {
        [Required]
        public string BSB { get; set; }

        [Required]
        public long AccountNumber { get; set; } 

        [Required]
        public string AccountName { get; set; }

        public string Reference { get; set; }

        [Required]
        public double Amount { get; set; }
    }
}