using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStore.Application.Services.Payment
{
    public interface IPaymentService
    {
        Charge CreatePayment(ChargeCreateOptions chargeOptions);
    }
}
