using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace RedPay
{
    public class CreditCardProcessor
    {
        Config config;

        public CreditCardProcessor(Config c)
        {
            config = c;
        }

        public Tuple<ResponseCode, RedPayResponse, string, string> Purchase(long amount,
            string cardnumber, string cardholdername, string expdate, string ccv, string zipcode,
            string track1data = null, string track2data = null, string signaturedata = null, 
            string avsaddress1 = null, string avsaddress2 = null, string avscity = null, 
            string cardholderemail = null, string cardholderphone = null, string method = null,
            string retrycount = null, string employeerefnum = null, string customerrefnum = null, 
            string orderrefnum = null, string terminalrefnum = null)
        {
            decimal amt = (decimal)amount;

            RedPayRequest req = new RedPayRequest();

            //Required
            req.action = "A";
            req.amount = amount;
            req.account = cardnumber;
            req.cardHolderName = cardholdername;
            req.expmmyyyy = expdate;
            req.cvv = ccv;
            req.avsZip = zipcode;

            //Optional
            if (!string.IsNullOrEmpty(track1data))
                req.track1Data = track1data;

            if (!string.IsNullOrEmpty(track2data))
                req.track2Data = track2data;

            if (!string.IsNullOrEmpty(signaturedata))
                req.signatureData = signaturedata;

            if (!string.IsNullOrEmpty(avsaddress1))
                req.avsAddress1 = avsaddress1;

            if (!string.IsNullOrEmpty(avsaddress2))
                req.avsAddress2 = avsaddress2;

            if (!string.IsNullOrEmpty(avscity))
                req.avsCity = avscity;

            if (!string.IsNullOrEmpty(cardholderemail))
                req.cardHolderEmail = cardholderemail;

            if (!string.IsNullOrEmpty(cardholderphone))
                req.cardHolderPhone= cardholderphone;

            if (!string.IsNullOrEmpty(method))
                req.method = method;

            if (!string.IsNullOrEmpty(retrycount))
                req.retryCount = retrycount;

            if (!string.IsNullOrEmpty(employeerefnum))
                req.employeeRefNum = employeerefnum;

            if (!string.IsNullOrEmpty(customerrefnum))
                req.customerRefNum = customerrefnum;

            if (!string.IsNullOrEmpty(orderrefnum))
                req.orderRefNum = orderrefnum;

            if (!string.IsNullOrEmpty(terminalrefnum))
                req.terminalRefNum = terminalrefnum;

            return SendEncryptedRequest(req);
        }
       
        public Tuple<ResponseCode, RedPayResponse, string, string> Refund(string transactionId, long amount)
        {
            decimal amt = (decimal)amount;

            RedPayRequest req = new RedPayRequest();

            req.transactionId = transactionId;
            req.action = "R";
            req.amount = amount;

            return SendEncryptedRequest(req);
        }

        public Tuple<ResponseCode, RedPayResponse, string, string> Void(string transactionId)
        {
            RedPayRequest req = new RedPayRequest();

            req.transactionId = transactionId;
            req.action = "V";

            return SendEncryptedRequest(req);
        }

        #region Private Methods

        private Tuple<ResponseCode, RedPayResponse, string, string> SendRequest(RedPayRequest req)
        {

            string res = string.Empty;
            ResponseCode resCode = ResponseCode.NotSet;
            RedPayResponse resp = null;

            string RequestString = JsonConvert.SerializeObject(req);
            JObject packet = new JObject();
            packet["app"] = config.App;
            packet["utcTime"] = DateTime.UtcNow;
            packet["data"] = JObject.Parse(RequestString);//it should be json object

            string json = JsonConvert.SerializeObject(packet);

            HttpWebRequest web_request = (HttpWebRequest)WebRequest.Create(config.ApiEndpoints + @"/card");
            web_request.Method = "POST";
            web_request.Accept = "*/*";
            web_request.ContentType = "application/json"; //REST XML

            using (StreamWriter stream_writer = new StreamWriter(web_request.GetRequestStream()))
            {
                stream_writer.Write(json);
            }

            string response_string;

            try
            {
                using (HttpWebResponse web_response = (HttpWebResponse)web_request.GetResponse())
                {
                    using (StreamReader response_stream = new StreamReader(web_response.GetResponseStream()))
                    {
                        response_string = response_stream.ReadToEnd();
                        //string data_string = GetDataString(response_string);
                        //response is Packet json - Deserialize Packet
                        JObject responsePacket = JsonConvert.DeserializeObject(response_string) as JObject;

                        if (responsePacket != null && !string.IsNullOrEmpty(response_string))
                        {
                            //string responseString = string.Empty;
                            RedPayResponse response = null;

                            if (responsePacket["data"] != null)
                                JsonConvert.DeserializeObject<RedPayResponse>(responsePacket["data"].ToString());

                            if (response != null)
                            {
                                resp = response;
                                res = responsePacket["data"].ToString();
                                resCode = GetResponseCode(resp);
                            }
                            else
                            {
                                resp = null;
                                res = null;
                                resCode = ResponseCode.Error;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resCode = ResponseCode.Exception;
                res = ex.ToString();
                resp = null;
            }

            return new Tuple<ResponseCode, RedPayResponse, string, string>(resCode, resp, res, RequestString);
        }

        private string GetDataString(string response_string)
        {
            try
            {
                int startIndex = response_string.IndexOf("\"data\":") + 7;
                int endIndex = response_string.IndexOf("},\"iv") + 1;

                return response_string.Substring(startIndex, endIndex - startIndex);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private Tuple<ResponseCode, RedPayResponse, string, string> SendEncryptedRequest(RedPayRequest req)
        {

            string res = string.Empty;
            ResponseCode resCode = ResponseCode.NotSet;
            RedPayResponse resp = null;

            JObject packet = new JObject();
            packet["app"] = config.App;
            packet["iv"] = GetRandomIV();
            packet["utcTime"] = DateTime.UtcNow;

            //Set Encrypted Packet Value
            string RequestString = JsonConvert.SerializeObject(req);
            string token = config.AuthToken;
            byte[] key = Convert.FromBase64String(token);
            byte[] IV = Convert.FromBase64String(packet["iv"].ToString());
            var r = AESCryptography.EncryptStringToBytes_Aes(RequestString, key, IV);
            packet["aesData"] = Convert.ToBase64String(r);


            string json = JsonConvert.SerializeObject(packet);

            HttpWebRequest web_request = (HttpWebRequest)WebRequest.Create(config.ApiEndpoints + @"/card");
            web_request.Method = "POST";
            web_request.Accept = "*/*";
            web_request.ContentType = "application/json";

            using (StreamWriter stream_writer = new StreamWriter(web_request.GetRequestStream()))
            {
                stream_writer.Write(json);
            }

            string response_string;

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
                            string responseString = string.Empty;

                            if (responsePacket["aesData"] != null && !string.IsNullOrEmpty(responsePacket["aesData"].ToString()))
                            {
                                //unencrypt value using iv
                                byte[] _cyper = Convert.FromBase64String(responsePacket["aesData"].ToString());
                                byte[] _iv = Convert.FromBase64String(responsePacket["iv"].ToString());
                                byte[] _key = Convert.FromBase64String(token);
                                responseString = AESCryptography.DecryptStringFromBytes_Aes(_cyper, _key, _iv);
                            }

                            if (!string.IsNullOrEmpty(responseString))
                            {
                                resp = JsonConvert.DeserializeObject<RedPayResponse>(responseString);
                                res = responseString;
                                resCode = GetResponseCode(resp);
                            }
                            else
                            {
                                resp = null;
                                res = responsePacket["responseCode"].ToString();
                                resCode = ResponseCode.Error;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resCode = ResponseCode.Exception;
                res = ex.ToString();
                resp = null;
            }

            return new Tuple<ResponseCode, RedPayResponse, string, string>(resCode, resp, res, RequestString);
        }

        private ResponseCode GetResponseCode(RedPayResponse resp)
        {
            if (resp == null)
                return ResponseCode.NotSet;

            if (resp.responseCode == null)
                return ResponseCode.NotSet;

            if (string.IsNullOrEmpty(resp.responseCode))
                return ResponseCode.NotSet;

            if (string.IsNullOrWhiteSpace(resp.responseCode))
                return ResponseCode.NotSet;

            switch (resp.responseCode.ToUpper())
            {
                case "A":
                    return ResponseCode.Approved;

                case "D":
                    return ResponseCode.Declined;

                case "O":
                    return ResponseCode.OK;

                //case "P":
                //    return ResponseCode.Partial;

                case "C":
                    return ResponseCode.CallForAuthorization;

                case "K":
                    return ResponseCode.PickupConfiscateCard;

                case "U":
                    return ResponseCode.Duplicate;

                case "R":
                    return ResponseCode.Retry;

                case "S":
                    return ResponseCode.Setup;

                case "T":
                    return ResponseCode.Timeout;

                case "F":
                    return ResponseCode.Fraud;

                case "E":
                    return ResponseCode.Error;

                case "X":
                    return ResponseCode.Exception;
            }

            return ResponseCode.NotSet;
        }

        private static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(toEncode);

            string returnValue = Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;
        }

        private static string GetRandomIV()
        {
            var random = new byte[16];           // whatever size you want
            var rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(random);         // Fill with non-zero random bytes

            return Convert.ToBase64String(random);  // convert to a string.
        }
        #endregion
    }
}
