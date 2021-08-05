using BankApplication.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BankApplication.Controllers
{
    [EnableCors(origins: "http://localhost:3001", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {        
        SqlConnection con = new SqlConnection(@"server=DESKTOP-BEK55HU; database=Atm; Integrated Security=true");
        
        // GET api/values/5
        public string Get(int id)
        {
            
            SqlDataAdapter da = new SqlDataAdapter("Select TOP 20 account_number, transaction_type, amount, date from Transactions where account_number='" + id+"' ORDER BY date DESC", con);
            try{
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
                return ex.Message;
            }
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
