--****************************************************************************************
-- Created 04/01/2014 - Sergey Ostrerov. Version v. 01.00.000.0001
-- Production database PEMSRBAC_AU_PRO script: all database objects.
--****************************************************************************************
USE [PEMSRBAC_AU_PRO]
GO
/****** Object:  Role [NetSqlAzMan_Readers]    Script Date: 04/01/2014 22:13:07 ******/
CREATE ROLE [NetSqlAzMan_Readers] AUTHORIZATION [dbo]
GO
/****** Object:  Role [NetSqlAzMan_Users]    Script Date: 04/01/2014 22:13:08 ******/
CREATE ROLE [NetSqlAzMan_Users] AUTHORIZATION [dbo]
GO
/****** Object:  Role [NetSqlAzMan_Managers]    Script Date: 04/01/2014 22:13:08 ******/
CREATE ROLE [NetSqlAzMan_Managers] AUTHORIZATION [dbo]
GO
/****** Object:  Role [NetSqlAzMan_Administrators]    Script Date: 04/01/2014 22:13:09 ******/
CREATE ROLE [NetSqlAzMan_Administrators] AUTHORIZATION [dbo]
GO
/****** Object:  User [dbschema_user]    Script Date: 04/01/2014 22:13:07 ******/
CREATE USER [dbschema_user] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\ssomanchi]    Script Date: 04/01/2014 22:13:07 ******/
CREATE USER [CM\ssomanchi] FOR LOGIN [CM\ssomanchi] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\sostrerov]    Script Date: 04/01/2014 22:13:07 ******/
CREATE USER [CM\sostrerov] FOR LOGIN [CM\sostrerov] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\sa_sqlaccount]    Script Date: 04/01/2014 22:13:07 ******/
CREATE USER [CM\sa_sqlaccount] FOR LOGIN [CM\sa_sqlaccount] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\rpauli]    Script Date: 04/01/2014 22:13:07 ******/
CREATE USER [CM\rpauli] FOR LOGIN [CM\rpauli] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\rhoward2]    Script Date: 04/01/2014 22:13:07 ******/
CREATE USER [CM\rhoward2] FOR LOGIN [CM\rhoward2] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\rgallardo]    Script Date: 04/01/2014 22:13:07 ******/
CREATE USER [CM\rgallardo] FOR LOGIN [CM\rgallardo] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\pbouch]    Script Date: 04/01/2014 22:13:07 ******/
CREATE USER [CM\pbouch] FOR LOGIN [CM\pbouch] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\mneupane]    Script Date: 04/01/2014 22:13:07 ******/
CREATE USER [CM\mneupane] FOR LOGIN [CM\mneupane] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\amyint]    Script Date: 04/01/2014 22:13:07 ******/
CREATE USER [CM\amyint] FOR LOGIN [CM\amyint] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [BUILTIN\Administrators]    Script Date: 04/01/2014 22:13:07 ******/
CREATE USER [BUILTIN\Administrators] FOR LOGIN [BUILTIN\Administrators]
GO
/****** Object:  User [au_duncan]    Script Date: 04/01/2014 22:13:07 ******/
CREATE USER [au_duncan] FOR LOGIN [au_duncan] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Schema [NetSqlAzMan_Users]    Script Date: 04/01/2014 22:13:09 ******/
CREATE SCHEMA [NetSqlAzMan_Users] AUTHORIZATION [dbo]
GO
/****** Object:  Schema [NetSqlAzMan_Readers]    Script Date: 04/01/2014 22:13:10 ******/
CREATE SCHEMA [NetSqlAzMan_Readers] AUTHORIZATION [dbo]
GO
/****** Object:  Schema [NetSqlAzMan_Managers]    Script Date: 04/01/2014 22:13:10 ******/
CREATE SCHEMA [NetSqlAzMan_Managers] AUTHORIZATION [dbo]
GO
/****** Object:  Schema [NetSqlAzMan_Administrators]    Script Date: 04/01/2014 22:13:11 ******/
CREATE SCHEMA [NetSqlAzMan_Administrators] AUTHORIZATION [dbo]
GO
/****** Object:  Schema [BUILTIN\Administrators]    Script Date: 04/01/2014 22:13:11 ******/
CREATE SCHEMA [BUILTIN\Administrators] AUTHORIZATION [dbo]
GO
/****** Object:  Table [dbo].[HelpMap]    Script Date: 04/01/2014 22:13:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HelpMap](
	[HelpMapId] [int] IDENTITY(1,1) NOT NULL,
	[CultureCode] [nvarchar](50) NOT NULL,
	[Area] [nvarchar](255) NULL,
	[Controller] [nvarchar](255) NOT NULL,
	[Action] [nvarchar](255) NOT NULL,
	[Server] [nvarchar](255) NULL,
	[File] [nvarchar](255) NOT NULL,
	[Topic] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_HelpMap] PRIMARY KEY CLUSTERED 
(
	[HelpMapId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MembershipEventTypes]    Script Date: 04/01/2014 22:13:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MembershipEventTypes](
	[EventTypeId] [int] NOT NULL,
	[EventName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_MembershipEventTypes] PRIMARY KEY CLUSTERED 
(
	[EventTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoginAttemptHistory]    Script Date: 04/01/2014 22:13:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoginAttemptHistory](
	[LoginAttemptHistoryId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](128) NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[IpAddress] [nvarchar](128) NULL,
	[AccessDate] [smalldatetime] NULL,
 CONSTRAINT [PK_LoginAttemptHistory] PRIMARY KEY CLUSTERED 
(
	[LoginAttemptHistoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LocaleResourcesCustom]    Script Date: 04/01/2014 22:13:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocaleResourcesCustom](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](255) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_LocaleResourcesCustom] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LocaleResources]    Script Date: 04/01/2014 22:13:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocaleResources](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](255) NULL,
	[Name] [nvarchar](128) NULL,
	[Value] [nvarchar](max) NULL,
	[CultureCode] [nvarchar](50) NULL,
 CONSTRAINT [PK_LocaleResources] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerGridTemplate]    Script Date: 04/01/2014 22:13:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerGridTemplate](
	[CustomerGridTemplateId] [int] IDENTITY(1,1) NOT NULL,
	[Controller] [nvarchar](128) NOT NULL,
	[Action] [nvarchar](128) NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_CustomerGridTemplate] PRIMARY KEY CLUSTERED 
(
	[CustomerGridTemplateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerReportsCategory]    Script Date: 04/01/2014 22:13:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomerReportsCategory](
	[CustomerReportsCategoryId] [int] NOT NULL,
	[Name] [varchar](32) NOT NULL,
 CONSTRAINT [PK_CustomerReportsCategory] PRIMARY KEY CLUSTERED 
(
	[CustomerReportsCategoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomerSettingType]    Script Date: 04/01/2014 22:13:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerSettingType](
	[CustomerSettingTypeId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerSettingTypeName] [nvarchar](50) NULL,
	[SettingTypeId] [int] NOT NULL,
	[IsRequired] [bit] NOT NULL,
	[DefaultValue] [nvarchar](255) NULL,
	[InternalValue] [nvarchar](255) NULL,
 CONSTRAINT [PK_CustomerSettingType] PRIMARY KEY CLUSTERED 
(
	[CustomerSettingTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ErrorMessages]    Script Date: 04/01/2014 22:13:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorMessages](
	[ErrorMessageID] [int] IDENTITY(1,1) NOT NULL,
	[ErrorCode] [nvarchar](10) NOT NULL,
	[ErrorMessage] [nvarchar](250) NOT NULL,
	[Locale] [nvarchar](6) NOT NULL,
	[Active] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
 CONSTRAINT [PK_ErrorMessages] PRIMARY KEY CLUSTERED 
(
	[ErrorMessageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerType]    Script Date: 04/01/2014 22:13:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerType](
	[CustomerTypeId] [int] NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_CustomerType] PRIMARY KEY CLUSTERED 
(
	[CustomerTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[netsqlazman_BizRulesTable]    Script Date: 04/01/2014 22:13:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[netsqlazman_BizRulesTable](
	[BizRuleId] [int] IDENTITY(1,1) NOT NULL,
	[BizRuleSource] [text] NOT NULL,
	[BizRuleLanguage] [tinyint] NOT NULL,
	[CompiledAssembly] [image] NOT NULL,
 CONSTRAINT [PK_BizRules] PRIMARY KEY CLUSTERED 
(
	[BizRuleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_DBVersion]    Script Date: 04/01/2014 22:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_DBVersion] ()  
RETURNS nvarchar(200) AS  
BEGIN 
	return '3.6.0.x'
END
GO
/****** Object:  Table [dbo].[netsqlazman_ItemsHierarchyTable]    Script Date: 04/01/2014 22:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[netsqlazman_ItemsHierarchyTable](
	[ItemId] [int] NOT NULL,
	[MemberOfItemId] [int] NOT NULL,
 CONSTRAINT [PK_ItemsHierarchy] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC,
	[MemberOfItemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_MergeAuthorizations]    Script Date: 04/01/2014 22:13:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_MergeAuthorizations](@AUTH1 tinyint, @AUTH2 tinyint)
RETURNS tinyint
AS
BEGIN
-- 0 Neutral 1 Allow 2 Deny 3 AllowWithDelegation
DECLARE @RESULT tinyint
IF @AUTH1 IS NULL 
BEGIN
	SET @RESULT = @AUTH2
END
ELSE 
IF @AUTH2 IS NULL 
BEGIN
SET @RESULT = @AUTH1
END
ELSE
BEGIN
	IF @AUTH1 = 2 SET @AUTH1 = 4 -- DENY WINS
	ELSE
	IF @AUTH2 = 2 SET @AUTH2 = 4 -- DENY WINS
	IF @AUTH1 >= @AUTH2
                	SET @RESULT = @AUTH1
	ELSE
	IF @AUTH1 < @AUTH2
		SET @RESULT = @AUTH2
	IF @RESULT = 4 SET @RESULT = 2
END
RETURN @RESULT
END
GO
/****** Object:  Table [dbo].[netsqlazman_LogTable]    Script Date: 04/01/2014 22:13:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[netsqlazman_LogTable](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[LogDateTime] [datetime] NOT NULL,
	[WindowsIdentity] [nvarchar](255) NOT NULL,
	[SqlIdentity] [nvarchar](128) NULL,
	[MachineName] [nvarchar](255) NOT NULL,
	[InstanceGuid] [uniqueidentifier] NOT NULL,
	[TransactionGuid] [uniqueidentifier] NULL,
	[OperationCounter] [int] NOT NULL,
	[ENSType] [nvarchar](255) NOT NULL,
	[ENSDescription] [nvarchar](4000) NOT NULL,
	[LogType] [char](1) NOT NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY NONCLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[netsqlazman_Settings]    Script Date: 04/01/2014 22:13:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[netsqlazman_Settings](
	[SettingName] [nvarchar](255) NOT NULL,
	[SettingValue] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED 
(
	[SettingName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_IAmAdmin]    Script Date: 04/01/2014 22:13:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_IAmAdmin] ()  
RETURNS bit AS  
BEGIN 
DECLARE @result bit
IF IS_MEMBER('db_owner')=1 OR IS_MEMBER('NetSqlAzMan_Administrators')=1
	SET @result = 1
ELSE
	SET @result = 0
RETURN @result
END
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_helplogins]    Script Date: 04/01/2014 22:19:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_helplogins](@rolename nvarchar(128))
AS

CREATE TABLE #temptable (
	[DBRole] sysname NOT NULL ,
	[MemberName] sysname NOT NULL ,
	[MemberSid] varbinary(85) NULL
	)

IF @rolename = 'NetSqlAzMan_Managers'
BEGIN
	INSERT INTO #temptable EXEC sp_helprolemember 'NetSqlAzMan_Managers'
END

IF @rolename = 'NetSqlAzMan_Users' 
BEGIN
	INSERT INTO #temptable EXEC sp_helprolemember 'NetSqlAzMan_Managers'
	INSERT INTO #temptable EXEC sp_helprolemember 'NetSqlAzMan_Users'
END

IF @rolename = 'NetSqlAzMan_Readers' 
BEGIN
	INSERT INTO #temptable EXEC sp_helprolemember 'NetSqlAzMan_Managers'
	INSERT INTO #temptable EXEC sp_helprolemember 'NetSqlAzMan_Users'
	INSERT INTO #temptable EXEC sp_helprolemember 'NetSqlAzMan_Readers'
END

SELECT DISTINCT SUSER_SNAME(MemberSid) SqlUserOrRole, CASE MemberSid WHEN NULL THEN 1 ELSE 0 END AS IsSqlRole
FROM #temptable
WHERE MemberName NOT IN ('NetSqlAzMan_Administrators', 'NetSqlAzMan_Managers', 'NetSqlAzMan_Users', 'NetSqlAzMan_Readers')
AND SUSER_SNAME(MemberSid) IS NOT NULL
ORDER BY SUSER_SNAME(MemberSid)

DROP TABLE #temptable
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ExecuteLDAPQuery]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ExecuteLDAPQuery](@LDAPPATH NVARCHAR(4000), @LDAPQUERY NVARCHAR(4000), @members_cur CURSOR VARYING OUTPUT)
AS
-- REMEMBER !!!
-- BEFORE executing ExecuteLDAPQuery procedure ... a Linked Server named 'ADSI' must be added:
-- --sp_addlinkedserver 'ADSI', 'Active Directory Service Interfaces', 'ADSDSOObject', 'adsdatasource'
CREATE TABLE #temp (objectSid VARBINARY(85))
IF @LDAPQUERY IS NULL OR RTRIM(LTRIM(@LDAPQUERY))='' OR @LDAPPATH IS NULL OR RTRIM(LTRIM(@LDAPPATH))=''
BEGIN
SET @members_cur = CURSOR STATIC FORWARD_ONLY FOR SELECT * FROM #temp
OPEN @members_cur
DROP TABLE #temp
RETURN
END
SET @LDAPPATH = REPLACE(@LDAPPATH, N'''', N'''''')
SET @LDAPQUERY = REPLACE(@LDAPQUERY, N'''', N'''''')
DECLARE @QUERY nvarchar(4000)
DECLARE @LDAPROOTDSEPART nvarchar(4000)
DECLARE @LDAPQUERYPART nvarchar(4000)
SET @LDAPROOTDSEPART = LTRIM(@LDAPQUERY)
IF CHARINDEX('[RootDSE:', @LDAPROOTDSEPART)=1
BEGIN
	SET @LDAPROOTDSEPART = SUBSTRING(@LDAPROOTDSEPART, 10, CHARINDEX(']', @LDAPROOTDSEPART)-10)
	SET @LDAPQUERYPART = SUBSTRING(@LDAPQUERY, CHARINDEX( ']', @LDAPQUERY)+1, 4000)
END
ELSE
BEGIN
	SET @LDAPROOTDSEPART = @LDAPPATH
	SET @LDAPQUERYPART = @LDAPQUERY
END
SET @QUERY = CHAR(39) + '<' + 'LDAP://'+ @LDAPROOTDSEPART + '>;(&(!(objectClass=computer))(&(|(objectClass=user)(objectClass=group)))' + @LDAPQUERYPART + ');objectSid;subtree' + CHAR(39) 
DECLARE @OPENQUERY nvarchar(4000)
SET @OPENQUERY = 'SELECT * FROM OPENQUERY(ADSI, ' + @QUERY + ')'
INSERT INTO #temp EXEC (@OPENQUERY)
SET @members_cur = CURSOR STATIC FORWARD_ONLY FOR SELECT * FROM #temp
OPEN @members_cur
DROP TABLE #temp
GO
/****** Object:  Table [dbo].[netsqlazman_StoresTable]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[netsqlazman_StoresTable](
	[StoreId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NOT NULL,
 CONSTRAINT [PK_Stores] PRIMARY KEY CLUSTERED 
(
	[StoreId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AccessLog]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccessLog](
	[AccessLogId] [int] IDENTITY(1,1) NOT NULL,
	[Area] [nvarchar](255) NULL,
	[City] [nvarchar](255) NULL,
	[Controller] [nvarchar](255) NULL,
	[Action] [nvarchar](255) NULL,
	[SessionID] [nvarchar](50) NULL,
	[UserId] [int] NULL,
	[AccessRights] [int] NOT NULL,
	[AccessDuration] [float] NULL,
	[AccessOverhead] [float] NULL,
	[AccessDate] [datetime] NOT NULL,
 CONSTRAINT [PK_AccessLog] PRIMARY KEY CLUSTERED 
(
	[AccessLogId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TimeZoneSet]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TimeZoneSet](
	[Name] [varchar](50) NOT NULL,
	[Hours] [int] NOT NULL,
	[Minutes] [int] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TimeZones]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TimeZones](
	[TimeZoneID] [int] NOT NULL,
	[LocalTimeUTCDifference] [int] NOT NULL,
	[DaylightSavingAdjustment] [int] NOT NULL,
	[UTCSummerTimeStart] [smalldatetime] NOT NULL,
	[UTCSummerTimeEnd] [smalldatetime] NOT NULL,
	[TimeZoneName] [varchar](50) NULL,
 CONSTRAINT [PK_TimeZones] PRIMARY KEY CLUSTERED 
(
	[TimeZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SystemInformation]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemInformation](
	[SystemInformationId] [int] IDENTITY(1,1) NOT NULL,
	[TableName] [nvarchar](255) NOT NULL,
	[PrimaryKey] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModifiedBy] [int] NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_SystemInformation] PRIMARY KEY CLUSTERED 
(
	[SystemInformationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResourceEventTypes]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResourceEventTypes](
	[EventTypeId] [int] NOT NULL,
	[EventName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_ResourceEventTypes] PRIMARY KEY CLUSTERED 
(
	[EventTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [db_owner].[Users]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [db_owner].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](56) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserProfile]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfile](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](56) NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[MiddleName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[Phone] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[RequirePasswordReset] [bit] NULL,
 CONSTRAINT [PK__UserProf__1788CC4C59C55456] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ__UserProf__C9F284565CA1C101] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[News]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[News](
	[ContentId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[ExpirationDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[Description] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[Display] [bit] NULL,
 CONSTRAINT [PK_News] PRIMARY KEY CLUSTERED 
(
	[ContentId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Parts]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Parts](
	[PartId] [bigint] IDENTITY(1,1) NOT NULL,
	[PartName] [varchar](500) NULL,
	[MeterGroup] [int] NOT NULL,
	[Category] [int] NOT NULL,
	[PartDesc] [varchar](500) NULL,
	[CostInCents] [int] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Parts] PRIMARY KEY CLUSTERED 
(
	[PartId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WorkOrderStatus]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkOrderStatus](
	[WorkOrderStatusId] [int] NOT NULL,
	[WorkOrderStatusDesc] [varchar](500) NOT NULL,
 CONSTRAINT [PK_WorkOrderStatus] PRIMARY KEY CLUSTERED 
(
	[WorkOrderStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Version]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Version](
	[DBVersionNumber] [varchar](100) NOT NULL,
	[CHANGE_DATE] [datetime] NOT NULL,
	[ChangeLog] [varchar](200) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[webpages_Roles]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_OAuthMembership]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_OAuthMembership](
	[Provider] [nvarchar](30) NOT NULL,
	[ProviderUserId] [nvarchar](100) NOT NULL,
	[UserId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Provider] ASC,
	[ProviderUserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_Membership]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_Membership](
	[UserId] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ConfirmationToken] [nvarchar](128) NULL,
	[IsConfirmed] [bit] NULL,
	[LastPasswordFailureDate] [datetime] NULL,
	[PasswordFailuresSinceLastSuccess] [int] NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[PasswordChangedDate] [datetime] NULL,
	[PasswordSalt] [nvarchar](128) NOT NULL,
	[PasswordVerificationToken] [nvarchar](128) NULL,
	[PasswordVerificationTokenExpirationDate] [datetime] NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserSettings]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSettings](
	[UserSettingsId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[SettingName] [nvarchar](64) NOT NULL,
	[SettingValue] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_UserSettings] PRIMARY KEY CLUSTERED 
(
	[UserSettingsId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkOrders]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkOrders](
	[WorkOrderId] [bigint] IDENTITY(1,1) NOT NULL,
	[ReportingUserId] [int] NULL,
	[MeterGroup] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[Location] [varchar](500) NULL,
	[ParkingSpaceId] [bigint] NULL,
	[Notes] [varchar](2000) NULL,
	[ZoneId] [int] NULL,
	[HighestSeverity] [int] NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
	[SLADue] [datetime] NOT NULL,
	[WorkOrderStatusId] [int] NOT NULL,
	[TechnicianId] [int] NULL,
	[AssignedDate] [datetime] NULL,
	[CompletedDate] [datetime] NULL,
	[CreatedById] [int] NULL,
	[CrossStreet] [varchar](2000) NULL,
	[ResolutionCode] [int] NULL,
	[ResolutionDesc] [varchar](2000) NULL,
	[AssignmentState] [int] NULL,
	[Mechanism] [int] NULL,
 CONSTRAINT [PK_WorkOrders] PRIMARY KEY CLUSTERED 
(
	[WorkOrderId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[webpages_UsersInRoles]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_UsersInRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[netsqlazman_StorePermissionsTable]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[netsqlazman_StorePermissionsTable](
	[StorePermissionId] [int] IDENTITY(1,1) NOT NULL,
	[StoreId] [int] NOT NULL,
	[SqlUserOrRole] [nvarchar](128) NOT NULL,
	[IsSqlRole] [bit] NOT NULL,
	[NetSqlAzManFixedServerRole] [tinyint] NOT NULL,
 CONSTRAINT [PK_StorePermissions] PRIMARY KEY CLUSTERED 
(
	[StorePermissionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NewsContent]    Script Date: 04/01/2014 22:19:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NewsContent](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ContentId] [int] NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[CultureCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_NewsContent] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertCustomerGridTemplate]    Script Date: 04/01/2014 22:19:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertCustomerGridTemplate]
      
      @Controller nvarchar(128),
      @Action nvarchar(128),
        @Version int = 0
AS
BEGIN
      -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SET NOCOUNT ON;

  INSERT [dbo].[CustomerGridTemplate] ( [Controller], [Action], [Version]) VALUES (@Controller,@Action, @Version)
  Return SCOPE_IDENTITY()
END
GO
/****** Object:  Table [dbo].[UserPasswordQuestion]    Script Date: 04/01/2014 22:19:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPasswordQuestion](
	[UserPasswordQuestionId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[QuestionNumber] [int] NOT NULL,
	[Question] [nvarchar](128) NOT NULL,
	[Answer] [nvarchar](64) NOT NULL,
 CONSTRAINT [PK_UserPasswordQuestion] PRIMARY KEY CLUSTERED 
(
	[UserPasswordQuestionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserPasswordHistory]    Script Date: 04/01/2014 22:19:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPasswordHistory](
	[UserPasswordHistoryId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[ChangeDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_UserPasswordHistory] PRIMARY KEY CLUSTERED 
(
	[UserPasswordHistoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserCustomerAccess]    Script Date: 04/01/2014 22:19:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserCustomerAccess](
	[UserCustomerAccessId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_UserCustomerAccess] PRIMARY KEY CLUSTERED 
(
	[UserCustomerAccessId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResourceEvents]    Script Date: 04/01/2014 22:19:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResourceEvents](
	[ResourceEventsId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[TimeStamp] [datetime] NULL,
	[EventTypeId] [int] NULL,
	[SessionID] [nvarchar](50) NULL,
	[Controller] [nvarchar](128) NULL,
	[Action] [nvarchar](128) NULL,
 CONSTRAINT [PK_ResourceEvents] PRIMARY KEY CLUSTERED 
(
	[ResourceEventsId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[netsqlazman_StoreGroupsTable]    Script Date: 04/01/2014 22:19:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[netsqlazman_StoreGroupsTable](
	[StoreGroupId] [int] IDENTITY(1,1) NOT NULL,
	[StoreId] [int] NOT NULL,
	[objectSid] [varbinary](85) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NOT NULL,
	[LDapQuery] [nvarchar](4000) NULL,
	[GroupType] [tinyint] NOT NULL,
 CONSTRAINT [PK_StoreGroups] PRIMARY KEY CLUSTERED 
(
	[StoreGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StoreInsert]    Script Date: 04/01/2014 22:19:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StoreInsert]
(
	@Name nvarchar(255),
	@Description nvarchar(1024)
)
AS
INSERT INTO [dbo].[netsqlazman_StoresTable] ([Name], [Description]) VALUES (@Name, @Description);
RETURN SCOPE_IDENTITY()
GO
/****** Object:  Table [dbo].[netsqlazman_StoreAttributesTable]    Script Date: 04/01/2014 22:19:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[netsqlazman_StoreAttributesTable](
	[StoreAttributeId] [int] IDENTITY(1,1) NOT NULL,
	[StoreId] [int] NOT NULL,
	[AttributeKey] [nvarchar](255) NOT NULL,
	[AttributeValue] [nvarchar](4000) NOT NULL,
 CONSTRAINT [PK_StoreAttributes] PRIMARY KEY CLUSTERED 
(
	[StoreAttributeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_GetDBUsers]    Script Date: 04/01/2014 22:19:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* 
    NetSqlAzMan GetDBUsers TABLE Function
    ************************************************************************
    Creation Date: August, 23  2006
    Purpose: Retrieve from a DB a list of custom Users (DBUserSid, DBUserName)
    Author: Andrea Ferendeles 
    Revision: 1.0.0.0
    Updated by: <put here your name>
    Parameters: 
	use: 
		1)     SELECT * FROM dbo.GetDBUsers(<storename>, <applicationname>, NULL, NULL)            -- to retrieve all DB Users
		2)     SELECT * FROM dbo.GetDBUsers(<storename>, <applicationname>, <customsid>, NULL)  -- to retrieve DB User with specified <customsid>
		3)     SELECT * FROM dbo.GetDBUsers(<storename>, <applicationname>, NULL, <username>)  -- to retrieve DB User with specified <username>

    Remarks: 
	- Update this Function with your CUSTOM CODE
	- Returned DBUserSid must be unique
	- Returned DBUserName must be unique
*/
CREATE FUNCTION [dbo].[netsqlazman_GetDBUsers] (@StoreName nvarchar(255), @ApplicationName nvarchar(255), @DBUserSid VARBINARY(85) = NULL, @DBUserName nvarchar(255) = NULL)  
RETURNS TABLE 
AS  
RETURN 
	SELECT TOP 100 PERCENT CONVERT(VARBINARY(85), UserID) AS DBUserSid, UserName AS DBUserName,FirstName + ' ' + LastName AS FullName FROM dbo.UserProfile
	WHERE 
		(@DBUserSid IS NOT NULL AND CONVERT(VARBINARY(85), UserID) = @DBUserSid OR @DBUserSid  IS NULL)
		AND
		(@DBUserName IS NOT NULL AND UserName = @DBUserName OR @DBUserName IS NULL)
	ORDER BY UserName
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- THIS CODE IS JUST FOR AN EXAMPLE: comment this section and customize "INSERT HERE YOUR CUSTOM T-SQL" section below
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_BizRuleInsert]    Script Date: 04/01/2014 22:19:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_BizRuleInsert]
(
	@BizRuleSource text,
	@BizRuleLanguage tinyint,
	@CompiledAssembly image
)
AS
INSERT INTO [dbo].[netsqlazman_BizRulesTable] ([BizRuleSource], [BizRuleLanguage], [CompiledAssembly]) VALUES (@BizRuleSource, @BizRuleLanguage, @CompiledAssembly);
RETURN SCOPE_IDENTITY()
GO
/****** Object:  Table [dbo].[CustomerSettingTypeList]    Script Date: 04/01/2014 22:19:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerSettingTypeList](
	[CustomerSettingTypeListId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerSettingTypeId] [int] NOT NULL,
	[SettingValue] [nvarchar](255) NOT NULL,
	[InternalValue] [nvarchar](255) NULL,
	[SortOrder] [int] NOT NULL,
	[Default] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerSettingTypeList] PRIMARY KEY CLUSTERED 
(
	[CustomerSettingTypeListId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerGridTemplateCol]    Script Date: 04/01/2014 22:19:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerGridTemplateCol](
	[CustomerGridTemplateColId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerGridTemplateId] [int] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Position] [int] NOT NULL,
	[OriginalTitle] [nvarchar](255) NOT NULL,
	[OriginalPosition] [int] NOT NULL,
	[IsHidden] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerGridTemplateCol] PRIMARY KEY CLUSTERED 
(
	[CustomerGridTemplateColId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[ChangeStoreId]    Script Date: 04/01/2014 22:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Create date: 02/25/2013
-- Description:	Creates a new NetSqlAzMan store 
--   with a specific id.  This should only be used
--   in conjunction with creating a store for a 
--   new customer for PEMS.
--
-- Modified: 03/21/2013
--   Exit proc if ids are the same.
-- =============================================
CREATE PROCEDURE [dbo].[ChangeStoreId]
	@OriginalId int, 
	@NewId int
AS
BEGIN

	declare @Name nvarchar(255)
	declare @Description nvarchar(1024)

	SET NOCOUNT ON;

	-- Are the Ids the same?  If so, get out... Nothing to do.
	if @OriginalId = @NewId
		return

	-- Get original store name and description.
	select @Name = [Name], @Description = [Description]
	from [dbo].[netsqlazman_StoresTable]
	WHERE [StoreId] = @OriginalId

	set identity_insert [dbo].[netsqlazman_StoresTable] ON

	-- Recreate store with new id.
	INSERT INTO [dbo].[netsqlazman_StoresTable] 
		([StoreId], [Name], [Description]) VALUES (@NewId, @Name, @Description);

	set identity_insert [dbo].[netsqlazman_StoresTable] OFF

	-- Delete original record.
	DELETE FROM [dbo].[netsqlazman_StoresTable]
	WHERE [StoreId] = @OriginalId

END
GO
/****** Object:  Table [dbo].[AccessLogMembershipEvents]    Script Date: 04/01/2014 22:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccessLogMembershipEvents](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[TimeStamp] [datetime] NULL,
	[EventTypeId] [int] NULL,
	[SessionID] [nvarchar](50) NULL,
	[IPAddress] [nvarchar](50) NULL,
 CONSTRAINT [PK__AccessLogMembershipEvents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerReports]    Script Date: 04/01/2014 22:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomerReports](
	[CustomerReportId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CustomerReportsCategoryId] [int] NOT NULL,
	[ReportAction] [varchar](32) NOT NULL,
	[Host] [varchar](255) NOT NULL,
	[Path] [varchar](255) NOT NULL,
	[Parameters] [varchar](255) NOT NULL,
 CONSTRAINT [PK_CustomerReports] PRIMARY KEY CLUSTERED 
(
	[CustomerReportId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomerProfile]    Script Date: 04/01/2014 22:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerProfile](
	[CustomerId] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[StatusChangeDate] [smalldatetime] NOT NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
	[DefaultLocale] [nvarchar](50) NULL,
	[CreatedOn] [smalldatetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedOn] [smalldatetime] NULL,
	[ModifiedBy] [int] NULL,
	[Is24HrFormat] [bit] NULL,
	[TimeZoneID] [int] NULL,
	[PEMSConnectionStringName] [nvarchar](50) NULL,
	[MaintenanceConnectionStringName] [nvarchar](50) NULL,
	[ReportingConnectionStringName] [nvarchar](50) NULL,
	[CustomerTypeId] [int] NOT NULL,
	[ConnectionStringName] [nvarchar](50) NULL,
 CONSTRAINT [PK_CustomerProfile] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[InsertCustomerSettingType]    Script Date: 04/01/2014 22:19:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Create date: 3/28/2013
-- Description:	Inserts a CustomerSettingType
-- =============================================
CREATE PROCEDURE [dbo].[InsertCustomerSettingType] 
	@SettingName [nvarchar](50),
	@SettingType [int],
	@IsRequired [bit] = false,
	@DefaultValue [nvarchar](255) = null,
	@InternalValue  [nvarchar](255) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF NOT EXISTS( select * from [dbo].[CustomerSettingType] where
		[CustomerSettingTypeName] = @SettingName)
	BEGIN
		INSERT INTO [dbo].[CustomerSettingType]
           ([CustomerSettingTypeName]
           ,[SettingTypeId]
           ,[IsRequired]
           ,[DefaultValue]
           ,[InternalValue])
		VALUES (
			@SettingName,
			@SettingType,
			@IsRequired,
			@DefaultValue,
			@InternalValue)
	END

	select [CustomerSettingTypeId] as 'Id'
		from [dbo].[CustomerSettingType] 
		where [CustomerSettingTypeName] = @SettingName
END
GO
/****** Object:  Table [dbo].[netsqlazman_ApplicationsTable]    Script Date: 04/01/2014 22:19:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[netsqlazman_ApplicationsTable](
	[ApplicationId] [int] IDENTITY(1,1) NOT NULL,
	[StoreId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NOT NULL,
 CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[netsqlazman_ApplicationPermissionsTable]    Script Date: 04/01/2014 22:19:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[netsqlazman_ApplicationPermissionsTable](
	[ApplicationPermissionId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationId] [int] NOT NULL,
	[SqlUserOrRole] [nvarchar](128) NOT NULL,
	[IsSqlRole] [bit] NOT NULL,
	[NetSqlAzManFixedServerRole] [tinyint] NOT NULL,
 CONSTRAINT [PK_ApplicationPermissions] PRIMARY KEY CLUSTERED 
(
	[ApplicationPermissionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[netsqlazman_ApplicationGroupsTable]    Script Date: 04/01/2014 22:19:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[netsqlazman_ApplicationGroupsTable](
	[ApplicationGroupId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationId] [int] NOT NULL,
	[objectSid] [varbinary](85) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NOT NULL,
	[LDapQuery] [nvarchar](4000) NULL,
	[GroupType] [tinyint] NOT NULL,
 CONSTRAINT [PK_Groups] PRIMARY KEY CLUSTERED 
(
	[ApplicationGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[netsqlazman_ApplicationAttributesTable]    Script Date: 04/01/2014 22:19:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[netsqlazman_ApplicationAttributesTable](
	[ApplicationAttributeId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationId] [int] NOT NULL,
	[AttributeKey] [nvarchar](255) NOT NULL,
	[AttributeValue] [nvarchar](4000) NOT NULL,
 CONSTRAINT [PK_ApplicationAttributes] PRIMARY KEY CLUSTERED 
(
	[ApplicationAttributeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MaintenanceGroupCustomer]    Script Date: 04/01/2014 22:19:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaintenanceGroupCustomer](
	[MaintenanceGroupCustomerId] [int] IDENTITY(1,1) NOT NULL,
	[MaintenanceGroupId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_MaintenanceGroupCustomer] PRIMARY KEY CLUSTERED 
(
	[MaintenanceGroupCustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[GetUserList]    Script Date: 04/01/2014 22:19:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[GetUserList]
AS
SELECT        up.UserId, up.UserName, up.FirstName, up.MiddleName, up.LastName, up.Email, up.Phone, up.CreatedDate, up.RequirePasswordReset, mem.CreateDate, 
                         mem.PasswordFailuresSinceLastSuccess,
                             (SELECT        TOP (1) TimeStamp
                               FROM            dbo.AccessLogMembershipEvents AS me
                               WHERE        (up.UserId = up.UserId)) AS LastLogin,
                             (SELECT        (CASE WHEN mem.[PasswordFailuresSinceLastSuccess] > 3 THEN 'Locked Out' WHEN mem.IsActive != 1 THEN 'Terminated' ELSE 'Active' END) 
                                                         AS Expr1) AS Status
FROM            dbo.UserProfile AS up INNER JOIN
                         dbo.webpages_Membership AS mem ON up.UserId = mem.UserId
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[34] 4[33] 2[15] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "up"
            Begin Extent = 
               Top = 6
               Left = 236
               Bottom = 125
               Right = 436
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "mem"
            Begin Extent = 
               Top = 126
               Left = 38
               Bottom = 245
               Right = 326
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 8265
         Alias = 2775
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'GetUserList'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'GetUserList'
GO
/****** Object:  StoredProcedure [dbo].[InsertCustomerSettingItem]    Script Date: 04/01/2014 22:19:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Create date: 3/28/2013
-- Description:	Inserts a CustomerSettingType
--   Item.
-- =============================================
CREATE PROCEDURE [dbo].[InsertCustomerSettingItem]
	@SettingName [nvarchar](50),
	@SettingValue [nvarchar](255),
	@InternalValue [nvarchar](255) = null,
	@SortOrder [int] = 0,
	@IsDefault [bit] = false
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @CustomerSettingTypeId int

	select @CustomerSettingTypeId = [CustomerSettingTypeId]
		from [dbo].[CustomerSettingType] 
		where [CustomerSettingTypeName] = @SettingName


	IF (@CustomerSettingTypeId IS NOT NULL)
	BEGIN
		INSERT INTO [dbo].[CustomerSettingTypeList]
           ([CustomerSettingTypeId]
           ,[SettingValue]
           ,[InternalValue]
           ,[SortOrder]
           ,[Default])
		VALUES (
			@CustomerSettingTypeId,
			@SettingValue,
			@InternalValue,
			@SortOrder,
			@IsDefault)

		select [CustomerSettingTypeListId] as 'Id'
			from [dbo].[CustomerSettingTypeList] 
			where [CustomerSettingTypeId] = @CustomerSettingTypeId
				and [SettingValue] = @SettingValue
	END
	ELSE
	BEGIN
		select -1 as 'Id'
	END


END
GO
/****** Object:  Table [dbo].[CustomerGridTemplateMap]    Script Date: 04/01/2014 22:19:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerGridTemplateMap](
	[CustomerGridTemplateMapId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Controller] [nvarchar](128) NOT NULL,
	[Action] [nvarchar](128) NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_CustomerGridTemplateMap] PRIMARY KEY CLUSTERED 
(
	[CustomerGridTemplateMapId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerGrids]    Script Date: 04/01/2014 22:19:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerGrids](
	[CustomerGridsId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Controller] [nvarchar](128) NOT NULL,
	[Action] [nvarchar](128) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Position] [int] NOT NULL,
	[OriginalTitle] [nvarchar](255) NOT NULL,
	[OriginalPosition] [int] NOT NULL,
	[IsHidden] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerGrids] PRIMARY KEY CLUSTERED 
(
	[CustomerGridsId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerSettingsGroup]    Script Date: 04/01/2014 22:19:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerSettingsGroup](
	[CustomerSettingsGroupId] [int] IDENTITY(1,1) NOT NULL,
	[SettingGroupName] [nvarchar](50) NOT NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_CustomerSettingsGroup] PRIMARY KEY CLUSTERED 
(
	[CustomerSettingsGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerSettings]    Script Date: 04/01/2014 22:19:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerSettings](
	[CustomerSettingsId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CustomerSettingTypeId] [int] NOT NULL,
	[SettingValue] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_CustomerSettings] PRIMARY KEY CLUSTERED 
(
	[CustomerSettingsId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[netsqlazman_DatabaseUsers]    Script Date: 04/01/2014 22:19:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[netsqlazman_DatabaseUsers]
AS
SELECT     *
FROM         dbo.[netsqlazman_GetDBUsers](NULL, NULL, DEFAULT, DEFAULT) GetDBUsers
GO
/****** Object:  Table [dbo].[netsqlazman_ItemsTable]    Script Date: 04/01/2014 22:19:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[netsqlazman_ItemsTable](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NOT NULL,
	[ItemType] [tinyint] NOT NULL,
	[BizRuleId] [int] NULL,
 CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[netsqlazman_StoreGroupMembersTable]    Script Date: 04/01/2014 22:19:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[netsqlazman_StoreGroupMembersTable](
	[StoreGroupMemberId] [int] IDENTITY(1,1) NOT NULL,
	[StoreGroupId] [int] NOT NULL,
	[objectSid] [varbinary](85) NOT NULL,
	[WhereDefined] [tinyint] NOT NULL,
	[IsMember] [bit] NOT NULL,
 CONSTRAINT [PK_StoreGroupMembers] PRIMARY KEY CLUSTERED 
(
	[StoreGroupMemberId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertCustomerGridTemplateCol]    Script Date: 04/01/2014 22:19:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[sp_InsertCustomerGridTemplateCol]
      @CustomerGridTemplateId int, @Title nvarchar(255), @Position int, @OriginalTitle nvarchar(255), @OriginalPosition int, @IsHidden bit
AS
BEGIN
      -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SET NOCOUNT ON;

  INSERT [dbo].[CustomerGridTemplateCol] ([CustomerGridTemplateId], [Title], [Position], [OriginalTitle], [OriginalPosition], [IsHidden])
   VALUES (@CustomerGridTemplateId, @Title, @Position,@OriginalTitle, @OriginalPosition, @IsHidden)
Return  SCOPE_IDENTITY()
END
GO
/****** Object:  Table [dbo].[WorkOrdersAudit]    Script Date: 04/01/2014 22:19:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkOrdersAudit](
	[WorkOrderAuditId] [bigint] IDENTITY(1,1) NOT NULL,
	[AuditDateTime] [datetime] NOT NULL,
	[UserId] [int] NULL,
	[WorkOrderId] [bigint] NOT NULL,
	[ReportingUserId] [int] NULL,
	[MeterGroup] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[Location] [varchar](500) NULL,
	[ParkingSpaceId] [bigint] NULL,
	[Notes] [varchar](2000) NULL,
	[ZoneId] [int] NULL,
	[HighestSeverity] [int] NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
	[SLADue] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
	[TechnicianId] [int] NULL,
	[AssignedDate] [datetime] NULL,
	[CompletedDate] [datetime] NULL,
	[CreatedById] [int] NULL,
	[CrossStreet] [varchar](2000) NULL,
	[ResolutionCode] [int] NULL,
	[ResolutionDesc] [varchar](2000) NULL,
	[AssignmentState] [int] NULL,
	[Mechanism] [int] NULL,
 CONSTRAINT [PK_WorkOrdersAudit] PRIMARY KEY CLUSTERED 
(
	[WorkOrderAuditId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WorkOrderPart]    Script Date: 04/01/2014 22:19:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkOrderPart](
	[WorkOrderPartId] [bigint] IDENTITY(1,1) NOT NULL,
	[WorkOrderId] [bigint] NOT NULL,
	[PartId] [bigint] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_WorkOrderPart] PRIMARY KEY CLUSTERED 
(
	[WorkOrderPartId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WorkOrderImage]    Script Date: 04/01/2014 22:19:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkOrderImage](
	[WorkOrderImageId] [bigint] IDENTITY(1,1) NOT NULL,
	[WorkOrderId] [bigint] NOT NULL,
	[ImageData] [image] NOT NULL,
	[DateTaken] [datetime] NOT NULL,
 CONSTRAINT [PK_WorkOrderImage] PRIMARY KEY CLUSTERED 
(
	[WorkOrderImageId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkOrderEvent]    Script Date: 04/01/2014 22:19:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkOrderEvent](
	[WorkOrderEventId] [bigint] IDENTITY(1,1) NOT NULL,
	[WorkOrderId] [bigint] NOT NULL,
	[EventId] [bigint] NOT NULL,
	[EventCode] [int] NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[SLADue] [datetime] NOT NULL,
	[EventDesc] [varchar](50) NULL,
	[AlarmTier] [int] NOT NULL,
	[Notes] [varchar](2000) NULL,
	[Automated] [bit] NOT NULL,
	[Vandalism] [bit] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_WorkOrderEvent] PRIMARY KEY CLUSTERED 
(
	[WorkOrderEventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertCustomerGridTemplateMapping]    Script Date: 04/01/2014 22:19:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertCustomerGridTemplateMapping] 
      @CustomerId int,
      @Controller nvarchar(128),
      @Action nvarchar(128),
      @Version int = 0
AS
BEGIN
      -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SET NOCOUNT ON;


delete from dbo.CustomerGridTemplateMap where CustomerId = @CustomerId 
and Controller = @Controller 
and Action = @Action


  -- Insert statements for procedure
      INSERT INTO CustomerGridTemplateMap
           ([CustomerId]
           ,[Controller]
           ,[Action]
           ,[Version])
     VALUES           (@CustomerId,          @Controller           ,@Action           ,@Version)
Return  SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertCustomerGridsFromTemplateMap]    Script Date: 04/01/2014 22:19:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertCustomerGridsFromTemplateMap]
	-- Add the parameters for the stored procedure here
	@CustomerId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--Map Items
	DECLARE @Controller nvarchar(128);
	DECLARE @Action nvarchar(128);
	DECLARE @Version int;

	--Template Items
	DECLARE @GridTemplateId int;

	--Template Column Items
	DECLARE @Title nvarchar(128);
	DECLARE @Position int;
	DECLARE @OriginalTitle nvarchar(128);
	DECLARE @OriginalPosition int;
	DECLARE @IsHidden bit;

--this will blow away any exisint items int he CustomerGrid table for this customer ID, then reset that data back in based on the customer grid template map. 
--it will be used when saving the customer girds page for a customer, when they are newly created, or if we are adding columns to the grids, or when adding new grids to the system

--delete all existing grid data for this customer
delete from CustomerGrids where CustomerId = @CustomerId;

--re-insert the data based on what is in the customer template mapping table
--Get the maps for this customer
	DECLARE cursorCustomerGridMap cursor for SELECT cgtm.Controller ,cgtm.Action,cgtm.Version  FROM CustomerGridTemplateMap cgtm where CustomerId = @CustomerId;
OPEN cursorCustomerGridMap
	FETCH NEXT FROM cursorCustomerGridMap into @Controller, @Action, @Version
	WHILE @@FETCH_STATUS = 0
	BEGIN		

	
	--now we have to get the grid tempate for this map
	set @GridTemplateId = (Select CustomerGridTemplateId from CustomerGridTemplate where Controller = @Controller and  Action = @Action and Version = @Version);
	--now get all of the customer template columns for this template
	DECLARE cursorCustomerGridColumns cursor for SELECT Title ,Position, OriginalTitle ,OriginalPosition,IsHidden  
	FROM CustomerGridTemplateCol where CustomerGridTemplateId = @GridTemplateId;
	
	--roll throught he columns and insert into the customerGrid table with this action/controller/customerId
	
	OPEN cursorCustomerGridColumns
	FETCH NEXT FROM cursorCustomerGridColumns into @Title ,@Position, @OriginalTitle ,@OriginalPosition,@IsHidden
	WHILE @@FETCH_STATUS = 0
	BEGIN		

-- Insert this grid row
			INSERT INTO [dbo].[CustomerGrids] 
				([CustomerId],[Controller],[Action],[Title],[Position],[OriginalTitle],[OriginalPosition], [IsHidden])
			VALUES
				(@CustomerId,@Controller,@Action,@Title,@Position,@OriginalTitle,@OriginalPosition, @IsHidden)

		-- Get next row
		 FETCH NEXT FROM cursorCustomerGridColumns into @Title ,@Position, @OriginalTitle ,@OriginalPosition,@IsHidden
	END
	CLOSE cursorCustomerGridColumns
	DEALLOCATE cursorCustomerGridColumns

		-- Get next row
		 FETCH NEXT FROM cursorCustomerGridMap into @Controller, @Action, @Version
	END
	CLOSE cursorCustomerGridMap
	DEALLOCATE cursorCustomerGridMap
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertAllCustomGridOptions]    Script Date: 04/01/2014 22:20:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertAllCustomGridOptions]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
SET NOCOUNT ON;

--remove all the existing custom options
delete from CustomerGridTemplateCol where CustomerGridTemplateId in (select CustomerGridTemplateId from CustomerGridTemplate )
delete from CustomerGridTemplate 

--
DECLARE @LastId int


---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- ALARMS / GETITEMS ------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Alarms', @Action = 'GetItems', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time of Complaint', @Position = 0, @OriginalTitle  = 'Time of Complaint',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Id', @Position = 1, @OriginalTitle  = 'Alarm Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Code', @Position = 2, @OriginalTitle  = 'Alarm Code',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Description', @Position = 3, @OriginalTitle  = 'Description',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 4, @OriginalTitle  = 'Asset Type',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 5, @OriginalTitle  = 'Asset Id',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 6, @OriginalTitle  = 'Asset Name',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 7, @OriginalTitle  = 'Street',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Severity', @Position = 8, @OriginalTitle  = 'Alarm Severity',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 9, @OriginalTitle  = 'Asset State',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Status', @Position = 10, @OriginalTitle  = 'Alarm Status',@OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Remaining Until Target Time', @Position = 11, @OriginalTitle  = 'Time Remaining Until Target Time',@OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Cleared', @Position = 12, @OriginalTitle  = 'Time Cleared',@OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 13, @OriginalTitle  = 'Area',@OriginalPosition = 13, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 14, @OriginalTitle  = 'Zone',@OriginalPosition = 14, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 15, @OriginalTitle  = 'Suburb',@OriginalPosition = 15, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Source', @Position = 16, @OriginalTitle  = 'Alarm Source',@OriginalPosition = 16, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Technician Id', @Position = 17, @OriginalTitle  = 'Technician Id',@OriginalPosition = 17, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- ASSETS / GETSUMMARYASSETS ----------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC  @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Assets', @Action = 'GetSummaryAssets', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'', @Position = 0, @OriginalTitle  = '',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 1, @OriginalTitle  = 'Asset Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 2, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Model', @Position = 3, @OriginalTitle  = 'Asset Model',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 4, @OriginalTitle  = 'Street',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 5, @OriginalTitle  = 'Operational Status',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 6, @OriginalTitle  = 'Area Id2',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 7, @OriginalTitle  = 'Zone Id',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 8, @OriginalTitle  = 'Suburb',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Latitude', @Position = 9, @OriginalTitle  = 'Latitude',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Longitude', @Position = 10, @OriginalTitle  = 'Longitude',@OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Spaces Count', @Position = 11, @OriginalTitle  = 'Spaces Count',@OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Demand Area', @Position = 12, @OriginalTitle  = 'Demand Status',@OriginalPosition = 12, @IsHidden = 0

EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Assets', @Action = 'GetSummaryAssets', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'', @Position = 0, @OriginalTitle  = '',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 1, @OriginalTitle  = 'Asset Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 2, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Model', @Position = 3, @OriginalTitle  = 'Asset Model',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 4, @OriginalTitle  = 'Street',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 5, @OriginalTitle  = 'Operational Status',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 6, @OriginalTitle  = 'Area Id2',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 7, @OriginalTitle  = 'Zone Id',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 12, @OriginalTitle  = 'Suburb',@OriginalPosition = 8, @IsHidden = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Latitude', @Position = 9, @OriginalTitle  = 'Latitude',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Longitude', @Position = 10, @OriginalTitle  = 'Longitude',@OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Spaces Count', @Position = 8, @OriginalTitle  = 'Spaces Count',@OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Demand Area', @Position = 11, @OriginalTitle  = 'Demand Status',@OriginalPosition = 12, @IsHidden = 1

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- ASSETS / GETOCCUPANCYASSETS --------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Assets', @Action = 'GetOccupancyAssets', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'', @Position = 0, @OriginalTitle  = '',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 1, @OriginalTitle  = 'Asset Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 2, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Name', @Position = 3, @OriginalTitle  = 'Meter Name',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Sensor Name', @Position = 4, @OriginalTitle  = 'Sensor Name',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 5, @OriginalTitle  = 'Operational Status',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status Date', @Position = 6, @OriginalTitle  = 'Operational Status Date',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Occupancy Status', @Position = 7, @OriginalTitle  = 'Occupancy Status',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Occupancy Status Date', @Position = 8, @OriginalTitle  = 'Occupancy Status Date',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Non Compliance Status', @Position = 9, @OriginalTitle  = 'Non Compliance Status',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Non Compliance Status Date', @Position = 10, @OriginalTitle  = 'Non Compliance Status Date',@OriginalPosition = 10, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- ASSETS / GETFUNCTIONALSTATUSASSETS -------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Assets', @Action = 'GetFunctionalStatusAssets', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'', @Position = 0, @OriginalTitle  = '',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 1, @OriginalTitle  = 'Asset Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 2, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Model', @Position = 3, @OriginalTitle  = 'Asset Model',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 4, @OriginalTitle  = 'Operational Status',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status Date', @Position = 5, @OriginalTitle  = 'Operational Status Date',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Class', @Position = 6, @OriginalTitle  = 'Alarm Class',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Code', @Position = 7, @OriginalTitle  = 'Alarm Code',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Duration', @Position = 8, @OriginalTitle  = 'Alarm Duration',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Repair Target Time', @Position = 9, @OriginalTitle  = 'Alarm Repair Target Time',@OriginalPosition = 9, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- ASSETS / GETCONFIGURATIONASSETS ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Assets', @Action = 'GetConfigurationAssets', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'', @Position = 0, @OriginalTitle  = '',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 1, @OriginalTitle  = 'Asset Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 2, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Model', @Position = 3, @OriginalTitle  = 'Asset Model',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Installed', @Position = 4, @OriginalTitle  = 'Date Installed',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Configuration Id', @Position = 5, @OriginalTitle  = 'Configuration Id',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Creation Date', @Position = 6, @OriginalTitle  = 'Config Creation Date',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Schedule Date', @Position = 7, @OriginalTitle  = 'Config Schedule Date',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Activation Date', @Position = 8, @OriginalTitle  = 'Config Activation Date',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Firmware Version', @Position = 9, @OriginalTitle  = 'Firmware Version',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Software Version', @Position = 10, @OriginalTitle  = 'Software Version',@OriginalPosition =10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Mpv Version', @Position = 11, @OriginalTitle  = 'Mpv Version',@OriginalPosition =11, @IsHidden = 0


---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- COLLECTIONS / GETAGGMETERS ---------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Collections', @Action = 'GetAggMeters', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Id', @Position = 0, @OriginalTitle  = 'Meter Id',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Name', @Position = 1, @OriginalTitle  = 'Meter Name',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Time', @Position = 2, @OriginalTitle  = 'Date Time',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area Id', @Position = 3, @OriginalTitle  = 'Area Id',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone Id', @Position = 4, @OriginalTitle  = 'Zone Id',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 5, @OriginalTitle  = 'Suburb',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 6, @OriginalTitle  = 'Street',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Meter', @Position = 7, @OriginalTitle  = 'Amt Meter',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Vendor', @Position = 8, @OriginalTitle  = 'Amt Vendor',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Chip', @Position = 9, @OriginalTitle  = 'Amt Chip',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Difference Flag', @Position = 10, @OriginalTitle  = 'Difference Flag',@OriginalPosition =10, @IsHidden = 0

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Collections', @Action = 'GetAggMeters', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Id', @Position = 0, @OriginalTitle  = 'Meter Id',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Name', @Position = 1, @OriginalTitle  = 'Meter Name',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Time', @Position = 2, @OriginalTitle  = 'Date Time',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area Id', @Position = 6, @OriginalTitle  = 'Area Id',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone Id', @Position = 7, @OriginalTitle  = 'Zone Id',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 9, @OriginalTitle  = 'Suburb',@OriginalPosition = 5, @IsHidden = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 8, @OriginalTitle  = 'Street',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Meter', @Position = 3, @OriginalTitle  = 'Amt Meter',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Vendor', @Position = 4, @OriginalTitle  = 'Amt Vendor',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Chip', @Position = 5, @OriginalTitle  = 'Amt Chip',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Difference Flag', @Position = 10, @OriginalTitle  = 'Difference Flag',@OriginalPosition =10, @IsHidden = 0


---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- COLLECTIONS / GETAGGREGATIONS ------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Collections', @Action = 'GetAggregations', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Collection Route Name', @Position = 0, @OriginalTitle  = 'Route Name',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Time', @Position = 1, @OriginalTitle  = 'Date Time',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Configuration ID', @Position = 2, @OriginalTitle  = 'Route Id',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Vendor Name', @Position = 3, @OriginalTitle  = 'Vendor Id',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meters To Collect', @Position = 4, @OriginalTitle  = 'Meters To Collect',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meters Collected', @Position = 5, @OriginalTitle  = 'Meters Collected',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Collected Meter', @Position = 6, @OriginalTitle  = 'Total Collected Meter',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Collected Chip', @Position = 7, @OriginalTitle  = 'Total Collected Chip',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Collected Vendor', @Position = 8, @OriginalTitle  = 'Total Collected Vendor',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Variance Amount', @Position = 9, @OriginalTitle  = 'Amount Diff',@OriginalPosition = 9, @IsHidden = 0

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Collections', @Action = 'GetAggregations', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Collection Route Name', @Position = 0, @OriginalTitle  = 'Route Name',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Time', @Position = 1, @OriginalTitle  = 'Date Time',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Configuration ID', @Position = 3, @OriginalTitle  = 'Route Id',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Vendor Name', @Position = 9, @OriginalTitle  = 'Vendor Id',@OriginalPosition = 3, @IsHidden = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meters To Collect', @Position = 4, @OriginalTitle  = 'Meters To Collect',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meters Collected', @Position = 5, @OriginalTitle  = 'Meters Collected',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Collected Meter', @Position = 6, @OriginalTitle  = 'Total Collected Meter',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Collected Chip', @Position = 7, @OriginalTitle  = 'Total Collected Chip',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Collected Vendor', @Position = 8, @OriginalTitle  = 'Total Collected Vendor',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Variance Amount', @Position = 2, @OriginalTitle  = 'Amount Diff',@OriginalPosition = 9, @IsHidden = 0


---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- COLLECTIONS / GETROUTES ------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Collections', @Action = 'GetRoutes', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Collection Route Name', @Position = 0, @OriginalTitle  = 'Route Name',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Configuration ID', @Position = 1, @OriginalTitle  = 'Route Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Status', @Position = 2, @OriginalTitle  = 'Status',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Created', @Position = 3, @OriginalTitle  = 'Date Created',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Activated', @Position = 4, @OriginalTitle  = 'Date Activated',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Number Of Meters', @Position = 5, @OriginalTitle  = 'Number Of Meters',@OriginalPosition = 5, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- DISCOUNTS / GETITEMS ---------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Discounts', @Action = 'GetItems', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Last Name', @Position = 0, @OriginalTitle  = 'Last Name',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'First Name', @Position = 1, @OriginalTitle  = 'First Name',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Email', @Position = 2, @OriginalTitle  = 'Email',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Pending', @Position = 3, @OriginalTitle  = 'Pending Count',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Approved', @Position = 4, @OriginalTitle  = 'Approved Count',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Rejected', @Position = 5, @OriginalTitle  = 'Rejected Count',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Account Status', @Position = 6, @OriginalTitle  = 'Account Status',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Creation Date', @Position = 7, @OriginalTitle  = 'Creation Date',@OriginalPosition = 7, @IsHidden = 0

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Discounts', @Action = 'GetItems', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Last Name', @Position = 0, @OriginalTitle  = 'Last Name',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'First Name', @Position = 1, @OriginalTitle  = 'First Name',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Email', @Position = 2, @OriginalTitle  = 'Email',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Pending', @Position = 5, @OriginalTitle  = 'Pending Count',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Approved', @Position =6, @OriginalTitle  = 'Approved Count',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Rejected', @Position = 7, @OriginalTitle  = 'Rejected Count',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Account Status', @Position = 3, @OriginalTitle  = 'Account Status',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Creation Date', @Position = 4, @OriginalTitle  = 'Creation Date',@OriginalPosition = 7, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETSUMMARYEVENTS ----------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC  @LastID =   [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetSummaryEvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position = 0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event ID', @Position = 1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Description', @Position = 2, @OriginalTitle  = 'Description',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 3, @OriginalTitle  = 'Asset Type',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 4, @OriginalTitle  = 'Asset ID',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 5, @OriginalTitle  = 'Asset Name',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 6, @OriginalTitle  = 'Street',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Class', @Position = 7, @OriginalTitle  = 'Event Class',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Code', @Position = 8, @OriginalTitle  = 'Event Code',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 9, @OriginalTitle  = 'Area',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 10, @OriginalTitle  = 'Zone',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 11, @OriginalTitle  = 'Suburb',@OriginalPosition =10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Demand Area', @Position = 12, @OriginalTitle  = 'Demand Area',@OriginalPosition =11, @IsHidden = 0

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetSummaryEvents', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position = 0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event ID', @Position = 1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Description', @Position = 2, @OriginalTitle  = 'Description',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 3, @OriginalTitle  = 'Asset Type',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 4, @OriginalTitle  = 'Asset ID',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 5, @OriginalTitle  = 'Asset Name',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Class', @Position = 6, @OriginalTitle  = 'Event Class',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Code', @Position = 7, @OriginalTitle  = 'Event Code',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 8, @OriginalTitle  = 'Street',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 9, @OriginalTitle  = 'Area',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 10, @OriginalTitle  = 'Zone',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 11, @OriginalTitle  = 'Suburb',@OriginalPosition =10, @IsHidden = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Demand Area', @Position = 12, @OriginalTitle  = 'Demand Area',@OriginalPosition =11, @IsHidden = 1

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETDIAGNOSTICEVENTS (Functional status)------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC  @LastID =   [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetDiagnosticEvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Date/Time', @Position = 0, @OriginalTitle  = 'Event Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Received Date/Time', @Position = 1, @OriginalTitle  = 'Received Date/Time',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event ID', @Position = 2, @OriginalTitle  = 'Event ID',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 3, @OriginalTitle  = 'Asset ID',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position =4, @OriginalTitle  = 'Asset Name',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Type', @Position = 5, @OriginalTitle  = 'Type',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Value', @Position = 6, @OriginalTitle  = 'Value',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 7, @OriginalTitle  = 'Street',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 8, @OriginalTitle  = 'Area',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 9, @OriginalTitle  = 'Zone',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 10, @OriginalTitle  = 'Suburb',@OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Demand Area', @Position = 11, @OriginalTitle  = 'Demand Area',@OriginalPosition = 11, @IsHidden = 0

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetDiagnosticEvents', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Date/Time', @Position = 0, @OriginalTitle  = 'Event Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Received Date/Time', @Position = 1, @OriginalTitle  = 'Received Date/Time',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event ID', @Position = 2, @OriginalTitle  = 'Event ID',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 3, @OriginalTitle  = 'Asset ID',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position =4, @OriginalTitle  = 'Asset Name',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Type', @Position = 5, @OriginalTitle  = 'Type',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Value', @Position = 6, @OriginalTitle  = 'Value',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 7, @OriginalTitle  = 'Street',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 8, @OriginalTitle  = 'Area',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 9, @OriginalTitle  = 'Zone',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 10, @OriginalTitle  = 'Suburb',@OriginalPosition = 10, @IsHidden = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Demand Area', @Position = 11, @OriginalTitle  = 'Demand Area',@OriginalPosition = 11, @IsHidden = 1

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETALARMEVENTS -------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetAlarmEvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position = 0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event ID', @Position = 1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 2, @OriginalTitle  = 'Asset Type',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 3, @OriginalTitle  = 'Asset ID',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 4, @OriginalTitle  = 'Asset Name',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Notified', @Position = 5, @OriginalTitle  = 'Time Notified',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Cleared', @Position = 6, @OriginalTitle  = 'Time Cleared',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Due SLA', @Position = 7, @OriginalTitle  = 'Time Due SLA',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Description', @Position = 8, @OriginalTitle  = 'Alarm Description',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Source' , @Position = 9, @OriginalTitle  = 'Source',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Severity' , @Position = 10, @OriginalTitle  = 'Alarm Severity',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Work Order Id', @Position = 11, @OriginalTitle  ='Work Order Id',@OriginalPosition =10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Resolution Code', @Position = 12, @OriginalTitle  ='Resolution Code',@OriginalPosition =11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Technician', @Position = 13, @OriginalTitle  ='Technician',@OriginalPosition =12, @IsHidden = 0

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetAlarmEvents', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position = 0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event ID', @Position = 1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 2, @OriginalTitle  = 'Asset Type',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 3, @OriginalTitle  = 'Asset ID',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 4, @OriginalTitle  = 'Asset Name',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Notified', @Position = 5, @OriginalTitle  = 'Time Notified',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Cleared', @Position = 6, @OriginalTitle  = 'Time Cleared',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Description', @Position = 7, @OriginalTitle  = 'Alarm Description',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Source' , @Position = 8, @OriginalTitle  = 'Source',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Severity' , @Position = 9, @OriginalTitle  = 'Alarm Severity',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Work Order Id', @Position = 10, @OriginalTitle  ='Work Order Id',@OriginalPosition =10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Resolution Code', @Position = 11, @OriginalTitle  ='Resolution Code',@OriginalPosition =11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Technician', @Position = 12, @OriginalTitle  ='Technician',@OriginalPosition =12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Due SLA', @Position = 13, @OriginalTitle  = 'Time Due SLA',@OriginalPosition = 6, @IsHidden = 1





---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETCOLLECTIONCBREVENTS -------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetCollectionCBREvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position = 0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Id', @Position =1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 2, @OriginalTitle  = 'Asset ID',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 3, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Removal Time', @Position = 4, @OriginalTitle  = 'Removal Time',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Insertion Time', @Position = 5, @OriginalTitle  = 'Insertion Time',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Active', @Position = 6, @OriginalTitle  = 'Time Active',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'CBID', @Position = 7, @OriginalTitle  = 'CBID',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Sequence Number', @Position = 8, @OriginalTitle  = 'Sequence Number',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amt Auto', @Position = 9, @OriginalTitle  = 'Amt Auto',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amt Manual', @Position = 10, @OriginalTitle  = 'Amt Manual',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amt Difference', @Position = 11, @OriginalTitle  = 'Amt Difference',@OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operator Id', @Position = 12, @OriginalTitle  = 'Operator Id',@OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Version', @Position = 13, @OriginalTitle  = 'Version',@OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Transaction File Name', @Position = 14, @OriginalTitle  = 'Transaction File Name',@OriginalPosition = 13, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETCOLLECTIONCOMMEVENTS -------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetCollectionCommEvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position = 0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Id', @Position = 1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 2, @OriginalTitle  = 'Asset ID',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 3, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Insertion Time', @Position = 4, @OriginalTitle  = 'Insertion Time',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount', @Position = 5, @OriginalTitle  = 'Amount',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Previous CBID', @Position = 6, @OriginalTitle  = 'Previous CBID',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'New CBID', @Position = 7, @OriginalTitle  = 'New CBID',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Sequence Number', @Position = 8, @OriginalTitle  = 'Sequence Number',@OriginalPosition = 7, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETCONNECTIONEVENTS -------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetConnectionEvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position =0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Id', @Position = 1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 2, @OriginalTitle  = 'Asset Type',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 3, @OriginalTitle  = 'Asset ID',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 4, @OriginalTitle  = 'Asset Name',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Connection Status', @Position = 5, @OriginalTitle  = 'Connection Status',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Error Description', @Position = 6, @OriginalTitle  = 'Error Description',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'End Time', @Position = 7, @OriginalTitle  = 'End Time',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Period', @Position = 8, @OriginalTitle  = 'Period',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Port', @Position =9, @OriginalTitle  = 'Port',@OriginalPosition = 8, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETTRANSACTIONEVENTS ------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetTransactionEvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position =0,  @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Transaction Id', @Position =1,  @OriginalTitle  = 'Transaction Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position =2,  @OriginalTitle  = 'Asset Type',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 3, @OriginalTitle  = 'Asset ID',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 4,  @OriginalTitle  = 'Asset Name',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Payment Type', @Position = 5,  @OriginalTitle  = 'Payment Type',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Bay', @Position = 6,  @OriginalTitle  = 'Bay',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Paid', @Position = 7,  @OriginalTitle  = 'Time Paid',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount', @Position = 8,  @OriginalTitle  = 'Amount',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Card Type', @Position = 9,  @OriginalTitle  = 'Card Type',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Card Status', @Position = 10,  @OriginalTitle  = 'Card Status',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Receipt Number', @Position = 11, @OriginalTitle  = 'Receipt Number',@OriginalPosition = 10, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- OCCUPANCY / GETITEMS ---------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Occupancy', @Action = 'GetItems', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Arrival Time', @Position = 0,   @OriginalTitle  = 'Arrival Time', @OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Bay Number', @Position = 1,   @OriginalTitle  = 'Bay Number', @OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Bay Name', @Position = 2,   @OriginalTitle  = 'Bay Name', @OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Departure Time', @Position = 3,   @OriginalTitle  = 'Departure Time', @OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Occupancy Status', @Position = 4,   @OriginalTitle  = 'Occupancy Status', @OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 5,   @OriginalTitle  = 'Operational Status', @OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Id', @Position = 6,   @OriginalTitle  = 'Meter Id', @OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Sensor Id', @Position = 7,   @OriginalTitle  = 'Sensor Id', @OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 8,   @OriginalTitle  = 'Street', @OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Duration Occupied', @Position = 9,   @OriginalTitle  = 'Duration Occupied', @OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Duration Purchased', @Position = 10,  @OriginalTitle  = 'Duration Purchased', @OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Paid', @Position = 11,  @OriginalTitle  = 'Total Paid', @OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 12,  @OriginalTitle  = 'Area', @OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 13,  @OriginalTitle  = 'Zone', @OriginalPosition = 13, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 14,  @OriginalTitle  = 'Suburb', @OriginalPosition = 14, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Type', @Position = 15,  @OriginalTitle  = 'Space Type', @OriginalPosition = 15, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Id', @Position = 16,  @OriginalTitle  = 'Space Id', @OriginalPosition = 16, @IsHidden = 0

EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Occupancy', @Action = 'GetItems', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Arrival Time', @Position = 0,   @OriginalTitle  = 'Arrival Time', @OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Bay Number', @Position = 1,   @OriginalTitle  = 'Bay Number', @OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Bay Name', @Position = 2,   @OriginalTitle  = 'Bay Name', @OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Departure Time', @Position = 3,   @OriginalTitle  = 'Departure Time', @OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Occupancy Status', @Position = 4,   @OriginalTitle  = 'Occupancy Status', @OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 5,   @OriginalTitle  = 'Operational Status', @OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Id', @Position = 6,   @OriginalTitle  = 'Meter Id', @OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Sensor Id', @Position = 7,   @OriginalTitle  = 'Sensor Id', @OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 8,   @OriginalTitle  = 'Street', @OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Duration Occupied', @Position = 9,   @OriginalTitle  = 'Duration Occupied', @OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Duration Purchased', @Position = 10,  @OriginalTitle  = 'Duration Purchased', @OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Paid', @Position = 11,  @OriginalTitle  = 'Total Paid', @OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 12,  @OriginalTitle  = 'Area', @OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 13,  @OriginalTitle  = 'Zone', @OriginalPosition = 13, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 16,  @OriginalTitle  = 'Suburb', @OriginalPosition = 14, @IsHidden = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Type', @Position = 14,  @OriginalTitle  = 'Space Type', @OriginalPosition = 15, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Id', @Position = 16,  @OriginalTitle  = 'Space Id', @OriginalPosition = 16, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- TRANSACTIONS / GETTRANSACTIONS -----------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Transactions', @Action = 'GetTransactions', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Time', @Position = 0,    @OriginalTitle  = 'Date Time', @OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Transaction Id', @Position = 1,    @OriginalTitle  = 'Transaction Id', @OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 2,    @OriginalTitle  = 'Asset Type', @OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 3,    @OriginalTitle  = 'Asset Id', @OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 4,    @OriginalTitle  = 'Asset Name', @OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 5,    @OriginalTitle  = 'Area', @OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 6,    @OriginalTitle  = 'Street', @OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Transaction Type', @Position = 7,    @OriginalTitle  = 'Transaction Type', @OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Id', @Position = 8,    @OriginalTitle  = 'Space Id', @OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Gateway Id', @Position = 9,    @OriginalTitle  = 'Gateway Id', @OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'CC Last 4', @Position = 10,   @OriginalTitle  = 'CC Last 4', @OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Paid', @Position = 11,   @OriginalTitle  = 'Amount Paid', @OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Paid', @Position = 12,   @OriginalTitle  = 'Time Paid', @OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Status', @Position = 13,   @OriginalTitle  = 'Space Status', @OriginalPosition = 13, @IsHidden = 0

EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Transactions', @Action = 'GetTransactions', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Time', @Position = 0,    @OriginalTitle  = 'Date Time', @OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Transaction Id', @Position = 1,    @OriginalTitle  = 'Transaction Id', @OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 2,    @OriginalTitle  = 'Asset Type', @OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 3,    @OriginalTitle  = 'Asset Id', @OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 4,    @OriginalTitle  = 'Asset Name', @OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 6,    @OriginalTitle  = 'Area', @OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 7,    @OriginalTitle  = 'Street', @OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Transaction Type', @Position = 5,    @OriginalTitle  = 'Transaction Type', @OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Id', @Position = 8,    @OriginalTitle  = 'Space Id', @OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Gateway Id', @Position = 9,    @OriginalTitle  = 'Gateway Id', @OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'CC Last 4', @Position = 10,   @OriginalTitle  = 'CC Last 4', @OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Paid', @Position = 11,   @OriginalTitle  = 'Amount Paid', @OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Paid', @Position = 12,   @OriginalTitle  = 'Time Paid', @OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Status', @Position = 13,   @OriginalTitle  = 'Space Status', @OriginalPosition = 13, @IsHidden = 1

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- WORKORDERS / GETWORKORDERS -----------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'WorkOrders', @Action = 'GetWorkOrders', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'', @Position = 0,    @OriginalTitle  = '', @OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Work Order', @Position = 1,    @OriginalTitle  = 'Date Time', @OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Customer', @Position = 2,    @OriginalTitle  = 'Transaction Id', @OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 3,    @OriginalTitle  = 'Asset Type', @OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 4,    @OriginalTitle  = 'Asset Id', @OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 5,    @OriginalTitle  = 'Asset Name', @OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 6,    @OriginalTitle  = 'Area', @OriginalPosition =6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 7,    @OriginalTitle  = 'Street', @OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Highest Priority', @Position = 8,    @OriginalTitle  = 'Transaction Type', @OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Creation Date', @Position = 9,    @OriginalTitle  = 'Space Id', @OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Deadline', @Position = 10,    @OriginalTitle  = 'Gateway Id', @OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'# of Event IDs', @Position = 11,   @OriginalTitle  = 'CC Last 4', @OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Work Order State', @Position = 12,   @OriginalTitle  = 'Amount Paid', @OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Technician', @Position = 13,   @OriginalTitle  = 'Time Paid', @OriginalPosition = 13, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Assigned Date', @Position = 14,   @OriginalTitle  = 'Space Status', @OriginalPosition = 14, @IsHidden = 0
END
GO
/****** Object:  Table [dbo].[User_CustomerGrids]    Script Date: 04/01/2014 22:20:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User_CustomerGrids](
	[CustomerGridsId] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Controller] [nvarchar](128) NOT NULL,
	[Action] [nvarchar](128) NOT NULL,
	[OriginalColName] [nvarchar](255) NOT NULL,
	[OriginalColPos] [int] NOT NULL,
 CONSTRAINT [PK_User_CustomerGrids] PRIMARY KEY CLUSTERED 
(
	[CustomerGridsId] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[netsqlazman_ItemAttributesTable]    Script Date: 04/01/2014 22:20:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[netsqlazman_ItemAttributesTable](
	[ItemAttributeId] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[AttributeKey] [nvarchar](255) NOT NULL,
	[AttributeValue] [nvarchar](4000) NOT NULL,
 CONSTRAINT [PK_ItemAttributes] PRIMARY KEY CLUSTERED 
(
	[ItemAttributeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_CheckStorePermissions]    Script Date: 04/01/2014 22:20:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* 
   @ROLEID = { 0 READERS, 1 USERS, 2 MANAGERS}
*/
CREATE FUNCTION [dbo].[netsqlazman_CheckStorePermissions](@STOREID int, @ROLEID tinyint)
RETURNS bit
AS
BEGIN
DECLARE @RESULT bit
IF @STOREID IS NULL OR @ROLEID IS NULL
	SET @RESULT = 0	
