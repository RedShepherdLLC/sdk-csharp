
public class RedPayResponse
{
    public string transferStatus { get; set; }

    public string responseCode { get; set; }

    public string transactionId { get; set; }

    public string batchId { get; set; }

    public string authCode { get; set; }

    public string softCode { get; set; }

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

    public string rawData { get; set; }

    public string clientIP { get; set; }

    public string sequenceNumber { get; set; }

    public string avsCode { get; set; }

    public string voidId { get; set; }

    public string refundId { get; set; }

    public string preAuthId { get; set; }
}

public class JsonRedPayResponse
{
    public JsonRedPayResponse(RedPayResponse response)
    {
        account = response.account;
        amount = response.amount;
        app = response.app;
        authCode = response.authCode;
        avsCode = response.avsCode;
        batchId = response.batchId;
        cardBrand = response.cardBrand;
        cardHolderName = response.cardHolderName;
        cardLevel = response.cardLevel;
        cardType = response.cardType;
        clientIP = response.clientIP;
        processorCode = response.processorCode;
        rawData = response.rawData;
        responseCode = response.responseCode;
        sequenceNumber = response.sequenceNumber;
        softCode = response.softCode;
        text = response.text;
        timeStamp = response.timeStamp;
        transactionId = response.timeStamp;
        transferStatus = response.transferStatus;
    }

    public string transferStatus { get; set; }

    public string responseCode { get; set; }

    public string transactionId { get; set; }

    public string batchId { get; set; }

    public string authCode { get; set; }

    public string softCode { get; set; }

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

    public string rawData { get; set; }

    public string clientIP { get; set; }

    public string sequenceNumber { get; set; }

    public string avsCode { get; set; }
}