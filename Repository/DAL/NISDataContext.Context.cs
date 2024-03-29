﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace nIS
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    using NIS.Repository.Entities;
    using System.Collections.Generic;

    public partial class NISEntities : DbContext
    {
        public NISEntities()
            : base("name=NISEntities")
        {
        }
    
        public NISEntities(string connectionString)
            : base(connectionString)
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<UserRecord> UserRecords { get; set; }
        public virtual DbSet<UserCredentialHistoryRecord> UserCredentialHistoryRecords { get; set; }
        public virtual DbSet<UserLoginRecord> UserLoginRecords { get; set; }
        public virtual DbSet<UserLoginActivityHistoryRecord> UserLoginActivityHistoryRecords { get; set; }
        public virtual DbSet<UserRoleMapRecord> UserRoleMapRecords { get; set; }
        public virtual DbSet<RolePrivilegeRecord> RolePrivilegeRecords { get; set; }
        public virtual DbSet<AssetRecord> AssetRecords { get; set; }
        public virtual DbSet<AssetLibraryRecord> AssetLibraryRecords { get; set; }
        public virtual DbSet<AssetPathSettingRecord> AssetPathSettingRecords { get; set; }
        public virtual DbSet<PageRecord> PageRecords { get; set; }
        public virtual DbSet<PageTypeRecord> PageTypeRecords { get; set; }
        public virtual DbSet<PageWidgetMapRecord> PageWidgetMapRecords { get; set; }
        public virtual DbSet<ScheduleRecord> ScheduleRecords { get; set; }
        public virtual DbSet<StatementRecord> StatementRecords { get; set; }
        public virtual DbSet<StatementPageMapRecord> StatementPageMapRecords { get; set; }
        public virtual DbSet<TenantRecord> TenantRecords { get; set; }
        public virtual DbSet<RoleRecord> RoleRecords { get; set; }
        public virtual DbSet<CityRecord> CityRecords { get; set; }
        public virtual DbSet<StateRecord> StateRecords { get; set; }
        public virtual DbSet<WidgetRecord> WidgetRecords { get; set; }
        public virtual DbSet<AssetSettingRecord> AssetSettingRecords { get; set; }
        public virtual DbSet<ScheduleRunHistoryRecord> ScheduleRunHistoryRecords { get; set; }
        public virtual DbSet<RenderEngineRecord> RenderEngineRecords { get; set; }
        public virtual DbSet<BatchDetailRecord> BatchDetailRecords { get; set; }
        public virtual DbSet<CustomerMediaRecord> CustomerMediaRecords { get; set; }
        public virtual DbSet<CustomerMasterRecord> CustomerMasterRecords { get; set; }
        public virtual DbSet<ReminderAndRecommendationRecord> ReminderAndRecommendationRecords { get; set; }
        public virtual DbSet<ScheduleLogDetailRecord> ScheduleLogDetailRecords { get; set; }
        public virtual DbSet<AccountTransactionRecord> AccountTransactionRecords { get; set; }
        public virtual DbSet<SavingTrendRecord> SavingTrendRecords { get; set; }
        public virtual DbSet<Top4IncomeSourcesRecord> Top4IncomeSourcesRecord { get; set; }
        //public virtual DbSet<TransactionDetailRecord> TransactionDetailRecords { get; set; }
        public virtual DbSet<AccountMasterRecord> AccountMasterRecords { get; set; }
        public virtual DbSet<BatchMasterRecord> BatchMasterRecords { get; set; }
        public virtual DbSet<AnalyticsDataRecord> AnalyticsDataRecords { get; set; }
        public virtual DbSet<View_ScheduleRecord> View_ScheduleRecord { get; set; }
        public virtual DbSet<View_Page> View_PageRecord { get; set; }
        public virtual DbSet<View_StatementDefinitionRecord> View_StatementDefinitionRecord { get; set; }
        public virtual DbSet<View_SourceDataRecord> View_SourceDataRecord { get; set; }
        public virtual DbSet<ContactTypeRecord> ContactTypeRecords { get; set; }
        public virtual DbSet<TenantContactRecord> TenantContactRecords { get; set; }
        public virtual DbSet<TenantUserRecord> TenantUserRecords { get; set; }
        public virtual DbSet<MultiTenantUserAccessMapRecord> MultiTenantUserAccessMapRecords { get; set; }
        public virtual DbSet<View_MultiTenantUserAccessMapRecord> View_MultiTenantUserAccessMapRecord { get; set; }
        public virtual DbSet<CountryRecord> CountryRecords { get; set; }
        public virtual DbSet<DynamicWidgetRecord> DynamicWidgetRecords { get; set; }
        public virtual DbSet<EntityFieldMapRecord> EntityFieldMapRecords { get; set; }
        public virtual DbSet<TenantEntityRecord> TenantEntityRecords { get; set; }
        public virtual DbSet<DynamicWidgetFilterDetail> DynamicWidgetFilterDetails { get; set; }
        public virtual DbSet<WidgetPageTypeMap> WidgetPageTypeMaps { get; set; }
        public virtual DbSet<View_PageWidgetMap> View_PageWidgetMap { get; set; }
        public virtual DbSet<ScheduleLogDetailArchiveRecord> ScheduleLogDetailArchiveRecords { get; set; }
        public virtual DbSet<StatementMetadataRecord> StatementMetadataRecords { get; set; }
        public virtual DbSet<StatementMetadataArchiveRecord> StatementMetadataArchiveRecords { get; set; }
        public virtual DbSet<View_DynamicWidgetRecord> View_DynamicWidgetRecord { get; set; }
        public virtual DbSet<TenantSubscriptionRecord> TenantSubscriptionRecords { get; set; }
        public virtual DbSet<ScheduleLogRecord> ScheduleLogRecords { get; set; }
        public virtual DbSet<ScheduleLogArchiveRecord> ScheduleLogArchiveRecords { get; set; }
        public virtual DbSet<View_TenantSubscriptionRecord> View_TenantSubscriptionRecord { get; set; }
        public virtual DbSet<TenantSecurityCodeFormatRecord> TenantSecurityCodeFormatRecords { get; set; }
        public virtual DbSet<View_StatementMetadataRecord> View_StatementMetadataRecord { get; set; }
        public virtual DbSet<View_ScheduleLog> View_ScheduleLog { get; set; }
        //public virtual DbSet<View_UserRecord> View_UserRecord { get; set; }
        public virtual DbSet<TenantConfigurationRecord> TenantConfigurationRecords { get; set; }
        //public virtual DbSet<DM_BranchMasterRecord> DM_BranchMasterRecord { get; set; }
        //public virtual DbSet<DM_CustomerMasterRecord> DM_CustomerMasterRecord { get; set; }
        //public virtual DbSet<DM_ExplanatoryNotesRecord> DM_ExplanatoryNotesRecord { get; set; }
        //public virtual DbSet<DM_InvestmentMasterRecord> DM_InvestmentMasterRecord { get; set; }
        ////public virtual DbSet<DM_InvestmentTransactionRecord> DM_InvestmentTransactionRecord { get; set; }
        //public virtual DbSet<DM_PersonalLoanArrearsRecord> DM_PersonalLoanArrearsRecord { get; set; }
        //public virtual DbSet<DM_PersonalLoanMasterRecord> DM_PersonalLoanMasterRecord { get; set; }
        //public virtual DbSet<DM_PersonalLoanTransactionRecord> DM_PersonalLoanTransactionRecord { get; set; }
        //public virtual DbSet<DM_SpecialMessagesRecord> DM_SpecialMessagesRecord { get; set; }
        //public virtual DbSet<DM_HomeLoanArrearsRecord> DM_HomeLoanArrearsRecord { get; set; }
        //public virtual DbSet<DM_HomeLoanMasterRecord> DM_HomeLoanMasterRecord { get; set; }
        //public virtual DbSet<DM_HomeLoanSummaryRecord> DM_HomeLoanSummaryRecord { get; set; }
        //public virtual DbSet<DM_HomeLoanTransactionRecord> DM_HomeLoanTransactionRecord { get; set; }
        //public virtual DbSet<DM_MarketingMessagesRecord> DM_MarketingMessagesRecord { get; set; }
        //public virtual DbSet<DM_AccountAnalysisRecord> DM_AccountAnalysisRecord { get; set; }
        //public virtual DbSet<DM_AccountSummaryRecord> DM_AccountSummaryRecord { get; set; }
        //public virtual DbSet<DM_CustomerNewsAndAlertsRecord> DM_CustomerNewsAndAlertsRecord { get; set; }
        //public virtual DbSet<DM_CustomerProductWiseRewardPointsRecord> DM_CustomerProductWiseRewardPointsRecord { get; set; }
        //public virtual DbSet<DM_CustomerReminderRecosRecord> DM_CustomerReminderRecosRecord { get; set; }
        //public virtual DbSet<DM_CustomerRewardPointsRecord> DM_CustomerRewardPointsRecord { get; set; }
        //public virtual DbSet<DM_CustomerRewardPointsRedeemedRecord> DM_CustomerRewardPointsRedeemedRecord { get; set; }
        //public virtual DbSet<DM_CustomerRewardSpendByCategoryRecord> DM_CustomerRewardSpendByCategoryRecord { get; set; }
        //public virtual DbSet<DM_GreenbacksMasterRecord> DM_GreenbacksMasterRecord { get; set; }
        //public virtual DbSet<DM_NewsAndAlertsRecord> DM_NewsAndAlertsRecord { get; set; }
        //public virtual DbSet<DM_ReminderRecosRecord> DM_ReminderRecosRecord { get; set; }
        public virtual DbSet<SystemActivityHistoryRecord> SystemActivityHistoryRecords { get; set; }
        //public virtual DbSet<NB_BatchMaster_Source> NB_BatchMaster_Source { get; set; }
        //public virtual DbSet<NB_BranchMaster> NB_BranchMaster { get; set; }
        //public virtual DbSet<NB_BranchMaster_Old> NB_BranchMaster_Old { get; set; }
        //public virtual DbSet<NB_Investment_Source> NB_Investment_Source { get; set; }
        //public virtual DbSet<NB_InvestmentMaster_Old> NB_InvestmentMaster_Old { get; set; }
        //public virtual DbSet<NB_InvestmentTransaction_Old> NB_InvestmentTransaction_Old { get; set; }
        public virtual DbSet<NB_SegmentMaster> NB_SegmentMaster { get; set; }
        //public virtual DbSet<NB_CorporateSaverMaster> NB_CorporateSaverMaster { get; set; }
        //public virtual DbSet<NB_CorporateSaverTax> NB_CorporateSaverTax { get; set; }
        //public virtual DbSet<NB_CorporateSaverTransactions> NB_CorporateSaverTransactions { get; set; }
        //public virtual DbSet<NB_MCATransaction> NB_MCATransaction { get; set; }
        //public virtual DbSet<NB_MCAMaster> NB_MCAMaster { get; set; }
        //public virtual DbSet<NB_CustomerMaster> NB_CustomerMaster { get; set; }
        public virtual DbSet<ProductPageTypeMapping> ProductPageTypeMappings { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        //public virtual DbSet<NB_InvestmentMaster> NB_InvestmentMaster { get; set; }
        //public virtual DbSet<NB_InvestmentTransaction> NB_InvestmentTransaction { get; set; }
        public virtual DbSet<EtlSchedules> EtlSchedules { get; set; }
        public virtual DbSet<EtlBatches> EtlBatches { get; set; }
        public virtual DbSet<SystemActivityHistory> SystemActivityHistory { get; set; }
        public virtual DbSet<View_ETLScheduleLog> View_ETLScheduleLog { get; set; }

        //[DbFunction("NISEntities", "FnUserTenant")]
        //public virtual IQueryable<FnUserTenant_Result> FnUserTenant(Nullable<int> userId)
        //{
        //    var userIdParameter = userId.HasValue ?
        //        new ObjectParameter("UserId", userId) :
        //        new ObjectParameter("UserId", typeof(int));
    
        //    return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<FnUserTenant_Result>("[NIS].[FnUserTenant](@UserId)", userIdParameter);
        //}

        public virtual List<FnUserTenant_Result> FnUserTenant(Nullable<int> userId)
        {
            var result = this.Database.SqlQuery<FnUserTenant_Result>($"select * from [NIS].[FnUserTenant]({userId.Value})").ToList();
            return result;

        }
        //[DbFunction("NISEntities", "FnGetParentAndChildTenant")]
        //public virtual IQueryable<FnGetParentAndChildTenant_Result> FnGetParentAndChildTenant(string parentTenantCode)
        //{
        //    var parentTenantCodeParameter = parentTenantCode != null ?
        //        new ObjectParameter("ParentTenantCode", parentTenantCode) :
        //        new ObjectParameter("ParentTenantCode", typeof(string));

        //    return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<FnGetParentAndChildTenant_Result>("[NIS].[FnGetParentAndChildTenant](@ParentTenantCode)", parentTenantCodeParameter);
        //}

        public virtual List<FnGetParentAndChildTenant_Result> FnGetParentAndChildTenant(string parentTenantCode)
        {
            var result = this.Database.SqlQuery<FnGetParentAndChildTenant_Result>($"select * from [NIS].[FnGetParentAndChildTenant]('{parentTenantCode}')").ToList();

            return result;
        }
        ////[DbFunction("NISEntities", "FnGetStaticAndDynamicWidgets")]
        //public virtual IQueryable<FnGetStaticAndDynamicWidgets_Result> FnGetStaticAndDynamicWidgets(Nullable<long> pageTypeId, string tenantCode)
        //{
        //    var pageTypeIdParameter = pageTypeId.HasValue ?
        //        new ObjectParameter("PageTypeId", pageTypeId) :
        //        new ObjectParameter("PageTypeId", typeof(long));

        //    var tenantCodeParameter = tenantCode != null ?
        //        new ObjectParameter("TenantCode", tenantCode) :
        //        new ObjectParameter("TenantCode", typeof(string));

        //    return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<FnGetStaticAndDynamicWidgets_Result>("[NIS].[FnGetStaticAndDynamicWidgets](@PageTypeId, @TenantCode)", pageTypeIdParameter, tenantCodeParameter);
        //}
        public virtual List<FnGetStaticAndDynamicWidgets_Result> FnGetStaticAndDynamicWidgets(Nullable<long> pageTypeId, string tenantCode)
        {
            var result = this.Database.SqlQuery<FnGetStaticAndDynamicWidgets_Result>($"select * from [NIS].[FnGetStaticAndDynamicWidgets]({pageTypeId}, '{tenantCode}')").ToList();
            return result;
        }

        public List<spIAA_PaymentDetail> spIAA_PaymentDetail_fspstatement()
        {
            //// Create parameters for the stored procedure
            //var param1 = new SqlParameter("@Parameter1", parameter1);
            //var param2 = new SqlParameter("@Parameter2", parameter2);spIAA_PaymentDetail_fspstatement

            // Execute the stored procedure using SqlQuery
            var result = this.Database.SqlQuery<spIAA_PaymentDetail>("spIAA_PaymentDetail_fspstatement").ToList();

            return result;
        }

        public List<spIAA_Commission_Detail> spIAA_Commission_Detail()
        {
            //// Create parameters for the stored procedure
            //var param1 = new SqlParameter("@Parameter1", parameter1);
            //var param2 = new SqlParameter("@Parameter2", parameter2);spIAA_PaymentDetail_fspstatement

            // Execute the stored procedure using SqlQuery
            var result = this.Database.SqlQuery<spIAA_Commission_Detail>("spIAA_Commission_Detail").ToList();

            return result;
        }
    }
}
