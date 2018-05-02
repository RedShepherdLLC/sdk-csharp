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
			RedPayResponse result1 = processor.Purchase(
										 1650,               //double amount in cents
										 "4111111111111111", //string cardnumber
										 "RED PAY TEST",     // string cardholdername
										 "07" + "2018",      //string expdate
										 "020",              //string cvv
										 "60603",             //string zipcode
										 null,
										 null,
										 null,
										 "720 W 27 st",
										 "#350",
										 "Los Angeles",
										 "demo@redshepherd.com",
										 "6547890231",
										 "CNP",
										 null,
										 "E7567",
										 "C123",
										 "O4355",
										 "T78678",
										 "E",
										 "r1",
										 "r2",
										 "r3",
										 "r4",
										 "r5"
										 );
			Console.WriteLine(result1.responseCode);

			//RedPayResponse result2 = processor.Void(
			//           result1.transactionId
			//              );

			//Console.WriteLine(result2.responseCode);

			RedPayResponse result3 = processor.Refund(
							result1.transactionId,
							result1.amount
								 );
			Console.WriteLine(result3.responseCode);

			Console.ReadKey();
		}
	}
}
