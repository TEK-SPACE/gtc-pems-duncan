USE [PEMS_US_SIT]

--    ------------------------------------------------------------------------------

if not exists (select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='AI_OFFICERS' )
begin 

/****** Object:  Table [dbo].[AI_OFFICERS]   ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[AI_OFFICERS](
	[OfficerId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[OfficerName] [varchar](200) NOT NULL,
	[OfficerIdentifier] [varchar](50) NOT NULL,
	[DateAdded] [datetime] NULL,
	[ActivityStatus] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[OfficerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF

ALTER TABLE [dbo].[AI_OFFICERS]  WITH CHECK ADD  CONSTRAINT [FK_AI_OFFICERS_AssetState] FOREIGN KEY([ActivityStatus])
REFERENCES [dbo].[AssetState] ([AssetStateId])


ALTER TABLE [dbo].[AI_OFFICERS] CHECK CONSTRAINT [FK_AI_OFFICERS_AssetState]


ALTER TABLE [dbo].[AI_OFFICERS]  WITH CHECK ADD  CONSTRAINT [FK_AI_OFFICERS_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])


ALTER TABLE [dbo].[AI_OFFICERS] CHECK CONSTRAINT [FK_AI_OFFICERS_Customers]


ALTER TABLE [dbo].[AI_OFFICERS] ADD  DEFAULT (getdate()) FOR [DateAdded]
print 'AI_OFFICERS table created'
end

-- ----------------------------------------------------------------------------------------
if not exists (select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='AI_PARKING' )
begin 
/****** Object:  Table [dbo].[AI_PARKING]    ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON


SET ANSI_PADDING ON


CREATE TABLE [dbo].[AI_PARKING](
	[ParkingId] [int] NOT NULL,
	[IssueDateTime] [datetime] NOT NULL,
	[UnitSerial] [varchar](12) NOT NULL,
	[IssueNoPfx] [varchar](20) NOT NULL,
	[IssueNo] [bigint] NOT NULL,
	[IssueNoSfx] [varchar](20) NOT NULL,
	[IssueNoChkDgt] [varchar](5) NOT NULL,
	[DueDate] [datetime] NOT NULL,
	[Remark1] [varchar](200) NOT NULL,
	[Remark2] [varchar](200) NOT NULL,
	[Remark3] [varchar](200) NOT NULL,
	[VoidStatus] [varchar](2) NOT NULL,
	[VoidStatusDate] [datetime] NOT NULL,
	[VoidReason] [varchar](80) NOT NULL,
	[VoidedInField] [varchar](1) NOT NULL,
	[Reissued] [varchar](1) NOT NULL,
	[OfficerID] [int] NOT NULL,
	[Agency] [varchar](30) NOT NULL,
	[Beat] [varchar](30) NOT NULL,
	[LocLot] [varchar](20) NOT NULL,
	[LocBlock] [varchar](20) NOT NULL,
	[LocStreet] [varchar](60) NOT NULL,
	[LocDescriptor] [varchar](20) NOT NULL,
	[LocCrossStreet1] [varchar](60) NOT NULL,
	[LocCrossStreet2] [varchar](60) NOT NULL,
	[LocLatitude] [real] NOT NULL,
	[LocLongitude] [real] NOT NULL,
	[LocDirection] [varchar](20) NOT NULL,
	[LocCity] [varchar](200) NOT NULL,
	[LocState] [varchar](20) NOT NULL,
	[LocSuburb] [int] NOT NULL,
	[VehLicNo] [varchar](20) NOT NULL,
	[VehLicState] [varchar](7) NOT NULL,
	[VehVIN] [varchar](50) NOT NULL,
	[VehColor1] [varchar](8) NOT NULL,
	[VehColor2] [varchar](8) NOT NULL,
	[VehMake] [varchar](30) NOT NULL,
	[VehModel] [varchar](30) NOT NULL,
	[VehBodyStyle] [varchar](15) NOT NULL,
	[VehLicExpDate] [datetime] NOT NULL,
	[VehVIN4] [varchar](20) NOT NULL,
	[PermitNo] [varchar](14) NOT NULL,
	[MeterNo] [varchar](10) NOT NULL,
	[MeterBayNo] [varchar](10) NOT NULL,
	[IsWarning] [varchar](1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterID] [int] NOT NULL,
	[ParkingSpaceId] [bigint] NOT NULL,
	[VEHLICTYPE] [varchar](80) NOT NULL,
	[VEHLABELNO] [varchar](80) NOT NULL,
	[VEHCHECKDIGIT] [varchar](50) NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Type] [varchar](20) NOT NULL,
	[LOCSIDEOFSTREET] [varchar](10) NOT NULL,
	[LOCZIP] [varchar](10) NOT NULL,
	[ViolationClass] [varchar](50) NOT NULL,
	[ViolationCode] [varchar](50) NOT NULL,
	[ViolationDesc] [varchar](250) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ParkingId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF


ALTER TABLE [dbo].[AI_PARKING]  WITH CHECK ADD  CONSTRAINT [FK_AI_PARKING_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterID])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])

ALTER TABLE [dbo].[AI_PARKING] CHECK CONSTRAINT [FK_AI_PARKING_Meters]


ALTER TABLE [dbo].[AI_PARKING]  WITH CHECK ADD  CONSTRAINT [FK_AI_PARKING_ParkingSpaces] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])


ALTER TABLE [dbo].[AI_PARKING] CHECK CONSTRAINT [FK_AI_PARKING_ParkingSpaces]

print 'AI_PARKING table created'
end


-- ----------------------------------------------------------------------------------------
if not exists (select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='AI_PARK_NOTE' )
begin 
/****** Object:  Table [dbo].[AI_PARK_NOTE]    ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


SET ANSI_PADDING ON


CREATE TABLE [dbo].[AI_PARK_NOTE](
	[ParkNoteId] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[ParkingId] [int] NULL,
	[NoteDateTime] [datetime] NULL,
	[OfficerID] [int] NULL,
	[NotesMemo] [varchar](500) NULL,
	[MultimediaNoteDataType] [varchar](30) NULL,
	[MultimediaNoteData] [image] NULL,
PRIMARY KEY CLUSTERED 
(
	[ParkNoteId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



SET ANSI_PADDING OFF


ALTER TABLE [dbo].[AI_PARK_NOTE]  WITH CHECK ADD  CONSTRAINT [FK_AI_PARK_NOTE_AI_PARKING] FOREIGN KEY([ParkingId])
REFERENCES [dbo].[AI_PARKING] ([ParkingId])


ALTER TABLE [dbo].[AI_PARK_NOTE] CHECK CONSTRAINT [FK_AI_PARK_NOTE_AI_PARKING]


ALTER TABLE [dbo].[AI_PARK_NOTE]  WITH CHECK ADD  CONSTRAINT [FK_AI_PARK_NOTE_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])


ALTER TABLE [dbo].[AI_PARK_NOTE] CHECK CONSTRAINT [FK_AI_PARK_NOTE_Customers]


print 'AI_PARK_NOTE table created'
end

--  ------------------------------------------------------------------------------
if not exists (select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='AI_ACTIVITYLOG' )
begin 
   SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


SET ANSI_PADDING ON


CREATE TABLE [dbo].[AI_ACTIVITYLOG](
	[ActivityLogId] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[StartDateTime] [datetime] NULL,
	[EndDateTime] [datetime] NULL,
	[OfficerId] [int] NULL,
	[PrimaryActivityName] [varchar](40) NULL,
	[PrimaryActivityCount] [int] NULL,
	[StartLocLatitude] [float] NULL,
	[StartLocLongitude] [float] NULL,
	[UnitSerial] [varchar](12) NULL,
PRIMARY KEY CLUSTERED 
(
	[ActivityLogId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]



SET ANSI_PADDING OFF


ALTER TABLE [dbo].[AI_ACTIVITYLOG]  WITH CHECK ADD  CONSTRAINT [FK_AI_ACTIVITYLOG_AI_OFFICERS] FOREIGN KEY([OfficerId])
REFERENCES [dbo].[AI_OFFICERS] ([OfficerId])


ALTER TABLE [dbo].[AI_ACTIVITYLOG] CHECK CONSTRAINT [FK_AI_ACTIVITYLOG_AI_OFFICERS]


ALTER TABLE [dbo].[AI_ACTIVITYLOG]  WITH CHECK ADD  CONSTRAINT [FK_AI_ACTIVITYLOG_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])


ALTER TABLE [dbo].[AI_ACTIVITYLOG] CHECK CONSTRAINT [FK_AI_ACTIVITYLOG_Customers]

    print 'AI_ACTIVITYLOG table created'
end

-- ----------------------------------------------------------------------------------------
if not exists (select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='AI_PARK_NOTE' )
begin 

/****** Object:  Table [dbo].[AI_PARK_NOTE]    ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


SET ANSI_PADDING ON


CREATE TABLE [dbo].[AI_PARK_NOTE](
	[ParkNoteId] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[ParkingId] [int] NULL,
	[NoteDateTime] [datetime] NULL,
	[OfficerID] [int] NULL,
	[NotesMemo] [varchar](500) NULL,
	[MultimediaNoteDataType] [varchar](30) NULL,
	[MultimediaNoteData] [image] NULL,
PRIMARY KEY CLUSTERED 
(
	[ParkNoteId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



SET ANSI_PADDING OFF


ALTER TABLE [dbo].[AI_PARK_NOTE]  WITH CHECK ADD  CONSTRAINT [FK_AI_PARK_NOTE_AI_PARKING] FOREIGN KEY([ParkingId])
REFERENCES [dbo].[AI_PARKING] ([ParkingId])


ALTER TABLE [dbo].[AI_PARK_NOTE] CHECK CONSTRAINT [FK_AI_PARK_NOTE_AI_PARKING]


ALTER TABLE [dbo].[AI_PARK_NOTE]  WITH CHECK ADD  CONSTRAINT [FK_AI_PARK_NOTE_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])


ALTER TABLE [dbo].[AI_PARK_NOTE] CHECK CONSTRAINT [FK_AI_PARK_NOTE_Customers]


print 'AI_PARK_NOTE table created'
end

-- ----------------------------------------------------------------------------------------
if not exists (select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='AI_EXPORT' )
begin 
/****** Object:  Table [dbo].[AI_EXPORT]    Script Date: 05/21/2014 10:42:42 ******/
/****** Object:  Table [dbo].[AI_PARKING_TRANSLINK]     ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON


SET ANSI_PADDING ON

CREATE TABLE [dbo].[AI_EXPORT](
	[ExportId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[ExportDateTime] [datetime] NOT NULL,
	[ParkingId] [int] NULL,
	[ExportType] [int] NULL,
	[ExportUserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ExportId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[AI_EXPORT]  WITH CHECK ADD  CONSTRAINT [FK_AI_EXPORT_AI_PARKING] FOREIGN KEY([ParkingId])
REFERENCES [dbo].[AI_PARKING] ([ParkingId])

ALTER TABLE [dbo].[AI_EXPORT] CHECK CONSTRAINT [FK_AI_EXPORT_AI_PARKING]


ALTER TABLE [dbo].[AI_EXPORT]  WITH CHECK ADD  CONSTRAINT [FK_AI_EXPORT_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])


ALTER TABLE [dbo].[AI_EXPORT] CHECK CONSTRAINT [FK_AI_EXPORT_Customers]


ALTER TABLE [dbo].[AI_EXPORT]  WITH CHECK ADD  CONSTRAINT [FK_AI_EXPORT_USERS] FOREIGN KEY([ExportUserId])
REFERENCES [dbo].[Users] ([UserID])


ALTER TABLE [dbo].[AI_EXPORT] CHECK CONSTRAINT [FK_AI_EXPORT_USERS]

print 'AI_EXPORT table created'

end


-- ----------------------------------------------------------------------------------------

-- ----------------------------------------------------------------------------------------
if not exists (select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='AI_PARKING_TRANSLINK' )
begin 
/****** Object:  Table [dbo].[AI_PARKING_TRANSLINK]     ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


SET ANSI_PADDING ON


CREATE TABLE [dbo].[AI_PARKING_TRANSLINK](
	[ParkingTransLinkId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[ParkingId] [int] NULL,
	[ExportId] [int] NULL,
	[StatusDateTime] [datetime] NULL,
	[StatusReason] [varchar](80) NULL,
	[StatusValue] [varchar](5) NULL,
	[OfficerId] [int] NULL
) ON [PRIMARY]



SET ANSI_PADDING OFF


ALTER TABLE [dbo].[AI_PARKING_TRANSLINK]  WITH CHECK ADD  CONSTRAINT [FK_AI_PARKING_TRANSLINK_AI_Export] FOREIGN KEY([ExportId])
REFERENCES [dbo].[AI_EXPORT] ([ExportId])


ALTER TABLE [dbo].[AI_PARKING_TRANSLINK] CHECK CONSTRAINT [FK_AI_PARKING_TRANSLINK_AI_Export]


ALTER TABLE [dbo].[AI_PARKING_TRANSLINK]  WITH CHECK ADD  CONSTRAINT [FK_AI_PARKING_TRANSLINK_AI_Officers] FOREIGN KEY([OfficerId])
REFERENCES [dbo].[AI_OFFICERS] ([OfficerId])


ALTER TABLE [dbo].[AI_PARKING_TRANSLINK] CHECK CONSTRAINT [FK_AI_PARKING_TRANSLINK_AI_Officers]


ALTER TABLE [dbo].[AI_PARKING_TRANSLINK]  WITH CHECK ADD  CONSTRAINT [FK_AI_PARKING_TRANSLINK_AI_PARKING] FOREIGN KEY([ParkingId])
REFERENCES [dbo].[AI_PARKING] ([ParkingId])


ALTER TABLE [dbo].[AI_PARKING_TRANSLINK] CHECK CONSTRAINT [FK_AI_PARKING_TRANSLINK_AI_PARKING]


ALTER TABLE [dbo].[AI_PARKING_TRANSLINK]  WITH CHECK ADD  CONSTRAINT [FK_AI_PARKING_TRANSLINK_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])


ALTER TABLE [dbo].[AI_PARKING_TRANSLINK] CHECK CONSTRAINT [FK_AI_PARKING_TRANSLINK_Customers]




print 'AI_PARKING_TRANSLINK table created'
end

-- ----------------------------------------------------------------------------------------
if not exists (select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='AI_PARKING_VIOS' )
begin 
/****** Object:  Table [dbo].[AI_PARKING_VIOS]    Script Date: 05/21/2014 10:39:56 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


SET ANSI_PADDING ON


CREATE TABLE [dbo].[AI_PARKING_VIOS](
	[ParkingViosId] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[ParkingId] [int] NULL,
	[OccurNo] [int] NULL,
	[VioCode] [varchar](30) NULL,
	[VioXferCode] [varchar](20) NULL,
	[VioDescription1] [varchar](80) NULL,
	[VioDescription2] [varchar](80) NULL,
	[VioFine] [real] NULL,
	[VioLateFee1] [real] NULL,
	[VioLateFee2] [real] NULL,
	[VioLateFee3] [real] NULL,
	[VioQueryType] [varchar](20) NULL,
	[VioSelect] [varchar](80) NULL,
PRIMARY KEY CLUSTERED 
(
	[ParkingViosId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]



SET ANSI_PADDING OFF


ALTER TABLE [dbo].[AI_PARKING_VIOS]  WITH CHECK ADD  CONSTRAINT [FK_AI_PARKING_VIOS_AI_PARKING] FOREIGN KEY([ParkingId])
REFERENCES [dbo].[AI_PARKING] ([ParkingId])


ALTER TABLE [dbo].[AI_PARKING_VIOS] CHECK CONSTRAINT [FK_AI_PARKING_VIOS_AI_PARKING]


ALTER TABLE [dbo].[AI_PARKING_VIOS]  WITH CHECK ADD  CONSTRAINT [FK_AI_PARKING_VIOS_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])


ALTER TABLE [dbo].[AI_PARKING_VIOS] CHECK CONSTRAINT [FK_AI_PARKING_VIOS_Customers]


print 'AI_PARKING_VIOS table created'
end


if not exists (select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='FDFilesAudit' )
begin 
/****** Object:  Table [dbo].[FDFilesAudit]    Script Date: 05/21/2014 10:45:49 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


SET ANSI_PADDING ON


CREATE TABLE [dbo].[FDFilesAudit](
	[AuditID] [bigint] IDENTITY(1,1) NOT NULL,
	[FileId] [bigint] NOT NULL,
	[FileStatus] [int] NULL,
	[UploadedBy] [varchar](50) NULL,
 CONSTRAINT [PK_FDFilesAudit_1] PRIMARY KEY CLUSTERED 
(
	[AuditID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]



SET ANSI_PADDING OFF


ALTER TABLE [dbo].[FDFilesAudit]  WITH CHECK ADD  CONSTRAINT [FK_FDFilesAudit_FDFiles] FOREIGN KEY([FileId])
REFERENCES [dbo].[FDFiles] ([FileID])


ALTER TABLE [dbo].[FDFilesAudit] CHECK CONSTRAINT [FK_FDFilesAudit_FDFiles]

print 'FDFilesAudit table created'
end