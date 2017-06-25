using System;
using System.Linq;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Marketing.Promotions;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Security;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Plugins.Payment;
using Mediachase.Commerce.Pricing;
using StructureMap;

namespace Star.Epi.CMS.Business.Migrations
{
    public class SetupData
    {
        private readonly ReferenceConverter _referenceConverter;
        private readonly IContentRepository _contentRepository;
        private readonly ILanguageBranchRepository _languageBranchRepository;
        private readonly ICurrentMarket _currentMarket;
        private readonly IMarketService _marketService;
        private readonly IPriceService _priceService;

        public SetupData(IContainer container)
        {
            _referenceConverter = container.GetInstance<ReferenceConverter>();
            _languageBranchRepository = container.GetInstance<ILanguageBranchRepository>();
            _contentRepository = container.GetInstance<IContentRepository>();
            _currentMarket = container.GetInstance<ICurrentMarket>();
            _marketService = container.GetInstance<IMarketService>();
            _priceService = container.GetInstance<IPriceService>();
        }

        public void Dump()
        {
            var languageBranch = _languageBranchRepository.ListAll().First();

            var taxCategory = CatalogTaxManager.CreateTaxCategory("Tax category", true);
            CatalogTaxManager.SaveTaxCategory(taxCategory);

            var jurisdictionDto = new JurisdictionDto();
            var applicationId = AppContext.Current.ApplicationId;
            jurisdictionDto.Jurisdiction.AddJurisdictionRow("Jurisdiction", null, languageBranch.LanguageID, (int)JurisdictionManager.JurisdictionType.Tax, null, null, null, null, null, null, applicationId, "ABC");
            jurisdictionDto.JurisdictionGroup.AddJurisdictionGroupRow(applicationId, "Group", (int)JurisdictionManager.JurisdictionType.Tax, "ABC");
            JurisdictionManager.SaveJurisdiction(jurisdictionDto);

            var taxDto = new TaxDto();
            taxDto.Tax.AddTaxRow((int)TaxType.SalesTax, "Tax", 1, applicationId);
            taxDto.TaxValue.AddTaxValueRow(20d, taxDto.Tax[0], "Tax category", jurisdictionDto.JurisdictionGroup[0].JurisdictionGroupId, DateTime.Now, Guid.Empty);
            TaxManager.SaveTax(taxDto);

            var currentMarket = _currentMarket.GetCurrentMarket();
            var market = (MarketImpl)_marketService.GetMarket(currentMarket.MarketId);
            market.DefaultCurrency = Currency.EUR;
            market.DefaultLanguage = languageBranch.Culture;
            _marketService.UpdateMarket(market);

            var rootLink = _referenceConverter.GetRootLink();
            var catalog = _contentRepository.GetDefault<CatalogContent>(rootLink, languageBranch.Culture);
            catalog.Name = "Catalog";
            catalog.DefaultCurrency = market.DefaultCurrency;
            catalog.CatalogLanguages = new ItemCollection<string> { languageBranch.LanguageID };
            catalog.DefaultLanguage = "en";
            catalog.WeightBase = "kg";
            catalog.LengthBase = "cm";
            var catalogRef = _contentRepository.Save(catalog, SaveAction.Publish, AccessLevel.NoAccess);

            var category = _contentRepository.GetDefault<NodeContent>(catalogRef);
            category.Name = "Category";
            category.DisplayName = "Category";
            category.Code = "category";
            var categoryRef = _contentRepository.Save(category, SaveAction.Publish, AccessLevel.NoAccess);

            var product = _contentRepository.GetDefault<ProductContent>(categoryRef);
            product.Name = "Product";
            product.DisplayName = "Product";
            product.Code = "product";
            var productRef = _contentRepository.Save(product, SaveAction.Publish, AccessLevel.NoAccess);

            var variant = _contentRepository.GetDefault<VariationContent>(productRef);
            variant.Name = "Variant";
            variant.DisplayName = "Variant";
            variant.Code = Constants.VariationCode;
            variant.TaxCategoryId = taxCategory.TaxCategory.First().TaxCategoryId;
            variant.MinQuantity = 1;
            variant.MaxQuantity = 100;
            _contentRepository.Save(variant, SaveAction.Publish, AccessLevel.NoAccess);

            var price = new PriceValue
            {
                UnitPrice = new Money(100, market.DefaultCurrency),
                CatalogKey = new CatalogKey(applicationId, variant.Code),
                MarketId = market.MarketId,
                ValidFrom = DateTime.Today.AddYears(-1),
                ValidUntil = DateTime.Today.AddYears(1),
                CustomerPricing = CustomerPricing.AllCustomers,
                MinQuantity = 0
            };

            _priceService.SetCatalogEntryPrices(price.CatalogKey, new[] { price });

            var campaign = _contentRepository.GetDefault<SalesCampaign>(SalesCampaignFolder.CampaignRoot);
            campaign.Name = "QuickSilver";
            campaign.Created = DateTime.UtcNow.AddHours(-1);
            campaign.IsActive = true;
            campaign.ValidFrom = DateTime.Today;
            campaign.ValidUntil = DateTime.Today.AddYears(1);
            var campaignRef = _contentRepository.Save(campaign, SaveAction.Publish, AccessLevel.NoAccess);

            var promotion = _contentRepository.GetDefault<BuyFromCategoryGetItemDiscount>(campaignRef);
            promotion.IsActive = true;
            promotion.Name = "25 % off";
            promotion.Category = categoryRef;
            promotion.Discount.UseAmounts = false;
            promotion.Discount.Percentage = 25m;
            _contentRepository.Save(promotion, SaveAction.Publish, AccessLevel.NoAccess);

            var paymentDto = new PaymentMethodDto();
            var created = DateTime.UtcNow.AddHours(-1);
            paymentDto.PaymentMethod.AddPaymentMethodRow(Constants.PaymentMethodId, "Payment", "Payment", languageBranch.LanguageID, "Keyword", true, true, typeof(GenericPaymentGateway).AssemblyQualifiedName, typeof(OtherPayment).AssemblyQualifiedName, false, 1, created, created, applicationId);
            paymentDto.MarketPaymentMethods.AddMarketPaymentMethodsRow(market.MarketId.Value, paymentDto.PaymentMethod[0]);
            PaymentManager.SavePayment(paymentDto);
        }
    }
}