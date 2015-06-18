USE [MembershipModel]
GO

/****** Object:  Table [dbo].[GroupProducts]    Script Date: 6/16/2015 11:53:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GroupProducts](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[groupId] [int] NOT NULL,
	[productId] [int] NOT NULL,
	[createdBy] [int] NOT NULL,
	[createdAt] [datetime] NOT NULL,
	[lastModifiedBy] [int] NOT NULL,
	[lastModifiedAt] [datetime] NOT NULL,
	[isDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_GroupProducts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GroupProducts] ADD  CONSTRAINT [DF_GroupProducts_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO

ALTER TABLE [dbo].[GroupProducts]  WITH CHECK ADD  CONSTRAINT [FK_GroupProducts_Groups] FOREIGN KEY([groupId])
REFERENCES [dbo].[Groups] ([id])
GO

ALTER TABLE [dbo].[GroupProducts] CHECK CONSTRAINT [FK_GroupProducts_Groups]
GO

ALTER TABLE [dbo].[GroupProducts]  WITH CHECK ADD  CONSTRAINT [FK_GroupProducts_Products] FOREIGN KEY([productId])
REFERENCES [dbo].[Products] ([id])
GO

ALTER TABLE [dbo].[GroupProducts] CHECK CONSTRAINT [FK_GroupProducts_Products]
GO

ALTER TABLE [dbo].[GroupProducts]  WITH CHECK ADD  CONSTRAINT [FK_GroupProducts_Users] FOREIGN KEY([createdBy])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[GroupProducts] CHECK CONSTRAINT [FK_GroupProducts_Users]
GO

ALTER TABLE [dbo].[GroupProducts]  WITH CHECK ADD  CONSTRAINT [FK_GroupProducts_Users1] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[GroupProducts] CHECK CONSTRAINT [FK_GroupProducts_Users1]
GO


