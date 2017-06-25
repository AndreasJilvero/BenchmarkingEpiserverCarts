using System.Web.Http;
using Star.Epi.CMS.Business.Benchmarks;

namespace Star.Epi.CMS.Controllers
{
    public class CartController : ApiController
    {
        private readonly IBenchmarks _benchmarks;

        public CartController(IBenchmarks benchmarks)
        {
            _benchmarks = benchmarks;
        }

        public IHttpActionResult Get(int operationsToExecute)
        {
            if (operationsToExecute >= 1)
            {
                _benchmarks.CreateEmptyCart();
            }

            if (operationsToExecute >= 2)
            {
                _benchmarks.AddLineItem();
            }

            if (operationsToExecute >= 3)
            {
                _benchmarks.ValidateAndApplyCampaigns();
            }

            if (operationsToExecute >= 4)
            {
                _benchmarks.ApplyPayment();
            }

            if (operationsToExecute >= 5)
            {
                _benchmarks.SaveAsPurchaseOrder();
            }

            return Ok();
        }
    }
}