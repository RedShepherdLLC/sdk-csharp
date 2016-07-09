using RedPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedPayTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Config config = new Config();
            config.App = "DEMO";
            config.AuthToken = "vZ9cvj3lONTEGWmuzTJ9tdjmDoEUEb7dPkdMdXyP1/4=";
            config.ApiEndpoints = "https://redpaydemo.azurewebsites.net";

            CreditCardProcessor card = new CreditCardProcessor(config);
            var result = card.Purchase(
                           1200,               //double amount in cents
                           "RED PAY TEST",        // string cardholdersname
                           "4111111111111111", //string cardnumber
                           "020",              //string ccv
                           "07" + "2018",      //string expdate
                           "60603",            //string zipcode
                           "CNP",              //string method
                           null,               //track1
                           null,               //track2
                           null,               //signaturedata
                           "DEMO",             //string businessid
                           "C123",             //string clientid
                           "P123",             //string paymentid
                           "USD"              //currency
                           );
            Console.WriteLine(result.Item2);
        }
    }
}
