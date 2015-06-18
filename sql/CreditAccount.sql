USE [MembershipModel]
GO

/****** Object:  Table [dbo].[CreditAccount]    Script Date: 6/16/2015 11:51:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CreditAccount](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[pcanId] [int] NOT NULL,
	[usersSubscribed] [varchar](300) NOT NULL,
	[financialTransactionId] [int] NOT NULL,
	[productId] [int] NOT NULL,
	[productSubscriptionModel] [varchar](100) NOT NULL,
	[fromDate] [datetime] NOT NULL,
	[toDate] [datetime] NOT NULL,
	[createdBy] [int] NOT NULL,
	[createdDate] [datetime] NOT NULL,
	[lastModifiedBy] [int] NOT NULL,
	[lastModifiedAt] [datetime] NOT NULL,
	[isDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CreditAccount] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CreditAccount] ADD  CONSTRAINT [DF_CreditAccount_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO

ALTER TABLE [dbo].[CreditAccount]  WITH CHECK ADD  CONSTRAINT [FK_CreditAccount_FinancialTransactions] FOREIGN KEY([financialTransactionId])
REFERENCES [dbo].[FinancialTransactions] ([id])
GO

ALTER TABLE [dbo].[CreditAccount] CHECK CONSTRAINT [FK_CreditAccount_FinancialTransactions]
GO

ALTER TABLE [dbo].[CreditAccount]  WITH CHECK ADD  CONSTRAINT [FK_CreditAccount_Products] FOREIGN KEY([productId])
REFERENCES [dbo].[Products] ([id])
GO

ALTER TABLE [dbo].[CreditAccount] CHECK CONSTRAINT [FK_CreditAccount_Products]
GO

ALTER TABLE [dbo].[CreditAccount]  WITH CHECK ADD  CONSTRAINT [FK_CreditAccount_Users] FOREIGN KEY([createdBy])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[CreditAccount] CHECK CONSTRAINT [FK_CreditAccount_Users]
GO

ALTER TABLE [dbo].[CreditAccount]  WITH CHECK ADD  CONSTRAINT [FK_CreditAccount_Users1] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[CreditAccount] CHECK CONSTRAINT [FK_CreditAccount_Users1]
GO


