USE [MembershipModel]
GO

/****** Object:  Table [dbo].[GroupMembers]    Script Date: 6/16/2015 11:52:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GroupMembers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[groupId] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[isAdmin] [bit] NOT NULL,
	[createdBy] [int] NOT NULL,
	[createdAt] [datetime] NOT NULL,
	[lastModifiedBy] [int] NOT NULL,
	[lastModifiedAt] [datetime] NOT NULL,
	[isDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_GroupMembers] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GroupMembers] ADD  CONSTRAINT [DF_GroupMembers_isAdmin]  DEFAULT ((0)) FOR [isAdmin]
GO

ALTER TABLE [dbo].[GroupMembers] ADD  CONSTRAINT [DF_GroupMembers_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO

ALTER TABLE [dbo].[GroupMembers]  WITH CHECK ADD  CONSTRAINT [FK_GroupMembers_Groups] FOREIGN KEY([groupId])
REFERENCES [dbo].[Groups] ([id])
GO

ALTER TABLE [dbo].[GroupMembers] CHECK CONSTRAINT [FK_GroupMembers_Groups]
GO

ALTER TABLE [dbo].[GroupMembers]  WITH CHECK ADD  CONSTRAINT [FK_GroupMembers_Users] FOREIGN KEY([userId])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[GroupMembers] CHECK CONSTRAINT [FK_GroupMembers_Users]
GO

ALTER TABLE [dbo].[GroupMembers]  WITH CHECK ADD  CONSTRAINT [FK_GroupMembers_Users1] FOREIGN KEY([createdBy])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[GroupMembers] CHECK CONSTRAINT [FK_GroupMembers_Users1]
GO

ALTER TABLE [dbo].[GroupMembers]  WITH CHECK ADD  CONSTRAINT [FK_GroupMembers_Users2] FOREIGN KEY([lastModifiedBy])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[GroupMembers] CHECK CONSTRAINT [FK_GroupMembers_Users2]
GO