ELSE
BEGIN
	IF EXISTS (
		SELECT     dbo.[netsqlazman_StorePermissionsTable].StoreId
		FROM         dbo.[netsqlazman_ApplicationsTable] RIGHT OUTER JOIN
		                      dbo.[netsqlazman_StoresTable] ON dbo.[netsqlazman_ApplicationsTable].StoreId = dbo.[netsqlazman_StoresTable].StoreId LEFT OUTER JOIN
		                      dbo.[netsqlazman_StorePermissionsTable] ON dbo.[netsqlazman_StoresTable].StoreId = dbo.[netsqlazman_StorePermissionsTable].StoreId LEFT OUTER JOIN
		                      dbo.[netsqlazman_ApplicationPermissionsTable] ON dbo.[netsqlazman_ApplicationsTable].ApplicationId = dbo.[netsqlazman_ApplicationPermissionsTable].ApplicationId
		WHERE 
		IS_MEMBER('db_owner')=1 OR IS_MEMBER('NetSqlAzMan_Administrators')=1 OR 
		(@ROLEID = 0 AND IS_MEMBER('NetSqlAzMan_Readers')=1 OR 
		@ROLEID = 1 AND IS_MEMBER('NetSqlAzMan_Users')=1 OR 
		@ROLEID = 2 AND IS_MEMBER('NetSqlAzMan_Managers')=1) AND
		(
		(dbo.[netsqlazman_StorePermissionsTable].StoreId = @STOREID AND dbo.[netsqlazman_StorePermissionsTable].NetSqlAzManFixedServerRole >= @ROLEID AND 
		(SUSER_SNAME(SUSER_SID())=[netsqlazman_StorePermissionsTable].SqlUserOrRole AND [netsqlazman_StorePermissionsTable].IsSqlRole = 0 OR
		IS_MEMBER([netsqlazman_StorePermissionsTable].SqlUserOrRole)=1 AND [netsqlazman_StorePermissionsTable].IsSqlRole = 1)) OR
	
		(@ROLEID = 0 AND dbo.[netsqlazman_StoresTable].StoreId = @STOREID AND dbo.[netsqlazman_ApplicationPermissionsTable].NetSqlAzManFixedServerRole >= @ROLEID AND 
		(SUSER_SNAME(SUSER_SID())=[netsqlazman_ApplicationPermissionsTable].SqlUserOrRole AND [netsqlazman_ApplicationPermissionsTable].IsSqlRole = 0
		OR IS_MEMBER([netsqlazman_ApplicationPermissionsTable].SqlUserOrRole)=1 AND [netsqlazman_ApplicationPermissionsTable].IsSqlRole = 1))))
	SET @RESULT = 1
	ELSE
	SET @RESULT = 0
