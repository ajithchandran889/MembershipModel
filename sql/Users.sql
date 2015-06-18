USE [MembershipModel]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 6/16/2015 11:56:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [varchar](50) NOT NULL,
	[email] [varchar](300) NOT NULL,
	[password] [text] NOT NULL,
	[name] [varchar](150) NULL,
	[companyName] [varchar](150) NULL,
	[address] [text] NOT NULL,
	[contactInfo] [varchar](300) NOT NULL,
	[isOwner] [bit] NOT NULL,
	[status] [bit] NOT NULL,
	[createdBy] [int] NULL,
	[createdAt] [datetime] NULL,
	[lastModifiedBy] [int] NULL,
	[lastModifiedAt] [datetime] NULL,
	[ipAddress] [varchar](50) NULL,
	[forgotPasswordRequestAt] [datetime] NULL,
	[forgotPasswordToken] [varchar](50) NULL,
	[changeEmailRequestAt] [datetime] NULL,
	[changeEmailToken] [varchar](50) NULL,
	[isDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[userId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_isOwner]  DEFAULT ((0)) FOR [isOwner]
GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO


