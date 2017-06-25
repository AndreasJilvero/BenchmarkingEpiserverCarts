using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Order;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;

namespace Star.Epi.CMS.Business.Benchmarks
{
    public abstract class AbstractOrderRepositoryBenchmarks<T> : IBenchmarks
        where T : class, ICart
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderGroupFactory _orderGroupFactory;
        private readonly ILineItemValidator _lineItemValidator;
        private readonly IPlacedPriceProcessor _placedPriceProcessor;
        private readonly IInventoryProcessor _inventoryProcessor;
        private readonly IPromotionEngine _promotionEngine;
        private readonly IOrderGroupCalculator _orderGroupCalculator;
        private readonly IPaymentProcessor _paymentProcessor;

        private ICart _cart;

        public AbstractOrderRepositoryBenchmarks(
            IOrderRepository orderRepository, 
            IOrderGroupFactory orderGroupFactory, 
            ILineItemValidator lineItemValidator, 
            IPlacedPriceProcessor placedPriceProcessor, 
            IInventoryProcessor inventoryProcessor, 
            IPromotionEngine promotionEngine,
            IOrderGroupCalculator orderGroupCalculator,
            IPaymentProcessor paymentProcessor)
        {
            _orderRepository = orderRepository;
            _orderGroupFactory = orderGroupFactory;
            _lineItemValidator = lineItemValidator;
            _placedPriceProcessor = placedPriceProcessor;
            _inventoryProcessor = inventoryProcessor;
            _promotionEngine = promotionEngine;
            _orderGroupCalculator = orderGroupCalculator;
            _paymentProcessor = paymentProcessor;
        }

        public void CreateEmptyCart()
        {
            _cart = _orderRepository.Create<T>(Guid.NewGuid(), Cart.DefaultName);
            _orderRepository.Save(_cart);
        }

        public void AddLineItem()
        {
            var lineItem = _cart.CreateLineItem(Constants.VariationCode, _orderGroupFactory);
            lineItem.Quantity = 1;
            lineItem.IsInventoryAllocated = true;
            lineItem.PlacedPrice = 100;
            lineItem.Properties["Custom"] = true;
            _cart.AddLineItem(lineItem, _orderGroupFactory);
            _orderRepository.Save(_cart);
        }

        public void ValidateAndApplyCampaigns()
        {
            var validationIssues = new Dictionary<ILineItem, List<ValidationIssue>>();
            _cart.ValidateOrRemoveLineItems((item, issue) => validationIssues.AddValidationIssues(item, issue), _lineItemValidator);
            _cart.UpdatePlacedPriceOrRemoveLineItems(CustomerContext.Current.GetContactById(_cart.CustomerId), (item, issue) => validationIssues.AddValidationIssues(item, issue), _placedPriceProcessor);
            _cart.UpdateInventoryOrRemoveLineItems((item, issue) => validationIssues.AddValidationIssues(item, issue), _inventoryProcessor);
            _cart.ApplyDiscounts(_promotionEngine, new PromotionEngineSettings());
            _orderRepository.Save(_cart);
        }

        public void ApplyPayment()
        {
            var paymentMethod = PaymentManager.GetPaymentMethod(Constants.PaymentMethodId).PaymentMethod.Single();
            var payment = _cart.CreatePayment(_orderGroupFactory);
            payment.Amount = _orderGroupCalculator.GetTotal(_cart).Amount;
            payment.PaymentMethodId = paymentMethod.PaymentMethodId;
            payment.PaymentMethodName = paymentMethod.Name;
            payment.PaymentType = PaymentType.Other;
            _cart.AddPayment(payment, _orderGroupFactory);
            _cart.ProcessPayments(_paymentProcessor, _orderGroupCalculator);
            _orderRepository.Save(_cart);
        }

        public void SaveAsPurchaseOrder()
        {
            _orderRepository.SaveAsPurchaseOrder(_cart);
        }
    }
}