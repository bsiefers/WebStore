using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStore.Application.Services.Payment
{

    public class PaymentService : IPaymentService
    {
        public virtual Charge CreatePayment(ChargeCreateOptions chargeOptions)
        {
            var service = new ChargeService();
            var charge = service.Create(chargeOptions);
            return charge;
        }
    }
}
