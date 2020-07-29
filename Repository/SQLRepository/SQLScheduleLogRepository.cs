﻿// <copyright file="SQLScheduleLogRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Unity;
    #endregion

    /// <summary>
    /// This class represents the methods to perform operation with database for schedule log and schedule log detail entity.
    /// </summary>
    ///
    public class SQLScheduleLogRepository : IScheduleLogRepository
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

        /// <summary>
        /// The statement repository.
        /// </summary>
        private IStatementRepository statementRepository = null;

        /// <summary>
        /// The Page repository.
        /// </summary>
        private IPageRepository pageRepository = null;

        #endregion

        #region Constructor

        public SQLScheduleLogRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.pageRepository = this.unityContainer.Resolve<IPageRepository>();
            this.statementRepository = this.unityContainer.Resolve<IStatementRepository>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method helps to get list of schedule log.
        /// </summary>
        /// <param name="scheduleLogSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<ScheduleLog> GetScheduleLogs(ScheduleLogSearchParameter scheduleLogSearchParameter, string tenantCode)
        {
            IList<ScheduleLog> scheduleLogs = new List<ScheduleLog>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(scheduleLogSearchParameter, tenantCode);
                IList<ScheduleLogRecord> scheduleLogRecords = new List<ScheduleLogRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (scheduleLogSearchParameter.PagingParameter.PageIndex > 0 && scheduleLogSearchParameter.PagingParameter.PageSize > 0)
                    {
                        scheduleLogRecords = nISEntitiesDataContext.ScheduleLogRecords
                        .OrderBy(scheduleLogSearchParameter.SortParameter.SortColumn + " " + scheduleLogSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((scheduleLogSearchParameter.PagingParameter.PageIndex - 1) * scheduleLogSearchParameter.PagingParameter.PageSize)
                        .Take(scheduleLogSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        scheduleLogRecords = nISEntitiesDataContext.ScheduleLogRecords.Where(whereClause)
                        .OrderBy(scheduleLogSearchParameter.SortParameter.SortColumn + " " + scheduleLogSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }
                }

                if (scheduleLogRecords != null && scheduleLogRecords.Count > 0)
                {
                    scheduleLogRecords.ToList().ForEach(logRecord => scheduleLogs.Add(new ScheduleLog()
                    {
                        Identifier = logRecord.Id,
                        ScheduleId = logRecord.ScheduleId,
                        ScheduleName = logRecord.ScheduleName,
                        CreateDate = logRecord.CreationDate,
                        LogFilePath = logRecord.LogFilePath,
                        NumberOfRetry = logRecord.NumberOfRetry,
                        RenderEngineId = logRecord.RenderEngineId,
                        RenderEngineName = logRecord.RenderEngineName,
                        RenderEngineURL = logRecord.RenderEngineURL,
                        ScheduleStatus = logRecord.Status,
                        ProcessingTime = "",
                        RecordProcessed = ""
                    }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return scheduleLogs;
        }

        /// <summary>
        /// This method helps to get count of schedule logs.
        /// </summary>
        /// <param name="scheduleLogSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public int GetScheduleLogsCount(ScheduleLogSearchParameter scheduleLogSearchParameter, string tenantCode)
        {
            int scheduleLogCount = 0;
            string whereClause = this.WhereClauseGenerator(scheduleLogSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    scheduleLogCount = nISEntitiesDataContext.ScheduleLogRecords.Where(whereClause.ToString()).Count();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return scheduleLogCount;
        }

        /// <summary>
        /// This method helps to get list of schedule log detail.
        /// </summary>
        /// <param name="scheduleLogSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<ScheduleLogDetail> GetScheduleLogDetails(ScheduleLogDetailSearchParameter scheduleLogDetailSearchParameter, string tenantCode)
        {
            IList<ScheduleLogDetail> scheduleLogDetails = new List<ScheduleLogDetail>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorLogDetail(scheduleLogDetailSearchParameter, tenantCode);
                IList<ScheduleLogDetailRecord> scheduleLogDetailRecords = new List<ScheduleLogDetailRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (scheduleLogDetailSearchParameter.PagingParameter.PageIndex > 0 && scheduleLogDetailSearchParameter.PagingParameter.PageSize > 0)
                    {
                        scheduleLogDetailRecords = nISEntitiesDataContext.ScheduleLogDetailRecords
                        .OrderBy(scheduleLogDetailSearchParameter.SortParameter.SortColumn + " " + scheduleLogDetailSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((scheduleLogDetailSearchParameter.PagingParameter.PageIndex - 1) * scheduleLogDetailSearchParameter.PagingParameter.PageSize)
                        .Take(scheduleLogDetailSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        scheduleLogDetailRecords = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(whereClause)
                        .OrderBy(scheduleLogDetailSearchParameter.SortParameter.SortColumn + " " + scheduleLogDetailSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }
                }

                if (scheduleLogDetailRecords != null && scheduleLogDetailRecords.Count > 0)
                {
                    scheduleLogDetailRecords.ToList().ForEach(logDetail => scheduleLogDetails.Add(new ScheduleLogDetail()
                    {
                        Identifier = logDetail.Id,
                        CustomerId = logDetail.CustomerId,
                        CustomerName = logDetail.CustomerName,
                        LogMessage = logDetail.LogMessage,
                        NumberOfRetry = logDetail.NumberOfRetry ?? 0,
                        ScheduleId = logDetail.ScheduleId,
                        ScheduleLogId = logDetail.ScheduleLogId,
                        Status = logDetail.Status,
                        CreateDate = logDetail.CreationDate
                    }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return scheduleLogDetails;
        }

        /// <summary>
        /// This method helps to get count of schedule log details.
        /// </summary>
        /// <param name="scheduleLogDetailSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public int GetScheduleLogDetailsCount(ScheduleLogDetailSearchParameter scheduleLogDetailSearchParameter, string tenantCode)
        {
            int scheduleLogDetailCount = 0;
            string whereClause = this.WhereClauseGeneratorLogDetail(scheduleLogDetailSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    scheduleLogDetailCount = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(whereClause.ToString()).Count();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return scheduleLogDetailCount;
        }

        /// <summary>
        /// This method helps to retry to generate html statements for failed customer records
        /// </summary>
        /// <param name="scheduleLogDetails">The schedule log detail object list</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if statements generates successfully runs successfully, false otherwise</returns>
        public bool RetryStatementForFailedCustomerReocrds(IList<ScheduleLogDetail> scheduleLogDetails, string baseURL, string tenantCode)
        {
            bool scheduleRunStatus = false;
            StringBuilder htmlbody = new StringBuilder();
            StringBuilder finalHtml = new StringBuilder();
            IList<ScheduleRecord> schedules = new List<ScheduleRecord>();

            try
            {
                IList<ScheduleLogDetailRecord> scheduleLogDetailRecords = null;
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", scheduleLogDetails.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                    query.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));

                    scheduleLogDetailRecords = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (scheduleLogDetailRecords == null || scheduleLogDetailRecords.Count <= 0 || scheduleLogDetailRecords.Count() != string.Join(",", scheduleLogDetailRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new ScheduleLogDetailNotFoundException(tenantCode);
                    }
                }

                scheduleLogDetailRecords.ToList().ForEach(scheduleLogDetail =>
                {
                    BatchMasterRecord batchMaster = new BatchMasterRecord();
                    ScheduleRecord scheduleRecord = new ScheduleRecord();
                    CustomerMasterRecord customerMaster = new CustomerMasterRecord();
                    IList<BatchDetailRecord> batchDetails = new List<BatchDetailRecord>();

                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        scheduleRecord = nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == scheduleLogDetail.ScheduleId && item.TenantCode == tenantCode).FirstOrDefault();
                        if (scheduleRecord != null)
                        {
                            batchMaster = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.ScheduleId == scheduleRecord.Id && item.TenantCode == tenantCode).FirstOrDefault();
                            if (batchMaster != null)
                            {
                                customerMaster = nISEntitiesDataContext.CustomerMasterRecords.Where(item => item.Id == scheduleLogDetail.CustomerId && item.BatchId == batchMaster.Id).FirstOrDefault();
                                batchDetails = nISEntitiesDataContext.BatchDetailRecords.Where(item => item.BatchId == batchMaster.Id)?.ToList();
                            }
                        }
                    }

                    if (scheduleRecord != null && batchMaster != null && customerMaster != null)
                    {
                        Statement statement = new Statement();
                        StatementSearchParameter statementSearchParameter = new StatementSearchParameter
                        {
                            Identifier = scheduleRecord.StatementId,
                            IsActive = true,
                            IsStatementPagesRequired = true,
                            PagingParameter = new PagingParameter
                            {
                                PageIndex = 0,
                                PageSize = 0,
                            },
                            SortParameter = new SortParameter()
                            {
                                SortOrder = SortOrder.Ascending,
                                SortColumn = "Name",
                            },
                            SearchMode = SearchMode.Equals
                        };
                        var statements = this.statementRepository.GetStatements(statementSearchParameter, tenantCode);
                        if (statements.Count > 0)
                        {
                            statement = statements[0];

                            //Start to generate common html string
                            var statementPages = statements[0].StatementPages.OrderBy(it => it.SequenceNumber).ToList();
                            if (statementPages.Count != 0)
                            {
                                string navbarHtml = HtmlConstants.NAVBAR_HTML.Replace("{{BrandLogo}}", "absa-logo.png");
                                navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.Now.ToString("dd MMM yyyy"));
                                StringBuilder navItemList = new StringBuilder();
                                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                                statement.Pages = new List<Page>();
                                for (int i = 0; i < statementPages.Count; i++)
                                {
                                    PageSearchParameter pageSearchParameter = new PageSearchParameter
                                    {
                                        Identifier = statementPages[i].ReferencePageId,
                                        IsPageWidgetsRequired = true,
                                        IsActive = true,
                                        PagingParameter = new PagingParameter
                                        {
                                            PageIndex = 0,
                                            PageSize = 0,
                                        },
                                        SortParameter = new SortParameter()
                                        {
                                            SortOrder = SortOrder.Ascending,
                                            SortColumn = "DisplayName",
                                        },
                                        SearchMode = SearchMode.Equals
                                    };
                                    var pages = this.pageRepository.GetPages(pageSearchParameter, tenantCode);
                                    if (pages.Count != 0)
                                    {
                                        for (int j = 0; j < pages.Count; j++)
                                        {
                                            var page = pages[j];
                                            statement.Pages.Add(page);
                                            string tabClassName = Regex.Replace((page.DisplayName + " " + page.Version), @"\s+", "-");
                                            navItemList.Append(" <li class='nav-item'><a class='nav-link " + (i == 0 ? "active" : "") + " " + tabClassName + "' href='javascript:void(0);'>" + page.DisplayName + "</a> </li> ");
                                            string ExtraClassName = i > 0 ? "d-none " + tabClassName : tabClassName;
                                            string widgetHtmlHeader = HtmlConstants.WIDGET_HTML_HEADER.Replace("{{ExtraClass}}", ExtraClassName);
                                            widgetHtmlHeader = widgetHtmlHeader.Replace("{{DivId}}", tabClassName);
                                            htmlbody.Append(widgetHtmlHeader);

                                            int tempRowWidth = 0; // variable to check col-lg div length (bootstrap)
                                            int max = 0;
                                            if (page.PageWidgets.Count > 0)
                                            {
                                                var completelst = new List<PageWidget>(page.PageWidgets);
                                                int currentYPosition = 0;
                                                var isRowComplete = false;

                                                while (completelst.Count != 0)
                                                {
                                                    var lst = completelst.Where(it => it.Yposition == currentYPosition).ToList();
                                                    if (lst.Count > 0)
                                                    {
                                                        max = max + lst.Max(it => it.Height);
                                                        var _lst = completelst.Where(it => it.Yposition < max && it.Yposition != currentYPosition).ToList();
                                                        var mergedlst = lst.Concat(_lst).OrderBy(it => it.Xposition).ToList();
                                                        currentYPosition = max;
                                                        for (int x = 0; x < mergedlst.Count; x++)
                                                        {
                                                            if (tempRowWidth == 0)
                                                            {
                                                                htmlbody.Append("<div class='row'>"); // to start new row class div 
                                                                isRowComplete = false;
                                                            }
                                                            int divLength = (mergedlst[x].Width * 12) / 20;
                                                            tempRowWidth = tempRowWidth + divLength;

                                                            // If current col-lg class length is greater than 12, 
                                                            //then end parent row class div and then start new row class div
                                                            if (tempRowWidth > 12)
                                                            {
                                                                tempRowWidth = divLength;
                                                                htmlbody.Append("</div>"); // to end row class div
                                                                htmlbody.Append("<div class='row'>"); // to start new row class div
                                                                isRowComplete = false;
                                                            }
                                                            htmlbody.Append("<div class='col-lg-" + divLength + "'>");
                                                            if (mergedlst[x].WidgetId == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_ID)
                                                            {
                                                                htmlbody.Append(HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}"));
                                                            }
                                                            else if (mergedlst[x].WidgetId == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_ID)
                                                            {
                                                                htmlbody.Append(HtmlConstants.ACCOUNT_INFORMATION_WIDGET_HTML);
                                                            }
                                                            else if (mergedlst[x].WidgetId == HtmlConstants.IMAGE_WIDGET_ID)
                                                            {
                                                                htmlbody.Append(HtmlConstants.IMAGE_WIDGET_HTML.Replace("{{ImageSource}}", "{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}"));
                                                            }
                                                            else if (mergedlst[x].WidgetId == HtmlConstants.VIDEO_WIDGET_ID)
                                                            {
                                                                htmlbody.Append(HtmlConstants.VIDEO_WIDGET_HTML.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}"));
                                                            }
                                                            else if (mergedlst[x].WidgetId == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_ID)
                                                            {
                                                                htmlbody.Append(HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_HTML);
                                                            }

                                                            // To end current col-lg class div
                                                            htmlbody.Append("</div>");

                                                            // if current col-lg class width is equal to 12 or end before complete col-lg-12 class, 
                                                            //then end parent row class div
                                                            if (tempRowWidth == 12 || (x == mergedlst.Count - 1))
                                                            {
                                                                tempRowWidth = 0;
                                                                htmlbody.Append("</div>"); //To end row class div
                                                                isRowComplete = true;
                                                            }
                                                        }
                                                        mergedlst.ForEach(it =>
                                                        {
                                                            completelst.Remove(it);
                                                        });
                                                    }
                                                    else
                                                    {
                                                        if (completelst.Count != 0)
                                                        {
                                                            currentYPosition = completelst.Min(it => it.Yposition);
                                                        }
                                                    }
                                                }
                                                //If row class div end before complete col-lg-12 class
                                                if (isRowComplete == false)
                                                {
                                                    htmlbody.Append("</div>");
                                                }
                                            }
                                            else
                                            {
                                                htmlbody.Append(HtmlConstants.NO_WIDGET_MESSAGE_HTML);
                                            }
                                            htmlbody.Append(HtmlConstants.WIDGET_HTML_FOOTER);
                                        }
                                    }
                                    else
                                    {
                                        htmlbody.Append(HtmlConstants.NO_WIDGET_MESSAGE_HTML);
                                    }
                                }

                                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);
                                navbarHtml = navbarHtml.Replace("{{NavItemList}}", navItemList.ToString());

                                finalHtml.Append(HtmlConstants.HTML_HEADER);
                                finalHtml.Append(navbarHtml);
                                finalHtml.Append(htmlbody.ToString());
                                //finalHtml.Append(HtmlConstants.TAB_NAVIGATION_SCRIPT);
                                finalHtml.Append(HtmlConstants.HTML_FOOTER);
                            }
                        }

                        var logDetailRecord = GenerateStatement(customerMaster, statement, finalHtml, batchMaster, batchDetails, baseURL);
                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                        {
                            ScheduleLogDetailRecord scheduleLogDetailRecord = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(item => item.Id == scheduleLogDetail.Id).FirstOrDefault();
                            if (logDetailRecord.Status != ScheduleLogStatus.Failed.ToString() && scheduleLogDetailRecord != null)
                            {
                                scheduleLogDetailRecord.LogMessage = logDetailRecord.LogMessage;
                                scheduleLogDetailRecord.Status = logDetailRecord.Status;
                                scheduleLogDetailRecord.StatementFilePath = logDetailRecord.StatementFilePath;
                                nISEntitiesDataContext.SaveChanges();
                            }
                        }
                    }

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return scheduleRunStatus;
        }

        /// <summary>
        /// This method helps to re run the schedule for failed customer records
        /// </summary>
        /// <param name="scheduleLogIdentifier">The schedule log identifier</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if statements generates successfully runs successfully, false otherwise</returns>
        public bool ReRunScheduleForFailedCases(long scheduleLogIdentifier, string baseURL, string tenantCode)
        {
            bool scheduleRunStatus = false;
            try
            {
                IList<ScheduleLogDetailRecord> failedScheduleLogDetailRecords = null;
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    failedScheduleLogDetailRecords = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(item => item.ScheduleLogId == scheduleLogIdentifier && item.Status == ScheduleLogStatus.Failed.ToString() && item.TenantCode == tenantCode).ToList();
                    if (failedScheduleLogDetailRecords == null || failedScheduleLogDetailRecords.Count == 0)
                    {
                        throw new ScheduleLogDetailNotFoundException(tenantCode);
                    }
                }

                IList<ScheduleLogDetail> scheduleLogDetails = new List<ScheduleLogDetail>();
                failedScheduleLogDetailRecords.ToList().ForEach(item => scheduleLogDetails.Add(new ScheduleLogDetail()
                {
                    Identifier = item.Id
                }));

                if (scheduleLogDetails.Count != 0)
                {
                    scheduleRunStatus = RetryStatementForFailedCustomerReocrds(scheduleLogDetails, baseURL, tenantCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return scheduleRunStatus;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="logSearchParameter">Schedule log search Parameters</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(ScheduleLogSearchParameter logSearchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            try
            {
                if (validationEngine.IsValidText(logSearchParameter.ScheduleLogId))
                {
                    queryString.Append("(" + string.Join("or ", logSearchParameter.ScheduleLogId.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(logSearchParameter.RenderEngineId))
                {
                    queryString.Append("(" + string.Join("or ", logSearchParameter.RenderEngineId.ToString().Split(',').Select(item => string.Format("RenderEngineId.Equals({0}) ", item))) + ") and");
                }
                if (validationEngine.IsValidText(logSearchParameter.ScheduleId))
                {
                    queryString.Append("(" + string.Join("or ", logSearchParameter.ScheduleId.ToString().Split(',').Select(item => string.Format("ScheduleId.Equals({0}) ", item))) + ") and");
                }
                if (validationEngine.IsValidText(logSearchParameter.ScheduleStatus))
                {
                    queryString.Append("(" + string.Join("or ", logSearchParameter.ScheduleStatus.ToString().Split(',').Select(item => string.Format("Status.Equals(\"{0}\") ", item))) + ") and ");
                }

                if (logSearchParameter.SearchMode == SearchMode.Equals)
                {
                    if (validationEngine.IsValidText(logSearchParameter.ScheduleName))
                    {
                        queryString.Append("(" + string.Join("or ", logSearchParameter.ScheduleName.ToString().Split(',').Select(item => string.Format("ScheduleName.Equals(\"{0}\") ", item))) + ") and ");
                    }
                    if (validationEngine.IsValidText(logSearchParameter.RenderEngineName))
                    {
                        queryString.Append("(" + string.Join("or ", logSearchParameter.RenderEngineName.ToString().Split(',').Select(item => string.Format("RenderEngineName.Equals(\"{0}\") ", item))) + ") and ");
                    }
                }
                if (logSearchParameter.SearchMode == SearchMode.Contains)
                {
                    if (validationEngine.IsValidText(logSearchParameter.ScheduleName))
                    {
                        queryString.Append("(" + string.Join("or ", logSearchParameter.ScheduleName.ToString().Split(',').Select(item => string.Format("ScheduleName.Contains(\"{0}\") ", item))) + ") and ");
                    }
                    if (validationEngine.IsValidText(logSearchParameter.RenderEngineName))
                    {
                        queryString.Append("(" + string.Join("or ", logSearchParameter.RenderEngineName.ToString().Split(',').Select(item => string.Format("RenderEngineName.Contains(\"{0}\") ", item))) + ") and ");
                    }
                }

                if (this.validationEngine.IsValidDate(logSearchParameter.StartDate) && !this.validationEngine.IsValidDate(logSearchParameter.EndDate))
                {
                    DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(logSearchParameter.StartDate), DateTimeKind.Utc);
                    queryString.Append("StartDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") and ");
                }
                if (this.validationEngine.IsValidDate(logSearchParameter.EndDate) && !this.validationEngine.IsValidDate(logSearchParameter.StartDate))
                {
                    DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(logSearchParameter.EndDate), DateTimeKind.Utc);
                    queryString.Append("EndDate <= DateTime(" + toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
                }
                if (this.validationEngine.IsValidDate(logSearchParameter.StartDate) && this.validationEngine.IsValidDate(logSearchParameter.EndDate))
                {
                    DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(logSearchParameter.StartDate), DateTimeKind.Utc);
                    DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(logSearchParameter.EndDate), DateTimeKind.Utc);

                    queryString.Append("StartDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") " +
                                   "and EndDate <= DateTime(" + +toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
                }

                queryString.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));
                return queryString.ToString();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="logDetailSearchParameter">Schedule log search Parameters</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGeneratorLogDetail(ScheduleLogDetailSearchParameter logDetailSearchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            try
            {
                if (validationEngine.IsValidText(logDetailSearchParameter.ScheduleLogDetailId))
                {
                    queryString.Append("(" + string.Join("or ", logDetailSearchParameter.ScheduleLogDetailId.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(logDetailSearchParameter.ScheduleLogId))
                {
                    queryString.Append("(" + string.Join("or ", logDetailSearchParameter.ScheduleLogId.ToString().Split(',').Select(item => string.Format("ScheduleLogId.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(logDetailSearchParameter.CustomerId))
                {
                    queryString.Append("(" + string.Join("or ", logDetailSearchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("CustomerId.Equals({0}) ", item))) + ") and ");
                }

                if (logDetailSearchParameter.SearchMode == SearchMode.Equals)
                {
                    if (validationEngine.IsValidText(logDetailSearchParameter.CustomerName))
                    {
                        queryString.Append("(" + string.Join("or ", logDetailSearchParameter.CustomerName.ToString().Split(',').Select(item => string.Format("CustomerName.Equals(\"{0}\") ", item))) + ") and ");
                    }
                    if (validationEngine.IsValidText(logDetailSearchParameter.Status))
                    {
                        queryString.Append("(" + string.Join("or ", logDetailSearchParameter.Status.ToString().Split(',').Select(item => string.Format("Status.Equals(\"{0}\") ", item))) + ") and ");
                    }
                }
                if (logDetailSearchParameter.SearchMode == SearchMode.Contains)
                {
                    if (validationEngine.IsValidText(logDetailSearchParameter.CustomerName))
                    {
                        queryString.Append("(" + string.Join("or ", logDetailSearchParameter.CustomerName.ToString().Split(',').Select(item => string.Format("CustomerName.Contains(\"{0}\") ", item))) + ") and ");
                    }
                    if (validationEngine.IsValidText(logDetailSearchParameter.Status))
                    {
                        queryString.Append("(" + string.Join("or ", logDetailSearchParameter.Status.ToString().Split(',').Select(item => string.Format("Status.Contains(\"{0}\") ", item))) + ") and ");
                    }
                }

                queryString.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));
                return queryString.ToString();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method help to generate statement for customer
        /// </summary>
        /// <param name="customer"> the customer object </param>
        /// <param name="statement"> the statement object </param>
        /// <param name="finalHtml"> the final html string for statement </param>
        /// <param name="batchMaster"> the batch master object </param>
        /// <param name="batchDetails"> the list of batch details records </param>
        /// <param name="baseURL"> the base URL of API </param>
        private ScheduleLogDetailRecord GenerateStatement(CustomerMasterRecord customer, Statement statement, StringBuilder finalHtml, BatchMasterRecord batchMaster, IList<BatchDetailRecord> batchDetails, string baseURL)
        {
            ScheduleLogDetailRecord logDetailRecord = new ScheduleLogDetailRecord();
            try
            {
                //start to render actual html content data
                StringBuilder currentCustomerHtmlStatement = new StringBuilder(finalHtml.ToString());
                for (int i = 0; i < statement.Pages.Count; i++)
                {
                    var page = statement.Pages[i];
                    var pagewidgets = page.PageWidgets;
                    for (int j = 0; j < pagewidgets.Count; j++)
                    {
                        var widget = pagewidgets[j];
                        if (widget.WidgetId == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_ID) //Customer Information Widget
                        {
                            IList<CustomerMediaRecord> customerMedias = new List<CustomerMediaRecord>();
                            currentCustomerHtmlStatement.Replace("{{CustomerName}}", (customer.FirstName + " " + customer.MiddleName + " " + customer.LastName));
                            currentCustomerHtmlStatement.Replace("{{Address1}}", customer.AddressLine1);
                            string address2 = (customer.AddressLine2 != "" ? customer.AddressLine2 + ", " : "") + (customer.City != "" ? customer.City + ", " : "") + (customer.State != "" ? customer.State + ", " : "") + (customer.Country != "" ? customer.Country + ", " : "") + (customer.Zip != "" ? customer.Zip : "");
                            currentCustomerHtmlStatement.Replace("{{Address2}}", address2);

                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                customerMedias = nISEntitiesDataContext.CustomerMediaRecords.Where(item => item.CustomerId == customer.Id && item.StatementId == statement.Identifier && item.WidgetId == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_ID)?.ToList();
                            }
                            var custMedia = customerMedias.Where(item => item.PageId == page.Identifier && item.WidgetId == widget.WidgetId)?.ToList()?.FirstOrDefault();
                            if (custMedia != null && custMedia.VideoURL != string.Empty)
                            {
                                currentCustomerHtmlStatement.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.WidgetId + "}}", custMedia.VideoURL);
                            }
                            else
                            {
                                var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.WidgetId && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                if (batchDetail != null && batchDetail.VideoURL != string.Empty)
                                {
                                    currentCustomerHtmlStatement.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.WidgetId + "}}", batchDetail.VideoURL);
                                }
                            }
                        }
                        if (widget.WidgetId == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_ID) //Account Information Widget
                        {
                            StringBuilder AccDivData = new StringBuilder();
                            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Date" + "</div><label class='list-value mb-0'>" + Convert.ToDateTime(customer.StatementDate).ToShortDateString() + "</label></div></div>");

                            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Period" + "</div><label class='list-value mb-0'>" + customer.StatementPeriod + "</label></div></div>");

                            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Cusomer ID" + "</div><label class='list-value mb-0'>" + customer.CustomerCode + "</label></div></div>");

                            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Name" + "</div><label class='list-value mb-0'>" + customer.RmName + "</label></div></div>");

                            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Contact Number" + "</div><label class='list-value mb-0'>" + customer.RmContactNumber + "</label></div></div>");
                            currentCustomerHtmlStatement.Replace("{{AccountInfoData}}", AccDivData.ToString());
                        }
                        if (widget.WidgetId == HtmlConstants.IMAGE_WIDGET_ID) //Image Widget
                        {
                            var imgAssetFilepath = string.Empty;
                            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                            {
                                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                if (widgetSetting.isPersonalize == false) //Is not dynamic image, then assign selected image from asset library
                                {
                                    imgAssetFilepath = baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                }
                                else //Is dynamic image, then assign it from database 
                                {
                                    ImageRecord imageRecord = new ImageRecord();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        imageRecord = nISEntitiesDataContext.ImageRecords.Where(item => item.BatchId == batchMaster.Id && item.StatementId == statement.Identifier && item.PageId == page.Identifier && item.WidgetId == widget.WidgetId)?.ToList()?.FirstOrDefault();
                                    }
                                    if (imageRecord != null && imageRecord.Image1 != string.Empty)
                                    {
                                        imgAssetFilepath = imageRecord.Image1;
                                    }
                                    else
                                    {
                                        var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.WidgetId && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                        if (batchDetail != null && batchDetail.ImageURL != string.Empty)
                                        {
                                            imgAssetFilepath = batchDetail.ImageURL;
                                        }
                                        else
                                        {
                                            logDetailRecord.LogMessage = "Image not found for Page: " + page.Identifier + " and Widget: " + widget.WidgetId + " for image widget..!!";
                                            logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                            break;
                                        }
                                    }
                                }
                                currentCustomerHtmlStatement.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.WidgetId + "}}", imgAssetFilepath);
                            }
                        }
                        if (widget.WidgetId == HtmlConstants.VIDEO_WIDGET_ID) //Video widget
                        {
                            var vdoAssetFilepath = string.Empty;
                            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                            {
                                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                if (widgetSetting.isPersonalize == false) //Is not dynamic video, then assign selected video from asset library
                                {
                                    vdoAssetFilepath = baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                }
                                else //Is dynamic video, then assign it from database 
                                {
                                    VideoRecord videoRecord = new VideoRecord();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        videoRecord = nISEntitiesDataContext.VideoRecords.Where(item => item.BatchId == batchMaster.Id && item.StatementId == statement.Identifier && item.PageId == page.Identifier && item.WidgetId == widget.WidgetId)?.ToList()?.FirstOrDefault();
                                    }
                                    if (videoRecord != null && videoRecord.Video1 != string.Empty)
                                    {
                                        vdoAssetFilepath = videoRecord.Video1;
                                    }
                                    else
                                    {
                                        var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.WidgetId && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                        if (batchDetail != null && batchDetail.VideoURL != string.Empty)
                                        {
                                            vdoAssetFilepath = batchDetail.VideoURL;
                                        }
                                        else
                                        {
                                            logDetailRecord.LogMessage = "Video not found for Page: " + page.Identifier + " and Widget: " + widget.WidgetId + " for video widget..!!";
                                            logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                            break;
                                        }
                                    }
                                }
                                currentCustomerHtmlStatement.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.WidgetId + "}}", vdoAssetFilepath);
                            }
                        }
                        if (widget.WidgetId == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_ID) //Summary at Glance Widget
                        {
                            IList<AccountMasterRecord> accountrecords = new List<AccountMasterRecord>();
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                accountrecords = nISEntitiesDataContext.AccountMasterRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id)?.ToList();
                            }
                            if (accountrecords.Count > 0)
                            {
                                StringBuilder accSummary = new StringBuilder();
                                var accRecords = accountrecords.GroupBy(item => item.AccountType).ToList();
                                accRecords.ToList().ForEach(acc =>
                                {
                                    accSummary.Append("<tr><td>" + acc.FirstOrDefault().AccountType + "</td><td>" + acc.FirstOrDefault().Currency + "</td><td>" + acc.Sum(it => it.Balance).ToString() + "</td></tr>");
                                });
                                currentCustomerHtmlStatement.Replace("{{AccountSummary}}", accSummary.ToString());
                            }
                            else
                            {
                                logDetailRecord.LogMessage = "Account master data is not available related to Summary at Glance widget..!!";
                                logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                break;
                            }
                        }
                    }
                }

                if (logDetailRecord.Status != ScheduleLogStatus.Failed.ToString())
                {
                    string fileName = "Statement_" + customer.Id + "_" + statement.Identifier + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";
                    string filePath = this.utility.WriteToFile(currentCustomerHtmlStatement.ToString(), fileName, batchMaster.Id, customer.Id);

                    logDetailRecord.StatementFilePath = filePath;
                    logDetailRecord.Status = ScheduleLogStatus.Completed.ToString();
                    logDetailRecord.LogMessage = "Statement generated successfully..!!";
                }
                return logDetailRecord;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Get Connection String

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

        #endregion

        #endregion
    }
}