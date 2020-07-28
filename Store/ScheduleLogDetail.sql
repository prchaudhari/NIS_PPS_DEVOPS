﻿CREATE TABLE [NIS].[ScheduleLogDetail]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ScheduleLogId] BIGINT NOT NULL,
	[ScheduleId] BIGINT NOT NULL,
	[NumberOfRetry] BIGINT NULL,
	[Status] NVARCHAR(20) NULL,
	[LogMessage] NVARCHAR(MAX) NULL,
	[CreationDate] DATETIME NOT NULL,
)
