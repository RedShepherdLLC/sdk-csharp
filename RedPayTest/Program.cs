using RedPay;
using System;

namespace RedPayTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Config config = new Config();
            config.App = "DEMO";
            config.AuthToken = "vZ9cvj3lONTEGWmuzTJ9tdjmDoEUEb7dPkdMdXyP1/4=";
            config.ApiEndpoint = "https://redpaydemo.azurewebsites.net";

            CardProcessor processor = new CardProcessor(config);
            RedPayResponse result = processor.Purchase(
                           1200,               //double amount in cents
                           "4111111111111111", //string cardnumber
                           "RED PAY TEST",     // string cardholdername
                           "07" + "2018",      //string expdate
                           "020",              //string cvv
                           "60603"             //string zipcode
                           );
            Console.WriteLine(result.responseCode);
            Console.ReadKey();
        }
    }
}
