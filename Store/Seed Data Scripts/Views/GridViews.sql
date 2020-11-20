﻿--For User Grid
IF OBJECT_ID (N'NIS.View_User', N'V') IS NOT NULL  
    DROP view NIS.View_User;  
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW NIS.View_User
AS
SELECT usr.*, r.Name 
FROM NIS.[User] usr 
INNER JOIN NIS.UserRoleMap urm ON usr.Id = urm.UserId 
INNER JOIN NIS.Role r ON urm.RoleId = r.Id
Go

--For Page Grid
IF OBJECT_ID (N'NIS.View_Page', N'V') IS NOT NULL  
    DROP view NIS.View_Page;  
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW NIS.View_Page
AS
SELECT p.*, pt.Name, usr1.FirstName+' '+usr1.LastName AS PublishedByName,  usr2.FirstName+' '+usr2.LastName AS PageOwnerName, ast.AssetLibraryId AS BackgroundImageAssetLibraryId
FROM NIS.Page p 
LEFT JOIN NIS.PageType pt ON p.PageTypeId = pt.Id
LEFT JOIN NIS.[User] usr1 ON p.PublishedBy = usr1.Id
LEFT JOIN NIS.[User] usr2 ON p.Owner = usr2.Id
LEFT JOIN NIS.[Asset] ast ON p.BackgroundImageAssetId = ast.Id
Go

--For Statement Definition grid
IF OBJECT_ID (N'NIS.View_StatementDefinition', N'V') IS NOT NULL  
    DROP view NIS.View_StatementDefinition;  
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW NIS.View_StatementDefinition
AS
SELECT s.*, usr1.FirstName+' '+usr1.LastName AS PublishedByName,  usr2.FirstName+' '+usr2.LastName AS OwnerName 
FROM NIS.Statement s 
LEFT JOIN NIS.[User] usr1 ON s.PublishedBy = usr1.Id
INNER JOIN NIS.[User] usr2 ON s.Owner = usr2.Id
Go

--For Schedule Management Grid
IF OBJECT_ID (N'NIS.View_Schedule', N'V') IS NOT NULL  
    DROP view NIS.View_Schedule;  
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW NIS.View_Schedule
AS
SELECT sc.*, s.Name As StatementName, (SELECT COUNT(Id) from NIS.BatchMaster where ScheduleId = sc.Id AND IsDataReady = 1 AND IsExecuted=1) AS ExecutedBatchCount
FROM NIS.Schedule sc 
INNER JOIN NIS.Statement s ON sc.StatementId = s.Id
Go

--For Source Data grid
IF OBJECT_ID (N'NIS.View_SourceData', N'V') IS NOT NULL  
    DROP view NIS.View_SourceData;  
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW NIS.View_SourceData
AS
SELECT ad.*, p.DisplayName AS PageName, w.DisplayName AS WidgetName, cm.FirstName+' '+cm.MiddleName+' '+cm.LastName AS CustomerName, pt.Name AS PageTypeName 
FROM NIS.AnalyticsData ad 
INNER JOIN NIS.CustomerMaster cm ON ad.CustomerId = cm.Id
LEFT JOIN NIS.Page p ON ad.PageId = p.Id
LEFT JOIN NIS.PageType pt ON p.PageTypeId = pt.Id
LEFT JOIN NIS.Widget w ON w.Id = ad.WidgetId
Go

--For Source Data grid
IF OBJECT_ID (N'NIS.View_ScheduleLog', N'V') IS NOT NULL  
    DROP view NIS.View_ScheduleLog;  
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW NIS.View_ScheduleLog
AS
SELECT sl.Id,sl.ScheduleName,CONVERT(VARCHAR(5),DATEDIFF(s, srh.StartDate, srh.EndDate)/3600)+' Hr '+
CONVERT(VARCHAR(5),DATEDIFF(s, srh.StartDate, srh.EndDate)%3600/60)+' Min '+
CONVERT(VARCHAR(5),(DATEDIFF(s, srh.StartDate, srh.EndDate)%60)) + ' Sec' AS ProcessingTime,
CONVERT(VARCHAR,(SELECT COUNT(Id) FROM nis.ScheduleLogDetail WHERE Status = 'Completed' AND ScheduleLogId = sl.Id))+ ' / '+CONVERT(VARCHAR, COUNT(sld.Id)) AS RecordProccessed,
sl.Status, sl.CreationDate AS ExecutionDate, sl.TenantCode
FROM nis.ScheduleLog sl
LEFT JOIN nis.ScheduleLogDetail sld ON sl.Id = sld.ScheduleLogId
LEFT JOIN nis.ScheduleRunHistory srh ON sl.Id = srh.ScheduleLogId
GROUP BY sl.ScheduleName, sl.CreationDate, sl.Status, srh.StartDate, srh.EndDate, sl.Id, sl.TenantCode
Go


/****** Object:  View [NIS].[View_MultiTenantUserAccessMap]    Script Date: 2020-09-29 14:33:01 ******/
IF OBJECT_ID (N'NIS.View_MultiTenantUserAccessMap', N'V') IS NOT NULL 
DROP VIEW [NIS].[View_MultiTenantUserAccessMap]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_MultiTenantUserAccessMap]
AS
SELECT tum.Id, tum.UserId, usr.FirstName+' '+usr.LastName AS UserName, usr.EmailAddress, 
tum.AssociatedTenantCode, t.TenantName AS AssociatedTenantName, t.TenantType AS AssociatedTenantType,
tum.OtherTenantCode, t1.TenantName AS OtherTenantName, t1.TenantType AS OtherTenantType,
r.Id AS RoleId, r.Name AS RoleName,tum.IsActive, tum.IsDeleted, tum.ParentTenantCode,
tum.LastUpdatedBy, usr1.FirstName+' '+usr1.LastName AS LastUpdatedByUserName, tum.LastUpdatedDate
FROM [NIS].[MultiTenantUserAccessMap] tum 
INNER JOIN [TenantManager].[Tenant] t ON tum.AssociatedTenantCode = t.TenantCode
INNER JOIN [TenantManager].[Tenant] t1 ON tum.OtherTenantCode = t1.TenantCode
INNER JOIN [NIS].[User] usr ON tum.UserId = usr.Id
INNER JOIN [NIS].[User] usr1 ON tum.LastUpdatedBy = usr1.Id
INNER JOIN [NIS].[Role] r ON tum.OtherTenantAccessRoleId = r.Id

GO



/****** Object:  View [NIS].[View_DynamicWidget]    Script Date: 2020-09-29 14:33:01 ******/
IF OBJECT_ID (N'NIS.View_DynamicWidget', N'V') IS NOT NULL 
DROP VIEW [NIS].View_DynamicWidget
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].View_DynamicWidget
AS

SELECT DISTINCT(s.Id) AS Id, MAX(s.WidgetName) AS WidgetName, MAX(s.WidgetType) AS WidgetType, s.EntityId,
MAX(s.Title) AS Title, MAX(s.ThemeType) AS ThemeType, MAX(s.ThemeCSS) AS ThemeCSS, MAX(s.WidgetSettings) AS WidgetSettings,
MAX(s.WidgetFilterSettings) AS WidgetFilterSettings, MAX(s.Status) AS Status, MAX(s.CreatedBy) AS CreatedBy, MAX(s.CreatedOn) AS CreatedOn,
s.LastUpdatedBy, s.PublishedBy, MAX(s.PublishedDate) AS PublishedDate, s.IsActive, s.IsDeleted,
MAX(s.TenantCode) AS TenantCode, s.CloneOfWidgetId, MAX(s.Version) As Version, 
MAX(s.PreviewData) AS PreviewData, usr1.FirstName+' '+usr1.LastName AS PublishedByName, usr2.FirstName+' '+usr2.LastName AS CreatedByName,
ent.Name AS EntityName, ent.APIPath, ent.RequestType, wpt.PageTypeId
FROM [NIS].[DynamicWidget] s 
LEFT JOIN [NIS].[User] usr1 ON s.PublishedBy = usr1.Id
INNER JOIN [NIS].[User] usr2 ON s.CreatedBy = usr2.Id
INNER JOIN [NIS].[TenantEntity] ent ON s.EntityId = ent.Id
INNER JOIN [NIS].[WidgetPageTypeMap] wpt ON wpt.WidgetId = s.Id AND wpt.IsDynamicWidget = 1
LEFT JOIN [NIS].[PageType] pt ON wpt.PageTypeId = pt.id
GROUP BY s.Id, s.EntityId, s.PublishedBy, s.CloneOfWidgetId, s.LastUpdatedBy, usr1.FirstName, usr1.LastName, 
usr2.FirstName, usr2.LastName, ent.Name, ent.APIPath, ent.RequestType, s.IsActive, s.IsDeleted, wpt.PageTypeId

GO


/****** Object:  View [NIS].[View_PageWidgetMap] Script Date: 2020-09-29 14:33:01 ******/
IF OBJECT_ID (N'NIS.View_PageWidgetMap', N'V') IS NOT NULL 
DROP VIEW [NIS].View_PageWidgetMap
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].View_PageWidgetMap
AS
SELECT pw.*, CASE WHEN pw.IsDynamicWidget = 0 THEN w.WidgetName ELSE dw.WidgetName END AS WidgetName
FROM [NIS].[PageWidgetMap] pw 
LEFT JOIN [NIS].[Widget] w ON pw.ReferenceWidgetid = w.id AND pw.IsDynamicWidget = 0
LEFT JOIN [NIS].[DynamicWidget] dw ON pw.ReferenceWidgetid = dw.id AND pw.IsDynamicWidget = 1

GO





