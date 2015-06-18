USE [MembershipModel]
GO

/****** Object:  Table [dbo].[Registration]    Script Date: 6/16/2015 11:55:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Registration](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[email] [varchar](300) NOT NULL,
	[password] [text] NOT NULL,
	[token] [varchar](50) NOT NULL,
	[ipAddress] [varchar](50) NULL,
	[systemDetails] [varchar](200) NULL,
	[createdAt] [datetime] NULL,
	[lastModifiedAt] [datetime] NULL,
	[isDeleted] [bit] NULL,
 CONSTRAINT [PK_Registration] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Registration] ADD  CONSTRAINT [DF_Registration_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO


