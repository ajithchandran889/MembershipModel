USE [MembershipModel]
GO

/****** Object:  Table [dbo].[Groups]    Script Date: 6/16/2015 11:53:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Groups](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](150) NOT NULL,
	[description] [text] NOT NULL,
	[groupOwner] [int] NOT NULL,
	[createdBy] [int] NOT NULL,
	[createdAt] [datetime] NOT NULL,
	[lastModifiedBy] [int] NOT NULL,
	[lastmodifiedAt] [datetime] NOT NULL,
	[isDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Groups] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Groups] ADD  CONSTRAINT [DF_Groups_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO

ALTER TABLE [dbo].[Groups]  WITH CHECK ADD  CONSTRAINT [FK_Groups_Users] FOREIGN KEY([groupOwner])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[Groups] CHECK CONSTRAINT [FK_Groups_Users]
GO

ALTER TABLE [dbo].[Groups]  WITH CHECK ADD  CONSTRAINT [FK_Groups_Users1] FOREIGN KEY([createdBy])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[Groups] CHECK CONSTRAINT [FK_Groups_Users1]
GO

ALTER TABLE [dbo].[Groups]  WITH CHECK ADD  CONSTRAINT [FK_Groups_Users2] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[Groups] CHECK CONSTRAINT [FK_Groups_Users2]
GO