END
RETURN @RESULT
END
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_CheckApplicationPermissions]    Script Date: 04/01/2014 22:20:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* 
   @ROLEID = { 0 READERS, 1 USERS, 2 MANAGERS}
*/
CREATE FUNCTION [dbo].[netsqlazman_CheckApplicationPermissions](@ApplicationId int, @ROLEID tinyint)
RETURNS bit
AS
BEGIN
DECLARE @RESULT bit
IF @ApplicationId IS NULL OR @ROLEID IS NULL
	SET @RESULT = 0	
ELSE
BEGIN
	IF EXISTS (
		SELECT     dbo.[netsqlazman_ApplicationPermissionsTable].ApplicationId
		FROM         dbo.[netsqlazman_ApplicationsTable] INNER JOIN
		                      dbo.[netsqlazman_StoresTable] ON dbo.[netsqlazman_ApplicationsTable].StoreId = dbo.[netsqlazman_StoresTable].StoreId LEFT OUTER JOIN
		                      dbo.[netsqlazman_StorePermissionsTable] ON dbo.[netsqlazman_StoresTable].StoreId = dbo.[netsqlazman_StorePermissionsTable].StoreId LEFT OUTER JOIN
		                      dbo.[netsqlazman_ApplicationPermissionsTable] ON dbo.[netsqlazman_ApplicationsTable].ApplicationId = dbo.[netsqlazman_ApplicationPermissionsTable].ApplicationId
		WHERE
		IS_MEMBER('db_owner')=1 OR IS_MEMBER('NetSqlAzMan_Administrators')=1 OR 
		(@ROLEID = 0 AND IS_MEMBER('NetSqlAzMan_Readers')=1 OR 
		@ROLEID = 1 AND IS_MEMBER('NetSqlAzMan_Users')=1 OR 
		@ROLEID = 2 AND IS_MEMBER('NetSqlAzMan_Managers')=1) AND
		(
		(dbo.[netsqlazman_ApplicationPermissionsTable].ApplicationId = @ApplicationId AND dbo.[netsqlazman_ApplicationPermissionsTable].NetSqlAzManFixedServerRole >= @ROLEID AND 
		(SUSER_SNAME(SUSER_SID())=[netsqlazman_ApplicationPermissionsTable].SqlUserOrRole AND [netsqlazman_ApplicationPermissionsTable].IsSqlRole = 0
		OR IS_MEMBER([netsqlazman_ApplicationPermissionsTable].SqlUserOrRole)=1 AND [netsqlazman_ApplicationPermissionsTable].IsSqlRole = 1)) OR
	
		dbo.[netsqlazman_ApplicationsTable].ApplicationId = @ApplicationId AND 
		(dbo.[netsqlazman_StorePermissionsTable].StoreId = dbo.[netsqlazman_ApplicationsTable].StoreId AND dbo.[netsqlazman_StorePermissionsTable].NetSqlAzManFixedServerRole >= @ROLEID AND 
		(SUSER_SNAME(SUSER_SID())=[netsqlazman_StorePermissionsTable].SqlUserOrRole AND [netsqlazman_StorePermissionsTable].IsSqlRole = 0 OR
		IS_MEMBER([netsqlazman_StorePermissionsTable].SqlUserOrRole)=1 AND [netsqlazman_StorePermissionsTable].IsSqlRole = 1))

))
	
	SET @RESULT = 1
	ELSE
	SET @RESULT = 0
END
RETURN @RESULT
END
GO
/****** Object:  Table [dbo].[CustomerSettingsGroupMember]    Script Date: 04/01/2014 22:20:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerSettingsGroupMember](
	[CustomerSettingsGroupMemberId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerSettingsGroupId] [int] NOT NULL,
	[CustomerSettingTypeId] [int] NOT NULL,
 CONSTRAINT [PK_CustomerSettingsGroupMember] PRIMARY KEY CLUSTERED 
(
	[CustomerSettingsGroupMemberId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[InitializeCustomerGrids]    Script Date: 04/01/2014 22:20:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[InitializeCustomerGrids]
	@CustomerId int
	As

	SET NOCOUNT ON

	-- For each distinct row in [CustomerGrids] for CustomerId = 0
	DECLARE @Controller nvarchar(128)
	DECLARE @Action nvarchar(128)
	DECLARE @Title nvarchar(255)
	DECLARE @Position int
	DECLARE @OriginalTitle nvarchar(255)
	DECLARE @OriginalPosition int
	DECLARE @IsHidden bit

	DECLARE cursorCustomerGrids cursor for 
		SELECT 
			cgt.[Controller], cgt.[Action], cgtc.Title, cgtc.Position, cgtc.OriginalTitle, cgtc.OriginalPosition, cgtc.IsHidden
		FROM [dbo].[CustomerGridTemplate] cgt
		JOIN [dbo].[CustomerGridTemplateCol] cgtc on cgt.CustomerGridTemplateId = cgtc.CustomerGridTemplateId
		WHERE cgt.IsDefault = 1

	OPEN cursorCustomerGrids
	FETCH NEXT FROM cursorCustomerGrids into @Controller, @Action, @Title, @Position, @OriginalTitle, @OriginalPosition, @IsHidden
	WHILE @@FETCH_STATUS = 0
	BEGIN		
		-- Has this controller/action position already been inserted in [CustomerGrids]?
		IF NOT EXISTS (SELECT * FROM [dbo].[CustomerGrids] 
			WHERE Controller = @Controller AND Action = @Action and Position = @Position and CustomerId = @CustomerId)
		BEGIN
			-- Insert this grid row
			INSERT INTO [dbo].[CustomerGrids] 
				([CustomerId],[Controller],[Action],[Title],[Position],[OriginalTitle],[OriginalPosition], [IsHidden])
			VALUES
				(@CustomerId,@Controller,@Action,@Title,@Position,@OriginalTitle,@OriginalPosition, @IsHidden)
		END

		-- Get next row
		FETCH NEXT FROM cursorCustomerGrids into @Controller, @Action, @Title, @Position, @OriginalTitle, @OriginalPosition, @IsHidden
	END
	CLOSE cursorCustomerGrids
	DEALLOCATE cursorCustomerGrids

	return
GO
/****** Object:  Table [dbo].[netsqlazman_AuthorizationsTable]    Script Date: 04/01/2014 22:20:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[netsqlazman_AuthorizationsTable](
	[AuthorizationId] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[ownerSid] [varbinary](85) NOT NULL,
	[ownerSidWhereDefined] [tinyint] NOT NULL,
	[objectSid] [varbinary](85) NOT NULL,
	[objectSidWhereDefined] [tinyint] NOT NULL,
	[AuthorizationType] [tinyint] NOT NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_Authorizations] PRIMARY KEY CLUSTERED 
(
	[AuthorizationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[netsqlazman_ApplicationGroupMembersTable]    Script Date: 04/01/2014 22:20:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[netsqlazman_ApplicationGroupMembersTable](
	[ApplicationGroupMemberId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationGroupId] [int] NOT NULL,
	[objectSid] [varbinary](85) NOT NULL,
	[WhereDefined] [tinyint] NOT NULL,
	[IsMember] [bit] NOT NULL,
 CONSTRAINT [PK_GroupMembers] PRIMARY KEY CLUSTERED 
(
	[ApplicationGroupMemberId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_Applications]    Script Date: 04/01/2014 22:20:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_Applications] ()
RETURNS TABLE
AS
RETURN
	SELECT * FROM dbo.[netsqlazman_ApplicationsTable]
	WHERE dbo.[netsqlazman_CheckApplicationPermissions](ApplicationId, 0) = 1
GO
/****** Object:  Table [dbo].[netsqlazman_AuthorizationAttributesTable]    Script Date: 04/01/2014 22:20:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[netsqlazman_AuthorizationAttributesTable](
	[AuthorizationAttributeId] [int] IDENTITY(1,1) NOT NULL,
	[AuthorizationId] [int] NOT NULL,
	[AttributeKey] [nvarchar](255) NOT NULL,
	[AttributeValue] [nvarchar](4000) NOT NULL,
 CONSTRAINT [PK_AuthorizationAttributes] PRIMARY KEY CLUSTERED 
(
	[AuthorizationAttributeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_BizRuleDelete]    Script Date: 04/01/2014 22:20:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_BizRuleDelete]
(
	@BizRuleId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT BizRuleId FROM dbo.[netsqlazman_BizRulesTable] WHERE BizRuleId = @BizRuleId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_BizRulesTable] WHERE [BizRuleId] = @BizRuleId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_RevokeStoreAccess]    Script Date: 04/01/2014 22:20:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_RevokeStoreAccess] (
	@StoreId int,
	@SqlUserOrRole sysname,
	@NetSqlAzManFixedServerRole tinyint)
AS
IF EXISTS(SELECT StoreId FROM dbo.[netsqlazman_StoresTable] WHERE StoreId = @StoreId) AND (dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1 AND @NetSqlAzManFixedServerRole BETWEEN 0 AND 1 OR (IS_MEMBER('db_owner')=1 OR IS_MEMBER('NetSqlAzMan_Administrators')=1) AND @NetSqlAzManFixedServerRole = 2)
BEGIN
	IF EXISTS(SELECT * FROM dbo.[netsqlazman_StorePermissionsTable] WHERE StoreId = @StoreId AND SqlUserOrRole = @SqlUserOrRole AND NetSqlAzManFixedServerRole = @NetSqlAzManFixedServerRole)
		DELETE FROM dbo.[netsqlazman_StorePermissionsTable] WHERE StoreId = @StoreId AND SqlUserOrRole = @SqlUserOrRole AND NetSqlAzManFixedServerRole = @NetSqlAzManFixedServerRole
	ELSE
		RAISERROR ('Permission not found. Revoke Store Access ignored.', -1, -1)
END
ELSE
	RAISERROR ('Store NOT Found or Store permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_GrantStoreAccess]    Script Date: 04/01/2014 22:20:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_GrantStoreAccess] (
	@StoreId int,
	@SqlUserOrRole sysname,
	@NetSqlAzManFixedServerRole tinyint)
AS
IF EXISTS(SELECT StoreId FROM dbo.[netsqlazman_StoresTable] WHERE StoreId = @StoreId) AND (dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1 AND @NetSqlAzManFixedServerRole BETWEEN 0 AND 1 OR (IS_MEMBER('db_owner')=1 OR IS_MEMBER('NetSqlAzMan_Administrators')=1) AND @NetSqlAzManFixedServerRole = 2)
BEGIN
	DECLARE @MEMBERUID int
	IF NOT (@NetSqlAzManFixedServerRole BETWEEN 0 AND 2)
	BEGIN
		RAISERROR ('NetSqlAzManFixedServerRole must be 0, 1 or 2 (Reader, User, Manager).', 16, 1)
		RETURN -1
	END
	 -- CHECK MEMBER NAME (ATTEMPT ADDING IMPLICIT ROW FOR NT NAME) --
	DECLARE @IsSqlRoleInt int
	DECLARE @IsNtGroupInt bit
	DECLARE @IsSqlRole bit
	SELECT @MEMBERUID = uid, @IsSqlRoleInt = issqlrole, @IsNtGroupInt = isntgroup  from sysusers where sid = SUSER_SID(@SqlUserOrRole) and isaliased = 0
	IF @IsSqlRoleInt = 1 OR @IsNtGroupInt = 1
		SET @IsSqlRole = 1
	ELSE
		SET @IsSqlRole = 0
	IF @MEMBERUID IS NULL
	BEGIN
		RAISERROR ('Sql User/Role Not Found. Grant Store Access ignored.', -1, -1)
		RETURN 0
	END
	IF EXISTS(SELECT * FROM dbo.[netsqlazman_StorePermissionsTable] WHERE StoreId = @StoreId AND SqlUserOrRole = @SqlUserOrRole AND NetSqlAzManFixedServerRole = @NetSqlAzManFixedServerRole)
		BEGIN
		RAISERROR ('NetSqlAzManFixedServerRole updated.', -1, -1)
		RETURN 0
		END
	ELSE
		BEGIN
		INSERT INTO dbo.[netsqlazman_StorePermissionsTable] (StoreId, SqlUserOrRole, IsSqlRole, NetSqlAzManFixedServerRole) VALUES (@StoreId, @SqlUserOrRole, @IsSqlRole, @NetSqlAzManFixedServerRole)
		RETURN SCOPE_IDENTITY()
		END
END
ELSE
	RAISERROR ('Store NOT Found or Store permission denied.', 16, 1)
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_Stores]    Script Date: 04/01/2014 22:20:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_Stores] ()
RETURNS TABLE 
AS
RETURN
	SELECT dbo.[netsqlazman_StoresTable].* FROM dbo.[netsqlazman_StoresTable]
	WHERE dbo.[netsqlazman_CheckStorePermissions]([netsqlazman_StoresTable].StoreId, 0) = 1
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StoreGroupUpdate]    Script Date: 04/01/2014 22:20:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StoreGroupUpdate]
(
	@StoreId int,
	@objectSid varbinary(85),
	@Name nvarchar(255),
	@Description nvarchar(1024),
	@LDapQuery nvarchar(4000),
	@GroupType tinyint,
	@Original_StoreGroupId int
)
AS
IF dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1
	UPDATE [dbo].[netsqlazman_StoreGroupsTable] SET [objectSid] = @objectSid, [Name] = @Name, [Description] = @Description, [LDapQuery] = @LDapQuery, [GroupType] = @GroupType WHERE [StoreGroupId] = @Original_StoreGroupId AND StoreId = @StoreId
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StoreGroupInsert]    Script Date: 04/01/2014 22:20:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StoreGroupInsert]
(
	@StoreId int,
	@objectSid varbinary(85),
	@Name nvarchar(255),
	@Description nvarchar(1024),
	@LDapQuery nvarchar(4000),
	@GroupType tinyint
)
AS
IF dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1
BEGIN
	INSERT INTO [dbo].[netsqlazman_StoreGroupsTable] ([StoreId], [objectSid], [Name], [Description], [LDapQuery], [GroupType]) VALUES (@StoreId, @objectSid, @Name, @Description, @LDapQuery, @GroupType);
	RETURN SCOPE_IDENTITY()
END
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StoreGroupDelete]    Script Date: 04/01/2014 22:20:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StoreGroupDelete]
(
	@Original_StoreGroupId int,
	@StoreId int
)
AS
IF dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_StoreGroupsTable] WHERE [StoreGroupId] = @Original_StoreGroupId AND [StoreId] = @StoreId
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[sp_RefreshAllCustomerGrids]    Script Date: 04/01/2014 22:20:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[sp_RefreshAllCustomerGrids]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


--run the master file to update all the Grid Template and Columns
EXEC  sp_InsertAllCustomGridOptions;

	declare @CustomerId int;
	DECLARE @MasterController nvarchar(128);
	DECLARE @MasterAction nvarchar(128);
	declare @CustomerGridTemplateMapId int;
		
	-- disctinct controller/action in the customerGridTemplate table, insert the version 0 into the mapping file if it doesnt exist for the customer.
	DECLARE cursorCustomerGrids cursor for SELECT distinct Controller ,Action  FROM CustomerGridTemplate;


-----------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------This section will update each customers template map to have all the valid options based on the CustomerTemplateMap table ---------------------
-----------------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------- FOREACH unique set of controller action -------------------------------------------------------------
OPEN cursorCustomerGrids
FETCH NEXT FROM cursorCustomerGrids into @MasterController, @MasterAction
WHILE @@FETCH_STATUS = 0
BEGIN		

	------------------------------------- for each customer, -------------------------------------------------------------------------
	--list of customer profiles. Each on in the system needs to be refreshed	
	DECLARE customerIds cursor for SELECT CustomerId  FROM CustomerProfile where CustomerId != 0;
	OPEN customerIds
	FETCH NEXT FROM customerIds into @CustomerId
	WHILE @@FETCH_STATUS = 0
	BEGIN		
	
		-- if they dont have a templateMap, insert the version 0 of one for each action / controller.
		IF NOT EXISTS  (select customergridtemplatemapid from CustomerGridTemplateMap 
			where CustomerId = @CustomerId  and Controller = @MasterController  and Action = @MasterAction)
			begin
			EXEC  sp_InsertCustomerGridTemplateMapping @CustomerId = @CustomerId, @Controller = @MasterController, @Action = @MasterAction, @Version = 0;
			end
		-- Get next row
	FETCH NEXT FROM customerIds into @CustomerId
	END
	CLOSE customerIds
	DEALLOCATE customerIds
-- Get next row
FETCH NEXT FROM cursorCustomerGrids into @MasterController, @MasterAction
END
CLOSE cursorCustomerGrids
DEALLOCATE cursorCustomerGrids

-----------------------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------Once they have all the correct options, update the customer grids with the mapping data-------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------------------------------

--foreach customer
--call the sp_InsertCustomerGridsFromTemplateMap
	DECLARE customers cursor for SELECT CustomerId  FROM CustomerProfile ;
	OPEN customers
	FETCH NEXT FROM customers into @CustomerId
	WHILE @@FETCH_STATUS = 0
	BEGIN	

		--call the stored proc to update the customer grids for this customer based on the templateMap
			EXEC  sp_InsertCustomerGridsFromTemplateMap @CustomerId = @CustomerId
	-- Get next row
	FETCH NEXT FROM customers into @CustomerId
	END
	CLOSE customers
	DEALLOCATE customers
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertCustomerGridTemplateMappingDefaults]    Script Date: 04/01/2014 22:20:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[sp_InsertCustomerGridTemplateMappingDefaults]
      @CustomerId int
      
AS
BEGIN
      -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SET NOCOUNT ON;

      DECLARE @MasterController nvarchar(128);
      DECLARE @MasterAction nvarchar(128);



--delete them all for this customer
delete from dbo.CustomerGridTemplateMap where CustomerId = @CustomerId ;


--foreach disctinct controller/action in the customerGridTemplate table, insert the version 0 into the mapping file.
      DECLARE cursorCustomerGrids cursor for SELECT distinct Controller ,Action  FROM CustomerGridTemplate;

OPEN cursorCustomerGrids
      FETCH NEXT FROM cursorCustomerGrids into @MasterController, @MasterAction
      WHILE @@FETCH_STATUS = 0
      BEGIN       
      EXEC  sp_InsertCustomerGridTemplateMapping @CustomerId = @CustomerId, @Controller = @MasterController, @Action = @MasterAction, @Version = 0


            -- Get next row
            FETCH NEXT FROM cursorCustomerGrids into @MasterController, @MasterAction
      END
      CLOSE cursorCustomerGrids
      DEALLOCATE cursorCustomerGrids



Return  SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StoreUpdate]    Script Date: 04/01/2014 22:20:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StoreUpdate]
(
	@Name nvarchar(255),
	@Description nvarchar(1024),
	@Original_StoreId int
)
AS
IF EXISTS(Select StoreId FROM dbo.[netsqlazman_Stores]() WHERE StoreId = @Original_StoreId) AND dbo.[netsqlazman_CheckStorePermissions](@Original_StoreId, 2) = 1
	UPDATE [dbo].[netsqlazman_StoresTable] SET [Name] = @Name, [Description] = @Description WHERE [StoreId] = @Original_StoreId
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_StorePermissions]    Script Date: 04/01/2014 22:20:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_StorePermissions]()
RETURNS TABLE 
AS  
RETURN
	SELECT     dbo.[netsqlazman_StorePermissionsTable].*
	FROM         dbo.[netsqlazman_StorePermissionsTable] INNER JOIN
	                      dbo.[netsqlazman_Stores]() Stores ON dbo.[netsqlazman_StorePermissionsTable].StoreId = Stores.StoreId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StorePermissionInsert]    Script Date: 04/01/2014 22:20:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StorePermissionInsert]
(
	@StoreId int,
	@SqlUserOrRole nvarchar(128),
	@IsSqlRole bit,
	@NetSqlAzManFixedServerRole tinyint
)
AS
IF EXISTS(SELECT StoreId FROM dbo.[netsqlazman_Stores]() WHERE StoreId = @StoreId) AND dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1
BEGIN
	INSERT INTO dbo.[netsqlazman_StorePermissionsTable] (StoreId, SqlUserOrRole, IsSqlRole, NetSqlAzManFixedServerRole) VALUES (@StoreId, @SqlUserOrRole, @IsSqlRole, @NetSqlAzManFixedServerRole)
	RETURN SCOPE_IDENTITY()
END
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StorePermissionDelete]    Script Date: 04/01/2014 22:20:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StorePermissionDelete]
(
	@StorePermissionId int,
	@StoreId int
)
AS
IF EXISTS(SELECT StoreId FROM dbo.[netsqlazman_Stores]() WHERE StoreId = @StoreId) AND dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1
	DELETE FROM dbo.[netsqlazman_StorePermissionsTable] WHERE StorePermissionId = @StorePermissionId AND StoreId = @StoreId
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_StoreGroups]    Script Date: 04/01/2014 22:20:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_StoreGroups] ()
RETURNS TABLE
AS
RETURN
	SELECT     dbo.[netsqlazman_StoreGroupsTable].*
	FROM         dbo.[netsqlazman_Stores]() Stores INNER JOIN
	                      dbo.[netsqlazman_StoreGroupsTable] ON Stores.StoreId = dbo.[netsqlazman_StoreGroupsTable].StoreId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StoreDelete]    Script Date: 04/01/2014 22:20:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StoreDelete]
(
	@Original_StoreId int
)
AS
IF EXISTS(Select StoreId FROM dbo.[netsqlazman_Stores]() WHERE StoreId = @Original_StoreId) AND dbo.[netsqlazman_CheckStorePermissions](@Original_StoreId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_StoresTable] WHERE [StoreId] = @Original_StoreId
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_StoreAttributes]    Script Date: 04/01/2014 22:20:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_StoreAttributes] ()
RETURNS TABLE
AS
RETURN
	SELECT     dbo.[netsqlazman_StoreAttributesTable].*
	FROM         dbo.[netsqlazman_StoreAttributesTable] INNER JOIN
	                      dbo.[netsqlazman_Stores]() Stores ON dbo.[netsqlazman_StoreAttributesTable].StoreId = Stores.StoreId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StoreAttributeInsert]    Script Date: 04/01/2014 22:20:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StoreAttributeInsert]
(
	@StoreId int,
	@AttributeKey nvarchar(255),
	@AttributeValue nvarchar(4000)
)
AS
IF EXISTS(Select StoreId FROM dbo.[netsqlazman_Stores]() WHERE StoreId = @StoreId) AND dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1
BEGIN
	INSERT INTO [dbo].[netsqlazman_StoreAttributesTable] ([StoreId], [AttributeKey], [AttributeValue]) VALUES (@StoreId, @AttributeKey, @AttributeValue);
	RETURN SCOPE_IDENTITY()
END
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_Items]    Script Date: 04/01/2014 22:20:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_Items] ()
RETURNS TABLE
AS
RETURN
	SELECT     dbo.[netsqlazman_ItemsTable].*
	FROM         dbo.[netsqlazman_ItemsTable] INNER JOIN
	                      dbo.[netsqlazman_Applications]() Applications ON dbo.[netsqlazman_ItemsTable].ApplicationId = Applications.ApplicationId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ItemInsert]    Script Date: 04/01/2014 22:20:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ItemInsert]
(
	@Name nvarchar(255),
	@Description nvarchar(1024),
	@ItemType tinyint,
	@BizRuleId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ApplicationId FROM dbo.[netsqlazman_Applications]() WHERE ApplicationId = @ApplicationId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
BEGIN
	INSERT INTO [dbo].[netsqlazman_ItemsTable] ([ApplicationId], [Name], [Description], [ItemType], [BizRuleId]) VALUES (@ApplicationId, @Name, @Description, @ItemType, @BizRuleId)
	RETURN SCOPE_IDENTITY()
END
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_GrantApplicationAccess]    Script Date: 04/01/2014 22:20:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_GrantApplicationAccess] (
	@ApplicationId int,
	@SqlUserOrRole sysname,
	@NetSqlAzManFixedServerRole tinyint)
AS
DECLARE @StoreId int
SET @StoreId = (SELECT StoreId FROM dbo.[netsqlazman_Applications]() WHERE ApplicationId = @ApplicationId)
IF EXISTS(SELECT ApplicationId FROM dbo.[netsqlazman_ApplicationsTable] WHERE ApplicationId = @ApplicationId) AND (dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1 AND @NetSqlAzManFixedServerRole BETWEEN 0 AND 1 OR dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1 AND @NetSqlAzManFixedServerRole = 2)
BEGIN
	DECLARE @MEMBERUID int
	IF NOT (@NetSqlAzManFixedServerRole BETWEEN 0 AND 2)
	BEGIN
		RAISERROR ('NetSqlAzManFixedServerRole must be 0, 1 or 2 (Reader, User, Manager).', 16, 1)
		RETURN -1
	END
	 -- CHECK MEMBER NAME (ATTEMPT ADDING IMPLICIT ROW FOR NT NAME) --
	DECLARE @IsSqlRoleInt int
	DECLARE @IsNtGroupInt bit
	DECLARE @IsSqlRole bit
	SELECT @MEMBERUID = uid, @IsSqlRoleInt = issqlrole, @IsNtGroupInt = isntgroup  from sysusers where sid = SUSER_SID(@SqlUserOrRole) and isaliased = 0
	IF @IsSqlRoleInt = 1 OR @IsNtGroupInt = 1
		SET @IsSqlRole = 1
	ELSE
		SET @IsSqlRole = 0
	IF @MEMBERUID IS NULL
	BEGIN
		RAISERROR ('Sql User/Role Not Found. Grant Store Access ignored.', -1, -1)
		RETURN 0
	END
	IF EXISTS(SELECT * FROM dbo.[netsqlazman_ApplicationPermissionsTable] WHERE ApplicationId = @ApplicationId AND SqlUserOrRole = @SqlUserOrRole AND NetSqlAzManFixedServerRole = @NetSqlAzManFixedServerRole)
		BEGIN
		RAISERROR ('NetSqlAzManFixedServerRole updated.', -1, -1)
		RETURN 0
		END
	ELSE
		BEGIN
		INSERT INTO dbo.[netsqlazman_ApplicationPermissionsTable] (ApplicationId, SqlUserOrRole, IsSqlRole, NetSqlAzManFixedServerRole) VALUES (@ApplicationId, @SqlUserOrRole, @IsSqlRole, @NetSqlAzManFixedServerRole)
		RETURN SCOPE_IDENTITY()
		END
END
ELSE
	RAISERROR ('Application NOT Found or Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_RevokeApplicationAccess]    Script Date: 04/01/2014 22:20:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_RevokeApplicationAccess] (
	@ApplicationId int,
	@SqlUserOrRole sysname,
	@NetSqlAzManFixedServerRole tinyint)
AS
DECLARE @StoreId int
SET @StoreId = (SELECT StoreId FROM dbo.[netsqlazman_Applications]() WHERE ApplicationId = @ApplicationId)
IF EXISTS(SELECT ApplicationId FROM dbo.[netsqlazman_ApplicationsTable] WHERE ApplicationId = @ApplicationId) AND (dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1 AND @NetSqlAzManFixedServerRole BETWEEN 0 AND 1 OR dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1 AND @NetSqlAzManFixedServerRole = 2)
BEGIN
	IF EXISTS(SELECT * FROM dbo.[netsqlazman_ApplicationPermissionsTable] WHERE ApplicationId = @ApplicationId AND SqlUserOrRole = @SqlUserOrRole AND NetSqlAzManFixedServerRole = @NetSqlAzManFixedServerRole)
		DELETE FROM dbo.[netsqlazman_ApplicationPermissionsTable] WHERE ApplicationId = @ApplicationId AND SqlUserOrRole = @SqlUserOrRole AND NetSqlAzManFixedServerRole = @NetSqlAzManFixedServerRole
	ELSE
		RAISERROR ('Permission not found. Revoke Application Access ignored.', -1, -1)
END
ELSE
	RAISERROR ('Application NOT Found or Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationUpdate]    Script Date: 04/01/2014 22:20:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationUpdate]
(
	@Name nvarchar(255),
	@Description nvarchar(1024),
	@Original_ApplicationId int
)
AS
IF EXISTS(SELECT ApplicationId FROM dbo.[netsqlazman_Applications]() WHERE ApplicationId = @Original_ApplicationId) AND dbo.[netsqlazman_CheckApplicationPermissions](@Original_ApplicationId, 2) = 1
	UPDATE [dbo].[netsqlazman_ApplicationsTable] SET [Name] = @Name, [Description] = @Description WHERE [ApplicationId] = @Original_ApplicationId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  View [dbo].[netsqlazman_ApplicationsView]    Script Date: 04/01/2014 22:20:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[netsqlazman_ApplicationsView]
AS
SELECT     [netsqlazman_Stores].StoreId, [netsqlazman_Stores].Name AS StoreName, [netsqlazman_Stores].Description AS StoreDescription, [netsqlazman_Applications].ApplicationId, [netsqlazman_Applications].Name AS ApplicationName, 
                      [netsqlazman_Applications].Description AS ApplicationDescription
FROM         dbo.[netsqlazman_Applications]() [netsqlazman_Applications] INNER JOIN
                      dbo.[netsqlazman_Stores]() [netsqlazman_Stores] ON [netsqlazman_Applications].StoreId = [netsqlazman_Stores].StoreId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationDelete]    Script Date: 04/01/2014 22:20:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationDelete]
(
	@StoreId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ApplicationId FROM dbo.[netsqlazman_Applications]() WHERE ApplicationId = @ApplicationId) AND dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_ApplicationsTable] WHERE [ApplicationId] = @ApplicationId AND [StoreId] = @StoreId
ELSE
	RAISERROR ('Store permission denied', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationGroupInsert]    Script Date: 04/01/2014 22:20:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationGroupInsert]
(
	@ApplicationId int,
	@objectSid varbinary(85),
	@Name nvarchar(255),
	@Description nvarchar(1024),
	@LDapQuery nvarchar(4000),
	@GroupType tinyint
)
AS
IF EXISTS(SELECT ApplicationId FROM dbo.[netsqlazman_Applications]() WHERE ApplicationId = @ApplicationId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
BEGIN
	INSERT INTO [dbo].[netsqlazman_ApplicationGroupsTable] ([ApplicationId], [objectSid], [Name], [Description], [LDapQuery], [GroupType]) VALUES (@ApplicationId, @objectSid, @Name, @Description, @LDapQuery, @GroupType)
	RETURN SCOPE_IDENTITY()
END
ELSE	
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_ApplicationGroups]    Script Date: 04/01/2014 22:20:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_ApplicationGroups] ()
RETURNS TABLE
AS
RETURN
	SELECT     dbo.[netsqlazman_ApplicationGroupsTable].*
	FROM         dbo.[netsqlazman_ApplicationGroupsTable] INNER JOIN
	                      dbo.[netsqlazman_Applications]() Applications ON dbo.[netsqlazman_ApplicationGroupsTable].ApplicationId = Applications.ApplicationId
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_ApplicationAttributes]    Script Date: 04/01/2014 22:20:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_ApplicationAttributes] ()
RETURNS TABLE
AS
RETURN 
	SELECT     dbo.[netsqlazman_ApplicationAttributesTable].*
	FROM         dbo.[netsqlazman_ApplicationAttributesTable] INNER JOIN
	                      dbo.[netsqlazman_Applications]() Applications ON dbo.[netsqlazman_ApplicationAttributesTable].ApplicationId = Applications.ApplicationId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationAttributeInsert]    Script Date: 04/01/2014 22:20:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationAttributeInsert]
(
	@ApplicationId int,
	@AttributeKey nvarchar(255),
	@AttributeValue nvarchar(4000)
)
AS
IF EXISTS(SELECT ApplicationId FROM dbo.[netsqlazman_Applications]() WHERE ApplicationId = @ApplicationId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
BEGIN
	INSERT INTO [dbo].[netsqlazman_ApplicationAttributesTable] ([ApplicationId], [AttributeKey], [AttributeValue]) VALUES (@ApplicationId, @AttributeKey, @AttributeValue)
	RETURN SCOPE_IDENTITY()
END
ELSE
BEGIN
	RAISERROR ('Application Permission denied.', 16, 1)
END
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_ApplicationPermissions]    Script Date: 04/01/2014 22:20:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_ApplicationPermissions]()
RETURNS TABLE 
AS  
RETURN
	SELECT     dbo.[netsqlazman_ApplicationPermissionsTable].*
	FROM         dbo.[netsqlazman_ApplicationPermissionsTable] INNER JOIN
	                      dbo.[netsqlazman_Applications]() Applications ON dbo.[netsqlazman_ApplicationPermissionsTable].ApplicationId = Applications.ApplicationId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationPermissionInsert]    Script Date: 04/01/2014 22:20:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationPermissionInsert]
(
	@ApplicationId int,
	@SqlUserOrRole nvarchar(128),
	@IsSqlRole bit,
	@NetSqlAzManFixedServerRole tinyint
)
AS
IF EXISTS(SELECT ApplicationId FROM dbo.[netsqlazman_Applications]() WHERE ApplicationId = @ApplicationId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
BEGIN
	INSERT INTO dbo.[netsqlazman_ApplicationPermissionsTable] (ApplicationId, SqlUserOrRole, IsSqlRole, NetSqlAzManFixedServerRole) VALUES (@ApplicationId, @SqlUserOrRole, @IsSqlRole, @NetSqlAzManFixedServerRole)
	RETURN SCOPE_IDENTITY()
END
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationPermissionDelete]    Script Date: 04/01/2014 22:20:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationPermissionDelete]
(
	@ApplicationPermissionId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ApplicationId FROM dbo.[netsqlazman_Applications]() WHERE ApplicationId = @ApplicationId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	DELETE FROM dbo.[netsqlazman_ApplicationPermissionsTable] WHERE ApplicationPermissionId = @ApplicationPermissionId AND ApplicationId = @ApplicationId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationInsert]    Script Date: 04/01/2014 22:20:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationInsert]
(
	@StoreId int,
	@Name nvarchar(255),
	@Description nvarchar(1024)
)
AS
IF EXISTS(SELECT StoreId FROM dbo.[netsqlazman_Stores]() WHERE StoreId = @StoreId) AND dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1
BEGIN
	INSERT INTO [dbo].[netsqlazman_ApplicationsTable] ([StoreId], [Name], [Description]) VALUES (@StoreId, @Name, @Description);
	RETURN SCOPE_IDENTITY()
END
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_Authorizations]    Script Date: 04/01/2014 22:20:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_Authorizations]()
RETURNS TABLE
AS
RETURN
	SELECT     dbo.[netsqlazman_AuthorizationsTable].*
	FROM         dbo.[netsqlazman_AuthorizationsTable] INNER JOIN
	                      dbo.[netsqlazman_Items]() Items ON dbo.[netsqlazman_AuthorizationsTable].ItemId = Items.ItemId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_AuthorizationInsert]    Script Date: 04/01/2014 22:20:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_AuthorizationInsert]
(
	@ItemId int,
	@ownerSid varbinary(85),
	@ownerSidWhereDefined tinyint,
	@objectSid varbinary(85),
	@objectSidWhereDefined tinyint,
	@AuthorizationType tinyint,
	@ValidFrom datetime,
	@ValidTo datetime,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ItemId FROM dbo.[netsqlazman_Items]() WHERE ItemId = @ItemId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
BEGIN
	INSERT INTO [dbo].[netsqlazman_AuthorizationsTable] ([ItemId], [ownerSid], [ownerSidWhereDefined], [objectSid], [objectSidWhereDefined], [AuthorizationType], [ValidFrom], [ValidTo]) VALUES (@ItemId, @ownerSid, @ownerSidWhereDefined, @objectSid, @objectSidWhereDefined, @AuthorizationType, @ValidFrom, @ValidTo)
	RETURN SCOPE_IDENTITY()
END
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationGroupUpdate]    Script Date: 04/01/2014 22:20:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationGroupUpdate]
(
	@objectSid varbinary(85),
	@Name nvarchar(255),
	@Description nvarchar(1024),
	@LDapQuery nvarchar(4000),
	@GroupType tinyint,
	@Original_ApplicationGroupId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ApplicationGroupId FROM dbo.[netsqlazman_ApplicationGroups]() WHERE ApplicationGroupId = @Original_ApplicationGroupId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	UPDATE [dbo].[netsqlazman_ApplicationGroupsTable] SET [objectSid] = @objectSid, [Name] = @Name, [Description] = @Description, [LDapQuery] = @LDapQuery, [GroupType] = @GroupType WHERE [ApplicationGroupId] = @Original_ApplicationGroupId AND [ApplicationId] = @ApplicationId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_ApplicationGroupMembers]    Script Date: 04/01/2014 22:20:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_ApplicationGroupMembers] ()
RETURNS TABLE
AS
RETURN
	SELECT     dbo.[netsqlazman_ApplicationGroupMembersTable].*
	FROM         dbo.[netsqlazman_ApplicationGroups]() ApplicationGroups INNER JOIN
	                      dbo.[netsqlazman_ApplicationGroupMembersTable] ON ApplicationGroups.ApplicationGroupId = dbo.[netsqlazman_ApplicationGroupMembersTable].ApplicationGroupId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationGroupMemberInsert]    Script Date: 04/01/2014 22:20:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationGroupMemberInsert]
(
	@ApplicationGroupId int,
	@objectSid varbinary(85),
	@WhereDefined tinyint,
	@IsMember bit,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ApplicationGroupId FROM dbo.[netsqlazman_ApplicationGroups]() WHERE ApplicationGroupId = @ApplicationGroupId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
BEGIN
	INSERT INTO [dbo].[netsqlazman_ApplicationGroupMembersTable] ([ApplicationGroupId], [objectSid], [WhereDefined], [IsMember]) VALUES (@ApplicationGroupId, @objectSid, @WhereDefined, @IsMember)
	RETURN SCOPE_IDENTITY()
END
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationAttributeDelete]    Script Date: 04/01/2014 22:20:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationAttributeDelete]
(
	@ApplicationId int,
	@ApplicationAttributeId int
)
AS
IF EXISTS(SELECT ApplicationAttributeId FROM dbo.[netsqlazman_ApplicationAttributes]() WHERE ApplicationAttributeId = @ApplicationAttributeId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_ApplicationAttributesTable] WHERE [ApplicationAttributeId] = @ApplicationAttributeId AND [ApplicationId] = @ApplicationId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationGroupDelete]    Script Date: 04/01/2014 22:20:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationGroupDelete]
(
	@ApplicationGroupId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ApplicationGroupId FROM dbo.[netsqlazman_ApplicationGroups]() WHERE ApplicationGroupId = @ApplicationGroupId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_ApplicationGroupsTable] WHERE [ApplicationGroupId] = @ApplicationGroupId AND [ApplicationId] = @ApplicationId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationAttributeUpdate]    Script Date: 04/01/2014 22:20:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationAttributeUpdate]
(
	@ApplicationId int,
	@AttributeKey nvarchar(255),
	@AttributeValue nvarchar(4000),
	@Original_ApplicationAttributeId int
)
AS
IF EXISTS(SELECT ApplicationAttributeId FROM dbo.[netsqlazman_ApplicationAttributes]() WHERE ApplicationAttributeId = @Original_ApplicationAttributeId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	UPDATE [dbo].[netsqlazman_ApplicationAttributesTable] SET [AttributeKey] = @AttributeKey, [AttributeValue] = @AttributeValue WHERE [ApplicationAttributeId] = @Original_ApplicationAttributeId AND [ApplicationId] = @ApplicationId
ELSE
	RAISERROR ('Applicaction Permission denied.', 16, 1)
GO
/****** Object:  View [dbo].[netsqlazman_ApplicationAttributesView]    Script Date: 04/01/2014 22:20:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[netsqlazman_ApplicationAttributesView]
AS
SELECT     [netsqlazman_Applications].ApplicationId, [netsqlazman_Applications].StoreId, [netsqlazman_Applications].Name, [netsqlazman_Applications].Description, ApplicationAttributes.ApplicationAttributeId, 
                      ApplicationAttributes.AttributeKey, ApplicationAttributes.AttributeValue
FROM         dbo.[netsqlazman_Applications]() [netsqlazman_Applications] INNER JOIN
                      dbo.[netsqlazman_ApplicationAttributes]() ApplicationAttributes ON [netsqlazman_Applications].ApplicationId = ApplicationAttributes.ApplicationId
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_BizRules]    Script Date: 04/01/2014 22:20:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_BizRules]()
RETURNS TABLE
AS
RETURN
	SELECT     dbo.[netsqlazman_BizRulesTable].*
	FROM         dbo.[netsqlazman_BizRulesTable] INNER JOIN
	                      dbo.[netsqlazman_Items]() Items ON dbo.[netsqlazman_BizRulesTable].BizRuleId = Items.BizRuleId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_CreateDelegate]    Script Date: 04/01/2014 22:20:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  Stored Procedure dbo.CreateDelegate    Script Date: 19/05/2006 19.11.19 ******/
