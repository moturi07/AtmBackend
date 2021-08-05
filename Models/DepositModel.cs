using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankApplication.Models
{
    public class DepositModel
    {
        public int account_number { get; set; }
        public decimal deposit_amount { get; set; }

    }
}