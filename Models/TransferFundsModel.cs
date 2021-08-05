using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankApplication.Models
{
    public class TransferFundsModel
    {
        public int account_number { get; set; }
        public decimal transfer_amount { get; set; }
        public int receiver_account { get; set; }
    }
}