CREATE PROCEDURE [dbo].[netsqlazman_CreateDelegate](@ITEMID INT, @OWNERSID VARBINARY(85), @OWNERSIDWHEREDEFINED TINYINT, @DELEGATEDUSERSID VARBINARY(85), @SIDWHEREDEFINED TINYINT, @AUTHORIZATIONTYPE TINYINT, @VALIDFROM DATETIME, @VALIDTO DATETIME, @AUTHORIZATIONID INT OUTPUT)
AS
DECLARE @ApplicationId int
SELECT @ApplicationId = ApplicationId FROM dbo.[netsqlazman_Items]() WHERE ItemId = @ItemId
IF @ApplicationId IS NOT NULL AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 1) = 1
BEGIN
	INSERT INTO dbo.[netsqlazman_AuthorizationsTable] (ItemId, ownerSid, ownerSidWhereDefined, objectSid, objectSidWhereDefined, AuthorizationType, ValidFrom, ValidTo)
		VALUES (@ITEMID, @OWNERSID, @OWNERSIDWHEREDEFINED, @DELEGATEDUSERSID, @SIDWHEREDEFINED, @AUTHORIZATIONTYPE, @VALIDFROM, @VALIDTO)
	SET @AUTHORIZATIONID = SCOPE_IDENTITY()
END
ELSE
	RAISERROR ('Item NOT Found or Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ClearBizRule]    Script Date: 04/01/2014 22:20:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ClearBizRule]
(
	@ItemId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ItemId FROM dbo.[netsqlazman_Items]() WHERE ItemId = @ItemId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	UPDATE [dbo].[netsqlazman_ItemsTable] SET BizRuleId = NULL WHERE [ItemId] = @ItemId AND [ApplicationId] = @ApplicationId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ItemsHierarchyInsert]    Script Date: 04/01/2014 22:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ItemsHierarchyInsert]
(
	@ItemId int,
	@MemberOfItemId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ItemId FROM dbo.[netsqlazman_Items]() WHERE ItemId = @ItemId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
BEGIN
	INSERT INTO [dbo].[netsqlazman_ItemsHierarchyTable] ([ItemId], [MemberOfItemId]) VALUES (@ItemId, @MemberOfItemId)
	RETURN SCOPE_IDENTITY()
END
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ItemsHierarchyDelete]    Script Date: 04/01/2014 22:20:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ItemsHierarchyDelete]
(
	@ItemId int,
	@MemberOfItemId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ItemId FROM dbo.[netsqlazman_Items]() WHERE ItemId = @ItemId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_ItemsHierarchyTable] WHERE [ItemId] = @ItemId AND [MemberOfItemId] = @MemberOfItemId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_ItemsHierarchy]    Script Date: 04/01/2014 22:20:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_ItemsHierarchy] ()
RETURNS TABLE
AS
RETURN
	SELECT     dbo.[netsqlazman_ItemsHierarchyTable].*
	FROM         dbo.[netsqlazman_ItemsHierarchyTable] INNER JOIN
	                      dbo.[netsqlazman_Items]() Items ON dbo.[netsqlazman_ItemsHierarchyTable].ItemId = Items.ItemId
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_ItemAttributes]    Script Date: 04/01/2014 22:20:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_ItemAttributes] ()
RETURNS TABLE
AS
RETURN
	SELECT     dbo.[netsqlazman_ItemAttributesTable].*
	FROM         dbo.[netsqlazman_ItemAttributesTable] INNER JOIN
	                      dbo.[netsqlazman_Items]() Items ON dbo.[netsqlazman_ItemAttributesTable].ItemId = Items.ItemId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ItemAttributeInsert]    Script Date: 04/01/2014 22:20:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ItemAttributeInsert]
(
	@ItemId int,
	@AttributeKey nvarchar(255),
	@AttributeValue nvarchar(4000),
	@ApplicationId int
)
AS
IF EXISTS(SELECT ItemId FROM dbo.[netsqlazman_Items]() WHERE ItemId = @ItemId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
BEGIN
	INSERT INTO [dbo].[netsqlazman_ItemAttributesTable] ([ItemId], [AttributeKey], [AttributeValue]) VALUES (@ItemId, @AttributeKey, @AttributeValue)
	RETURN SCOPE_IDENTITY()
END
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ReloadBizRule]    Script Date: 04/01/2014 22:20:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ReloadBizRule]
(
	@ItemId int,
	@BizRuleId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ItemId FROM dbo.[netsqlazman_Items]() WHERE ItemId = @ItemId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	UPDATE [dbo].[netsqlazman_ItemsTable] SET BizRuleId = @BizRuleId WHERE [ItemId] = @ItemId AND [ApplicationId] = @ApplicationId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ItemUpdate]    Script Date: 04/01/2014 22:20:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ItemUpdate]
(
	@Name nvarchar(255),
	@Description nvarchar(1024),
	@ItemType tinyint,
	@Original_ItemId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ItemId FROM dbo.[netsqlazman_Items]() WHERE ItemId = @Original_ItemId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	UPDATE [dbo].[netsqlazman_ItemsTable] SET [Name] = @Name, [Description] = @Description, [ItemType] = @ItemType WHERE [ItemId] = @Original_ItemId AND [ApplicationId] = @ApplicationId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_GetStoreGroupSidMembers]    Script Date: 04/01/2014 22:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_GetStoreGroupSidMembers](@ISMEMBER BIT, @GROUPOBJECTSID VARBINARY(85), @NETSQLAZMANMODE bit, @LDAPPATH nvarchar(4000), @member_cur CURSOR VARYING OUTPUT)
AS
DECLARE @RESULT TABLE (objectSid VARBINARY(85))
DECLARE @GROUPID INT
DECLARE @GROUPTYPE TINYINT
DECLARE @LDAPQUERY nvarchar(4000)
DECLARE @sub_members_cur CURSOR
DECLARE @OBJECTSID VARBINARY(85)
SELECT @GROUPID = StoreGroupId, @GROUPTYPE = GroupType, @LDAPQUERY = LDapQuery FROM dbo.[netsqlazman_StoreGroups]() WHERE objectSid = @GROUPOBJECTSID
IF @GROUPTYPE = 0 -- BASIC
BEGIN
	--memo: WhereDefined can be:0 - Store; 1 - Application; 2 - LDAP; 3 - Local; 4 - Database
	-- Windows SIDs
	INSERT INTO @RESULT (objectSid) 
	SELECT objectSid 
	FROM dbo.[netsqlazman_StoreGroupMembersTable]
	WHERE 
	StoreGroupId = @GROUPID AND IsMember = @ISMEMBER AND
	((@NETSQLAZMANMODE = 0 AND (WhereDefined = 2 OR WhereDefined = 4)) OR (@NETSQLAZMANMODE = 1 AND WhereDefined BETWEEN 2 AND 4))
	-- Store Groups Members
	DECLARE @MemberObjectSid VARBINARY(85)
	DECLARE @MemberType bit
	DECLARE @NotMemberType bit
	DECLARE nested_Store_groups_cur CURSOR LOCAL FAST_FORWARD FOR
		SELECT objectSid, IsMember FROM dbo.[netsqlazman_StoreGroupMembersTable] WHERE StoreGroupId = @GROUPID AND WhereDefined = 0
	
	OPEN nested_Store_groups_cur
	FETCH NEXT FROM nested_Store_groups_cur INTO @MemberObjectSid, @MemberType
	WHILE @@FETCH_STATUS = 0
	BEGIN
	        -- recursive call
		IF @ISMEMBER = 1
		BEGIN
			IF @MemberType = 0 
				SET @NotMemberType = 0
			ELSE
				SET @NotMemberType = 1
		END
		ELSE
		BEGIN
			IF @MemberType = 0 
				SET @NotMemberType = 1
			ELSE
				SET @NotMemberType = 0
		END
		EXEC dbo.[netsqlazman_GetStoreGroupSidMembers] @NotMemberType, @MemberObjectSid, @NETSQLAZMANMODE, @LDAPPATH, @sub_members_cur OUTPUT
		FETCH NEXT FROM @sub_members_cur INTO @OBJECTSID
		WHILE @@FETCH_STATUS=0
		BEGIN
			INSERT INTO @RESULT VALUES (@OBJECTSID)
			FETCH NEXT FROM @sub_members_cur INTO @OBJECTSID
		END		
		CLOSE @sub_members_cur
		DEALLOCATE @sub_members_cur	

		FETCH NEXT FROM nested_Store_groups_cur INTO @MemberObjectSid, @MemberType
	END
	CLOSE nested_Store_groups_cur
	DEALLOCATE nested_Store_groups_cur
END
ELSE IF @GROUPTYPE = 1 AND @ISMEMBER = 1 -- LDAP QUERY
BEGIN
	EXEC dbo.[netsqlazman_ExecuteLDAPQuery] @LDAPPATH, @LDAPQUERY, @sub_members_cur OUTPUT
	FETCH NEXT FROM @sub_members_cur INTO @OBJECTSID
	WHILE @@FETCH_STATUS=0
	BEGIN
		INSERT INTO @RESULT (objectSid) VALUES (@OBJECTSID)
		FETCH NEXT FROM @sub_members_cur INTO @OBJECTSID
	END
	CLOSE @sub_members_cur
	DEALLOCATE @sub_members_cur
END
SET @member_cur = CURSOR STATIC FORWARD_ONLY FOR SELECT * FROM @RESULT
OPEN @member_cur
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_GetNameFromSid]    Script Date: 04/01/2014 22:20:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Andrea Ferendeles
-- Create date: 13/04/2006
-- Description:	Get Name From Sid
-- =============================================
CREATE FUNCTION [dbo].[netsqlazman_GetNameFromSid] (@StoreName nvarchar(255), @ApplicationName nvarchar(255), @sid varbinary(85), @SidWhereDefined tinyint)
RETURNS nvarchar(255)
AS
BEGIN

DECLARE @Name nvarchar(255)
SET @Name = NULL

IF (@SidWhereDefined=0) --Store
BEGIN
SET @Name = (SELECT TOP 1 Name FROM dbo.[netsqlazman_StoreGroups]() WHERE objectSid = @sid)
END
ELSE IF (@SidWhereDefined=1) --Application 
BEGIN
SET @Name = (SELECT TOP 1 Name FROM dbo.[netsqlazman_ApplicationGroups]() WHERE objectSid = @sid)
END
ELSE IF (@SidWhereDefined=2 OR @SidWhereDefined=3) --LDAP or LOCAL
BEGIN
SET @Name = (SELECT Suser_Sname(@sid))
END
ELSE IF (@SidWhereDefined=4) --Database
BEGIN
SET @Name = (SELECT DBUserName FROM dbo.[netsqlazman_GetDBUsers](@StoreName, @ApplicationName, @sid, NULL))
END
IF (@Name IS NULL)
BEGIN
	SET @Name = @sid
END
RETURN @Name
END
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ItemDelete]    Script Date: 04/01/2014 22:20:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ItemDelete]
(
	@ItemId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ItemId FROM dbo.[netsqlazman_Items]() WHERE ItemId = @ItemId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_ItemsTable] WHERE [ItemId] = @ItemId AND [ApplicationId] = @ApplicationId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StoreAttributeDelete]    Script Date: 04/01/2014 22:20:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StoreAttributeDelete]
(
	@StoreId int,
	@StoreAttributeId int
)
AS
IF EXISTS(Select StoreAttributeId FROM dbo.[netsqlazman_StoreAttributes]() WHERE StoreAttributeId = @StoreAttributeId) AND dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_StoreAttributesTable] WHERE [StoreAttributeId] = @StoreAttributeId AND [StoreId] = @StoreId
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_StoreGroupMembers]    Script Date: 04/01/2014 22:20:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_StoreGroupMembers] ()
RETURNS TABLE
AS
RETURN
	SELECT     dbo.[netsqlazman_StoreGroupMembersTable].*
	FROM         dbo.[netsqlazman_StoreGroupMembersTable] INNER JOIN
	                      dbo.[netsqlazman_StoreGroups]() StoreGroups ON dbo.[netsqlazman_StoreGroupMembersTable].StoreGroupId = StoreGroups.StoreGroupId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StoreGroupMemberInsert]    Script Date: 04/01/2014 22:20:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StoreGroupMemberInsert]
(
	@StoreId int,
	@StoreGroupId int,
	@objectSid varbinary(85),
	@WhereDefined tinyint,
	@IsMember bit
)
AS
IF EXISTS(SELECT StoreGroupId FROM dbo.[netsqlazman_StoreGroups]() WHERE StoreGroupId = @StoreGroupId) AND dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1
BEGIN
	INSERT INTO [dbo].[netsqlazman_StoreGroupMembersTable] ([StoreGroupId], [objectSid], [WhereDefined], [IsMember]) VALUES (@StoreGroupId, @objectSid, @WhereDefined, @IsMember)
	RETURN SCOPE_IDENTITY()
END
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StoreAttributeUpdate]    Script Date: 04/01/2014 22:20:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StoreAttributeUpdate]
(
	@StoreId int,
	@AttributeKey nvarchar(255),
	@AttributeValue nvarchar(4000),
	@Original_StoreAttributeId int
)
AS
IF EXISTS(Select StoreAttributeId FROM dbo.[netsqlazman_StoreAttributes]() WHERE StoreAttributeId = @Original_StoreAttributeId) AND dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1
	UPDATE [dbo].[netsqlazman_StoreAttributesTable] SET [AttributeKey] = @AttributeKey, [AttributeValue] = @AttributeValue WHERE [StoreAttributeId] = @Original_StoreAttributeId AND [StoreId] = @StoreId 
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  View [dbo].[netsqlazman_StoreAttributesView]    Script Date: 04/01/2014 22:20:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[netsqlazman_StoreAttributesView]
AS
SELECT     [netsqlazman_Stores].StoreId, [netsqlazman_Stores].Name, [netsqlazman_Stores].Description, [netsqlazman_StoreAttributes].StoreAttributeId, [netsqlazman_StoreAttributes].AttributeKey, [netsqlazman_StoreAttributes].AttributeValue
FROM         dbo.[netsqlazman_Stores]() [netsqlazman_Stores] INNER JOIN
                      dbo.[netsqlazman_StoreAttributes]() [netsqlazman_StoreAttributes] ON [netsqlazman_Stores].StoreId = [netsqlazman_StoreAttributes].StoreId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_DeleteDelegate]    Script Date: 04/01/2014 22:20:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_DeleteDelegate](@AUTHORIZATIONID INT, @OWNERSID VARBINARY(85))
AS
DECLARE @ApplicationId int
SELECT @ApplicationId = Items.ApplicationId FROM dbo.[netsqlazman_Items]() Items INNER JOIN dbo.[netsqlazman_Authorizations]() Authorizations ON Items.ItemId = Authorizations.ItemId WHERE Authorizations.AuthorizationId = @AUTHORIZATIONID
IF @ApplicationId IS NOT NULL AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 1) = 1
	DELETE FROM dbo.[netsqlazman_AuthorizationsTable] WHERE AuthorizationId = @AUTHORIZATIONID AND ownerSid = @OWNERSID
ELSE
	RAISERROR ('Item NOT Found or Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StoreGroupMemberUpdate]    Script Date: 04/01/2014 22:20:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StoreGroupMemberUpdate]
(
	@StoreId int,
	@StoreGroupId int,
	@objectSid varbinary(85),
	@WhereDefined tinyint,
	@IsMember bit,
	@Original_StoreGroupMemberId int
)
AS
IF EXISTS(SELECT StoreGroupMemberId FROM dbo.[netsqlazman_StoreGroupMembers]() WHERE StoreGroupMemberId = @Original_StoreGroupMemberId) AND dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1
	UPDATE [dbo].[netsqlazman_StoreGroupMembersTable] SET [StoreGroupId] = @StoreGroupId, [objectSid] = @objectSid, [WhereDefined] = @WhereDefined, [IsMember] = @IsMember WHERE [StoreGroupMemberId] = @Original_StoreGroupMemberId
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  View [dbo].[netsqlazman_StoreGroupMembersView]    Script Date: 04/01/2014 22:20:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[netsqlazman_StoreGroupMembersView]
AS
SELECT     StoreGroupMembers.StoreGroupMemberId, StoreGroupMembers.StoreGroupId, StoreGroups.Name AS StoreGroup, dbo.[netsqlazman_GetNameFromSid](Stores.Name, NULL, 
                      StoreGroupMembers.objectSid, StoreGroupMembers.WhereDefined) AS Name, StoreGroupMembers.objectSid, 
                      CASE WhereDefined WHEN 0 THEN 'Store' WHEN 1 THEN 'Application' WHEN 2 THEN 'LDap' WHEN 3 THEN 'Local' WHEN 4 THEN 'DATABASE' END AS WhereDefined,
                       CASE IsMember WHEN 1 THEN 'Member' WHEN 0 THEN 'Non-Member' END AS MemberType
FROM         dbo.[netsqlazman_StoreGroupMembers]() StoreGroupMembers INNER JOIN
                      dbo.[netsqlazman_StoreGroups]() StoreGroups ON StoreGroupMembers.StoreGroupId = StoreGroups.StoreGroupId INNER JOIN
                      dbo.[netsqlazman_Stores]() Stores ON StoreGroups.StoreId = Stores.StoreId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_StoreGroupMemberDelete]    Script Date: 04/01/2014 22:20:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_StoreGroupMemberDelete]
(
	@StoreId int,
	@StoreGroupMemberId int
)
AS
IF EXISTS(SELECT StoreGroupMemberId FROM dbo.[netsqlazman_StoreGroupMembers]() WHERE StoreGroupMemberId = @StoreGroupMemberId) AND dbo.[netsqlazman_CheckStorePermissions](@StoreId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_StoreGroupMembersTable] WHERE [StoreGroupMemberId] = @StoreGroupMemberId
ELSE
	RAISERROR ('Store permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ItemAttributeUpdate]    Script Date: 04/01/2014 22:20:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ItemAttributeUpdate]
(
	@AttributeKey nvarchar(255),
	@AttributeValue nvarchar(4000),
	@Original_ItemAttributeId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ItemAttributeId FROM dbo.[netsqlazman_ItemAttributes]() WHERE ItemAttributeId = @Original_ItemAttributeId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	UPDATE [dbo].[netsqlazman_ItemAttributesTable] SET [AttributeKey] = @AttributeKey, [AttributeValue] = @AttributeValue WHERE [ItemAttributeId] = @Original_ItemAttributeId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  View [dbo].[netsqlazman_ItemAttributesView]    Script Date: 04/01/2014 22:20:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[netsqlazman_ItemAttributesView]
AS
SELECT     [netsqlazman_Items].ItemId, [netsqlazman_Items].ApplicationId, [netsqlazman_Items].Name, [netsqlazman_Items].Description, 
                      CASE [netsqlazman_Items].ItemType WHEN 0 THEN 'Role' WHEN 1 THEN 'Task' WHEN 2 THEN 'Operation' END AS ItemType, [netsqlazman_ItemAttributes].ItemAttributeId, 
                      [netsqlazman_ItemAttributes].AttributeKey, [netsqlazman_ItemAttributes].AttributeValue
FROM         dbo.[netsqlazman_Items]() [netsqlazman_Items] INNER JOIN
                      dbo.[netsqlazman_ItemAttributes]() [netsqlazman_ItemAttributes] ON [netsqlazman_Items].ItemId = [netsqlazman_ItemAttributes].ItemId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_GetApplicationGroupSidMembers]    Script Date: 04/01/2014 22:20:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_GetApplicationGroupSidMembers](@ISMEMBER BIT, @GROUPOBJECTSID VARBINARY(85), @NETSQLAZMANMODE bit, @LDAPPATH nvarchar(4000), @member_cur CURSOR VARYING OUTPUT)
