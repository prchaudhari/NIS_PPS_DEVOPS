﻿// <copyright file="ModelConstant.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------
namespace nIS
{
    /// <summary>
    /// Represents Model Constant Class
    /// </summary>
    public partial class ModelConstant
    {

        #region Common
        /// <summary>
        /// The common section key
        /// </summary>
        public const string COMMON_SECTION = "nIS";

        /// <summary>
        /// Invalid Pagin Parameter
        /// </summary>
        public const string INVALID_PAGING_PARAMETER = "InvalidPagingParameter";

        /// <summary>
        ///Invalid sort parameter
        /// </summary>
        public const string INVALID_SORT_PARAMETER = "InvalidSortParameter";

        /// <summary>
        /// Key for tenant code
        /// </summary>
        public const string TENANT_CODE_KEY = "TenantCode";

        /// <summary>
        /// Default identifier for the tenant
        /// </summary>
        public const string DEFAULT_TENANT_CODE = "00000000-0000-0000-0000-000000000000";

        /// <summary>
        /// The time zone key
        /// </summary>
        public const string TIME_ZONE_KEY = "Timezone";

        /// <summary>
        /// The off set key
        /// </summary>
        public const string OFF_SET_KEY = "Offset";

        /// <summary>
        /// The nVidYo connection string key
        /// </summary>
        public const string NIS_CONNECTION_STRING = "nISConnectionString";

        /// <summary>
        /// The key for configuration base URL
        /// </summary>
        public const string CONFIGURATON_BASE_URL = "ConfigurationBaseURL";

        /// <summary>
        /// The key for resource base URL
        /// </summary>
        public const string RESOURCE_BASE_URL = "ResourceBaseURL";

        /// <summary>
        /// The tenant base URL
        /// </summary>
        public const string TENANT_BASE_URL = "TenantManagerAPIURL";

        /// <summary>
        /// The entity base URL
        /// </summary>
        public const string ENTITY_BASE_URL = "EntityBaseURL";

        /// <summary>
        /// The tenant base URL
        /// </summary>
        public const string SORT_COLUMN = "Id";

        /// <summary>
        /// The tenant base URL
        /// </summary>
        public const string TENANT_ADMIN_ROLE = "Tenant Admin";

        /// <summary>
        /// The tenant base URL
        /// </summary>
        public const string GROUP_MANAGER_ROLE = "Group Manager";

        /// <summary>
        /// The tenant base URL
        /// </summary>
        public const string TENANT_PRIMARY_CONTACT = "Primary";


        /// <summary>
        /// The entity sort column
        /// </summary>
        public const string ENTITYSORTCOLUMN = "Id";

        /// <summary>
        /// The component code
        /// </summary>
        public const string COMPONENTCODE = "nIS";

        #endregion

        #region Operations

        /// <summary>
        /// Indicates add operation constant value.
        /// </summary>
        public const string ADD_OPERATION = "AddOperation";

        /// <summary>
        /// Indicates update operation constant value..
        /// </summary>
        public const string UPDATE_OPERATION = "UpdateOperation";

        /// <summary>
        /// Indicates delete operation constant value.
        /// </summary>
        public const string DELETE_OPERATION = "DeleteOperation";

        /// <summary>
        /// Indicates change activation status operation constant value.
        /// </summary>
        public const string CHANGE_ACTIVATION_STATUS = "ChangeActivationStatus";

        /// <summary>
        /// Separation by
        /// </summary>
        public const string SEPARATION_BY = ",";


        #endregion

        #region Widget

        /// <summary>
        /// Widget section
        /// </summary>
        public const string WIDGET_SECTION = "Widget";

        /// <summary>
        /// Invalid widget id
        /// </summary>
        public const string INVALID_WIDGET_ID = "InvalidWidgetId";

        /// <summary>
        /// Invalid widget name
        /// </summary>
        public const string INVALID_WIDGET_NAME = "InvalidWidgetName";

        #endregion

        #region Product Type

        /// <summary>
        /// Product type section
        /// </summary>
        public const string PRODUCTTYPE_SECTION = "ProductType";

        /// <summary>
        /// Invalid product type id
        /// </summary>
        public const string INVALID_PRODUCTTYPE_ID = "InvalidProductTypeId";

        /// <summary>
        /// Invalid product name
        /// </summary>
        public const string INVALID_PRODUCTTYPE_NAME = "InvalidProductTypeName";

