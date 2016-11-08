using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace RedPay
{
    /// <summary>
    /// RedPay Card Processing API
    /// </summary>
    public class CardProcessor
    {
        private readonly Config config;

        public CardProcessor(Config c)
        {
            config = c;
        }

        /// <summary>
        /// Charge a Card
        /// </summary>
        /// <param name="amount">Amount in CENTS</param>
        /// <param name="account">Card number, All numeric. No dashes or spaces</param>
        /// <param name="cardHolderName">Name of Cardholder</param>
        /// <param name="expmmyyyy">Card expiration date. All numeric. No slashes or spaces</param>
        /// <param name="ccv">Card CVV number. All numeric</param>
        /// <param name="avsZip">Card zip code. All numeric</param>
        /// <param name="track1Data">Swiped card data</param>
        /// <param name="track2Data">Swiped card data</param>
        /// <param name="signatureData">Stringified signature data</param>
        /// <param name="avsAddress1">Card address line 1</param>
        /// <param name="avsAddress2">Card address line 2</param>
        /// <param name="avsCity">Card city</param>
        /// <param name="cardHolderEmail">Email address</param>
        /// <param name="cardHolderPhone">Phone number. All numeric. No dashes or spaces</param>
        /// <param name="method">CP - card present; CNP - card not present</param>
        /// <param name="retryCount">1 - retry upon failure or null</param>
        /// <param name="employeeRefNum">Employee id number</param>
        /// <param name="customerRefNum">Customer id number</param>
        /// <param name="orderRefNum">Purchase id number</param>
        /// <param name="terminalRefNum">Terminal id number</param>
        /// <returns>RedPayResponse</returns>
        public RedPayResponse Purchase(long amount,
            string account, string cardHolderName, string expmmyyyy, string ccv, string avsZip,
            string track1Data = null, string track2Data = null, string signatureData = null, 
            string avsAddress1 = null, string avsAddress2 = null, string avsCity = null, 
            string cardHolderEmail = null, string cardHolderPhone = null, string method = null,
            string retryCount = null, string employeeRefNum = null, string customerRefNum = null, string orderRefNum = null, string terminalRefNum = null,
            string productRef = null, string ref1 = null, string ref2 = null, string ref3 = null, string ref4 = null, string ref5 = null)
        {
            RedPayRequest req = new RedPayRequest();

            //Required
            req.action = "A";
            req.amount = amount;
            req.account = account;
            req.cardHolderName = cardHolderName;
            req.expmmyyyy = expmmyyyy;
            req.cvv = ccv;
            req.avsZip = avsZip;

            //Optional
            if (!string.IsNullOrEmpty(track1Data))
                req.track1Data = track1Data;

            if (!string.IsNullOrEmpty(track2Data))
                req.track2Data = track2Data;

            if (!string.IsNullOrEmpty(signatureData))
                req.signatureData = signatureData;

            if (!string.IsNullOrEmpty(avsAddress1))
                req.avsAddress1 = avsAddress1;

            if (!string.IsNullOrEmpty(avsAddress2))
                req.avsAddress2 = avsAddress2;

            if (!string.IsNullOrEmpty(avsCity))
                req.avsCity = avsCity;

            if (!string.IsNullOrEmpty(cardHolderEmail))
                req.cardHolderEmail = cardHolderEmail;

            if (!string.IsNullOrEmpty(cardHolderPhone))
                req.cardHolderPhone= cardHolderPhone;

            if (!string.IsNullOrEmpty(method))
                req.method = method;

            if (!string.IsNullOrEmpty(retryCount))
                req.retryCount = retryCount;

            if (!string.IsNullOrEmpty(employeeRefNum))
                req.employeeRefNum = employeeRefNum;

            if (!string.IsNullOrEmpty(customerRefNum))
                req.customerRefNum = customerRefNum;

            if (!string.IsNullOrEmpty(orderRefNum))
                req.orderRefNum = orderRefNum;

            if (!string.IsNullOrEmpty(terminalRefNum))
                req.terminalRefNum = terminalRefNum;

            if (!string.IsNullOrEmpty(productRef))
                req.productRef = productRef;

            if (!string.IsNullOrEmpty(ref1))
                req.ref1 = ref1;

            if (!string.IsNullOrEmpty(ref2))
                req.ref2 = ref2;

            if (!string.IsNullOrEmpty(ref3))
                req.ref3 = ref3;

            if (!string.IsNullOrEmpty(ref4))
                req.ref4 = ref4;

            if (!string.IsNullOrEmpty(ref5))
                req.ref5 = ref5;

            return SendEncryptedRequest(req);
        }

        /// <summary>
        /// Refund a charge
        /// </summary>
        /// <param name="transactionId">Charge id</param>
        /// <param name="amount">Amount charged</param>
        /// <returns>RedPayResponse</returns>
        public RedPayResponse Refund(string transactionId, long amount)
        {
            RedPayRequest req = new RedPayRequest();

            req.transactionId = transactionId;
            req.action = "R";
            req.amount = amount;

            return SendEncryptedRequest(req);
        }

        /// <summary>
        /// Void a charge
        /// </summary>
        /// <param name="transactionId">Charge id</param>
        /// <returns>RedPayResponse</returns>
        public RedPayResponse Void(string transactionId)
        {
            RedPayRequest req = new RedPayRequest();

            req.transactionId = transactionId;
            req.action = "V";

            return SendEncryptedRequest(req);
        }

        #region Private Methods

        private RedPayResponse SendEncryptedRequest(RedPayRequest redpayRequest)
        {
            string res = string.Empty;

            JObject packet = new JObject();
            packet["app"] = config.App;
            packet["iv"] = AESCryptography.GetRandomIV();
            packet["utcTime"] = DateTime.UtcNow;

            //Set Encrypted Packet Value
            string request_string = JsonConvert.SerializeObject(redpayRequest);
            string token = config.AuthToken;
            byte[] key = Convert.FromBase64String(token);
            byte[] iv = Convert.FromBase64String(packet["iv"].ToString());
            byte[] aesData = AESCryptography.EncryptStringToBytes_Aes(request_string, key, iv);
            packet["aesData"] = Convert.ToBase64String(aesData);

            string json = JsonConvert.SerializeObject(packet);

            HttpWebRequest web_request = (HttpWebRequest)WebRequest.Create(config.ApiEndpoint + @"/card");
            web_request.Method = "POST";
            web_request.Accept = "*/*";
            web_request.ContentType = "application/json";

            using (StreamWriter stream_writer = new StreamWriter(web_request.GetRequestStream()))
            {
                stream_writer.Write(json);
            }

            string response_string = string.Empty;
            RedPayResponse redpayResponse = null;

            try
            {
                using (HttpWebResponse web_response = (HttpWebResponse)web_request.GetResponse())
                {
                    using (StreamReader response_stream = new StreamReader(web_response.GetResponseStream()))
                    {
                        response_string = response_stream.ReadToEnd();

                        //response is Packet json - Deserialize Packet
                        JObject responsePacket = JsonConvert.DeserializeObject(response_string) as JObject;

                        if (responsePacket != null)
                        {
                            if (responsePacket["aesData"] != null && !string.IsNullOrEmpty(responsePacket["aesData"].ToString()))
                            {
                                //unencrypt value using iv
                                byte[] _cyper = Convert.FromBase64String(responsePacket["aesData"].ToString());
                                byte[] _iv = Convert.FromBase64String(responsePacket["iv"].ToString());
                                byte[] _key = Convert.FromBase64String(token);
                                response_string = AESCryptography.DecryptStringFromBytes_Aes(_cyper, _key, _iv);
                            }

                            // GOOD CASE, WE GOT SOME RESPONSE FROM REDPAY GATEWAY
                            try
                            {
                                if (!string.IsNullOrEmpty(response_string))
                                    redpayResponse = JsonConvert.DeserializeObject<RedPayResponse>(response_string);
                            }
                            catch (Exception ex)
                            {
                                response_string = ex.Message;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response_string = ex.Message;
            }

            if(redpayResponse == null)
            {
                redpayResponse = new RedPayResponse();
                redpayResponse.responseCode = "X";
                redpayResponse.text = response_string;
            }

            return redpayResponse;
        }

        private static string EncodeToBase64(string data)
        {
            byte[] utf8EncodedData = System.Text.Encoding.UTF8.GetBytes(data);

            string base64EncodedData = Convert.ToBase64String(utf8EncodedData);

            return base64EncodedData;
        }
        #endregion
    }
}
