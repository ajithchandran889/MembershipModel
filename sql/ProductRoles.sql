USE [MembershipModel]
GO

/****** Object:  Table [dbo].[ProductRoles]    Script Date: 6/16/2015 11:54:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ProductRoles](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[productId] [int] NOT NULL,
	[roleName] [varchar](100) NOT NULL,
	[roleDescription] [text] NOT NULL,
	[createdBy] [int] NOT NULL,
	[createdAt] [datetime] NOT NULL,
	[lastModifiedBy] [int] NOT NULL,
	[lastModifedAt] [datetime] NOT NULL,
	[isDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ProductRoles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ProductRoles] ADD  CONSTRAINT [DF_ProductRoles_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO

ALTER TABLE [dbo].[ProductRoles]  WITH CHECK ADD  CONSTRAINT [FK_ProductRoles_Products] FOREIGN KEY([productId])
REFERENCES [dbo].[Products] ([id])
GO

ALTER TABLE [dbo].[ProductRoles] CHECK CONSTRAINT [FK_ProductRoles_Products]
GO

ALTER TABLE [dbo].[ProductRoles]  WITH CHECK ADD  CONSTRAINT [FK_ProductRoles_Users] FOREIGN KEY([createdBy])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[ProductRoles] CHECK CONSTRAINT [FK_ProductRoles_Users]
GO

ALTER TABLE [dbo].[ProductRoles]  WITH CHECK ADD  CONSTRAINT [FK_ProductRoles_Users1] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[ProductRoles] CHECK CONSTRAINT [FK_ProductRoles_Users1]
GO


