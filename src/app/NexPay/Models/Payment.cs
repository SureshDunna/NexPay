using System.ComponentModel.DataAnnotations;

namespace NexPay.Models
{
    public class Payment
    {
        [Required]
        public string BSB { get; set; }

        [Required]
        public long AccNo { get; set; } 

        [Required]
        public string AccName { get; set; }

        public string PayRef { get; set; }

        [Required]
        public double Amount { get; set; }
    }
}