USE [master]
GO

alter database [DevelopmentHell.Hubba.CollaboratorProfiles] set single_user with rollback immediate
DROP DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles]
GO
USE [master]
GO

/****** Object:  Database [DevelopmentHell.Hubba.CollaboratorProfiles]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DevelopmentHell.Hubba.CollaboratorProfiles', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.CollaboratorProfiles.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DevelopmentHell.Hubba.CollaboratorProfiles_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.CollaboratorProfiles_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DevelopmentHell.Hubba.CollaboratorProfiles].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET ARITHABORT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET  MULTI_USER 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'DevelopmentHell.Hubba.CollaboratorProfiles', N'ON'
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET QUERY_STORE = ON
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
/****** Object:  Login [NT SERVICE\Winmgmt]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [NT SERVICE\Winmgmt] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLWriter]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [NT SERVICE\SQLWriter] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY$SQLEXPRESS]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [NT SERVICE\SQLTELEMETRY$SQLEXPRESS] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT Service\MSSQL$SQLEXPRESS]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [NT Service\MSSQL$SQLEXPRESS] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT AUTHORITY\SYSTEM]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.User] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Notification]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] WITH PASSWORD=N'password', DEFAULT_DATABASE=[DevelopmentHell.Hubba.Notifications], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Logging]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.ListingProfile]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] ENABLE
GO
/****** Object:  Login [DESKTOP-NZXT\NZXT ASRock]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [DESKTOP-NZXT\NZXT ASRock] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [BUILTIN\Users]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [BUILTIN\Users] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyTsqlExecutionLogin##]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [##MS_PolicyTsqlExecutionLogin##] WITH PASSWORD=N'D79cIU/IqdMmR3J/+S25Gmsasj7gyPpcxL/my8QVME4=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [##MS_PolicyTsqlExecutionLogin##] DISABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyEventProcessingLogin##]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE LOGIN [##MS_PolicyEventProcessingLogin##] WITH PASSWORD=N'YT+0csUNuZyuD5ZKdaLEbkcfgKeLInGRlhZoJfouz48=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [##MS_PolicyEventProcessingLogin##] DISABLE
GO
ALTER AUTHORIZATION ON DATABASE::[DevelopmentHell.Hubba.CollaboratorProfiles] TO [DESKTOP-NZXT\NZXT ASRock]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\Winmgmt]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\SQLWriter]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT Service\MSSQL$SQLEXPRESS]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [DESKTOP-NZXT\NZXT ASRock]
GO
USE [DevelopmentHell.Hubba.CollaboratorProfiles]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile]
GO
GRANT CONNECT TO [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] AS [dbo]
GO
GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [public] AS [dbo]
GO
GRANT VIEW ANY COLUMN MASTER KEY DEFINITION TO [public] AS [dbo]
GO
/****** Object:  FullTextCatalog [CollaboratorsCatalog]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE FULLTEXT CATALOG [CollaboratorsCatalog] WITH ACCENT_SENSITIVITY = ON
AUTHORIZATION [dbo]
GO
/****** Object:  Table [dbo].[CollaboratorFiles]    Script Date: 4/24/2023 9:13:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CollaboratorFiles](
	[CollaboratorId] [int] NOT NULL,
	[FileId] [int] NOT NULL,
 CONSTRAINT [PK_CollaboratorFiles] PRIMARY KEY CLUSTERED 
(
	[CollaboratorId] ASC,
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[CollaboratorFiles] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[Collaborators]    Script Date: 4/24/2023 9:13:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Collaborators](
	[CollaboratorId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](70) NOT NULL,
	[ProfilePicture] [int] NULL,
	[ContactInfo] [nvarchar](1000) NOT NULL,
	[Tags] [nvarchar](2000) NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Availability] [nvarchar](1000) NULL,
	[OwnerId] [int] NOT NULL,
	[LastModifiedUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NULL,
	[Published] [bit] NOT NULL,
 CONSTRAINT [PK_Collaborators] PRIMARY KEY CLUSTERED 
(
	[CollaboratorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[Collaborators] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[Files]    Script Date: 4/24/2023 9:13:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Files](
	[FileId] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[FileType] [nvarchar](50) NOT NULL,
	[FileSize] [bigint] NOT NULL,
	[FileUrl] [nvarchar](100) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[LastModifiedUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [IX_Files] UNIQUE NONCLUSTERED 
(
	[FileUrl] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[Files] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[UserVotes]    Script Date: 4/24/2023 9:13:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserVotes](
	[CollaboratorId] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
 CONSTRAINT [PK_UserVotes] PRIMARY KEY CLUSTERED 
(
	[CollaboratorId] ASC,
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[UserVotes] TO  SCHEMA OWNER 
GO
/****** Object:  Index [IX_Collaborators]    Script Date: 4/24/2023 9:13:46 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Collaborators] ON [dbo].[Collaborators]
(
	[OwnerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  FullTextIndex     Script Date: 4/24/2023 9:13:46 AM ******/
