using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Order;
using EPiServer.Commerce.Order.Internal;

namespace Star.Epi.CMS.Business.Benchmarks
{
    public class SerializableCartBenchmarks : AbstractOrderRepositoryBenchmarks<SerializableCart>
    {
        public SerializableCartBenchmarks(IOrderRepository orderRepository, IOrderGroupFactory orderGroupFactory, ILineItemValidator lineItemValidator, IPlacedPriceProcessor placedPriceProcessor, IInventoryProcessor inventoryProcessor, IPromotionEngine promotionEngine, IOrderGroupCalculator orderGroupCalculator, IPaymentProcessor paymentProcessor)
            : base(orderRepository, orderGroupFactory, lineItemValidator, placedPriceProcessor, inventoryProcessor, promotionEngine, orderGroupCalculator, paymentProcessor)
        {
        }
    }
}