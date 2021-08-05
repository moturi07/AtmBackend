using BankApplication.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BankApplication.Controllers
{
    
    [EnableCors(origins: "http://localhost:3001", headers: "*", methods: "*")]
    public class AccountsController : ApiController
    {
        public string current_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        SqlConnection con = new SqlConnection(@"server=DESKTOP-BEK55HU; database=Atm; Integrated Security=true");
        // GET api/Get
        public string Get()
        {
            /*
            SqlDataAdapter da = new SqlDataAdapter("Select * from accounts", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(dt);
            }
            else
            {
                return "No data found";
            }
            */
            return "Not Allowed";
        }

        // GET api/Accounts/GetAccount/1
        public string GetAccount(string id)
        {
            SqlDataAdapter da = new SqlDataAdapter("Select a.id, a.first_name, a.last_name,a.email, b.account_number, FORMAT(b.balance, 'C2') 'balance' from Users a INNER JOIN Accounts b on b.user_id=a.id where a.email='" + id + "'", con);
            try
            {

                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return JsonConvert.SerializeObject(dt);
                }
                else
                {
                    return "No data found";
                }
            }
            catch(Exception ex)
            {
                return "Error";
            }
        }

        public string GetAccountNumber(string id)
        {
            SqlDataAdapter da = new SqlDataAdapter("Select b.account_number from Users a INNER JOIN Accounts b on b.user_id=a.id where a.email='" + id + "'", con);
            try
            {

                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return JsonConvert.SerializeObject(dt);
                }
                else
                {
                    return "No data found";
                }
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        // POST api/Accounts/Post
        public string Post([FromBody] string value)
        {
            return "error";
        }


        // PUT api/Accounts/MakeDeposit/5
        public string MakeDeposit(int id, [FromBody] DepositModel depositModel)
        {
            
            decimal accountNumber = depositModel.account_number;
            string transaction_type = "Deposit";
            decimal amountTransacted = depositModel.deposit_amount;
            if (amountTransacted<5)
            {
                return "Deposit Amount must be above Kes 5";
            }
            SqlCommand cmd1 = new SqlCommand("SELECT balance FROM Accounts WHERE account_number = '" + accountNumber + "'", con);
            try
            {
                con.Open();
                decimal user_balance = (decimal)cmd1.ExecuteScalar();
                decimal new_balance = user_balance + amountTransacted;
                SqlCommand cmd2 = new SqlCommand("Update Accounts SET balance='" + new_balance + "' WHERE account_number='" + accountNumber + "'", con);

                SqlCommand cmd3 = new SqlCommand("Insert into Transactions(account_number, transaction_type, amount, date) Values('" + accountNumber + "', '" + transaction_type + "', '" + amountTransacted + "', '" + current_date + "')", con);

                int i = cmd2.ExecuteNonQuery();
                int x = cmd3.ExecuteNonQuery();
                con.Close();

                if (i == 1 && x == 1)
                {
                    return "You have deposited " + amountTransacted + " to your account";
                }
                else
                {
                    return "Deposit Failed";
                }
            }catch(Exception e)
            {
                return e.Message;
            }
            
        }
        // PUT api/Accounts/MakeWithdrawal/5
        public string MakeWithdrawal(int id, [FromBody] WithdrawModel withdrawalModel)
        {
            decimal account_number = withdrawalModel.account_number;
            string transaction_type = "Withdraw";
            decimal amount_transacted = withdrawalModel.withdraw_amount;
            if (amount_transacted < 5)
            {
                return "Withdrawal Amount must be above Kes 5";
            }
            SqlCommand cmd1 = new SqlCommand("SELECT balance FROM Accounts WHERE account_number = '" + account_number + "'", con);
            try
            {
                con.Open();
                decimal user_balance = (decimal)cmd1.ExecuteScalar();
                if (user_balance >= amount_transacted)
                {
                    decimal new_balance = user_balance - amount_transacted;
                    SqlCommand cmd2 = new SqlCommand("Update Accounts SET balance='" + new_balance + "' WHERE account_number='" + account_number + "'", con);

                    SqlCommand cmd3 = new SqlCommand("Insert into Transactions(account_number, transaction_type, amount, date) Values('" + account_number + "', '" + transaction_type + "', '" + amount_transacted + "', '" + current_date + "')", con);

                    int i = cmd2.ExecuteNonQuery();
                    int x = cmd3.ExecuteNonQuery();
                    con.Close();
                    if (i == 1 && x == 1)
                    {
                        return "You have withdrawn " + amount_transacted + " from your account";
                    }
                    else
                    {
                        return "Withdrawal Failed";
                    }

                }
                else
                {
                    return "Insufficient Balance";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

    // PUT api/Accounts/TransferFunds/5
    public string TransferFunds(int id, [FromBody] TransferFundsModel makeTransfer)
        {
            decimal depositer_account = makeTransfer.account_number;
            decimal receiving_account = makeTransfer.receiver_account;
            decimal amount_transacted = makeTransfer.transfer_amount;
            if (amount_transacted < 5)
            {
                return "Transfer Amount must be above Kes 5";
            }
            con.Open();
            SqlCommand cmd1 = new SqlCommand("SELECT balance FROM Accounts WHERE account_number = '" + depositer_account + "'", con);
            if (cmd1.ExecuteScalar() == null)
            {
                return "The Account does not Exist";
            }
            decimal depositer_balance = (decimal)cmd1.ExecuteScalar();
            SqlCommand cmd2 = new SqlCommand("SELECT balance FROM Accounts WHERE account_number = '" + receiving_account + "'", con);
            if (cmd2.ExecuteScalar() == null)
            {
                return "The Account You are transferring to does not Exist";
            }
            decimal receiver_balance = (decimal)cmd2.ExecuteScalar();

            if (depositer_balance >= amount_transacted)
            {
                decimal new_depositer_balance = depositer_balance - amount_transacted;
                decimal new_receiver_balance = receiver_balance + amount_transacted;
                SqlCommand cmd3 = new SqlCommand("Update Accounts SET balance='" + new_depositer_balance + "' WHERE account_number='" + depositer_account + "'", con);
                int a = cmd3.ExecuteNonQuery();
                SqlCommand cmd4 = new SqlCommand("Update Accounts SET balance='" + new_receiver_balance + "' WHERE account_number='" + receiving_account + "'", con);
                int b = cmd4.ExecuteNonQuery();
                SqlCommand cmd5 = new SqlCommand("Insert into Transactions(account_number, transaction_type, amount, date) Values('" + depositer_account + "', 'Withdrawal', '" + amount_transacted + "', '" + current_date + "')", con);
                int c = cmd5.ExecuteNonQuery();
                SqlCommand cmd6 = new SqlCommand("Insert into Transactions(account_number, transaction_type, amount, date) Values('" + receiving_account + "', 'Deposit', '" + amount_transacted + "', '" + current_date + "')", con);
                int d = cmd6.ExecuteNonQuery();
                con.Close();
                if (a == 1 && b == 1 && c == 1 && d == 1)
                {
                    return "You have Deposited " + amount_transacted + " from your account to "+receiving_account;
                }
                else
                {
                    return "Deposit Failed";
                }
            }
            else
            {
                return "Insufficient Balance";
            }

        }
    }
}
