using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankApplication.Models
{
    public class WithdrawModel
    {
        public int account_number { get; set; }
        public decimal withdraw_amount { get; set; }
    }
}