CREATE FULLTEXT INDEX ON [dbo].[Collaborators](
[Availability] LANGUAGE 'English', 
[ContactInfo] LANGUAGE 'English', 
[Description] LANGUAGE 'English', 
[Name] LANGUAGE 'English', 
[Tags] LANGUAGE 'English')
KEY INDEX [PK_Collaborators]ON ([CollaboratorsCatalog], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)

GO
ALTER TABLE [dbo].[CollaboratorFiles]  WITH CHECK ADD  CONSTRAINT [FK_CollaboratorFiles_Collaborators] FOREIGN KEY([CollaboratorId])
REFERENCES [dbo].[Collaborators] ([CollaboratorId])
GO
ALTER TABLE [dbo].[CollaboratorFiles] CHECK CONSTRAINT [FK_CollaboratorFiles_Collaborators]
GO
ALTER TABLE [dbo].[CollaboratorFiles]  WITH CHECK ADD  CONSTRAINT [FK_CollaboratorFiles_Files] FOREIGN KEY([FileId])
REFERENCES [dbo].[Files] ([FileId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CollaboratorFiles] CHECK CONSTRAINT [FK_CollaboratorFiles_Files]
GO
ALTER TABLE [dbo].[Collaborators]  WITH CHECK ADD  CONSTRAINT [FK_Collaborators_Collaborators] FOREIGN KEY([CollaboratorId])
REFERENCES [dbo].[Collaborators] ([CollaboratorId])
GO
ALTER TABLE [dbo].[Collaborators] CHECK CONSTRAINT [FK_Collaborators_Collaborators]
GO
ALTER TABLE [dbo].[Files]  WITH CHECK ADD  CONSTRAINT [FK_Files_Collaborators] FOREIGN KEY([OwnerId])
REFERENCES [dbo].[Collaborators] ([OwnerId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Files] CHECK CONSTRAINT [FK_Files_Collaborators]
GO
ALTER TABLE [dbo].[Files]  WITH CHECK ADD  CONSTRAINT [FK_Files_Files] FOREIGN KEY([FileId])
REFERENCES [dbo].[Files] ([FileId])
GO
ALTER TABLE [dbo].[Files] CHECK CONSTRAINT [FK_Files_Files]
GO
ALTER TABLE [dbo].[UserVotes]  WITH CHECK ADD  CONSTRAINT [FK_UserVotes_Collaborators] FOREIGN KEY([CollaboratorId])
REFERENCES [dbo].[Collaborators] ([CollaboratorId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserVotes] CHECK CONSTRAINT [FK_UserVotes_Collaborators]
GO
/****** Object:  StoredProcedure [dbo].[CurateCollaborators]    Script Date: 4/24/2023 9:13:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- CurateCollaborators
CREATE PROCEDURE [dbo].[CurateCollaborators] @Offset INT
AS
SELECT C.CollaboratorId, C.Name, ISNULL(V.TotalVotes, 0) AS TotalVotes
FROM [DevelopmentHell.Hubba.CollaboratorProfiles].[dbo].[Collaborators] AS C
LEFT JOIN (
    SELECT CollaboratorId, COUNT(AccountId) as TotalVotes
    FROM [DevelopmentHell.Hubba.CollaboratorProfiles].[dbo].[UserVotes]
    GROUP BY CollaboratorId
) as V
ON C.CollaboratorId = V.CollaboratorId
WHERE C.Published = 1
ORDER BY V.TotalVotes DESC
OFFSET @Offset ROWS
FETCH NEXT 50 ROWS ONLY;
GO
ALTER AUTHORIZATION ON [dbo].[CurateCollaborators] TO  SCHEMA OWNER 
GO
GRANT EXECUTE ON [dbo].[CurateCollaborators] TO [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] AS [dbo]
GO
/****** Object:  StoredProcedure [dbo].[SearchCollaborators]    Script Date: 4/24/2023 9:13:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- SearchCollaborators
CREATE PROCEDURE [dbo].[SearchCollaborators] @Query NVARCHAR(200), @Offset INT, @FTTableRankWeight FLOAT, @VotesCountRankWeight FLOAT
AS
SELECT C.CollaboratorId, C.Name, ISNULL(V.TotalVotes, 0) as TotalVotes,
    (CAST(FT.Rank AS FLOAT) / ISNULL((NULLIF(MAX(FT.Rank) OVER(), 0)), 1) * @FTTableRankWeight                          -- FTTableRank
    + (CAST(ISNULL(V.TotalVotes, 0) AS FLOAT) / ISNULL(MAX(V.TotalVotes) OVER(), 1)) * @VotesCountRankWeight) AS Score -- VotesCountRank
FROM [DevelopmentHell.Hubba.CollaboratorProfiles].[dbo].[Collaborators] AS C
INNER JOIN FREETEXTTABLE(
    [DevelopmentHell.Hubba.CollaboratorProfiles].[dbo].[Collaborators],
    (Name, ContactInfo, Tags, Description, Availability),
    @Query
) AS FT
ON C.CollaboratorId = FT.[Key]
LEFT JOIN (
    SELECT CollaboratorId, COUNT(AccountId) as TotalVotes
    FROM [DevelopmentHell.Hubba.CollaboratorProfiles].[dbo].[UserVotes]
    GROUP BY CollaboratorId
) as V
ON C.CollaboratorId = V.CollaboratorId
WHERE C.Published = 1
ORDER BY Score DESC
OFFSET @Offset ROWS
FETCH NEXT 50 ROWS ONLY;
GO
ALTER AUTHORIZATION ON [dbo].[SearchCollaborators] TO  SCHEMA OWNER 
GO
GRANT EXECUTE ON [dbo].[SearchCollaborators] TO [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] AS [dbo]
GO
/****** Object:  Trigger [dbo].[trg_Collaborators]    Script Date: 4/24/2023 9:13:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[trg_Collaborators]
ON [dbo].[Collaborators]
AFTER INSERT, UPDATE
AS
BEGIN
  IF EXISTS (
    SELECT 1
    FROM inserted AS i
    LEFT JOIN [DevelopmentHell.Hubba.Users].dbo.UserAccounts AS ua
      ON i.OwnerId = ua.Id
    LEFT JOIN [DevelopmentHell.Hubba.Users].dbo.UserAccounts AS ua2
      ON i.LastModifiedUser = ua2.Id
    WHERE ua.Id IS NULL OR ua2.Id IS NULL
  )
  BEGIN
    RAISERROR('Referenced record does not exist in target database', 16, 1)
    ROLLBACK TRANSACTION
    RETURN
  END
END
GO
ALTER TABLE [dbo].[Collaborators] ENABLE TRIGGER [trg_Collaborators]
GO
/****** Object:  Trigger [dbo].[trg_UserVotes]    Script Date: 4/24/2023 9:13:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[trg_UserVotes]
ON [dbo].[UserVotes]
AFTER INSERT, UPDATE
AS
BEGIN
  IF EXISTS (
    SELECT 1
	FROM inserted AS i
    LEFT JOIN [DevelopmentHell.Hubba.Users].dbo.UserAccounts AS ua
      ON i.AccountId = ua.Id
    WHERE ua.Id IS NULL
  )
  BEGIN
    RAISERROR('Referenced record does not exist in target database', 16, 1)
    ROLLBACK TRANSACTION
    RETURN
  END
END
GO
ALTER TABLE [dbo].[UserVotes] ENABLE TRIGGER [trg_UserVotes]
GO
USE [master]
GO
ALTER DATABASE [DevelopmentHell.Hubba.CollaboratorProfiles] SET  READ_WRITE 
GO

