/*
   den 19 januari 201119:28:33
   User: 
   Server: .\SQLEXPRESS2010
   Database: 
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Global_Errors_Log
	(
	uid int NOT NULL IDENTITY (1, 1),
	Application_Id uniqueidentifier NULL,
	User_ID uniqueidentifier NULL,
	User_Email varchar(50) NULL,
	Load_Date varchar(50) NULL,
	Error_Message text NULL,
	Error_Previous_URL text NULL,
	Error_URL text NULL,
	Error_Target text NULL,
	Error_Source text NULL,
	Trace_Error text NULL,
	Error_Trace text NULL,
	Last_Exception text NULL,
	Session_Data text NULL,
	Version varchar(15) NULL,
	Date_Time datetime NULL,
	Reviewed int NOT NULL,
	Domain nvarchar(MAX) NULL,
	Error_Url_Path nvarchar(MAX) NULL,
	Error_Url_QS nvarchar(MAX) NULL,
	Error_Previous_Url_Path nvarchar(MAX) NULL,
	Error_Previous_Url_QS nvarchar(MAX) NULL,
	AdditionalInformation text NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Global_Errors_Log SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Global_Errors_Log ON
GO
IF EXISTS(SELECT * FROM dbo.Global_Errors_Log)
	 EXEC('INSERT INTO dbo.Tmp_Global_Errors_Log (uid, Application_Id, User_ID, User_Email, Load_Date, Error_Message, Error_Previous_URL, Error_URL, Error_Target, Error_Source, Trace_Error, Error_Trace, Last_Exception, Session_Data, Version, Date_Time, Reviewed, Error_Url_Path, Error_Url_QS, Error_Previous_Url_Path, Error_Previous_Url_QS, AdditionalInformation)
		SELECT uid, Application_Id, User_ID, User_Email, Load_Date, Error_Message, Error_Previous_URL, Error_URL, Error_Target, Error_Source, Trace_Error, Error_Trace, Last_Exception, Session_Data, Version, Date_Time, Reviewed, CONVERT(nvarchar(MAX), ExtraColumn1), CONVERT(nvarchar(MAX), ExtraColumn2), CONVERT(nvarchar(MAX), ExtraColumn3), CONVERT(nvarchar(MAX), ExtraColumn4), AdditionalInformation FROM dbo.Global_Errors_Log WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Global_Errors_Log OFF
GO
DROP TABLE dbo.Global_Errors_Log
GO
EXECUTE sp_rename N'dbo.Tmp_Global_Errors_Log', N'Global_Errors_Log', 'OBJECT' 
GO
ALTER TABLE dbo.Global_Errors_Log ADD CONSTRAINT
	PK_Global_Errors_Log PRIMARY KEY CLUSTERED 
	(
	uid
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
select Has_Perms_By_Name(N'dbo.Global_Errors_Log', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Global_Errors_Log', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Global_Errors_Log', 'Object', 'CONTROL') as Contr_Per 