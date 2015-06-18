USE [MembershipModel]
GO

/****** Object:  Table [dbo].[FinancialTransactions]    Script Date: 6/16/2015 11:52:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[FinancialTransactions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[paidMedium] [varchar](100) NOT NULL,
	[additionalTransactionalDetails] [varchar](300) NOT NULL,
	[amount] [float] NOT NULL,
	[dateOfTransaction] [datetime] NOT NULL,
	[ipAddress] [varchar](50) NOT NULL,
	[systemDetails] [varchar](300) NOT NULL,
	[createdBy] [int] NOT NULL,
	[createdAt] [datetime] NOT NULL,
	[lastModifiedBy] [int] NOT NULL,
	[lastModifiedAt] [datetime] NOT NULL,
	[isDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_FinancialTransactions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[FinancialTransactions] ADD  CONSTRAINT [DF_FinancialTransactions_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO

ALTER TABLE [dbo].[FinancialTransactions]  WITH CHECK ADD  CONSTRAINT [FK_FinancialTransactions_Users] FOREIGN KEY([createdBy])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[FinancialTransactions] CHECK CONSTRAINT [FK_FinancialTransactions_Users]
GO

ALTER TABLE [dbo].[FinancialTransactions]  WITH CHECK ADD  CONSTRAINT [FK_FinancialTransactions_Users1] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[FinancialTransactions] CHECK CONSTRAINT [FK_FinancialTransactions_Users1]
GO


