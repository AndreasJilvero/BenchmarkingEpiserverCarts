namespace Star.Epi.CMS.Business.Benchmarks
{
    public interface IBenchmarks
    {
        void CreateEmptyCart();

        void AddLineItem();

        void ValidateAndApplyCampaigns();

        void ApplyPayment();

        void SaveAsPurchaseOrder();
    }
}