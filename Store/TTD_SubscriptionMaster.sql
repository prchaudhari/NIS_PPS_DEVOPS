﻿CREATE TABLE [NIS].[TTD_SubscriptionMaster]
(
	Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
	VendorName	NVARCHAR(100) NOT NULL,
	Subscription NVARCHAR(100) NOT NULL,	
	EmployeeID	NVARCHAR(100) NOT NULL,
	EmployeeName	NVARCHAR(100) NOT NULL,
	Email	NVARCHAR(100) NOT NULL,
	StartDate DateTime  NULL,	
	EndDate DateTime  NULL,
)