AS
DECLARE @RESULT TABLE (objectSid VARBINARY(85))
DECLARE @GROUPID INT
DECLARE @GROUPTYPE TINYINT
DECLARE @LDAPQUERY nvarchar(4000)
DECLARE @sub_members_cur CURSOR
DECLARE @OBJECTSID VARBINARY(85)
SELECT @GROUPID = ApplicationGroupId, @GROUPTYPE = GroupType, @LDAPQUERY = LDapQuery FROM [netsqlazman_ApplicationGroupsTable] WHERE objectSid = @GROUPOBJECTSID
IF @GROUPTYPE = 0 -- BASIC
BEGIN
	--memo: WhereDefined can be:0 - Store; 1 - Application; 2 - LDAP; 3 - Local; 4 - Database
	-- Windows SIDs
	INSERT INTO @RESULT (objectSid) 
	SELECT objectSid 
	FROM dbo.[netsqlazman_ApplicationGroupMembersTable]
	WHERE 
	ApplicationGroupId = @GROUPID AND IsMember = @ISMEMBER AND
	((@NETSQLAZMANMODE = 0 AND (WhereDefined = 2 OR WhereDefined = 4)) OR (@NETSQLAZMANMODE = 1 AND WhereDefined BETWEEN 2 AND 4))
	-- Store Groups Members
	DECLARE @MemberObjectSid VARBINARY(85)
	DECLARE @MemberType bit
	DECLARE @NotMemberType bit
	DECLARE nested_Store_groups_cur CURSOR LOCAL FAST_FORWARD FOR
		SELECT objectSid, IsMember FROM dbo.[netsqlazman_ApplicationGroupMembersTable] WHERE ApplicationGroupId = @GROUPID AND WhereDefined = 0
	
	OPEN nested_Store_groups_cur
	FETCH NEXT FROM nested_Store_groups_cur INTO @MemberObjectSid, @MemberType
	WHILE @@FETCH_STATUS = 0
	BEGIN
	        -- recursive call
		IF @ISMEMBER = 1
		BEGIN
			IF @MemberType = 0 
				SET @NotMemberType = 0
			ELSE
				SET @NotMemberType = 1
		END
		ELSE
		BEGIN
			IF @MemberType = 0 
				SET @NotMemberType = 1
			ELSE
				SET @NotMemberType = 0
		END
		EXEC dbo.[netsqlazman_GetStoreGroupSidMembers] @NotMemberType, @MemberObjectSid, @NETSQLAZMANMODE, @LDAPPATH, @sub_members_cur OUTPUT
		FETCH NEXT FROM @sub_members_cur INTO @OBJECTSID
		WHILE @@FETCH_STATUS=0
		BEGIN
			INSERT INTO @RESULT VALUES (@OBJECTSID)
			FETCH NEXT FROM @sub_members_cur INTO @OBJECTSID
		END		
		CLOSE @sub_members_cur
		DEALLOCATE @sub_members_cur			

		FETCH NEXT FROM nested_Store_groups_cur INTO @MemberObjectSid, @MemberType
	END
	CLOSE nested_Store_groups_cur
	DEALLOCATE nested_Store_groups_cur
	
	-- Application Groups Members
	DECLARE nested_Application_groups_cur CURSOR LOCAL FAST_FORWARD FOR
		SELECT objectSid, IsMember FROM dbo.[netsqlazman_ApplicationGroupMembersTable] WHERE ApplicationGroupId = @GROUPID AND WhereDefined = 1
	
	OPEN nested_Application_groups_cur
	FETCH NEXT FROM nested_Application_groups_cur INTO @MemberObjectSid, @MemberType
	WHILE @@FETCH_STATUS = 0
	BEGIN
	        -- recursive call
		IF @ISMEMBER = 1
		BEGIN
			IF @MemberType = 0 
				SET @NotMemberType = 0
			ELSE
				SET @NotMemberType = 1
		END
		ELSE
		BEGIN
			IF @MemberType = 0 
				SET @NotMemberType = 1
			ELSE
				SET @NotMemberType = 0
		END
		EXEC dbo.[netsqlazman_GetApplicationGroupSidMembers] @NotMemberType, @MemberObjectSid, @NETSQLAZMANMODE, @LDAPPATH, @sub_members_cur OUTPUT
		FETCH NEXT FROM @sub_members_cur INTO @OBJECTSID
		WHILE @@FETCH_STATUS=0
		BEGIN
			INSERT INTO @RESULT VALUES (@OBJECTSID)
			FETCH NEXT FROM @sub_members_cur INTO @OBJECTSID
		END		
		CLOSE @sub_members_cur
		DEALLOCATE @sub_members_cur	

		FETCH NEXT FROM nested_Application_groups_cur INTO @MemberObjectSid, @MemberType
	END
	CLOSE nested_Application_groups_cur
	DEALLOCATE nested_Application_groups_cur
	END
ELSE IF @GROUPTYPE = 1 AND @ISMEMBER = 1 -- LDAP QUERY
BEGIN
	EXEC dbo.[netsqlazman_ExecuteLDAPQuery] @LDAPPATH, @LDAPQUERY, @sub_members_cur OUTPUT
	FETCH NEXT FROM @sub_members_cur INTO @OBJECTSID
	WHILE @@FETCH_STATUS=0
	BEGIN
		INSERT INTO @RESULT (objectSid) VALUES (@OBJECTSID)
		FETCH NEXT FROM @sub_members_cur INTO @OBJECTSID
	END
	CLOSE @sub_members_cur
	DEALLOCATE @sub_members_cur