        #endregion

        #region Page

        /// <summary>
        /// Page section
        /// </summary>
        public const string PAGE_SECTION = "Page";

        /// <summary>
        /// Invalid page id
        /// </summary>
        public const string INVALID_PAGE_ID = "InvalidPageId";

        /// <summary>
        /// Invalid page display name
        /// </summary>
        public const string INVALID_PAGE_DISPLAY_NAME = "InvalidageDisplayName";

        /// <summary>
        /// Invalid page(product) type id
        /// </summary>
        public const string INVALID_PAGE_TYPE_ID = "InvalidPageTypeId";

        #endregion

        #region User

        /// <summary>
        /// The user model section
        /// </summary>
        public const string USER_MODEL_SECTION = "UserModelSection";

        /// <summary>
        /// The invalid user name
        /// </summary>
        public const string INVALID_USER_NAME = "InvalidUserName";

        /// <summary>
        /// The invalid user name
        /// </summary>
        public const string INVALID_USER_CONTACT_NUMBER = "InvalidUserContactNumber";
        /// <summary>
        /// The invalid user name
        /// </summary>
        public const string INVALID_USER_EMAIL = "InvalidUserEmailAdress";
        /// <summary>
        /// The invalid user name
        /// </summary>
        public const string INVALID_USER_IMAGE = "InvalidUserImage";

        /// <summary>
        /// The invalid user name
        /// </summary>
        public const string INVALID_USER_ROLE = "InvalidUserRole";

        #endregion

        #region Role


        /// <summary>
        /// The role model section
        /// </summary>
        public const string ROLE_MODEL_SECTION = "RoleModel";

        public const string SUPER_ADMIN_ROLE = "Super Admin";

        /// <summary>
        /// The invalid role name
        /// </summary>
        public const string INVALID_ROLE_NAME = "InvalidRoleName";

        #endregion

        #region Schdeule

        /// <summary>
        /// The schedule model section
        /// </summary>
        public const string SCHEDULE_MODEL_SECTION = "ScheduleModel";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_NAME = "InvalidScheduleName";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_STARTDATE = "InvalidScheduleStartDate";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_ENDDATE = "InvalidScheduleEndDate";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_DAYOFMONTH = "InvalidScheduleDayOfMonth";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_HOUROFDAY = "InvalidScheduleHourOfDay";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_MINOFDAY = "InvalidScheduleMinuteOfDay";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_STATUS = "InvalidScheduleStatus";

        #endregion

        #region Role Privilege

        /// <summary>
        /// The role privilege model
        /// </summary>
        public const string ROLEPRIVILEGEMODELSECTION = "RolePrivilegeModel";

        /// <summary>
        /// Invalid entity name
        /// </summary>
        public const string INVALIDENTITYNAME = "InvalidEntityName";

        /// <summary>
        /// Invalid role privilege operations
        /// </summary>
        public const string INVALIDROLEPRIVILEGEOPERATIONS = "InvalidRolePrivilegeoOperations";

        #endregion

        #region Role Privilege Operation

        /// <summary>
        /// Role privilege operation model section
        /// </summary>
        public const string ROLEPRIVILEGEOPERATIONMODELSECTION = "RolePrivilegeOperationModel";

        /// <summary>
        /// Invalid operation name
        /// </summary>
        public const string INVALIDOPERATIONNAME = "InvalidOperationName";

        #endregion

        #region Mail contents

        /// <summary>
        /// Indicates mail subject when user will be added.
        /// </summary>
        public const string NEWLYADDEDUSERMAILSUBJECT = "NewlyAddedUserMailSubject";

        /// <summary>
        /// Indicates mail message when user will be added.
        /// </summary>
        public const string NEWLYADDEDUSERMAILMESSAGE = "NewlyAddedUserMailMessage";

        /// <summary>
        /// Indicates the link which will attach with mail for reset password.
        /// </summary>
        public const string CHANGEPASSWORDLINK = "ChangePasswordLink";

        /// <summary>
        /// Indicates mail subject when user is going to change password.
        /// </summary>
        public const string USERFORGOTPASSWORDSUBJECT = "UserForgotPasswordSubject";

        /// <summary>
        /// Indicates mail message when user is going to change password.
        /// </summary>
        public const string USERFORGOTPASSWORDMESSAGE = "UserForgotPasswordMessage";

        #endregion
    }
}
