/*
** Indexes
*/

-- {objectQualifier}Registry

if exists(select 1 from dbo.sysindexes where name=N'IX_Name' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Registry]'))
	drop index [{databaseOwner}].[{objectQualifier}Registry].[IX_Name]
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Registry_Name' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Registry]'))
	CREATE  INDEX [IX_{objectQualifier}Registry_Name] ON [{databaseOwner}].[{objectQualifier}Registry]([BoardID],[Name])
go

-- {objectQualifier}PollVote

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}PollVote_RemoteIP' and id=object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]'))
 CREATE  INDEX [IX_{objectQualifier}PollVote_RemoteIP] ON [{databaseOwner}].[{objectQualifier}PollVote]([RemoteIP])
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}PollVote_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]'))
 CREATE  INDEX [IX_{objectQualifier}PollVote_UserID] ON [{databaseOwner}].[{objectQualifier}PollVote]([UserID])
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}PollVote_PollID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]'))
 CREATE  INDEX [IX_{objectQualifier}PollVote_PollID] ON [{databaseOwner}].[{objectQualifier}PollVote]([PollID])
go

-- {objectQualifier}UserGroup

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}UserGroup_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}UserGroup]'))
 CREATE  INDEX [IX_{objectQualifier}UserGroup_UserID] ON [{databaseOwner}].[{objectQualifier}UserGroup]([UserID])
go

-- {objectQualifier}Message

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Message_TopicID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Message]'))
 CREATE  INDEX [IX_{objectQualifier}Message_TopicID] ON [{databaseOwner}].[{objectQualifier}Message]([TopicID])
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Message_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Message]'))
 CREATE  INDEX [IX_{objectQualifier}Message_UserID] ON [{databaseOwner}].[{objectQualifier}Message]([UserID])
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Message_Flags' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Message]'))
 CREATE  INDEX [IX_{objectQualifier}Message_Flags] ON [{databaseOwner}].[{objectQualifier}Message]([Flags])
go

-- {objectQualifier}Topic

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Topic_ForumID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Topic]'))
 CREATE  INDEX [IX_{objectQualifier}Topic_ForumID] ON [{databaseOwner}].[{objectQualifier}Topic]([ForumID])
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Topic_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Topic]'))
 CREATE  INDEX [IX_{objectQualifier}Topic_UserID] ON [{databaseOwner}].[{objectQualifier}Topic]([UserID])
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Topic_Flags' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Topic]'))
 CREATE  INDEX [IX_{objectQualifier}Topic_Flags] ON [{databaseOwner}].[{objectQualifier}Topic]([Flags])
go

-- {objectQualifier}Forum

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Forum_CategoryID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Forum]'))
 CREATE  INDEX [IX_{objectQualifier}Forum_CategoryID] ON [{databaseOwner}].[{objectQualifier}Forum]([CategoryID])
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Forum_ParentID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Forum]'))
 CREATE  INDEX [IX_{objectQualifier}Forum_ParentID] ON [{databaseOwner}].[{objectQualifier}Forum]([ParentID])
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Forum_Flags' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Forum]'))
 CREATE  INDEX [IX_{objectQualifier}Forum_Flags] ON [{databaseOwner}].[{objectQualifier}Forum]([Flags])
go

-- {objectQualifier}User

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}User_Flags' and id=object_id(N'[{databaseOwner}].[{objectQualifier}User]'))
 CREATE  INDEX [IX_{objectQualifier}User_Flags] ON [{databaseOwner}].[{objectQualifier}User]([Flags])
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}User_ProviderUserKey' and id=object_id(N'[{databaseOwner}].[{objectQualifier}User]'))
 CREATE  INDEX [IX_{objectQualifier}User_ProviderUserKey] ON [{databaseOwner}].[{objectQualifier}User]([ProviderUserKey])
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}User_Name' and id=object_id(N'[{databaseOwner}].[{objectQualifier}User]'))
 CREATE  INDEX [IX_{objectQualifier}User_Name] ON [{databaseOwner}].[{objectQualifier}User]([Name])
go

-- {objectQualifier}ForumAccess

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}ForumAccess_ForumID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}ForumAccess]'))
 CREATE  INDEX [IX_{objectQualifier}ForumAccess_ForumID] ON [{databaseOwner}].[{objectQualifier}ForumAccess]([ForumID])
go

-- {objectQualifier}UserPMessage

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}UserPMessage_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}UserPMessage]'))
 CREATE  INDEX [IX_{objectQualifier}UserPMessage_UserID] ON [{databaseOwner}].[{objectQualifier}UserPMessage]([UserID])
go

-- {objectQualifier}Category

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Category_BoardID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Category]'))
 CREATE  INDEX [IX_{objectQualifier}Category_BoardID] ON [{databaseOwner}].[{objectQualifier}Category]([BoardID])
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Category_Name' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Category]'))
 CREATE  INDEX [IX_{objectQualifier}Category_Name] ON [{databaseOwner}].[{objectQualifier}Category]([Name])
go