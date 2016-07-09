using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedPay
{
    public enum ResponseCode
    {
        NotSet,
        Approved,
        Declined,
        CallForAuthorization,
        PickupConfiscateCard,
        Retry,
        Setup,
        Duplicate,
        Fraud,
        Error,
        Exception,
        Timeout,
        OK
    }
}
