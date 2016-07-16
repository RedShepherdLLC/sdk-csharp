public class RedPayRequest
{
    public string action { get; set; }

    public string transactionId { get; set; }

    public string account { get; set; }

    public string track1Data { get; set; }

    public string track2Data { get; set; }

    public string signatureData { get; set; }

    public string expmmyyyy { get; set; }

    public string cvv { get; set; }

    public long amount { get; set; }

    public string cardHolderName { get; set; }

    public string method { get; set; }

    public string currency { get; set; }

    public string authCode { get; set; }

    public string retryCount { get; set; }

    public string avsAddress1 { get; set; }

    public string avsAddress2 { get; set; }

    public string avsCity { get; set; }

    public string avsZip { get; set; }

    public string cardHolderEmail { get; set; }

    public string cardHolderPhone { get; set; }

    public string employeeRefNum { get; set; }

    public string customerRefNum { get; set; }

    public string orderRefNum { get; set; }

    public string terminalRefNum { get; set; }
}