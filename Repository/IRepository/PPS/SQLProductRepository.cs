﻿namespace nIS
{
    #region References
    using Newtonsoft.Json;
    using nIS.Models;
    using NIS.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using Unity;
    #endregion
    public class SQLProductRepository : IProductRepository
    {
        #region Private Members

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The configurationutility
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializing instance of class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public SQLProductRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Functions

        public IList<ProductViewModel> Get_Products(string tenantCode)
        {
            IList<ProductViewModel> Records = new List<ProductViewModel>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    return nISEntitiesDataContext.Products.Select(x=> new ProductViewModel() { 
                        Id = x.Id,
                        Name = x.Name
                    }).OrderBy(m => m.Name).ToList();                    
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ProductViewModel Get_ProductById(int id, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    return nISEntitiesDataContext.Products.Where(m=>m.Id == id).Select(x=> new ProductViewModel() { Id = x.Id, Name=x.Name }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IList<ProductPageTypeMappingViewModel> Get_ProductPageTypeMappingByProductId(int productId, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var result = nISEntitiesDataContext.ProductPageTypeMapping.Join(nISEntitiesDataContext.PageType, ppt => ppt.PageTypeId, pt => pt.Id,
                        (ppt, pt) => new ProductPageTypeMappingViewModel()
                        {
                            ProductId = ppt.ProductId,
                            PageTypeId = ppt.PageTypeId,
                            PageTypeName = pt.Name
                        }
                    ).Where(m => m.ProductId == productId).ToList();

                    foreach (var item in result)
                    {
                        var statementViewModel = (from spm in nISEntitiesDataContext.StatementPageMap
                                                  join pr in nISEntitiesDataContext.Page on spm.ReferencePageId equals pr.Id
                                                  join ppt in nISEntitiesDataContext.ProductPageTypeMapping on pr.PageTypeId equals ppt.PageTypeId
                                                  join st in nISEntitiesDataContext.Statement on spm.StatementId equals st.Id
                                                  where ppt.PageTypeId == item.PageTypeId && st.IsDeleted == false && st.IsActive == true
                                                  && pr.IsDeleted == false && pr.IsActive == true
                                                  select new StatementViewModel
                                                  {
                                                      Identifier = st.Id,
                                                      Name = st.Name
                                                  }).ToList();
                        item.StatementViewModel = statementViewModel;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Private Method

        /// <summary>
        /// This method help to set and validate connection string
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <exception cref="NedBankException.ConnectionStringNotFoundException"></exception>
        private void SetAndValidateConnectionString(string tenantCode)
        {
            try
            {
                //this.connectionString = this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
                this.connectionString = CommonVariable.ConnectionString;

                if (!this.validationEngine.IsValidText(this.connectionString))
                {
                    throw new ConnectionStringNotFoundException(tenantCode);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string WhereClauseGeneratorForCustomerCorporateSaver(CustomerCorporateSaverSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            //send account id value to this property when account master data fetching
            if (validationEngine.IsValidLong(searchParameter.Identifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            }

            //if (validationEngine.IsValidLong(searchParameter.CustomerId))
            //{
            //    queryString.Append("(" + string.Join("or ", searchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("CustomerId.Equals({0}) ", item))) + ") and ");
            //}

            if (validationEngine.IsValidLong(Convert.ToInt64(searchParameter.InvestorId)))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.InvestorId.ToString().Split(',').Select(item => string.Format("InvestorId.Equals({0}) ", item))) + ") and ");
            }

            if (validationEngine.IsValidLong(searchParameter.BatchId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.BatchId.ToString().Split(',').Select(item => string.Format("BatchId.Equals({0}) ", item))) + ") ");
            }

            if (searchParameter.WidgetFilterSetting != null && searchParameter.WidgetFilterSetting != string.Empty)
            {
                var filterEntities = JsonConvert.DeserializeObject<List<DynamicWidgetFilterEntity>>(searchParameter.WidgetFilterSetting);
                filterEntities.ForEach(filterEntity =>
                {
                    queryString.Append(this.QueryGenerator(filterEntity));
                });
            }

            queryString.Append(string.Format(" and TenantCode.Equals(\"{0}\") ", tenantCode));
            return queryString.ToString();
        }

        #endregion

        #region Private Methods
        private string QueryGenerator(DynamicWidgetFilterEntity filterEntity)
        {
            var queryString = string.Empty;
            var condtionalOp = filterEntity.ConditionalOperator != null && filterEntity.ConditionalOperator != string.Empty && filterEntity.ConditionalOperator != "0" ? filterEntity.ConditionalOperator : " and ";
            if (filterEntity.Operator == "EqualsTo")
            {
                queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + ".Equals(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "NotEqualsTo")
            {
                queryString = queryString + condtionalOp + " " + (string.Format("!" + filterEntity.FieldName + ".Equals(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "Contains")
            {
                queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + ".Contains(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "NotContains")
            {
                queryString = queryString + condtionalOp + " " + (string.Format("!" + filterEntity.FieldName + ".Contains(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "LessThan")
            {
                queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + " < " + filterEntity.Value + " "));
            }
            else if (filterEntity.Operator == "GreaterThan")
            {
                queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + " > " + filterEntity.Value + " "));
            }
            return queryString;
        }

        #endregion
    }
}
