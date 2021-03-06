﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Korea
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class koreaEntities1 : DbContext
    {
        public koreaEntities1()
            : base("name=koreaEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<CustomOption> CustomOptions { get; set; }
        public virtual DbSet<FileLibrary> FileLibraries { get; set; }
        public virtual DbSet<ImportLog> ImportLogs { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<OnlineClientInfo> OnlineClientInfoes { get; set; }
        public virtual DbSet<OptionPriceType> OptionPriceTypes { get; set; }
        public virtual DbSet<Option> Options { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductFile> ProductFiles { get; set; }
        public virtual DbSet<ProductPropertyValue> ProductPropertyValues { get; set; }
        public virtual DbSet<ProductVideo> ProductVideos { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<PropertyValue> PropertyValues { get; set; }
        public virtual DbSet<Ratio> Ratios { get; set; }
        public virtual DbSet<RelatedProduct> RelatedProducts { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<Tax> Taxes { get; set; }
        public virtual DbSet<TaxRegionRate> TaxRegionRates { get; set; }
        public virtual DbSet<BottomMenu> BottomMenus { get; set; }
        public virtual DbSet<Carousel> Carousels { get; set; }
        public virtual DbSet<GeoCity> GeoCities { get; set; }
        public virtual DbSet<MainMenu> MainMenus { get; set; }
        public virtual DbSet<PointCity> PointCities { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<StaticBlock> StaticBlocks { get; set; }
        public virtual DbSet<StaticPage> StaticPages { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Country_ru> Country_ru { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerCertificate> CustomerCertificates { get; set; }
        public virtual DbSet<CustomerCoupon> CustomerCoupons { get; set; }
        public virtual DbSet<CustomerGroup> CustomerGroups { get; set; }
        public virtual DbSet<GeoIP> GeoIPs { get; set; }
        public virtual DbSet<RecentlyViewsData> RecentlyViewsDatas { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<RoleAction> RoleActions { get; set; }
        public virtual DbSet<ASPStateTempApplication> ASPStateTempApplications { get; set; }
        public virtual DbSet<ASPStateTempSession> ASPStateTempSessions { get; set; }
        public virtual DbSet<BreakMessage> BreakMessages { get; set; }
        public virtual DbSet<MessageLog> MessageLogs { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<SaasData> SaasDatas { get; set; }
        public virtual DbSet<Subscribe> Subscribes { get; set; }
        public virtual DbSet<SubscribeDeactivateReason> SubscribeDeactivateReasons { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<AppRestartLog> AppRestartLogs { get; set; }
        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderByRequest> OrderByRequests { get; set; }
        public virtual DbSet<OrderConfirmation> OrderConfirmations { get; set; }
        public virtual DbSet<OrderContact> OrderContacts { get; set; }
        public virtual DbSet<OrderCurrency> OrderCurrencies { get; set; }
        public virtual DbSet<OrderCustomer> OrderCustomers { get; set; }
        public virtual DbSet<OrderCustomOption> OrderCustomOptions { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<OrderPickPoint> OrderPickPoints { get; set; }
        public virtual DbSet<OrderPriceDiscount> OrderPriceDiscounts { get; set; }
        public virtual DbSet<OrderStatu> OrderStatus { get; set; }
        public virtual DbSet<OrderTax> OrderTaxes { get; set; }
        public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<PaymentParam> PaymentParams { get; set; }
        public virtual DbSet<ShippingMethod> ShippingMethods { get; set; }
        public virtual DbSet<ShippingParam> ShippingParams { get; set; }
        public virtual DbSet<MetaInfo> MetaInfoes { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<AuthorizeLog> AuthorizeLogs { get; set; }
        public virtual DbSet<ExportFeedHistory> ExportFeedHistories { get; set; }
        public virtual DbSet<ExportFeedSelectedCategory> ExportFeedSelectedCategories { get; set; }
        public virtual DbSet<ExportFeedSelectedProduct> ExportFeedSelectedProducts { get; set; }
        public virtual DbSet<GiftCertificateTax> GiftCertificateTaxes { get; set; }
        public virtual DbSet<InternalSetting> InternalSettings { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<MailFormat> MailFormats { get; set; }
        public virtual DbSet<MailFormatType> MailFormatTypes { get; set; }
        public virtual DbSet<ModuleSetting> ModuleSettings { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<NewsCategory> NewsCategories { get; set; }
        public virtual DbSet<ProfitPlan> ProfitPlans { get; set; }
        public virtual DbSet<Ranx> Ranges { get; set; }
        public virtual DbSet<Redirect> Redirects { get; set; }
        public virtual DbSet<Settings1> Settings1 { get; set; }
        public virtual DbSet<SearchStatistic> SearchStatistics { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<VoiceTheme> VoiceThemes { get; set; }
        public virtual DbSet<ClientInfo> ClientInfoes { get; set; }
        public virtual DbSet<Country_en> Country_en { get; set; }
        public virtual DbSet<CustomerRoleAction> CustomerRoleActions { get; set; }
        public virtual DbSet<OpenIdLinkCustomer> OpenIdLinkCustomers { get; set; }
        public virtual DbSet<AdminMessage> AdminMessages { get; set; }
        public virtual DbSet<OrderPaymentInfo> OrderPaymentInfoes { get; set; }
        public virtual DbSet<GiftCertificatePayment> GiftCertificatePayments { get; set; }
        public virtual DbSet<ViewAbandonedShoppingCart> ViewAbandonedShoppingCarts { get; set; }
    }
}
