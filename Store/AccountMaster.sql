﻿CREATE TABLE [NIS].[AccountMaster](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[AccountNumber] [nvarchar](50) NOT NULL,
	[AccountType] [nvarchar](50) NOT NULL,
	[Currency] [nvarchar](50) NOT NULL,
	[Balance] [decimal](8, 2) NOT NULL,
	[TotalDeposit] [decimal](8, 2) NOT NULL,
	[TotalSpend] [decimal](8, 2) NULL,
	[ProfitEarned] [decimal](8, 2) NULL,
	[Indicator] [nvarchar](50) NULL,
	[FeesPaid] [decimal](8, 2) NULL,
	[GrandTotal] [decimal](8, 2) NULL,
 CONSTRAINT [PK_AccountMaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
