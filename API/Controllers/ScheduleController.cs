﻿// <copyright file="ScheduleController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    using nIS.NedBank;
    #region References

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using Unity;

    #endregion

    /// <summary>
    /// This class represent api controller for schedule
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Schedule")]
    public class ScheduleController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The schedule manager object.
        /// </summary>
        private ScheduleManager scheduleManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        /// <summary>
        /// The tenant config manager object.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;


        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleController"/> class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public ScheduleController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.tenantConfigurationManager = new TenantConfigurationManager(unityContainer);
            this.scheduleManager = new ScheduleManager(this.unityContainer);
        }

        #endregion

        #region Public Methods

        #region Schedule 

        /// <summary>
        /// This method helps to add schedules
        /// </summary>
        /// <param name="schedules">The schedules.</param>
        /// <returns>
        /// boolean value
        /// </returns>
        [HttpPost]
        public bool Add(IList<Schedule> schedules)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.scheduleManager.AddSchedules(schedules, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// Adds the with language.
        /// </summary>
        /// <param name="schedules">The schedules.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddWithLanguage")]
        public bool AddWithLanguage(IList<Schedule> schedules)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.scheduleManager.AddSchedulesWithLanguage(schedules, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to update schedules.
        /// </summary>
        /// <param name="schedules">The schedules.</param>
        /// <returns>
        /// boolean value
        /// </returns>
        [HttpPost]
        public bool Update(IList<Schedule> schedules)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.scheduleManager.UpdateSchedules(schedules, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// Updates the with language.
        /// </summary>
        /// <param name="schedules">The schedules.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateWithLanguage")]
        public bool UpdateWithLanguage(IList<Schedule> schedules)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.scheduleManager.UpdateSchedulesWithLanguage(schedules, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to delete schedules.
        /// </summary>
        /// <param name="schedules">The schedules.</param>
        /// <returns>
        /// boolean value
        /// </returns>
        [HttpPost]
        public bool Delete(IList<Schedule> schedules)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.scheduleManager.DeleteSchedules(schedules, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to get schedules list based on the search parameters.
        /// </summary>
        /// <param name="scheduleSearchParameter">The schedule search parameter.</param>
        /// <returns>
        /// List of schedules
        /// </returns>
        [HttpPost]
        public IList<ScheduleListModel> List(ScheduleSearchParameter scheduleSearchParameter)
        {
            IList<ScheduleListModel> schedules = new List<ScheduleListModel>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                //schedules = this.scheduleManager.GetSchedules(scheduleSearchParameter, tenantCode);
                schedules = this.scheduleManager.GetSchedulesWithProduct(scheduleSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.scheduleManager.GetScheduleCount(scheduleSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return schedules;
        }

        [HttpPost]
        [Route("ListWithLanguage")]
        public IList<Schedule> ListWithLanguage(ScheduleSearchParameter scheduleSearchParameter)
        {
            IList<Schedule> schedules = new List<Schedule>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                schedules = this.scheduleManager.GetSchedulesWithLanguage(scheduleSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.scheduleManager.GetScheduleCount(scheduleSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return schedules;
        }

        /// <summary>
        /// This method helps to get schedule based on given identifier.
        /// </summary>
        /// <param name="scheduleIdentifier">The schedule identifier.</param>
        /// <returns>
        /// schedule record
        /// </returns>
        [HttpGet]
        public Schedule Detail(long scheduleIdentifier)
        {
            IList<Schedule> schedules = new List<Schedule>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                ScheduleSearchParameter scheduleSearchParameter = new ScheduleSearchParameter();
                scheduleSearchParameter.Identifier = scheduleIdentifier.ToString();
                scheduleSearchParameter.SortParameter.SortColumn = "Id";
                schedules = this.scheduleManager.GetSchedules(scheduleSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return schedules.First();
        }

        /// <summary>
        /// This method helps to activate the schedule
        /// </summary>
        /// <param name="scheduleIdentifier">The schedule identifier</param>
        /// <returns>
        /// True if schedule activated successfully false otherwise
        /// </returns>
        [HttpGet]
        public bool Activate(long scheduleIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                this.scheduleManager.ActivateSchedule(scheduleIdentifier, tenantCode);
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method helps to deactivate the schedule
        /// </summary>
        /// <param name="scheduleIdentifier">The schedule identifier</param>
        /// <returns>
        /// True if schedule deactivated successfully false otherwise
        /// </returns>
        [HttpGet]
        public bool Deactivate(long scheduleIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                this.scheduleManager.DeactivateSchedule(scheduleIdentifier, tenantCode);
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <returns>
        /// True if schedule runs successfully false otherwise
        /// </returns>
        [HttpPost]
        public bool RunSchedule()
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                var baseURL = Url.Content("~/");
                var outputLocation = AppDomain.CurrentDomain.BaseDirectory;
                return this.scheduleManager.RunScheduleNew(baseURL, outputLocation, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to run the schedule now
        /// </summary>
        /// <param name="batchMaster">The batch object</param>
        /// <returns>
        /// True if schedule runs successfully false otherwise
        /// </returns>
        [HttpPost]
        public bool RunScheduleNow(BatchMaster batchMaster)
        {
            try
            {
                if (batchMaster == null)
                {
                    return false;
                }
                string tenantCode = Helper.CheckTenantCode(Request.Headers);

                var baseURL = Url.Content("~/");
                var outputLocation = AppDomain.CurrentDomain.BaseDirectory;
                var tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                if (tenantConfiguration != null && !string.IsNullOrEmpty(tenantConfiguration.OutputHTMLPath))
                {
                    baseURL = tenantConfiguration.OutputHTMLPath;
                    outputLocation = tenantConfiguration.OutputHTMLPath;
                }
                return this.scheduleManager.RunScheduleNowNew(batchMaster, baseURL, outputLocation, tenantConfiguration, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public bool RunScheduleNowWithMultipleBatches(string ids)
        {
            try
            {
                if (ids == null)
                {
                    return false;
                }
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                var baseURL = Url.Content("~/");
                var outputLocation = AppDomain.CurrentDomain.BaseDirectory;
                var tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                if (tenantConfiguration != null && !string.IsNullOrEmpty(tenantConfiguration.OutputHTMLPath))
                {
                    baseURL = tenantConfiguration.OutputHTMLPath;
                    outputLocation = tenantConfiguration.OutputHTMLPath;
                }
                var batchesId = ids.Split(',').ToList();
                var result = 0;
                foreach (var item in batchesId)
                {
                    var batchMaster = GetBatchMastersById(Convert.ToInt64(item))?.FirstOrDefault();
                    if (batchMaster != null)
                    {
                        batchMaster.Status = "Running";
                        var scheduleResult = this.scheduleManager.RunScheduleNowNew(batchMaster, baseURL, outputLocation, tenantConfiguration, tenantCode);
                        if (scheduleResult == true)
                        {
                            result += 0;
                        }
                        else
                        {
                            result += 1;
                        }
                    }
                }
                if (result == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Batch master

        /// <summary>
        /// Gets the batch master.
        /// </summary>
        /// <param name="scheduleIdentifier">The schedule identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public IList<BatchMaster> GetBatchMaster(long scheduleIdentifier)
        {
            IList<BatchMaster> batchMasters = new List<BatchMaster>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                batchMasters = this.scheduleManager.GetBatchMasters(scheduleIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return batchMasters;
        }

        [HttpPost]
        public IList<BatchMaster> GetBatchMastersById(long id)
        {
            IList<BatchMaster> batchMasters = new List<BatchMaster>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                batchMasters = this.scheduleManager.GetBatchMastersById(id, tenantCode);
            }
            catch (Exception exception)
            {
                throw;
            }

            return batchMasters;
        }


        [HttpPost]
        public IList<BatchMaster> GetBatchMastersByProductBatchName(string productBatchName)
        {
            IList<BatchMaster> batchMasters = new List<BatchMaster>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                batchMasters = this.scheduleManager.GetBatchMastersByProductBatchName(productBatchName, tenantCode);
            }
            catch (Exception exception)
            {
                throw;
            }

            return batchMasters;
        }

        [HttpPost]
        [Route("GetBatchMastersByLanguage")]
        public IList<BatchMaster> GetBatchMastersByLanguage(long scheduleIdentifier)
        {
            IList<BatchMaster> batchMasters = new List<BatchMaster>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                batchMasters = this.scheduleManager.GetBatchMastersByLanguage(scheduleIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return batchMasters;
        }

        /// <summary>
        /// This method helps to approve batch of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        [HttpPost]
        public bool ValidateApproveScheduleBatch(long BatchIdentifier)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.scheduleManager.ValidateApproveScheduleBatch(BatchIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to approve batch of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        [HttpPost]
        public bool ApproveScheduleBatch(long BatchIdentifier)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.scheduleManager.ApproveScheduleBatch(BatchIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public bool ValidateApproveScheduleBatches(string batchIdentifiers)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(batchIdentifiers))
                {
                    return false;
                }
                var batchesId = batchIdentifiers.Split(',').Select(x => Convert.ToInt64(x)).ToList();
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.scheduleManager.ValidateApproveScheduleBatches(batchesId, tenantCode);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// This method helps to clean batch and related data of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        [HttpPost]
        public bool CleanScheduleBatch(long BatchIdentifier)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.scheduleManager.CleanScheduleBatch(BatchIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public bool CleanScheduleBatches(string BatchIdentifier)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(BatchIdentifier))
                {
                    return false;
                }
                var batchesId = BatchIdentifier.Split(',').Select(x => Convert.ToInt64(x)).ToList();
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.scheduleManager.CleanScheduleBatches(batchesId, tenantCode);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// this method get visibility of delete button.
        /// </summary>
        /// <param name="scheduleId">the schedule Identifier.</param>
        /// <returns>True if visible, otherwise false</returns>
        [HttpPost]
        public bool GetDeleteButtonVisibility(long scheduleId)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.scheduleManager.GetDeleteButtonVisibility(scheduleId, tenantCode);
            }
            catch (Exception exception)
            {
                throw;
            }
            return result;
        }

        #endregion

        #endregion

        #region ScheduleRunHistory 

        /// <summary>
        /// This method helps to add schedules
        /// </summary>
        /// <param name="schedules">The schedules.</param>
        /// <returns>
        /// boolean value
        /// </returns>
        [HttpPost]
        public bool AddScheduleHistory(IList<ScheduleRunHistory> schedules)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.scheduleManager.AddScheduleRunHistorys(schedules, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to get schedules list based on the search parameters.
        /// </summary>
        /// <param name="scheduleSearchParameter">The schedule search parameter.</param>
        /// <returns>
        /// List of schedules
        /// </returns>
        [HttpPost]
        public IList<ScheduleRunHistory> GetScheduleRunHistories(ScheduleSearchParameter scheduleSearchParameter)
        {
            IList<ScheduleRunHistory> schedules = new List<ScheduleRunHistory>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                schedules = this.scheduleManager.GetScheduleRunHistorys(scheduleSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.scheduleManager.GetScheduleRunHistoryCount(scheduleSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return schedules;
        }

        #endregion


        #region Download

        /// <summary>
        /// Downloads the specified scheduel history identifier.
        /// </summary>
        /// <param name="scheduelHistoryIdentifier">The scheduel history identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
        [HttpGet]
        [Route("ScheduleHistory/Download")]
        public HttpResponseMessage Download(string scheduelHistoryIdentifier)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                string path = string.Empty;
                ScheduleRunHistory history = this.scheduleManager.GetScheduleRunHistorys(new ScheduleSearchParameter()
                {
                    ScheduleHistoryIdentifier = scheduelHistoryIdentifier,
                    SortParameter = new SortParameter() { SortColumn = ModelConstant.SORT_COLUMN }
                }, tenantCode).FirstOrDefault();

                string FileName = history.StatementFilePath.Split('\'').ToList().LastOrDefault();
                path = history.StatementFilePath;
                if (!File.Exists(path))
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            byte[] bytes = new byte[file.Length];
                            file.Read(bytes, 0, (int)file.Length);
                            ms.Write(bytes, 0, (int)file.Length);

                            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                            httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                            httpResponseMessage.Content.Headers.Add("x-filename", FileName);
                            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                            httpResponseMessage.Content.Headers.ContentDisposition.FileName = FileName;
                            httpResponseMessage.StatusCode = HttpStatusCode.OK;
                            return httpResponseMessage;
                        }
                    }
                }
                catch (IOException)
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
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
