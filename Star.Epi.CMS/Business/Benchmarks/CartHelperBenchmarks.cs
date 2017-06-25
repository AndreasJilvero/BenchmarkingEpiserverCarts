using System.Linq;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Website.Helpers;

namespace Star.Epi.CMS.Business.Benchmarks
{
    public class CartHelperBenchmarks : IBenchmarks
    {
        private readonly ICatalogSystem _catalogSystem;

        private CartHelper _helper;

        public CartHelperBenchmarks(ICatalogSystem catalogSystem)
        {
            _catalogSystem = catalogSystem;
        }

        public void CreateEmptyCart()
        {
            _helper = new CartHelper(Cart.DefaultName);
            _helper.Cart.AcceptChanges();
        }

        public void AddLineItem()
        {
            var entry = _catalogSystem.GetCatalogEntry(Constants.VariationCode);
            _helper.AddEntry(entry);
            var lineItem = _helper.LineItems.First();
            lineItem.IsInventoryAllocated = true;
            lineItem.Quantity = 1;
            lineItem["Custom"] = true;
            _helper.Cart.AcceptChanges();
        }

        // TODO: For some reason, this does not apply any discounts.
        public void ValidateAndApplyCampaigns()
        {
            OrderGroupWorkflowManager.RunWorkflow(_helper.Cart, OrderGroupWorkflowManager.CartValidateWorkflowName);
            _helper.Cart.AcceptChanges();
        }

        public void ApplyPayment()
        {
            var paymentMethod = PaymentManager.GetPaymentMethod(Constants.PaymentMethodId).PaymentMethod.Single();
            var orderForm = _helper.Cart.OrderForms.Single();
            var payment = orderForm.Payments.AddNew(typeof(OtherPayment));
            payment.PaymentMethodId = paymentMethod.PaymentMethodId;
            payment.PaymentMethodName = paymentMethod.Name;
            payment.Amount = _helper.Cart.Total;
            payment.TransactionType = TransactionType.Other.ToString();
            OrderGroupWorkflowManager.RunWorkflow(_helper.Cart, OrderGroupWorkflowManager.CartPrepareWorkflowName);
            OrderGroupWorkflowManager.RunWorkflow(_helper.Cart, OrderGroupWorkflowManager.CartCheckOutWorkflowName);
            _helper.Cart.AcceptChanges();
        }

        public void SaveAsPurchaseOrder()
        {
            _helper.Cart.SaveAsPurchaseOrder();
        }
    }
}