using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Order;
using Mediachase.Commerce.Orders;

namespace Star.Epi.CMS.Business.Benchmarks
{
    public class OrderRepositoryBenchmarks : AbstractOrderRepositoryBenchmarks<Cart>
    {
        public OrderRepositoryBenchmarks(IOrderRepository orderRepository, IOrderGroupFactory orderGroupFactory, ILineItemValidator lineItemValidator, IPlacedPriceProcessor placedPriceProcessor, IInventoryProcessor inventoryProcessor, IPromotionEngine promotionEngine, IOrderGroupCalculator orderGroupCalculator, IPaymentProcessor paymentProcessor)
            : base(orderRepository, orderGroupFactory, lineItemValidator, placedPriceProcessor, inventoryProcessor, promotionEngine, orderGroupCalculator, paymentProcessor)
        {
        }
    }
}