END
SET @member_cur = CURSOR STATIC FORWARD_ONLY FOR SELECT * FROM @RESULT
OPEN @member_cur
GO
/****** Object:  View [dbo].[netsqlazman_ItemsHierarchyView]    Script Date: 04/01/2014 22:21:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[netsqlazman_ItemsHierarchyView]
AS
SELECT     [netsqlazman_Items].ItemId, [netsqlazman_Items].ApplicationId, [netsqlazman_Items].Name, [netsqlazman_Items].Description, 
                      CASE [netsqlazman_Items].ItemType WHEN 0 THEN 'Role' WHEN 1 THEN 'Task' WHEN 2 THEN 'Operation' END AS ItemType, Items_1.ItemId AS MemberItemId, 
                      Items_1.ApplicationId AS MemberApplicationId, Items_1.Name AS MemberName, Items_1.Description AS MemberDescription, 
                      CASE Items_1.ItemType WHEN 0 THEN 'Role' WHEN 1 THEN 'Task' WHEN 2 THEN 'Operation' END AS MemberType
FROM         dbo.[netsqlazman_Items]() Items_1 INNER JOIN
                      dbo.[netsqlazman_ItemsHierarchy]() [netsqlazman_ItemsHierarchy] ON Items_1.ItemId = [netsqlazman_ItemsHierarchy].ItemId INNER JOIN
                      dbo.[netsqlazman_Items]() [netsqlazman_Items] ON [netsqlazman_ItemsHierarchy].MemberOfItemId = [netsqlazman_Items].ItemId INNER JOIN
                      dbo.[netsqlazman_Applications]() [netsqlazman_Applications] ON [netsqlazman_Items].ApplicationId = [netsqlazman_Applications].ApplicationId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ItemAttributeDelete]    Script Date: 04/01/2014 22:21:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ItemAttributeDelete]
(
	@ItemAttributeId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ItemAttributeId FROM dbo.[netsqlazman_ItemAttributes]() WHERE ItemAttributeId = @ItemAttributeId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_ItemAttributesTable] WHERE [ItemAttributeId] = @ItemAttributeId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_BuildUserPermissionCache]    Script Date: 04/01/2014 22:21:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_BuildUserPermissionCache](@STORENAME nvarchar(255), @APPLICATIONNAME nvarchar(255))
AS 
-- Hierarchy
SET NOCOUNT ON
SELECT     Items.Name AS ItemName, Items_1.Name AS ParentItemName
FROM         dbo.[netsqlazman_Items]() Items_1 INNER JOIN
                      dbo.[netsqlazman_ItemsHierarchy]() ItemsHierarchy ON Items_1.ItemId = ItemsHierarchy.MemberOfItemId RIGHT OUTER JOIN
                      dbo.[netsqlazman_Applications]() Applications INNER JOIN
                      dbo.[netsqlazman_Stores]() Stores ON Applications.StoreId = Stores.StoreId INNER JOIN
                      dbo.[netsqlazman_Items]() Items ON Applications.ApplicationId = Items.ApplicationId ON ItemsHierarchy.ItemId = Items.ItemId
WHERE     (Stores.Name = @STORENAME) AND (Applications.Name = @APPLICATIONNAME)

-- Item Authorizations
SELECT DISTINCT Items.Name AS ItemName, Authorizations.ValidFrom, Authorizations.ValidTo
FROM         dbo.[netsqlazman_Authorizations]() Authorizations INNER JOIN
                      dbo.[netsqlazman_Items]() Items ON Authorizations.ItemId = Items.ItemId INNER JOIN
                      dbo.[netsqlazman_Stores]() Stores INNER JOIN
                      dbo.[netsqlazman_Applications]() Applications ON Stores.StoreId = Applications.StoreId ON Items.ApplicationId = Applications.ApplicationId
WHERE     (Authorizations.AuthorizationType <> 0) AND (Stores.Name = @STORENAME) AND (Applications.Name = @APPLICATIONNAME)
UNION
SELECT DISTINCT Items.Name AS ItemName, NULL ValidFrom, NULL ValidTo
FROM         dbo.[netsqlazman_Items]() Items INNER JOIN
                      dbo.[netsqlazman_Stores]() Stores INNER JOIN
                      dbo.[netsqlazman_Applications]() Applications ON Stores.StoreId = Applications.StoreId ON Items.ApplicationId = Applications.ApplicationId
WHERE     (Stores.Name = @STORENAME) AND (Applications.Name = @APPLICATIONNAME) AND Items.BizRuleId IS NOT NULL
SET NOCOUNT OFF
GO
/****** Object:  View [dbo].[netsqlazman_BizRuleView]    Script Date: 04/01/2014 22:21:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[netsqlazman_BizRuleView]
AS
SELECT     [netsqlazman_Items].ItemId, [netsqlazman_Items].ApplicationId, [netsqlazman_Items].Name, [netsqlazman_Items].Description, [netsqlazman_Items].ItemType, [netsqlazman_BizRules].BizRuleSource, [netsqlazman_BizRules].BizRuleLanguage, 
                      [netsqlazman_BizRules].CompiledAssembly
FROM         dbo.[netsqlazman_Items]() [netsqlazman_Items] INNER JOIN
                      dbo.[netsqlazman_BizRules]() [netsqlazman_BizRules] ON [netsqlazman_Items].BizRuleId = [netsqlazman_BizRules].BizRuleId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_BizRuleUpdate]    Script Date: 04/01/2014 22:21:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_BizRuleUpdate]
(
	@BizRuleSource text,
	@BizRuleLanguage tinyint,
	@CompiledAssembly image,
	@Original_BizRuleId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT BizRuleId FROM dbo.[netsqlazman_BizRules]() WHERE BizRuleId = @Original_BizRuleId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	UPDATE [dbo].[netsqlazman_BizRulesTable] SET [BizRuleSource] = @BizRuleSource, [BizRuleLanguage] = @BizRuleLanguage, [CompiledAssembly] = @CompiledAssembly WHERE [BizRuleId] = @Original_BizRuleId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  View [dbo].[netsqlazman_AuthorizationView]    Script Date: 04/01/2014 22:21:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[netsqlazman_AuthorizationView]
AS
SELECT     [netsqlazman_Authorizations].AuthorizationId, [netsqlazman_Authorizations].ItemId, dbo.[netsqlazman_GetNameFromSid]([netsqlazman_Stores].Name, [netsqlazman_Applications].Name, [netsqlazman_Authorizations].ownerSid, 
                      [netsqlazman_Authorizations].ownerSidWhereDefined) AS Owner, dbo.[netsqlazman_GetNameFromSid]([netsqlazman_Stores].Name, [netsqlazman_Applications].Name, [netsqlazman_Authorizations].objectSid, 
                      [netsqlazman_Authorizations].objectSidWhereDefined) AS Name, [netsqlazman_Authorizations].objectSid, 
                      CASE objectSidWhereDefined WHEN 0 THEN 'Store' WHEN 1 THEN 'Application' WHEN 2 THEN 'LDAP' WHEN 3 THEN 'Local' WHEN 4 THEN 'DATABASE' END AS SidWhereDefined,
                       CASE AuthorizationType WHEN 0 THEN 'NEUTRAL' WHEN 1 THEN 'ALLOW' WHEN 2 THEN 'DENY' WHEN 3 THEN 'ALLOWWITHDELEGATION' END AS AuthorizationType,
                       [netsqlazman_Authorizations].ValidFrom, [netsqlazman_Authorizations].ValidTo
FROM         dbo.[netsqlazman_Authorizations]() [netsqlazman_Authorizations] INNER JOIN
                      dbo.[netsqlazman_Items]() [netsqlazman_Items] ON [netsqlazman_Authorizations].ItemId = [netsqlazman_Items].ItemId INNER JOIN
                      dbo.[netsqlazman_Applications]() [netsqlazman_Applications] ON [netsqlazman_Items].ApplicationId = [netsqlazman_Applications].ApplicationId INNER JOIN
                      dbo.[netsqlazman_Stores]() [netsqlazman_Stores] ON [netsqlazman_Applications].StoreId = [netsqlazman_Stores].StoreId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_AuthorizationUpdate]    Script Date: 04/01/2014 22:21:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_AuthorizationUpdate]
(
	@ItemId int,
	@ownerSid varbinary(85),
	@ownerSidWhereDefined tinyint,
	@objectSid varbinary(85),
	@objectSidWhereDefined tinyint,
	@AuthorizationType tinyint,
	@ValidFrom datetime,
	@ValidTo datetime,
	@Original_AuthorizationId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT AuthorizationId FROM dbo.[netsqlazman_Authorizations]() WHERE AuthorizationId = @Original_AuthorizationId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	UPDATE [dbo].[netsqlazman_AuthorizationsTable] SET [ownerSid] = @ownerSid, [ownerSidWhereDefined] = @ownerSidWhereDefined, [objectSid] = @objectSid, [objectSidWhereDefined] = @objectSidWhereDefined, [AuthorizationType] = @AuthorizationType, [ValidFrom] = @ValidFrom, [ValidTo] = @ValidTo WHERE [AuthorizationId] = @Original_AuthorizationId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  UserDefinedFunction [dbo].[netsqlazman_AuthorizationAttributes]    Script Date: 04/01/2014 22:21:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[netsqlazman_AuthorizationAttributes] ()
RETURNS TABLE
AS
RETURN
	SELECT     dbo.[netsqlazman_AuthorizationAttributesTable].*
	FROM         dbo.[netsqlazman_AuthorizationAttributesTable] INNER JOIN
	                      dbo.[netsqlazman_Authorizations]() as Authorizations ON dbo.[netsqlazman_AuthorizationAttributesTable].AuthorizationId = Authorizations.AuthorizationId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_AuthorizationAttributeInsert]    Script Date: 04/01/2014 22:21:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_AuthorizationAttributeInsert]
(
	@AuthorizationId int,
	@AttributeKey nvarchar(255),
	@AttributeValue nvarchar(4000),
	@ApplicationId int
)
AS
IF EXISTS(SELECT AuthorizationId FROM dbo.[netsqlazman_Authorizations]() WHERE AuthorizationId = @AuthorizationId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 1) = 1
BEGIN
	INSERT INTO [dbo].[netsqlazman_AuthorizationAttributesTable] ([AuthorizationId], [AttributeKey], [AttributeValue]) VALUES (@AuthorizationId, @AttributeKey, @AttributeValue)
	RETURN SCOPE_IDENTITY()
END
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationGroupMemberUpdate]    Script Date: 04/01/2014 22:21:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationGroupMemberUpdate]
(
	@ApplicationGroupId int,
	@objectSid varbinary(85),
	@WhereDefined tinyint,
	@IsMember bit,
	@Original_ApplicationGroupMemberId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ApplicationGroupMemberId FROM dbo.[netsqlazman_ApplicationGroupMembers]() WHERE ApplicationGroupMemberId = @Original_ApplicationGroupMemberId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	UPDATE [dbo].[netsqlazman_ApplicationGroupMembersTable] SET [objectSid] = @objectSid, [WhereDefined] = @WhereDefined, [IsMember] = @IsMember WHERE [ApplicationGroupMemberId] = @Original_ApplicationGroupMemberId
ELSE	
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  View [dbo].[netsqlazman_ApplicationGroupMembersView]    Script Date: 04/01/2014 22:21:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[netsqlazman_ApplicationGroupMembersView]
AS
SELECT     [netsqlazman_Stores].StoreId, [netsqlazman_Applications].ApplicationId, [netsqlazman_ApplicationGroupMembers].ApplicationGroupMemberId, [netsqlazman_ApplicationGroupMembers].ApplicationGroupId, 
                      [netsqlazman_ApplicationGroups].Name AS ApplicationGroup, dbo.[netsqlazman_GetNameFromSid]([netsqlazman_Stores].Name, [netsqlazman_Applications].Name, [netsqlazman_ApplicationGroupMembers].objectSid, 
                      [netsqlazman_ApplicationGroupMembers].WhereDefined) AS Name, [netsqlazman_ApplicationGroupMembers].objectSid, 
                      CASE WhereDefined WHEN 0 THEN 'Store' WHEN 1 THEN 'Application' WHEN 2 THEN 'LDap' WHEN 3 THEN 'Local' WHEN 4 THEN 'DATABASE' END AS WhereDefined,
                       CASE IsMember WHEN 1 THEN 'Member' WHEN 0 THEN 'Non-Member' END AS MemberType
FROM         dbo.[netsqlazman_ApplicationGroupMembers]() [netsqlazman_ApplicationGroupMembers] INNER JOIN
                      dbo.[netsqlazman_ApplicationGroups]() [netsqlazman_ApplicationGroups] ON [netsqlazman_ApplicationGroupMembers].ApplicationGroupId = [netsqlazman_ApplicationGroups].ApplicationGroupId INNER JOIN
                      dbo.[netsqlazman_Applications]() [netsqlazman_Applications] ON [netsqlazman_ApplicationGroups].ApplicationId = [netsqlazman_Applications].ApplicationId INNER JOIN
                      dbo.[netsqlazman_Stores]() [netsqlazman_Stores] ON [netsqlazman_Applications].StoreId = [netsqlazman_Stores].StoreId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_ApplicationGroupMemberDelete]    Script Date: 04/01/2014 22:21:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_ApplicationGroupMemberDelete]
(
	@ApplicationGroupMemberId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT ApplicationGroupMemberId FROM dbo.[netsqlazman_ApplicationGroupMembers]() WHERE ApplicationGroupMemberId = @ApplicationGroupMemberId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_ApplicationGroupMembersTable] WHERE [ApplicationGroupMemberId] = @ApplicationGroupMemberId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_AuthorizationDelete]    Script Date: 04/01/2014 22:21:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_AuthorizationDelete]
(
	@AuthorizationId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT AuthorizationId FROM dbo.[netsqlazman_Authorizations]() WHERE AuthorizationId = @AuthorizationId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 2) = 1
	DELETE FROM [dbo].[netsqlazman_AuthorizationsTable] WHERE [AuthorizationId] = @AuthorizationId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_AuthorizationAttributeUpdate]    Script Date: 04/01/2014 22:21:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_AuthorizationAttributeUpdate]
(
	@AttributeKey nvarchar(255),
	@AttributeValue nvarchar(4000),
	@Original_AuthorizationAttributeId int,
	@ApplicationId int
)
AS
IF EXISTS(SELECT AuthorizationAttributeId FROM dbo.[netsqlazman_AuthorizationAttributes]() WHERE AuthorizationAttributeId = @Original_AuthorizationAttributeId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 1) = 1
	UPDATE [dbo].[netsqlazman_AuthorizationAttributesTable] SET [AttributeKey] = @AttributeKey, [AttributeValue] = @AttributeValue WHERE [AuthorizationAttributeId] = @Original_AuthorizationAttributeId
ELSE
	RAISERROR ('Application permission denied.', 16 ,1)
GO
/****** Object:  View [dbo].[netsqlazman_AuthorizationAttributesView]    Script Date: 04/01/2014 22:21:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[netsqlazman_AuthorizationAttributesView]
AS
SELECT     dbo.[netsqlazman_AuthorizationView].AuthorizationId, dbo.[netsqlazman_AuthorizationView].ItemId, dbo.[netsqlazman_AuthorizationView].Owner, dbo.[netsqlazman_AuthorizationView].Name, dbo.[netsqlazman_AuthorizationView].objectSid, 
                      dbo.[netsqlazman_AuthorizationView].SidWhereDefined, dbo.[netsqlazman_AuthorizationView].AuthorizationType, dbo.[netsqlazman_AuthorizationView].ValidFrom, dbo.[netsqlazman_AuthorizationView].ValidTo, 
                      [netsqlazman_AuthorizationAttributes].AuthorizationAttributeId, [netsqlazman_AuthorizationAttributes].AttributeKey, [netsqlazman_AuthorizationAttributes].AttributeValue
FROM         dbo.[netsqlazman_AuthorizationView] INNER JOIN
                      dbo.[netsqlazman_AuthorizationAttributes]() [netsqlazman_AuthorizationAttributes] ON dbo.[netsqlazman_AuthorizationView].AuthorizationId = [netsqlazman_AuthorizationAttributes].AuthorizationId
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_AuthorizationAttributeDelete]    Script Date: 04/01/2014 22:21:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_AuthorizationAttributeDelete]
(
	@AuthorizationAttributeId int,
	@ApplicationId int
)
AS
IF  EXISTS(SELECT AuthorizationAttributeId FROM dbo.[netsqlazman_AuthorizationAttributes]() WHERE AuthorizationAttributeId = @AuthorizationAttributeId) AND dbo.[netsqlazman_CheckApplicationPermissions](@ApplicationId, 1) = 1
	DELETE FROM [dbo].[netsqlazman_AuthorizationAttributesTable] WHERE [AuthorizationAttributeId] = @AuthorizationAttributeId
ELSE
	RAISERROR ('Application permission denied.', 16, 1)
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_CheckAccess]    Script Date: 04/01/2014 22:21:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_CheckAccess] (@ITEMID INT, @USERSID VARBINARY(85), @VALIDFOR DATETIME, @LDAPPATH nvarchar(4000), @AUTHORIZATION_TYPE TINYINT OUTPUT, @NETSQLAZMANMODE BIT, @RETRIEVEATTRIBUTES BIT) 
AS
---------------------------------------------------
-- VARIABLES DECLARATION
-- 0 - Neutral; 1 - Allow; 2 - Deny; 3 - AllowWithDelegation
SET NOCOUNT ON
DECLARE @PARENTITEMID INT
DECLARE @PKID INT
DECLARE @PARENTRESULT TINYINT
DECLARE @APP VARBINARY(85)
DECLARE @members_cur CURSOR
DECLARE @OBJECTSID VARBINARY(85)
DECLARE @ITEM_AUTHORIZATION_TYPE TINYINT
---------------------------------------------------
-- INITIALIZE VARIABLES
SET @ITEM_AUTHORIZATION_TYPE = 0 -- Neutral
SET @AUTHORIZATION_TYPE = 0 -- Neutral
------------------------------------------------------
-- CHECK ACCESS ON PARENTS
-- Get Items Where Item is A Member
DECLARE ItemsWhereIAmAMember_cur CURSOR LOCAL FAST_FORWARD READ_ONLY FOR SELECT MemberOfItemId FROM dbo.[netsqlazman_ItemsHierarchyTable] WHERE ItemId = @ITEMID
OPEN ItemsWhereIAmAMember_cur
FETCH NEXT FROM ItemsWhereIAmAMember_cur INTO @PARENTITEMID
WHILE @@FETCH_STATUS = 0
BEGIN
	-- Recursive Call
	EXEC dbo.[netsqlazman_CheckAccess] @PARENTITEMID, @USERSID, @VALIDFOR, @LDAPPATH, @PARENTRESULT OUTPUT, @NETSQLAZMANMODE, @RETRIEVEATTRIBUTES
	SELECT @AUTHORIZATION_TYPE = dbo.[netsqlazman_MergeAuthorizations](@AUTHORIZATION_TYPE, @PARENTRESULT)
	FETCH NEXT FROM ItemsWhereIAmAMember_cur INTO @PARENTITEMID
END
CLOSE ItemsWhereIAmAMember_cur
DEALLOCATE ItemsWhereIAmAMember_cur

IF @AUTHORIZATION_TYPE = 3 
BEGIN
	SET @AUTHORIZATION_TYPE = 1 -- AllowWithDelegation becomes Just Allow (if comes from parents)
END
---------------------------------------------
-- GET ITEM ATTRIBUTES
---------------------------------------------
IF @RETRIEVEATTRIBUTES = 1
	INSERT INTO #ATTRIBUTES_TABLE SELECT AttributeKey, AttributeValue, @ITEMID FROM dbo.[netsqlazman_ItemAttributesTable] WHERE ItemId = @ITEMID
---------------------------------------------
-- CHECK ACCESS ON ITEM
-- AuthorizationType can be:  0 - Neutral; 1 - Allow; 2 - Deny; 3 - AllowWithDelegation
-- objectSidWhereDefined can be:0 - Store; 1 - Application; 2 - LDAP; 3 - Local; 4 - Database
DECLARE @PARTIAL_RESULT TINYINT
--CHECK ACCESS FOR USER AUTHORIZATIONS
DECLARE checkaccessonitem_cur CURSOR  LOCAL FAST_FORWARD READ_ONLY FOR 
	SELECT AuthorizationType, AuthorizationId
	FROM dbo.[netsqlazman_AuthorizationsTable] WHERE 
	ItemId = @ITEMID AND
	objectSid = @USERSID AND
	(ValidFrom IS NULL AND ValidTo IS NULL OR
	@VALIDFOR >= ValidFrom  AND ValidTo IS NULL OR
	@VALIDFOR <= ValidTo AND ValidFrom IS NULL OR
	@VALIDFOR BETWEEN ValidFrom AND ValidTo) AND
        AuthorizationType<>0 AND
	((@NETSQLAZMANMODE = 0 AND (objectSidWhereDefined=2 OR objectSidWhereDefined=4)) OR (@NETSQLAZMANMODE = 1 AND objectSidWhereDefined BETWEEN 2 AND 4)) -- if Mode = Administrator SKIP CHECK for local Authorizations

OPEN checkaccessonitem_cur
FETCH NEXT FROM checkaccessonitem_cur INTO @PARTIAL_RESULT, @PKID
WHILE @@FETCH_STATUS = 0
BEGIN
	--CHECK FOR DENY
	IF @PARTIAL_RESULT IS NOT NULL
	BEGIN
		SELECT @AUTHORIZATION_TYPE = dbo.[netsqlazman_MergeAuthorizations](@AUTHORIZATION_TYPE, @PARTIAL_RESULT)
		SELECT @ITEM_AUTHORIZATION_TYPE  = dbo.[netsqlazman_MergeAuthorizations](@ITEM_AUTHORIZATION_TYPE, @PARTIAL_RESULT)
		IF @RETRIEVEATTRIBUTES = 1 
			INSERT INTO #ATTRIBUTES_TABLE SELECT AttributeKey, AttributeValue, NULL FROM dbo.[netsqlazman_AuthorizationAttributesTable] WHERE AuthorizationId = @PKID
	END
	ELSE
	BEGIN
		SET @PARTIAL_RESULT = 0 -- NEUTRAL
	END
	FETCH NEXT FROM checkaccessonitem_cur INTO @PARTIAL_RESULT, @PKID
END

CLOSE checkaccessonitem_cur
DEALLOCATE checkaccessonitem_cur

--CHECK ACCESS FOR USER GROUPS AUTHORIZATIONS
DECLARE usergroupsauthz_cur CURSOR LOCAL FAST_FORWARD READ_ONLY FOR 
	SELECT AuthorizationType, AuthorizationID
	FROM dbo.[netsqlazman_AuthorizationsTable] Authorizations INNER JOIN #USERGROUPS as usergroups
	ON Authorizations.objectSid = usergroups.objectSid WHERE 
	ItemId = @ITEMID AND
	(ValidFrom IS NULL AND ValidTo IS NULL OR
	@VALIDFOR >= ValidFrom  AND ValidTo IS NULL OR
	@VALIDFOR <= ValidTo AND ValidFrom IS NULL OR
	@VALIDFOR BETWEEN ValidFrom AND ValidTo) AND
        AuthorizationType<>0 AND
	((@NETSQLAZMANMODE = 0 AND (objectSidWhereDefined=2 OR objectSidWhereDefined=4)) OR (@NETSQLAZMANMODE = 1 AND objectSidWhereDefined BETWEEN 2 AND 4)) -- if Mode = Administrator SKIP CHECK for local Authorizations

OPEN usergroupsauthz_cur
FETCH NEXT FROM usergroupsauthz_cur INTO @PARTIAL_RESULT, @PKID
WHILE @@FETCH_STATUS = 0
BEGIN
	IF @PARTIAL_RESULT IS NOT NULL
	BEGIN
		SELECT @AUTHORIZATION_TYPE = dbo.[netsqlazman_MergeAuthorizations](@AUTHORIZATION_TYPE, @PARTIAL_RESULT)
		SELECT @ITEM_AUTHORIZATION_TYPE = dbo.[netsqlazman_MergeAuthorizations](@ITEM_AUTHORIZATION_TYPE, @PARTIAL_RESULT)
		IF @RETRIEVEATTRIBUTES = 1
			INSERT INTO #ATTRIBUTES_TABLE SELECT AttributeKey, AttributeValue, NULL FROM dbo.[netsqlazman_AuthorizationAttributesTable] WHERE AuthorizationId = @PKID
	END
	ELSE
	BEGIN
		SET @PARTIAL_RESULT = 0 -- NEUTRAL
	END
	FETCH NEXT FROM usergroupsauthz_cur INTO @PARTIAL_RESULT, @PKID
END

CLOSE usergroupsauthz_cur
DEALLOCATE usergroupsauthz_cur

--CHECK ACCESS FOR STORE/APPLICATION GROUPS AUTHORIZATIONS
DECLARE @GROUPOBJECTSID VARBINARY(85)
DECLARE @GROUPWHEREDEFINED TINYINT
DECLARE @GROUPSIDMEMBERS table (objectSid VARBINARY(85))
DECLARE @ISMEMBER BIT
SET @ISMEMBER = 1
DECLARE groups_authorizations_cur CURSOR LOCAL FAST_FORWARD READ_ONLY 
FOR 	SELECT objectSid, objectSidWhereDefined, AuthorizationType, AuthorizationId FROM dbo.[netsqlazman_AuthorizationsTable]
	WHERE ItemId = @ITEMID AND objectSidWhereDefined BETWEEN 0 AND 1 AND
        AuthorizationType<>0 AND
	(ValidFrom IS NULL AND ValidTo IS NULL OR
	@VALIDFOR >= ValidFrom  AND ValidTo IS NULL OR
	@VALIDFOR <= ValidTo AND ValidFrom IS NULL OR
	@VALIDFOR BETWEEN ValidFrom AND ValidTo)

OPEN groups_authorizations_cur
FETCH NEXT FROM groups_authorizations_cur INTO @GROUPOBJECTSID, @GROUPWHEREDEFINED, @PARTIAL_RESULT, @PKID
WHILE @@FETCH_STATUS=0
BEGIN
SET @ISMEMBER = 1
--check if user is a non-member
IF @GROUPWHEREDEFINED = 0 -- store group members
BEGIN
--store groups members of type 'non-member'
	DELETE FROM @GROUPSIDMEMBERS

	EXEC dbo.[netsqlazman_GetStoreGroupSidMembers] 0, @GROUPOBJECTSID, @NETSQLAZMANMODE, @LDAPPATH, @members_cur OUTPUT -- non-members
	FETCH NEXT FROM @members_cur INTO @OBJECTSID
	WHILE @@FETCH_STATUS=0
	BEGIN
		INSERT INTO @GROUPSIDMEMBERS VALUES (@OBJECTSID)
		FETCH NEXT FROM @members_cur INTO @OBJECTSID
	END
	CLOSE @members_cur
	DEALLOCATE @members_cur

	IF EXISTS(SELECT * FROM @GROUPSIDMEMBERS WHERE objectSid = @USERSID) OR
	     EXISTS(SELECT * FROM @GROUPSIDMEMBERS groupsidmembers INNER JOIN #USERGROUPS as usergroups ON groupsidmembers.objectSid = usergroups.objectSid)
	BEGIN
	-- user is a non-member
	SET @ISMEMBER = 0
	END
	IF @ISMEMBER = 1
	BEGIN
		DELETE FROM @GROUPSIDMEMBERS

		EXEC dbo.[netsqlazman_GetStoreGroupSidMembers] 1, @GROUPOBJECTSID, @NETSQLAZMANMODE, @LDAPPATH, @members_cur OUTPUT -- members
		FETCH NEXT FROM @members_cur INTO @OBJECTSID
		WHILE @@FETCH_STATUS=0
		BEGIN
			INSERT INTO @GROUPSIDMEMBERS VALUES (@OBJECTSID)
			FETCH NEXT FROM @members_cur INTO @OBJECTSID
		END
		CLOSE @members_cur
		DEALLOCATE @members_cur

		IF EXISTS (SELECT * FROM @GROUPSIDMEMBERS WHERE objectSid = @USERSID) OR
		     EXISTS (SELECT * FROM @GROUPSIDMEMBERS groupsidmembers INNER JOIN #USERGROUPS usergroups ON groupsidmembers.objectSid = usergroups.ObjectSId)
		BEGIN
		-- user is a member
		SET @ISMEMBER = 1
		END
		ELSE
		BEGIN
		-- user is not present
		SET @ISMEMBER = 0
		END
	END
	-- if a member ... get authorization
	IF @ISMEMBER = 1
	BEGIN
		SET @AUTHORIZATION_TYPE = (SELECT dbo.[netsqlazman_MergeAuthorizations](@AUTHORIZATION_TYPE, @PARTIAL_RESULT))
		SET @ITEM_AUTHORIZATION_TYPE = (SELECT dbo.[netsqlazman_MergeAuthorizations](@ITEM_AUTHORIZATION_TYPE, @PARTIAL_RESULT))
		IF @PKID IS NOT NULL AND @RETRIEVEATTRIBUTES = 1
			INSERT INTO #ATTRIBUTES_TABLE SELECT AttributeKey, AttributeValue, NULL FROM dbo.[netsqlazman_AuthorizationAttributesTable] WHERE AuthorizationId = @PKID
	END
END
	ELSE
IF @GROUPWHEREDEFINED = 1 -- application group members
BEGIN
	--application groups members of type 'non-member'
	DELETE FROM @GROUPSIDMEMBERS

	EXEC dbo.[netsqlazman_GetApplicationGroupSidMembers] 0, @GROUPOBJECTSID, @NETSQLAZMANMODE, @LDAPPATH, @members_cur OUTPUT -- non-members
	FETCH NEXT FROM @members_cur INTO @OBJECTSID
	WHILE @@FETCH_STATUS=0
	BEGIN
		INSERT INTO @GROUPSIDMEMBERS VALUES (@OBJECTSID)
		FETCH NEXT FROM @members_cur INTO @OBJECTSID
	END
	CLOSE @members_cur
	DEALLOCATE @members_cur

	IF EXISTS(SELECT * FROM @GROUPSIDMEMBERS WHERE objectSid = @USERSID) OR
	     EXISTS (SELECT* FROM @GROUPSIDMEMBERS groupsidmembers INNER JOIN #USERGROUPS as usergroups ON groupsidmembers.objectSid = usergroups.objectSid)
	BEGIN	-- user is a non-member
	SET @ISMEMBER = 0
	END
	IF @ISMEMBER = 1 
	BEGIN
		DELETE FROM @GROUPSIDMEMBERS

		EXEC dbo.[netsqlazman_GetApplicationGroupSidMembers] 1, @GROUPOBJECTSID, @NETSQLAZMANMODE, @LDAPPATH, @members_cur OUTPUT -- members
		FETCH NEXT FROM @members_cur INTO @OBJECTSID
		WHILE @@FETCH_STATUS=0
		BEGIN
			INSERT INTO @GROUPSIDMEMBERS VALUES (@OBJECTSID)
			FETCH NEXT FROM @members_cur INTO @OBJECTSID
		END
		CLOSE @members_cur
		DEALLOCATE @members_cur

		IF EXISTS(SELECT * FROM @GROUPSIDMEMBERS WHERE objectSid = @USERSID) OR
		     EXISTS (SELECT * FROM @GROUPSIDMEMBERS groupsidmembers INNER JOIN #USERGROUPS as usergroups ON groupsidmembers.objectSid = usergroups.objectSid)
		BEGIN
		-- user is a member
		SET @ISMEMBER = 1
		END
		ELSE
		BEGIN
		-- user is not present
		SET @ISMEMBER = 0
		END
	END
	-- if a member ... get authorization
	IF @ISMEMBER = 1
	BEGIN
		SET @AUTHORIZATION_TYPE = (SELECT dbo.[netsqlazman_MergeAuthorizations](@AUTHORIZATION_TYPE, @PARTIAL_RESULT))
		SET @ITEM_AUTHORIZATION_TYPE = (SELECT dbo.[netsqlazman_MergeAuthorizations](@ITEM_AUTHORIZATION_TYPE, @PARTIAL_RESULT))
		IF @PKID IS NOT NULL AND @RETRIEVEATTRIBUTES = 1 
			INSERT INTO #ATTRIBUTES_TABLE SELECT AttributeKey, AttributeValue, NULL FROM dbo.[netsqlazman_AuthorizationAttributesTable] WHERE AuthorizationId = @PKID
	END
END
	FETCH NEXT FROM groups_authorizations_cur INTO @GROUPOBJECTSID, @GROUPWHEREDEFINED, @PARTIAL_RESULT, @PKID
END
CLOSE groups_authorizations_cur
DEALLOCATE groups_authorizations_cur

-- PREPARE RESULTSET FOR BIZ RULE CHECKING
----------------------------------------------------------------------------------------
INSERT INTO #PARTIAL_RESULTS_TABLE 
SELECT     Items.ItemId, Items.Name, Items.ItemType, @ITEM_AUTHORIZATION_TYPE,BizRules.BizRuleId, BizRules.BizRuleSource, BizRules.BizRuleLanguage
FROM         dbo.[netsqlazman_ItemsTable] Items LEFT OUTER JOIN
                      dbo.[netsqlazman_BizRulesTable] BizRules ON Items.BizRuleId = BizRules.BizRuleId WHERE Items.ItemId = @ITEMID
SET NOCOUNT OFF
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_IsAMemberOfGroup]    Script Date: 04/01/2014 22:21:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_IsAMemberOfGroup](@GROUPTYPE bit, @GROUPOBJECTSID VARBINARY(85), @NETSQLAZMANMODE bit, @LDAPPATH nvarchar(4000), @TOKEN IMAGE, @USERGROUPSCOUNT INT)  
AS  
DECLARE @member_cur CURSOR
DECLARE @MemberSid VARBINARY(85)
DECLARE @USERSID VARBINARY(85)
DECLARE @USERGROUPS TABLE(objectSid VARBINARY(85))
DECLARE @I INT
DECLARE @INDEX INT
DECLARE @APP VARBINARY(85)
DECLARE @COUNT int

-- Get User Sid
IF @USERGROUPSCOUNT>0
BEGIN
	SET @USERSID = SUBSTRING(@TOKEN,1,85)
	SET @I = CHARINDEX(0x01, @USERSID)
	SET @USERSID = SUBSTRING(@USERSID, @I, 85-@I+1)
	-- Get User Groups Sid
	SET @I = 0
	WHILE (@I<@USERGROUPSCOUNT)
	BEGIN
		SET @APP = SUBSTRING(@TOKEN,(@I+1)*85+1,85) --GET USER GROUP TOKEN PORTION
		SET @INDEX = CHARINDEX(0x01, @APP) -- FIND TOKEN START (0x01)
		SET @APP = SUBSTRING(@APP, @INDEX, 85-@INDEX+1) -- EXTRACT USER GROUP SID
		INSERT INTO @USERGROUPS (objectSid) VALUES (@APP)
		SET @I = @I + 1
	END
END
ELSE
BEGIN
	SET @USERSID = @TOKEN
END

-- CHECK IF IS A NON-MEMBER
IF @GROUPTYPE = 0 -- STORE GROUP
	EXEC dbo.[netsqlazman_GetStoreGroupSidMembers] 0, @GROUPOBJECTSID, @NETSQLAZMANMODE, @LDAPPATH, @member_cur OUTPUT
ELSE -- APPLICATON GROUP
	EXEC dbo.[netsqlazman_GetApplicationGroupSidMembers] 0, @GROUPOBJECTSID, @NETSQLAZMANMODE, @LDAPPATH, @member_cur OUTPUT

FETCH NEXT FROM @member_cur INTO @MemberSid
WHILE @@FETCH_STATUS = 0
BEGIN
	IF @MemberSid = @USERSID
	BEGIN
		CLOSE @member_cur
		DEALLOCATE @member_cur
		SELECT CONVERT(bit, 0) -- true
		RETURN
	END		
	SELECT @COUNT =  COUNT(*)  FROM @USERGROUPS WHERE objectSid = @MemberSid
	IF @COUNT>0
	BEGIN
		CLOSE @member_cur
		DEALLOCATE @member_cur
		SELECT CONVERT(bit, 0) -- true
		RETURN
	END		
	FETCH NEXT FROM @member_cur INTO @MemberSid
END
CLOSE @member_cur
DEALLOCATE @member_cur

-- CHECK IF IS A MEMBER
IF @GROUPTYPE = 0 -- STORE GROUP
	EXEC dbo.[netsqlazman_GetStoreGroupSidMembers] 1, @GROUPOBJECTSID, @NETSQLAZMANMODE, @LDAPPATH, @member_cur OUTPUT
ELSE -- APPLICATON GROUP
	EXEC dbo.[netsqlazman_GetApplicationGroupSidMembers] 1, @GROUPOBJECTSID, @NETSQLAZMANMODE, @LDAPPATH, @member_cur OUTPUT

FETCH NEXT FROM @member_cur INTO @MemberSid
WHILE @@FETCH_STATUS = 0
BEGIN
	IF @MemberSid = @USERSID
	BEGIN
		CLOSE @member_cur
		DEALLOCATE @member_cur
		SELECT CONVERT(bit,1) -- true
		RETURN
	END		
	SELECT @COUNT =  COUNT(*)  FROM @USERGROUPS WHERE objectSid = @MemberSid
	IF @COUNT>0
	BEGIN
		CLOSE @member_cur
		DEALLOCATE @member_cur
		SELECT CONVERT(bit, 1) -- true
		RETURN
	END		
	FETCH NEXT FROM @member_cur INTO @MemberSid
END
CLOSE @member_cur
DEALLOCATE @member_cur

SELECT CONVERT(bit, 0) -- true
GO
/****** Object:  StoredProcedure [dbo].[netsqlazman_DirectCheckAccess]    Script Date: 04/01/2014 22:21:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[netsqlazman_DirectCheckAccess] (@STORENAME nvarchar(255), @APPLICATIONNAME nvarchar(255), @ITEMNAME nvarchar(255), @OPERATIONSONLY BIT, @TOKEN IMAGE, @USERGROUPSCOUNT INT, @VALIDFOR DATETIME, @LDAPPATH nvarchar(4000), @AUTHORIZATION_TYPE TINYINT OUTPUT, @RETRIEVEATTRIBUTES BIT) 
AS
--Memo: 0 - Role; 1 - Task; 2 - Operation
SET NOCOUNT ON
DECLARE @STOREID int
DECLARE @ApplicationId int
DECLARE @ITEMID INT

-- CHECK STORE EXISTANCE/PERMISSIONS
Select @STOREID = StoreId FROM dbo.[netsqlazman_Stores]() WHERE Name = @STORENAME
IF @STOREID IS NULL
	BEGIN
	RAISERROR ('Store not found or Store permission denied.', 16, 1)
	RETURN 1
	END
-- CHECK APPLICATION EXISTANCE/PERMISSIONS
Select @ApplicationId = ApplicationId FROM dbo.[netsqlazman_Applications]() WHERE Name = @APPLICATIONNAME And StoreId = @STOREID
IF @ApplicationId IS NULL
	BEGIN
	RAISERROR ('Application not found or Application permission denied.', 16, 1)
	RETURN 1
	END

SELECT @ITEMID = Items.ItemId
	FROM         dbo.[netsqlazman_Applications]() Applications INNER JOIN
	                      dbo.[netsqlazman_Items]() Items ON Applications.ApplicationId = Items.ApplicationId INNER JOIN
	                      dbo.[netsqlazman_Stores]() Stores ON Applications.StoreId = Stores.StoreId
	WHERE     (Stores.StoreId = @STOREID) AND (Applications.ApplicationId = @ApplicationId) AND (Items.Name = @ITEMNAME) AND (@OPERATIONSONLY = 1 AND Items.ItemType=2 OR @OPERATIONSONLY = 0)
IF @ITEMID IS NULL
	BEGIN
	RAISERROR ('Item not found.', 16, 1)
	RETURN 1
	END
-- PREPARE RESULTSET FOR BIZ RULE CHECKING
CREATE TABLE #PARTIAL_RESULTS_TABLE 
	(   
		[ItemId] [int] NOT NULL ,
		[ItemName] [nvarchar] (255)  NOT NULL ,
		[ItemType] [tinyint] NOT NULL,
		[AuthorizationType] TINYINT,
		[BizRuleId] [int] NULL ,
		[BizRuleSource] TEXT,
		[BizRuleLanguage] TINYINT
	)
-- PREPARE RESULTSET FOR ATTRIBUTES
IF @RETRIEVEATTRIBUTES = 1
BEGIN
	CREATE TABLE #ATTRIBUTES_TABLE 
	(   
		[AttributeKey] [nvarchar] (255)  NOT NULL,
		[AttributeValue] [nvarchar] (4000)  NOT NULL,
		[ItemId] INT NULL
	)
--------------------------------------------------------------------------------
-- GET STORE AND APPLICATION ATTRIBUTES
--------------------------------------------------------------------------------
	INSERT INTO #ATTRIBUTES_TABLE SELECT AttributeKey, AttributeValue, NULL FROM dbo.[netsqlazman_StoreAttributesTable] StoreAttributes INNER JOIN dbo.[netsqlazman_StoresTable] Stores ON StoreAttributes.StoreId = Stores.StoreId WHERE Stores.StoreId = @STOREID
	INSERT INTO #ATTRIBUTES_TABLE SELECT AttributeKey, AttributeValue, NULL FROM dbo.[netsqlazman_ApplicationAttributesTable] ApplicationAttributes INNER JOIN dbo.[netsqlazman_ApplicationsTable] Applications ON ApplicationAttributes.ApplicationId = Applications.ApplicationId WHERE Applications.ApplicationId = @ApplicationId
END
--------------------------------------------------------------------------------
DECLARE @USERSID varbinary(85)
DECLARE @I INT
DECLARE @INDEX INT
DECLARE @APP VARBINARY(85)
DECLARE @SETTINGVALUE nvarchar(255)
DECLARE @NETSQLAZMANMODE bit

SELECT @SETTINGVALUE = SettingValue FROM dbo.[netsqlazman_Settings] WHERE SettingName = 'Mode'
IF @SETTINGVALUE = 'Developer' 
BEGIN
	SET @NETSQLAZMANMODE = 1 
END
ELSE 
BEGIN
	SET @NETSQLAZMANMODE = 0
END

CREATE TABLE #USERGROUPS (objectSid VARBINARY(85))
-- Get User Sid
IF @USERGROUPSCOUNT>0
BEGIN
	SET @USERSID = SUBSTRING(@TOKEN,1,85)
	SET @I = CHARINDEX(0x01, @USERSID)
	SET @USERSID = SUBSTRING(@USERSID, @I, 85-@I+1)
	-- Get User Groups Sid
	SET @I = 0
	WHILE (@I<@USERGROUPSCOUNT)
	BEGIN
		SET @APP = SUBSTRING(@TOKEN,(@I+1)*85+1,85) --GET USER GROUP TOKEN PORTION
		SET @INDEX = CHARINDEX(0x01, @APP) -- FIND TOKEN START (0x01)
		SET @APP = SUBSTRING(@APP, @INDEX, 85-@INDEX+1) -- EXTRACT USER GROUP SID
		INSERT INTO #USERGROUPS (objectSid) VALUES (@APP)
		SET @I = @I + 1
	END
END
ELSE
BEGIN
	SET @USERSID = @TOKEN
END

EXEC dbo.[netsqlazman_CheckAccess] @ITEMID, @USERSID, @VALIDFOR, @LDAPPATH, @AUTHORIZATION_TYPE OUTPUT, @NETSQLAZMANMODE, @RETRIEVEATTRIBUTES
SELECT * FROM #PARTIAL_RESULTS_TABLE
IF @RETRIEVEATTRIBUTES = 1
	SELECT * FROM #ATTRIBUTES_TABLE
DROP TABLE #PARTIAL_RESULTS_TABLE
IF @RETRIEVEATTRIBUTES = 1
	DROP TABLE #ATTRIBUTES_TABLE
DROP TABLE #USERGROUPS
SET NOCOUNT OFF
GO
/****** Object:  Default [DF_LoginAttemptHistory_AccessDate]    Script Date: 04/01/2014 22:13:28 ******/
ALTER TABLE [dbo].[LoginAttemptHistory] ADD  CONSTRAINT [DF_LoginAttemptHistory_AccessDate]  DEFAULT (getdate()) FOR [AccessDate]
GO
/****** Object:  Default [DF_CustomerGridTemplate_Version]    Script Date: 04/01/2014 22:13:28 ******/
ALTER TABLE [dbo].[CustomerGridTemplate] ADD  CONSTRAINT [DF_CustomerGridTemplate_Version]  DEFAULT ((0)) FOR [Version]
GO
/****** Object:  Default [DF_CustomerSetting_IsRequired]    Script Date: 04/01/2014 22:13:28 ******/
ALTER TABLE [dbo].[CustomerSettingType] ADD  CONSTRAINT [DF_CustomerSetting_IsRequired]  DEFAULT ((0)) FOR [IsRequired]
GO
/****** Object:  Default [DF_ErrorMessages_Active]    Script Date: 04/01/2014 22:13:28 ******/
ALTER TABLE [dbo].[ErrorMessages] ADD  CONSTRAINT [DF_ErrorMessages_Active]  DEFAULT ((1)) FOR [Active]
GO
/****** Object:  Default [DF_Log_SqlIdentity]    Script Date: 04/01/2014 22:13:45 ******/
ALTER TABLE [dbo].[netsqlazman_LogTable] ADD  CONSTRAINT [DF_Log_SqlIdentity]  DEFAULT (suser_sname()) FOR [SqlIdentity]
GO
/****** Object:  Default [DF_AccessLog_AccessDate]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[AccessLog] ADD  CONSTRAINT [DF_AccessLog_AccessDate]  DEFAULT (getdate()) FOR [AccessDate]
GO
/****** Object:  Default [DF_UserProfile_RequirePasswordReset]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[UserProfile] ADD  CONSTRAINT [DF_UserProfile_RequirePasswordReset]  DEFAULT ((1)) FOR [RequirePasswordReset]
GO
/****** Object:  Default [DF_News_CreatedDate]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[News] ADD  CONSTRAINT [DF_News_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
/****** Object:  Default [DF_News_Display]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[News] ADD  CONSTRAINT [DF_News_Display]  DEFAULT ((1)) FOR [Display]
GO
/****** Object:  Default [DF__Version__CHANGE___7C1A6C5A]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[Version] ADD  DEFAULT (getdate()) FOR [CHANGE_DATE]
GO
/****** Object:  Default [DF__webpages___IsCon__7D0E9093]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [IsConfirmed]
GO
/****** Object:  Default [DF__webpages___Passw__7E02B4CC]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [PasswordFailuresSinceLastSuccess]
GO
/****** Object:  Default [DF_webpages_Membership_IsActive]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[webpages_Membership] ADD  CONSTRAINT [DF_webpages_Membership_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_UserPasswordHistory_ChangeDate]    Script Date: 04/01/2014 22:19:46 ******/
ALTER TABLE [dbo].[UserPasswordHistory] ADD  CONSTRAINT [DF_UserPasswordHistory_ChangeDate]  DEFAULT (getdate()) FOR [ChangeDate]
GO
/****** Object:  Default [DF_CustomerGridTemplateCol_IsHidden]    Script Date: 04/01/2014 22:19:49 ******/
ALTER TABLE [dbo].[CustomerGridTemplateCol] ADD  CONSTRAINT [DF_CustomerGridTemplateCol_IsHidden]  DEFAULT ((0)) FOR [IsHidden]
GO
/****** Object:  Default [DF_CustomerProfile_Status]    Script Date: 04/01/2014 22:19:50 ******/
ALTER TABLE [dbo].[CustomerProfile] ADD  CONSTRAINT [DF_CustomerProfile_Status]  DEFAULT ((0)) FOR [Status]
GO
/****** Object:  Default [DF_CustomerProfile_StatusChangeDate]    Script Date: 04/01/2014 22:19:50 ******/
ALTER TABLE [dbo].[CustomerProfile] ADD  CONSTRAINT [DF_CustomerProfile_StatusChangeDate]  DEFAULT (getdate()) FOR [StatusChangeDate]
GO
/****** Object:  Default [DF_CustomerProfile_CustomerTypeId]    Script Date: 04/01/2014 22:19:50 ******/
ALTER TABLE [dbo].[CustomerProfile] ADD  CONSTRAINT [DF_CustomerProfile_CustomerTypeId]  DEFAULT ((1)) FOR [CustomerTypeId]
GO
/****** Object:  Default [DF_CustomerGrids_IsHidden]    Script Date: 04/01/2014 22:19:56 ******/
ALTER TABLE [dbo].[CustomerGrids] ADD  CONSTRAINT [DF_CustomerGrids_IsHidden]  DEFAULT ((0)) FOR [IsHidden]
GO
/****** Object:  Check [CK_Log]    Script Date: 04/01/2014 22:13:45 ******/
ALTER TABLE [dbo].[netsqlazman_LogTable]  WITH CHECK ADD  CONSTRAINT [CK_Log] CHECK  (([LogType]='I' OR [LogType]='W' OR [LogType]='E'))
GO
ALTER TABLE [dbo].[netsqlazman_LogTable] CHECK CONSTRAINT [CK_Log]
GO
/****** Object:  Check [CK_Settings]    Script Date: 04/01/2014 22:13:45 ******/
ALTER TABLE [dbo].[netsqlazman_Settings]  WITH CHECK ADD  CONSTRAINT [CK_Settings] CHECK  (([SettingName]='Mode' AND ([SettingValue]='Developer' OR [SettingValue]='Administrator') OR [SettingName]='LogErrors' AND ([SettingValue]='True' OR [SettingValue]='False') OR [SettingName]='LogWarnings' AND ([SettingValue]='True' OR [SettingValue]='False') OR [SettingName]='LogInformations' AND ([SettingValue]='True' OR [SettingValue]='False') OR [SettingName]='LogOnEventLog' AND ([SettingValue]='True' OR [SettingValue]='False') OR [SettingName]='LogOnDb' AND ([SettingValue]='True' OR [SettingValue]='False')))
GO
ALTER TABLE [dbo].[netsqlazman_Settings] CHECK CONSTRAINT [CK_Settings]
GO
/****** Object:  Check [CK_StorePermissions]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[netsqlazman_StorePermissionsTable]  WITH CHECK ADD  CONSTRAINT [CK_StorePermissions] CHECK  (([NetSqlAzManFixedServerRole]>=(0) AND [NetSqlAzManFixedServerRole]<=(2)))
GO
ALTER TABLE [dbo].[netsqlazman_StorePermissionsTable] CHECK CONSTRAINT [CK_StorePermissions]
GO
/****** Object:  Check [CK_StoreGroups_GroupType_Check]    Script Date: 04/01/2014 22:19:46 ******/
ALTER TABLE [dbo].[netsqlazman_StoreGroupsTable]  WITH CHECK ADD  CONSTRAINT [CK_StoreGroups_GroupType_Check] CHECK  (([GroupType]>=(0) AND [GroupType]<=(1)))
GO
ALTER TABLE [dbo].[netsqlazman_StoreGroupsTable] CHECK CONSTRAINT [CK_StoreGroups_GroupType_Check]
GO
/****** Object:  Check [CK_ApplicationPermissions]    Script Date: 04/01/2014 22:19:51 ******/
ALTER TABLE [dbo].[netsqlazman_ApplicationPermissionsTable]  WITH CHECK ADD  CONSTRAINT [CK_ApplicationPermissions] CHECK  (([NetSqlAzManFixedServerRole]>=(0) AND [NetSqlAzManFixedServerRole]<=(2)))
GO
ALTER TABLE [dbo].[netsqlazman_ApplicationPermissionsTable] CHECK CONSTRAINT [CK_ApplicationPermissions]
GO
/****** Object:  Check [CK_ApplicationGroups_GroupType_Check]    Script Date: 04/01/2014 22:19:51 ******/
ALTER TABLE [dbo].[netsqlazman_ApplicationGroupsTable]  WITH CHECK ADD  CONSTRAINT [CK_ApplicationGroups_GroupType_Check] CHECK  (([GroupType]>=(0) AND [GroupType]<=(1)))
GO
ALTER TABLE [dbo].[netsqlazman_ApplicationGroupsTable] CHECK CONSTRAINT [CK_ApplicationGroups_GroupType_Check]
GO
/****** Object:  Check [CK_Items_ItemTypeCheck]    Script Date: 04/01/2014 22:19:57 ******/
ALTER TABLE [dbo].[netsqlazman_ItemsTable]  WITH CHECK ADD  CONSTRAINT [CK_Items_ItemTypeCheck] CHECK  (([ItemType]>=(0) AND [ItemType]<=(2)))
GO
ALTER TABLE [dbo].[netsqlazman_ItemsTable] CHECK CONSTRAINT [CK_Items_ItemTypeCheck]
GO
/****** Object:  Check [CK_WhereDefinedCheck]    Script Date: 04/01/2014 22:19:57 ******/
ALTER TABLE [dbo].[netsqlazman_StoreGroupMembersTable]  WITH CHECK ADD  CONSTRAINT [CK_WhereDefinedCheck] CHECK  (([WhereDefined]=(0) OR [WhereDefined]>=(2) AND [WhereDefined]<=(4)))
GO
ALTER TABLE [dbo].[netsqlazman_StoreGroupMembersTable] CHECK CONSTRAINT [CK_WhereDefinedCheck]
GO
/****** Object:  Check [CK_AuthorizationTypeCheck]    Script Date: 04/01/2014 22:20:03 ******/
ALTER TABLE [dbo].[netsqlazman_AuthorizationsTable]  WITH CHECK ADD  CONSTRAINT [CK_AuthorizationTypeCheck] CHECK  (([AuthorizationType]>=(0) AND [AuthorizationType]<=(3)))
GO
ALTER TABLE [dbo].[netsqlazman_AuthorizationsTable] CHECK CONSTRAINT [CK_AuthorizationTypeCheck]
GO
/****** Object:  Check [CK_objectSidWhereDefinedCheck]    Script Date: 04/01/2014 22:20:03 ******/
ALTER TABLE [dbo].[netsqlazman_AuthorizationsTable]  WITH CHECK ADD  CONSTRAINT [CK_objectSidWhereDefinedCheck] CHECK  (([objectSidWhereDefined]>=(0) AND [objectSidWhereDefined]<=(4)))
GO
ALTER TABLE [dbo].[netsqlazman_AuthorizationsTable] CHECK CONSTRAINT [CK_objectSidWhereDefinedCheck]
GO
/****** Object:  Check [CK_ownerSidWhereDefined]    Script Date: 04/01/2014 22:20:03 ******/
ALTER TABLE [dbo].[netsqlazman_AuthorizationsTable]  WITH CHECK ADD  CONSTRAINT [CK_ownerSidWhereDefined] CHECK  (([ownerSidWhereDefined]>=(2) AND [ownerSidWhereDefined]<=(4)))
GO
ALTER TABLE [dbo].[netsqlazman_AuthorizationsTable] CHECK CONSTRAINT [CK_ownerSidWhereDefined]
GO
/****** Object:  Check [CK_ValidFromToCheck]    Script Date: 04/01/2014 22:20:03 ******/
ALTER TABLE [dbo].[netsqlazman_AuthorizationsTable]  WITH CHECK ADD  CONSTRAINT [CK_ValidFromToCheck] CHECK  (([ValidFrom] IS NULL OR [ValidTo] IS NULL OR [ValidFrom]<=[ValidTo]))
GO
ALTER TABLE [dbo].[netsqlazman_AuthorizationsTable] CHECK CONSTRAINT [CK_ValidFromToCheck]
GO
/****** Object:  Check [CK_WhereDefinedNotValid]    Script Date: 04/01/2014 22:20:03 ******/
ALTER TABLE [dbo].[netsqlazman_ApplicationGroupMembersTable]  WITH CHECK ADD  CONSTRAINT [CK_WhereDefinedNotValid] CHECK  (([WhereDefined]>=(0) AND [WhereDefined]<=(4)))
GO
ALTER TABLE [dbo].[netsqlazman_ApplicationGroupMembersTable] CHECK CONSTRAINT [CK_WhereDefinedNotValid]
GO
/****** Object:  ForeignKey [FK_webpages_Membership_UserProfile]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[webpages_Membership]  WITH CHECK ADD  CONSTRAINT [FK_webpages_Membership_UserProfile] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserProfile] ([UserId])
GO
ALTER TABLE [dbo].[webpages_Membership] CHECK CONSTRAINT [FK_webpages_Membership_UserProfile]
GO
/****** Object:  ForeignKey [FK_UserSettings_UserProfile]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[UserSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserSettings_UserProfile] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserProfile] ([UserId])
GO
ALTER TABLE [dbo].[UserSettings] CHECK CONSTRAINT [FK_UserSettings_UserProfile]
GO
/****** Object:  ForeignKey [FK_WorkOrders_WorkOrderStatus]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[WorkOrders]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrders_WorkOrderStatus] FOREIGN KEY([WorkOrderStatusId])
REFERENCES [dbo].[WorkOrderStatus] ([WorkOrderStatusId])
GO
ALTER TABLE [dbo].[WorkOrders] CHECK CONSTRAINT [FK_WorkOrders_WorkOrderStatus]
GO
/****** Object:  ForeignKey [fk_RoleId]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[webpages_UsersInRoles]  WITH CHECK ADD  CONSTRAINT [fk_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[webpages_Roles] ([RoleId])
GO
ALTER TABLE [dbo].[webpages_UsersInRoles] CHECK CONSTRAINT [fk_RoleId]
GO
/****** Object:  ForeignKey [fk_UserId]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[webpages_UsersInRoles]  WITH CHECK ADD  CONSTRAINT [fk_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserProfile] ([UserId])
GO
ALTER TABLE [dbo].[webpages_UsersInRoles] CHECK CONSTRAINT [fk_UserId]
GO
/****** Object:  ForeignKey [FK_StorePermissions_StoresTable]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[netsqlazman_StorePermissionsTable]  WITH CHECK ADD  CONSTRAINT [FK_StorePermissions_StoresTable] FOREIGN KEY([StoreId])
REFERENCES [dbo].[netsqlazman_StoresTable] ([StoreId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[netsqlazman_StorePermissionsTable] CHECK CONSTRAINT [FK_StorePermissions_StoresTable]
GO
/****** Object:  ForeignKey [FK_NewsContent_News]    Script Date: 04/01/2014 22:19:45 ******/
ALTER TABLE [dbo].[NewsContent]  WITH CHECK ADD  CONSTRAINT [FK_NewsContent_News] FOREIGN KEY([ContentId])
REFERENCES [dbo].[News] ([ContentId])
GO
ALTER TABLE [dbo].[NewsContent] CHECK CONSTRAINT [FK_NewsContent_News]
GO
/****** Object:  ForeignKey [FK_UserPasswordQuestion_UserProfile]    Script Date: 04/01/2014 22:19:46 ******/
ALTER TABLE [dbo].[UserPasswordQuestion]  WITH CHECK ADD  CONSTRAINT [FK_UserPasswordQuestion_UserProfile] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserProfile] ([UserId])
GO
ALTER TABLE [dbo].[UserPasswordQuestion] CHECK CONSTRAINT [FK_UserPasswordQuestion_UserProfile]
GO
/****** Object:  ForeignKey [FK_UserPasswordHistory_UserProfile]    Script Date: 04/01/2014 22:19:46 ******/
ALTER TABLE [dbo].[UserPasswordHistory]  WITH CHECK ADD  CONSTRAINT [FK_UserPasswordHistory_UserProfile] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserProfile] ([UserId])
GO
ALTER TABLE [dbo].[UserPasswordHistory] CHECK CONSTRAINT [FK_UserPasswordHistory_UserProfile]
GO
/****** Object:  ForeignKey [FK_UserCustomerAccess_Customer]    Script Date: 04/01/2014 22:19:46 ******/
ALTER TABLE [dbo].[UserCustomerAccess]  WITH CHECK ADD  CONSTRAINT [FK_UserCustomerAccess_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[netsqlazman_StoresTable] ([StoreId])
GO
ALTER TABLE [dbo].[UserCustomerAccess] CHECK CONSTRAINT [FK_UserCustomerAccess_Customer]
GO
/****** Object:  ForeignKey [FK_UserCustomerAccess_User]    Script Date: 04/01/2014 22:19:46 ******/
ALTER TABLE [dbo].[UserCustomerAccess]  WITH CHECK ADD  CONSTRAINT [FK_UserCustomerAccess_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserProfile] ([UserId])
GO
ALTER TABLE [dbo].[UserCustomerAccess] CHECK CONSTRAINT [FK_UserCustomerAccess_User]
GO
/****** Object:  ForeignKey [FK_ResourceEvents_ResourceEventTypes]    Script Date: 04/01/2014 22:19:46 ******/
ALTER TABLE [dbo].[ResourceEvents]  WITH CHECK ADD  CONSTRAINT [FK_ResourceEvents_ResourceEventTypes] FOREIGN KEY([EventTypeId])
REFERENCES [dbo].[ResourceEventTypes] ([EventTypeId])
GO
ALTER TABLE [dbo].[ResourceEvents] CHECK CONSTRAINT [FK_ResourceEvents_ResourceEventTypes]
GO
/****** Object:  ForeignKey [FK_StoreGroups_Stores]    Script Date: 04/01/2014 22:19:46 ******/
ALTER TABLE [dbo].[netsqlazman_StoreGroupsTable]  WITH CHECK ADD  CONSTRAINT [FK_StoreGroups_Stores] FOREIGN KEY([StoreId])
REFERENCES [dbo].[netsqlazman_StoresTable] ([StoreId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[netsqlazman_StoreGroupsTable] CHECK CONSTRAINT [FK_StoreGroups_Stores]
GO
/****** Object:  ForeignKey [FK_StoreAttributes_Stores]    Script Date: 04/01/2014 22:19:47 ******/
ALTER TABLE [dbo].[netsqlazman_StoreAttributesTable]  WITH CHECK ADD  CONSTRAINT [FK_StoreAttributes_Stores] FOREIGN KEY([StoreId])
REFERENCES [dbo].[netsqlazman_StoresTable] ([StoreId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[netsqlazman_StoreAttributesTable] CHECK CONSTRAINT [FK_StoreAttributes_Stores]
GO
/****** Object:  ForeignKey [FK_CustomerSettingTypeList_CustomerSettingType]    Script Date: 04/01/2014 22:19:49 ******/
ALTER TABLE [dbo].[CustomerSettingTypeList]  WITH CHECK ADD  CONSTRAINT [FK_CustomerSettingTypeList_CustomerSettingType] FOREIGN KEY([CustomerSettingTypeId])
REFERENCES [dbo].[CustomerSettingType] ([CustomerSettingTypeId])
GO
ALTER TABLE [dbo].[CustomerSettingTypeList] CHECK CONSTRAINT [FK_CustomerSettingTypeList_CustomerSettingType]
GO
/****** Object:  ForeignKey [FK_CustomerGridTemplateCol_CustomerGridTemplate]    Script Date: 04/01/2014 22:19:49 ******/
ALTER TABLE [dbo].[CustomerGridTemplateCol]  WITH CHECK ADD  CONSTRAINT [FK_CustomerGridTemplateCol_CustomerGridTemplate] FOREIGN KEY([CustomerGridTemplateId])
REFERENCES [dbo].[CustomerGridTemplate] ([CustomerGridTemplateId])
GO
ALTER TABLE [dbo].[CustomerGridTemplateCol] CHECK CONSTRAINT [FK_CustomerGridTemplateCol_CustomerGridTemplate]
GO
/****** Object:  ForeignKey [FK_AccessLogMembershipEvents_MembershipEventTypes]    Script Date: 04/01/2014 22:19:50 ******/
ALTER TABLE [dbo].[AccessLogMembershipEvents]  WITH CHECK ADD  CONSTRAINT [FK_AccessLogMembershipEvents_MembershipEventTypes] FOREIGN KEY([EventTypeId])
REFERENCES [dbo].[MembershipEventTypes] ([EventTypeId])
GO
ALTER TABLE [dbo].[AccessLogMembershipEvents] CHECK CONSTRAINT [FK_AccessLogMembershipEvents_MembershipEventTypes]
GO
/****** Object:  ForeignKey [FK_CustomerReports_CustomerReportsCategory]    Script Date: 04/01/2014 22:19:50 ******/
ALTER TABLE [dbo].[CustomerReports]  WITH CHECK ADD  CONSTRAINT [FK_CustomerReports_CustomerReportsCategory] FOREIGN KEY([CustomerReportsCategoryId])
REFERENCES [dbo].[CustomerReportsCategory] ([CustomerReportsCategoryId])
GO
ALTER TABLE [dbo].[CustomerReports] CHECK CONSTRAINT [FK_CustomerReports_CustomerReportsCategory]
GO
/****** Object:  ForeignKey [FK_CustomerProfile_CustomerType]    Script Date: 04/01/2014 22:19:50 ******/
ALTER TABLE [dbo].[CustomerProfile]  WITH CHECK ADD  CONSTRAINT [FK_CustomerProfile_CustomerType] FOREIGN KEY([CustomerTypeId])
REFERENCES [dbo].[CustomerType] ([CustomerTypeId])
GO
ALTER TABLE [dbo].[CustomerProfile] CHECK CONSTRAINT [FK_CustomerProfile_CustomerType]
GO
/****** Object:  ForeignKey [FK_Applications_Stores]    Script Date: 04/01/2014 22:19:51 ******/
ALTER TABLE [dbo].[netsqlazman_ApplicationsTable]  WITH CHECK ADD  CONSTRAINT [FK_Applications_Stores] FOREIGN KEY([StoreId])
REFERENCES [dbo].[netsqlazman_StoresTable] ([StoreId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[netsqlazman_ApplicationsTable] CHECK CONSTRAINT [FK_Applications_Stores]
GO
/****** Object:  ForeignKey [FK_ApplicationPermissions_ApplicationsTable]    Script Date: 04/01/2014 22:19:51 ******/
ALTER TABLE [dbo].[netsqlazman_ApplicationPermissionsTable]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationPermissions_ApplicationsTable] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[netsqlazman_ApplicationsTable] ([ApplicationId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[netsqlazman_ApplicationPermissionsTable] CHECK CONSTRAINT [FK_ApplicationPermissions_ApplicationsTable]
GO
/****** Object:  ForeignKey [FK_ApplicationGroups_Applications]    Script Date: 04/01/2014 22:19:51 ******/
ALTER TABLE [dbo].[netsqlazman_ApplicationGroupsTable]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationGroups_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[netsqlazman_ApplicationsTable] ([ApplicationId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[netsqlazman_ApplicationGroupsTable] CHECK CONSTRAINT [FK_ApplicationGroups_Applications]
GO
/****** Object:  ForeignKey [FK_ApplicationAttributes_Applications]    Script Date: 04/01/2014 22:19:51 ******/
ALTER TABLE [dbo].[netsqlazman_ApplicationAttributesTable]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationAttributes_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[netsqlazman_ApplicationsTable] ([ApplicationId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[netsqlazman_ApplicationAttributesTable] CHECK CONSTRAINT [FK_ApplicationAttributes_Applications]
GO
/****** Object:  ForeignKey [FK_MaintenanceGroupCustomer_Customer]    Script Date: 04/01/2014 22:19:51 ******/
ALTER TABLE [dbo].[MaintenanceGroupCustomer]  WITH CHECK ADD  CONSTRAINT [FK_MaintenanceGroupCustomer_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[CustomerProfile] ([CustomerId])
GO
ALTER TABLE [dbo].[MaintenanceGroupCustomer] CHECK CONSTRAINT [FK_MaintenanceGroupCustomer_Customer]
GO
/****** Object:  ForeignKey [FK_MaintenanceGroupCustomer_MaintenanceGroup]    Script Date: 04/01/2014 22:19:51 ******/
ALTER TABLE [dbo].[MaintenanceGroupCustomer]  WITH CHECK ADD  CONSTRAINT [FK_MaintenanceGroupCustomer_MaintenanceGroup] FOREIGN KEY([MaintenanceGroupId])
REFERENCES [dbo].[CustomerProfile] ([CustomerId])
GO
ALTER TABLE [dbo].[MaintenanceGroupCustomer] CHECK CONSTRAINT [FK_MaintenanceGroupCustomer_MaintenanceGroup]
GO
/****** Object:  ForeignKey [FK_CustomerGridTempateMap_CustomerProfile]    Script Date: 04/01/2014 22:19:56 ******/
ALTER TABLE [dbo].[CustomerGridTemplateMap]  WITH CHECK ADD  CONSTRAINT [FK_CustomerGridTempateMap_CustomerProfile] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[CustomerProfile] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerGridTemplateMap] CHECK CONSTRAINT [FK_CustomerGridTempateMap_CustomerProfile]
GO
/****** Object:  ForeignKey [FK_CustomerGrids_CustomerProfile]    Script Date: 04/01/2014 22:19:56 ******/
ALTER TABLE [dbo].[CustomerGrids]  WITH CHECK ADD  CONSTRAINT [FK_CustomerGrids_CustomerProfile] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[CustomerProfile] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerGrids] CHECK CONSTRAINT [FK_CustomerGrids_CustomerProfile]
GO
/****** Object:  ForeignKey [FK_CustomerSettingsGroup_CustomerProfile]    Script Date: 04/01/2014 22:19:56 ******/
ALTER TABLE [dbo].[CustomerSettingsGroup]  WITH CHECK ADD  CONSTRAINT [FK_CustomerSettingsGroup_CustomerProfile] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[CustomerProfile] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerSettingsGroup] CHECK CONSTRAINT [FK_CustomerSettingsGroup_CustomerProfile]
GO
/****** Object:  ForeignKey [FK_CustomerSettings_CustomerProfile]    Script Date: 04/01/2014 22:19:56 ******/
ALTER TABLE [dbo].[CustomerSettings]  WITH CHECK ADD  CONSTRAINT [FK_CustomerSettings_CustomerProfile] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[CustomerProfile] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerSettings] CHECK CONSTRAINT [FK_CustomerSettings_CustomerProfile]
GO
/****** Object:  ForeignKey [FK_CustomerSettings_CustomerSetting]    Script Date: 04/01/2014 22:19:56 ******/
ALTER TABLE [dbo].[CustomerSettings]  WITH CHECK ADD  CONSTRAINT [FK_CustomerSettings_CustomerSetting] FOREIGN KEY([CustomerSettingTypeId])
REFERENCES [dbo].[CustomerSettingType] ([CustomerSettingTypeId])
GO
ALTER TABLE [dbo].[CustomerSettings] CHECK CONSTRAINT [FK_CustomerSettings_CustomerSetting]
GO
/****** Object:  ForeignKey [FK_Items_Applications]    Script Date: 04/01/2014 22:19:57 ******/
ALTER TABLE [dbo].[netsqlazman_ItemsTable]  WITH CHECK ADD  CONSTRAINT [FK_Items_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[netsqlazman_ApplicationsTable] ([ApplicationId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[netsqlazman_ItemsTable] CHECK CONSTRAINT [FK_Items_Applications]
GO
/****** Object:  ForeignKey [FK_Items_BizRules]    Script Date: 04/01/2014 22:19:57 ******/
ALTER TABLE [dbo].[netsqlazman_ItemsTable]  WITH CHECK ADD  CONSTRAINT [FK_Items_BizRules] FOREIGN KEY([BizRuleId])
REFERENCES [dbo].[netsqlazman_BizRulesTable] ([BizRuleId])
GO
ALTER TABLE [dbo].[netsqlazman_ItemsTable] CHECK CONSTRAINT [FK_Items_BizRules]
GO
/****** Object:  ForeignKey [FK_StoreGroupMembers_StoreGroup]    Script Date: 04/01/2014 22:19:57 ******/
ALTER TABLE [dbo].[netsqlazman_StoreGroupMembersTable]  WITH CHECK ADD  CONSTRAINT [FK_StoreGroupMembers_StoreGroup] FOREIGN KEY([StoreGroupId])
REFERENCES [dbo].[netsqlazman_StoreGroupsTable] ([StoreGroupId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[netsqlazman_StoreGroupMembersTable] CHECK CONSTRAINT [FK_StoreGroupMembers_StoreGroup]
GO
/****** Object:  ForeignKey [FK_WorkOrdersAudit_WorkOrders]    Script Date: 04/01/2014 22:19:57 ******/
ALTER TABLE [dbo].[WorkOrdersAudit]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrdersAudit_WorkOrders] FOREIGN KEY([WorkOrderId])
REFERENCES [dbo].[WorkOrders] ([WorkOrderId])
GO
ALTER TABLE [dbo].[WorkOrdersAudit] CHECK CONSTRAINT [FK_WorkOrdersAudit_WorkOrders]
GO
/****** Object:  ForeignKey [FK_WorkOrderPart_Parts]    Script Date: 04/01/2014 22:19:57 ******/
ALTER TABLE [dbo].[WorkOrderPart]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrderPart_Parts] FOREIGN KEY([PartId])
REFERENCES [dbo].[Parts] ([PartId])
GO
ALTER TABLE [dbo].[WorkOrderPart] CHECK CONSTRAINT [FK_WorkOrderPart_Parts]
GO
/****** Object:  ForeignKey [FK_WorkOrderPart_WorkOrders]    Script Date: 04/01/2014 22:19:57 ******/
ALTER TABLE [dbo].[WorkOrderPart]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrderPart_WorkOrders] FOREIGN KEY([WorkOrderId])
REFERENCES [dbo].[WorkOrders] ([WorkOrderId])
GO
ALTER TABLE [dbo].[WorkOrderPart] CHECK CONSTRAINT [FK_WorkOrderPart_WorkOrders]
GO
/****** Object:  ForeignKey [FK_WorkOrderImage_WorkOrders]    Script Date: 04/01/2014 22:19:57 ******/
ALTER TABLE [dbo].[WorkOrderImage]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrderImage_WorkOrders] FOREIGN KEY([WorkOrderId])
REFERENCES [dbo].[WorkOrders] ([WorkOrderId])
GO
ALTER TABLE [dbo].[WorkOrderImage] CHECK CONSTRAINT [FK_WorkOrderImage_WorkOrders]
GO
/****** Object:  ForeignKey [FK_WorkOrderEvent_WorkOrders]    Script Date: 04/01/2014 22:19:57 ******/
ALTER TABLE [dbo].[WorkOrderEvent]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrderEvent_WorkOrders] FOREIGN KEY([WorkOrderId])
REFERENCES [dbo].[WorkOrders] ([WorkOrderId])
GO
ALTER TABLE [dbo].[WorkOrderEvent] CHECK CONSTRAINT [FK_WorkOrderEvent_WorkOrders]
GO
/****** Object:  ForeignKey [FK_User_CustomerGrids]    Script Date: 04/01/2014 22:20:00 ******/
ALTER TABLE [dbo].[User_CustomerGrids]  WITH CHECK ADD  CONSTRAINT [FK_User_CustomerGrids] FOREIGN KEY([CustomerGridsId])
REFERENCES [dbo].[CustomerGrids] ([CustomerGridsId])
GO
ALTER TABLE [dbo].[User_CustomerGrids] CHECK CONSTRAINT [FK_User_CustomerGrids]
GO
/****** Object:  ForeignKey [FK_ItemAttributes_Items]    Script Date: 04/01/2014 22:20:00 ******/
ALTER TABLE [dbo].[netsqlazman_ItemAttributesTable]  WITH CHECK ADD  CONSTRAINT [FK_ItemAttributes_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[netsqlazman_ItemsTable] ([ItemId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[netsqlazman_ItemAttributesTable] CHECK CONSTRAINT [FK_ItemAttributes_Items]
GO
/****** Object:  ForeignKey [FK_CustomerSettingsGroupMember_CustomerSetting]    Script Date: 04/01/2014 22:20:02 ******/
ALTER TABLE [dbo].[CustomerSettingsGroupMember]  WITH CHECK ADD  CONSTRAINT [FK_CustomerSettingsGroupMember_CustomerSetting] FOREIGN KEY([CustomerSettingTypeId])
REFERENCES [dbo].[CustomerSettingType] ([CustomerSettingTypeId])
GO
ALTER TABLE [dbo].[CustomerSettingsGroupMember] CHECK CONSTRAINT [FK_CustomerSettingsGroupMember_CustomerSetting]
GO
/****** Object:  ForeignKey [FK_CustomerSettingsGroupMember_CustomerSettingsGroup]    Script Date: 04/01/2014 22:20:02 ******/
ALTER TABLE [dbo].[CustomerSettingsGroupMember]  WITH CHECK ADD  CONSTRAINT [FK_CustomerSettingsGroupMember_CustomerSettingsGroup] FOREIGN KEY([CustomerSettingsGroupId])
REFERENCES [dbo].[CustomerSettingsGroup] ([CustomerSettingsGroupId])
GO
ALTER TABLE [dbo].[CustomerSettingsGroupMember] CHECK CONSTRAINT [FK_CustomerSettingsGroupMember_CustomerSettingsGroup]
GO
/****** Object:  ForeignKey [FK_Authorizations_Items]    Script Date: 04/01/2014 22:20:03 ******/
ALTER TABLE [dbo].[netsqlazman_AuthorizationsTable]  WITH CHECK ADD  CONSTRAINT [FK_Authorizations_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[netsqlazman_ItemsTable] ([ItemId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[netsqlazman_AuthorizationsTable] CHECK CONSTRAINT [FK_Authorizations_Items]
GO
/****** Object:  ForeignKey [FK_ApplicationGroupMembers_ApplicationGroup]    Script Date: 04/01/2014 22:20:03 ******/
ALTER TABLE [dbo].[netsqlazman_ApplicationGroupMembersTable]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationGroupMembers_ApplicationGroup] FOREIGN KEY([ApplicationGroupId])
REFERENCES [dbo].[netsqlazman_ApplicationGroupsTable] ([ApplicationGroupId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[netsqlazman_ApplicationGroupMembersTable] CHECK CONSTRAINT [FK_ApplicationGroupMembers_ApplicationGroup]
GO
/****** Object:  ForeignKey [FK_AuthorizationAttributes_Authorizations]    Script Date: 04/01/2014 22:20:04 ******/
ALTER TABLE [dbo].[netsqlazman_AuthorizationAttributesTable]  WITH CHECK ADD  CONSTRAINT [FK_AuthorizationAttributes_Authorizations] FOREIGN KEY([AuthorizationId])
REFERENCES [dbo].[netsqlazman_AuthorizationsTable] ([AuthorizationId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[netsqlazman_AuthorizationAttributesTable] CHECK CONSTRAINT [FK_AuthorizationAttributes_Authorizations]
GO
