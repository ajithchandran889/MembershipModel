USE [MembershipModel]
GO

/****** Object:  Table [dbo].[LogHistory]    Script Date: 6/16/2015 11:54:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[LogHistory](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[actionDescription] [text] NOT NULL,
	[ipAddress] [varchar](50) NOT NULL,
	[createdAt] [datetime] NOT NULL,
 CONSTRAINT [PK_LogHistory] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[LogHistory]  WITH CHECK ADD  CONSTRAINT [FK_LogHistory_Users] FOREIGN KEY([userId])
REFERENCES [dbo].[Users] ([id])
GO

ALTER TABLE [dbo].[LogHistory] CHECK CONSTRAINT [FK_LogHistory_Users]
GO


