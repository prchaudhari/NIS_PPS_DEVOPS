﻿// <copyright file="SQLPageRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using Unity;
    #endregion

    public class SQLPageRepository : IPageRepository
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
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        #endregion

        #region Constructor

        public SQLPageRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method reference to publish page
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool PublishPage(long pageIdentifier, string tenantCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method adds the specified list of pages in page repository.
        /// </summary>
        /// <param name="pages">The list of pages</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if pages are added successfully, else false.
        /// </returns>
        public bool AddPages(IList<Page> pages, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicatePage(pages, "AddOperation", tenantCode))
                {
                    throw new DuplicatePageFoundException(tenantCode);
                }
                
                IList<PageRecord> pageRecords = new List<PageRecord>();
                pages.ToList().ForEach(page =>
                {
                    pageRecords.Add(new PageRecord()
                    {
                        DisplayName = page.DisplayName,
                        PageTypeId = page.PageTypeId,
                        PublishedBy = page.PublishedBy,
                        IsActive = true,
                        IsDeleted = false,
                        Status = "New",
                        CreatedDate = DateTime.Now,
                        LastUpdatedDate = DateTime.Now,
                        UpdateBy = page.UpdatedBy,
                        Version = "V.1.0.0",
                        Owner = page.PageOwner,
                        TenantCode = tenantCode
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.PageRecords.AddRange(pageRecords);
                    nISEntitiesDataContext.SaveChanges();
                }

                pages.ToList().ForEach(page =>
                {
                    page.Identifier = pageRecords.Where(p => p.DisplayName == page.DisplayName && p.PageTypeId == page.PageTypeId).Single().Id;

                    //Add page widgets in to page widget map
                    if (page.PageWidgets?.Count > 0)
                    {
                        this.AddPageWidgets(page.PageWidgets, page.Identifier, tenantCode);
                    }
                });
                result = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// This method reference to preview page
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool PreviewPage(long pageIdentifier, string tenantCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method deletes the specified list of pages from page repository.
        /// </summary>
        /// <param name="pageIdentifier">The page identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if pages are deleted successfully, else false.
        /// </returns>
        public bool DeletePages(long pageIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<PageRecord> pageRecords = nISEntitiesDataContext.PageRecords.Where(itm => itm.Id == pageIdentifier).ToList();
                    if (pageRecords == null || pageRecords.Count <= 0)
                    {
                        throw new RoleNotFoundException(tenantCode);
                    }

                    pageRecords.ToList().ForEach(item =>
                    {
                        item.IsDeleted = true;
                    });

                    nISEntitiesDataContext.SaveChanges();
                }
                result = true;
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method gets the specified list of pages from page repository.
        /// </summary>
        /// <param name="pageSearchParameter">The page search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of pages
        /// </returns>
        public IList<Page> GetPages(PageSearchParameter pageSearchParameter, string tenantCode)
        {
            IList<Page> pages = new List<Page>();

            try 
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(pageSearchParameter, tenantCode);
                IList<PageRecord> pageRecords = new List<PageRecord>();
                IList<UserRecord> userRecords = new List<UserRecord>();
                IList<PageTypeRecord> pageTypeRecords = new List<PageTypeRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString)) 
                {
                    if (pageSearchParameter.PagingParameter.PageIndex > 0 && pageSearchParameter.PagingParameter.PageSize > 0)
                    {
                        pageRecords = nISEntitiesDataContext.PageRecords
                        .OrderBy(pageSearchParameter.SortParameter.SortColumn + " " + pageSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((pageSearchParameter.PagingParameter.PageIndex - 1) * pageSearchParameter.PagingParameter.PageSize)
                        .Take(pageSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        pageRecords = nISEntitiesDataContext.PageRecords
                        .Where(whereClause)
                        .OrderBy(pageSearchParameter.SortParameter.SortColumn + " " + pageSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (pageSearchParameter.IsPageWidgetsRequired)
                    {
                        pageRecords.ToList().ForEach(pageRecord =>
                        {
                            pageRecord.PageWidgetMapRecords = nISEntitiesDataContext.PageWidgetMapRecords.Where(itm => itm.PageId == pageRecord.Id && itm.TenantCode == tenantCode).ToList();
                        });
                    }

                    if (pageRecords != null && pageRecords.ToList().Count > 0)
                    {
                        //userRecords = nISEntitiesDataContext.UserRecords.Where(item => item.IsActive == true && item.IsDeleted == false).ToList();
                        pageTypeRecords = nISEntitiesDataContext.PageTypeRecords.Where(itm => itm.IsActive == true && itm.IsDeleted == false).ToList();                      
                    }
                }

                if (pageRecords != null && pageRecords.ToList().Count > 0)
                {
                    pageRecords?.ToList().ForEach(pageRecord =>
                    {
                        IList<PageWidget> pageWidgets = new List<PageWidget>();
                        if (pageRecord.PageWidgetMapRecords?.ToList().Count > 0)
                        {
                            pageRecord.PageWidgetMapRecords?.ToList().ForEach(pageWidgetRecord =>
                            {
                                pageWidgets.Add(new PageWidget
                                {
                                    Identifier = pageWidgetRecord.Id,
                                    PageId = pageWidgetRecord.PageId,
                                    Height = pageWidgetRecord.Height,
                                    WidgetId = pageWidgetRecord.ReferenceWidgetId,
                                    Width = pageWidgetRecord.Width,
                                    Xposition = pageWidgetRecord.Xposition,
                                    Yposition = pageWidgetRecord.Yposition
                                });
                            });
                        }

                        pages.Add(new Page
                        {
                            Identifier = pageRecord.Id,
                            DisplayName = pageRecord.DisplayName,
                            CreatedDate = pageRecord.CreatedDate ?? (DateTime)pageRecord.CreatedDate,
                            IsActive = pageRecord.IsActive,
                            IsDeleted = pageRecord.IsDeleted,
                            LastUpdatedDate = pageRecord.LastUpdatedDate ?? (DateTime)pageRecord.LastUpdatedDate,
                            PageOwner = pageRecord.Owner,
                           // PageOwnerName = userRecords.Where(usr => usr.Id == pageRecord.Owner).ToList()?.Select(itm => new { FullName = itm.FirstName + " " + itm.LastName })?.FirstOrDefault().FullName,
                            PageWidgets = pageWidgets,
                            Status = pageRecord.Status,
                            Version = pageRecord.Version,
                            PageTypeId = pageRecord.PageTypeId,
                            PageTypeName = pageTypeRecords.FirstOrDefault(itm => itm.Id == pageRecord.PageTypeId)?.Name,
                            PublishedBy = pageRecord.PublishedBy,
                           // PagePublishedByUserName = userRecords.Where(usr => usr.Id == pageRecord.PublishedBy).ToList()?.Select(itm => new { FullName = itm.FirstName + " " + itm.LastName })?.FirstOrDefault().FullName,
                            PublishedOn = pageRecord.PublishedOn ?? DateTime.MinValue,
                            UpdatedBy = pageRecord.UpdateBy,
                           // PageUpdatedByUserName = userRecords.Where(usr => usr.Id == pageRecord.UpdateBy).ToList()?.Select(itm => new { FullName = itm.FirstName + " " + itm.LastName })?.FirstOrDefault().FullName,
                        });
                    });
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pages;
        }

        /// <summary>
        /// This method reference to get page count
        /// </summary>
        /// <param name="pageSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Page count</returns>
        public int GetPageCount(PageSearchParameter pageSearchParameter, string tenantCode)
        {
            int pageCount = 0;
            string whereClause = this.WhereClauseGenerator(pageSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    pageCount = nISEntitiesDataContext.PageRecords.Where(whereClause.ToString()).Count();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return pageCount;
        }

        /// <summary>
        /// This method updates the specified list of pages in page repository.
        /// </summary>
        /// <param name="pages">The list of pages</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if pages are updated successfully, else false.
        /// </returns>
        public bool UpdatePages(IList<Page> pages, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicatePage(pages, "UpdateOperation", tenantCode))
                {
                    throw new DuplicatePageFoundException(tenantCode);
                }

                IList<PageRecord> pageRecords = new List<PageRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", pages.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");

                    pageRecords = nISEntitiesDataContext.PageRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (pageRecords == null || pageRecords.Count <= 0 || pageRecords.Count() != string.Join(",", pageRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new PageNotFoundException(tenantCode);
                    }

                    pages.ToList().ForEach(item =>
                    {
                        PageRecord pageRecord = pageRecords.FirstOrDefault(data => data.Id == item.Identifier && data.TenantCode == tenantCode && data.IsDeleted == false);
                        pageRecord.DisplayName = item.DisplayName;
                        //pageRecord.PageTypeId = item.PageTypeId;
                        pageRecord.TenantCode = tenantCode;
                    });

                    nISEntitiesDataContext.SaveChanges();
                }

                pages.ToList().ForEach(item =>
                {
                    item.Identifier = pageRecords.ToList().Where(pageRec => pageRec.DisplayName == item.DisplayName && pageRec.PageTypeId == item.PageTypeId).FirstOrDefault().Id;

                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        var existingPageWidgetMappingRecords = nISEntitiesDataContext.PageWidgetMapRecords.Where(itm => itm.PageId == item.Identifier).ToList();
                        nISEntitiesDataContext.PageWidgetMapRecords.RemoveRange(existingPageWidgetMappingRecords);
                        nISEntitiesDataContext.SaveChanges();
                    }

                    if (item.PageWidgets?.Count > 0)
                    {
                        AddPageWidgets(item.PageWidgets, item.Identifier, tenantCode);
                    }
                });

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">Role search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(PageSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (searchParameter.SearchMode == SearchMode.Equals)
            {
                if (validationEngine.IsValidLong(searchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(searchParameter.DisplayName))
                {
                    queryString.Append(string.Format("DisplayName.Equals(\"{0}\") and ", searchParameter.DisplayName));
                }
            }
            if (searchParameter.SearchMode == SearchMode.Contains)
            {

                if (validationEngine.IsValidText(searchParameter.DisplayName))
                {
                    queryString.Append(string.Format("DisplayName.Contains(\"{0}\") and ", searchParameter.DisplayName));
                }
            }
            if (validationEngine.IsValidText(searchParameter.Status))
            {
                queryString.Append(string.Format("Status.Equals(\"{0}\") and ", searchParameter.Status));
            }
            if (validationEngine.IsValidLong(searchParameter.PageTypeId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.PageTypeId.ToString().Split(',').Select(item => string.Format("PageTypeId.Equals({0}) ", item))) + ") and ");
            }
            if (searchParameter.IsActive != null)
            {
                queryString.Append(string.Format("IsActive.Equals({0}) and ", searchParameter.IsActive));
            }
            if (this.validationEngine.IsValidDate(searchParameter.StartDate) && !this.validationEngine.IsValidDate(searchParameter.EndDate))
            {
                queryString.Append("PublishedOn >= DateTime(" + searchParameter.StartDate.Year + "," + searchParameter.StartDate.Month + "," + searchParameter.StartDate.Day + ") and ");
            }

            if (this.validationEngine.IsValidDate(searchParameter.EndDate) && !this.validationEngine.IsValidDate(searchParameter.StartDate))
            {
                queryString.Append("PublishedOn <= DateTime(" + searchParameter.EndDate.Year + "," + searchParameter.EndDate.Month + "," + searchParameter.EndDate.Day + ") and ");
            }

            if (this.validationEngine.IsValidDate(searchParameter.StartDate) && this.validationEngine.IsValidDate(searchParameter.EndDate))
            {
                queryString.Append("PublishedOn >= DateTime(" + searchParameter.StartDate.Year + "," + searchParameter.StartDate.Month + "," + searchParameter.StartDate.Day + ") and PublishedOn <= DateTime(" + searchParameter.EndDate.Year + ", " + searchParameter.EndDate.Month + ", " + searchParameter.EndDate.Day + ") and ");
            }

            queryString.Append(string.Format("TenantCode.Equals(\"{0}\") and IsDeleted.Equals(false)", tenantCode));
            return queryString.ToString();
        }

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="pages">The pages to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        private bool IsDuplicatePage(IList<Page> pages, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();

                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", pages.Select(item => string.Format("DisplayName.Equals(\"{0}\")", item.DisplayName)).ToList()) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", pages.Select(item => string.Format("(DisplayName.Equals(\"{0}\") and !Id.Equals({1}))", item.DisplayName, item.Identifier))) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<PageRecord> pageRecords = nISEntitiesDataContext.PageRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (pageRecords.Count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="pages">The pages to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        private bool IsDuplicatePageWidget(IList<PageWidget> pageWidgets, long pageIdentifier, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();

                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", pageWidgets.Select(item => string.Format("PageId.equals({0}) and ReferenceWidgetId.Equals({1}) ", pageIdentifier, item.WidgetId)).ToList()) + ") and TenantCode.equals(\"" + tenantCode + "\")");
                }
                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", pageWidgets.Select(item => string.Format("(PageId.Equals({0}) and ReferenceWidgetId.Equals({1}) and !Id.Equals({2}))", pageIdentifier, item.WidgetId, item.Identifier))) + ") and TenantCode.Equals(\"" + tenantCode + "\")");
                }
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<PageWidgetMapRecord> pageWidgetMapRecords = nISEntitiesDataContext.PageWidgetMapRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (pageWidgetMapRecords.Count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// This method help to set and validate connection string
        /// </summary>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        private void SetAndValidateConnectionString(string tenantCode)
        {
            try
            {
                this.connectionString = validationEngine.IsValidText(this.connectionString) ? this.connectionString : this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
                if (!this.validationEngine.IsValidText(this.connectionString))
                {
                    throw new ConnectionStringNotFoundException(tenantCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool AddPageWidgets(IList<PageWidget> pageWidgets, long pageIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicatePageWidget(pageWidgets, pageIdentifier, "AddOperation", tenantCode))
                {
                    throw new DuplicatePageWidgetFoundException(tenantCode);
                }

                IList<PageWidgetMapRecord> pageWidgetRecords = new List<PageWidgetMapRecord>();
                pageWidgets.ToList().ForEach(pageWidget =>
                {
                    pageWidgetRecords.Add(new PageWidgetMapRecord()
                    {
                        Height = pageWidget.Height,
                        PageId = pageIdentifier,
                        ReferenceWidgetId = pageWidget.WidgetId,
                        TenantCode = tenantCode,
                        WidgetSetting = pageWidget.WidgetSetting,
                        Width = pageWidget.Width,
                        Xposition = pageWidget.Xposition,
                        Yposition = pageWidget.Yposition
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.PageWidgetMapRecords.AddRange(pageWidgetRecords);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;

        }

        #endregion
    }
}
