/// <summary>
/// Represents a response from RedPay
/// </summary>
public class RedPayResponse
{
    public string transferStatus { get; set; }

    public string responseCode { get; set; }

    public string transactionId { get; set; }

    public string authCode { get; set; }

    public string cardLevel { get; set; }

    public string cardBrand { get; set; }

    public string cardType { get; set; }

    public string processorCode { get; set; }

    public string app { get; set; }

    public string account { get; set; }

    public string cardHolderName { get; set; }

    public long amount { get; set; }

    public string timeStamp { get; set; }

    public string text { get; set; }

    public string clientIP { get; set; }

    public string avsCode { get; set; }
}