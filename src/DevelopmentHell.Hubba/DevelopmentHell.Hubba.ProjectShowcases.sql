/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2022 (16.0.1050)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2022
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [master]
GO

alter database [DevelopmentHell.Hubba.ProjectShowcases] set single_user with rollback immediate
DROP DATABASE [DevelopmentHell.Hubba.ProjectShowcases]
GO
USE [master]
GO

/****** Object:  Database [DevelopmentHell.Hubba.ProjectShowcases]    Script Date: 5/1/2023 3:51:19 PM ******/
CREATE DATABASE [DevelopmentHell.Hubba.ProjectShowcases]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DevelopmentHell.Hubba.ProjectShowcases', FILENAME = N'/var/opt/mssql/data/DevelopmentHell.Hubba.ProjectShowcases.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DevelopmentHell.Hubba.ProjectShowcases_log', FILENAME = N'/var/opt/mssql/data/DevelopmentHell.Hubba.ProjectShowcases_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET ARITHABORT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET RECOVERY FULL 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET  MULTI_USER 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'DevelopmentHell.Hubba.ProjectShowcases', N'ON'
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET QUERY_STORE = ON
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
/****** Object:  Login [NT SERVICE\Winmgmt]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [NT SERVICE\Winmgmt] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLWriter]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [NT SERVICE\SQLWriter] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [NT SERVICE\SQLTELEMETRY] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLSERVERAGENT]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [NT SERVICE\SQLSERVERAGENT] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT Service\MSSQLSERVER]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [NT Service\MSSQLSERVER] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT AUTHORITY\SYSTEM]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [distributor_admin]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [distributor_admin] WITH PASSWORD=N'rV9aKJL7wiCh977yreG9BP9ZhXU6c+D14+jKQzX3220=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [distributor_admin] DISABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.User] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Notification]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] WITH PASSWORD=N'password', DEFAULT_DATABASE=[DevelopmentHell.Hubba.Notifications], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Logging]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.ListingProfile]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] ENABLE
GO
/****** Object:  Login [BUILTIN\Users]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [BUILTIN\Users] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [BRYANS-LAPTOP\bryan]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [BRYANS-LAPTOP\bryan] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyTsqlExecutionLogin##]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [##MS_PolicyTsqlExecutionLogin##] WITH PASSWORD=N'nXh2J6ab9HNyI4BCq1ZJDd8a6EsDEZaWpyBfS0CB75k=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [##MS_PolicyTsqlExecutionLogin##] DISABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyEventProcessingLogin##]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE LOGIN [##MS_PolicyEventProcessingLogin##] WITH PASSWORD=N'4xIbvC890QVUcBhoth01qbOTG3yQx7DlSTrPc+Akx5g=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [##MS_PolicyEventProcessingLogin##] DISABLE
GO
ALTER AUTHORIZATION ON DATABASE::[DevelopmentHell.Hubba.ProjectShowcases] TO [BRYANS-LAPTOP\bryan]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\Winmgmt]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\SQLWriter]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\SQLSERVERAGENT]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT Service\MSSQLSERVER]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [distributor_admin]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [BRYANS-LAPTOP\bryan]
GO
USE [DevelopmentHell.Hubba.ProjectShowcases]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.User] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.User]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.User]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]
GO
GRANT CONNECT TO [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] AS [dbo]
GO
GRANT CONNECT TO [DevelopmentHell.Hubba.SqlUser.User] AS [dbo]
GO
GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [public] AS [dbo]
GO
GRANT VIEW ANY COLUMN MASTER KEY DEFINITION TO [public] AS [dbo]
GO
/****** Object:  FullTextCatalog [ShowcasesCatalog]    Script Date: 5/1/2023 3:51:20 PM ******/
CREATE FULLTEXT CATALOG [ShowcasesCatalog] WITH ACCENT_SENSITIVITY = ON
AUTHORIZATION [dbo]
GO
/****** Object:  Table [dbo].[ShowcaseCommentReports]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShowcaseCommentReports](
	[CommentId] [bigint] NOT NULL,
	[ReporterId] [int] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[IsResolved] [bit] NOT NULL,
	[Reason] [nvarchar](250) NOT NULL
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[ShowcaseCommentReports] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[ShowcaseComments]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShowcaseComments](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CommenterId] [int] NOT NULL,
	[ShowcaseId] [nvarchar](100) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[Text] [nvarchar](250) NOT NULL,
	[Rating] [int] NOT NULL,
	[EditTimestamp] [datetime] NULL,
 CONSTRAINT [PK_ShowcaseComments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_ShowcaseComments] UNIQUE NONCLUSTERED 
(
	[CommenterId] ASC,
	[Timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[ShowcaseComments] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[ShowcaseCommentVotes]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShowcaseCommentVotes](
	[VoterId] [int] NOT NULL,
	[CommentId] [bigint] NOT NULL,
	[IsUpvote] [bit] NOT NULL,
 CONSTRAINT [PK_ShowcaseCommentVotes] PRIMARY KEY CLUSTERED 
(
	[VoterId] ASC,
	[CommentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[ShowcaseCommentVotes] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[ShowcaseReports]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShowcaseReports](
	[ShowcaseId] [nvarchar](100) NOT NULL,
	[ReporterId] [int] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[IsResolved] [bit] NOT NULL,
	[Reason] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_ShowcaseReports] PRIMARY KEY CLUSTERED 
(
	[ShowcaseId] ASC,
	[ReporterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[ShowcaseReports] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[Showcases]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Showcases](
	[Id] [nvarchar](100) NOT NULL,
	[ShowcaseUserId] [int] NOT NULL,
	[ListingId] [int] NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](3000) NOT NULL,
	[IsPublished] [bit] NOT NULL,
	[Rating] [float] NOT NULL,
	[PublishTimestamp] [datetime] NULL,
	[EditTimestamp] [datetime] NULL,
 CONSTRAINT [PK_Showcases] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_Showcases] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[Showcases] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[ShowcaseVotes]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShowcaseVotes](
	[UserAccountId] [int] NOT NULL,
	[ShowcaseId] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ShowcaseVotes] PRIMARY KEY CLUSTERED 
(
	[UserAccountId] ASC,
	[ShowcaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[ShowcaseVotes] TO  SCHEMA OWNER 
GO
/****** Object:  FullTextIndex     Script Date: 5/1/2023 3:51:21 PM ******/
CREATE FULLTEXT INDEX ON [dbo].[Showcases](
[Description] LANGUAGE 'English', 
[Id] LANGUAGE 'English', 
[Title] LANGUAGE 'English')
KEY INDEX [PK_Showcases]ON ([ShowcasesCatalog], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)

GO
ALTER TABLE [dbo].[Showcases] ADD  CONSTRAINT [DF_Showcases_IsPublished]  DEFAULT ((0)) FOR [IsPublished]
GO
ALTER TABLE [dbo].[Showcases] ADD  CONSTRAINT [DF_Showcases_Rating]  DEFAULT ((0)) FOR [Rating]
GO
ALTER TABLE [dbo].[ShowcaseCommentReports]  WITH CHECK ADD  CONSTRAINT [FK_ShowcaseCommentReports_ShowcaseComments] FOREIGN KEY([CommentId])
REFERENCES [dbo].[ShowcaseComments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShowcaseCommentReports] CHECK CONSTRAINT [FK_ShowcaseCommentReports_ShowcaseComments]
GO
ALTER TABLE [dbo].[ShowcaseComments]  WITH CHECK ADD  CONSTRAINT [FK_ShowcaseComments_ShowcaseComments] FOREIGN KEY([Id])
REFERENCES [dbo].[ShowcaseComments] ([Id])
GO
ALTER TABLE [dbo].[ShowcaseComments] CHECK CONSTRAINT [FK_ShowcaseComments_ShowcaseComments]
GO
ALTER TABLE [dbo].[ShowcaseComments]  WITH CHECK ADD  CONSTRAINT [FK_ShowcaseComments_Showcases] FOREIGN KEY([ShowcaseId])
REFERENCES [dbo].[Showcases] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShowcaseComments] CHECK CONSTRAINT [FK_ShowcaseComments_Showcases]
GO
ALTER TABLE [dbo].[ShowcaseCommentVotes]  WITH CHECK ADD  CONSTRAINT [FK_ShowcaseCommentVotes_ShowcaseComments] FOREIGN KEY([CommentId])
REFERENCES [dbo].[ShowcaseComments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShowcaseCommentVotes] CHECK CONSTRAINT [FK_ShowcaseCommentVotes_ShowcaseComments]
GO
ALTER TABLE [dbo].[ShowcaseReports]  WITH CHECK ADD  CONSTRAINT [FK_ShowcaseReports_Showcases] FOREIGN KEY([ShowcaseId])
REFERENCES [dbo].[Showcases] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShowcaseReports] CHECK CONSTRAINT [FK_ShowcaseReports_Showcases]
GO
ALTER TABLE [dbo].[ShowcaseVotes]  WITH CHECK ADD  CONSTRAINT [FK_ShowcaseVotes_Showcases] FOREIGN KEY([ShowcaseId])
REFERENCES [dbo].[Showcases] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShowcaseVotes] CHECK CONSTRAINT [FK_ShowcaseVotes_Showcases]
GO
/****** Object:  StoredProcedure [dbo].[CurateShowcases]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CurateShowcases] @Offset INT
AS
SELECT S.Id, S.Title, S.Rating, S.Description
FROM [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[Showcases] AS S
WHERE S.IsPublished = 1
ORDER BY S.Rating DESC
OFFSET @Offset ROWS
FETCH NEXT 5 ROWS ONLY;
GO
ALTER AUTHORIZATION ON [dbo].[CurateShowcases] TO  SCHEMA OWNER 
GO
GRANT EXECUTE ON [dbo].[CurateShowcases] TO [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] AS [dbo]
GO
/****** Object:  StoredProcedure [dbo].[SearchShowcases]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- SearchShowcases
CREATE PROCEDURE [dbo].[SearchShowcases] @Query NVARCHAR(200), @Offset INT, @FTTableRankWeight FLOAT, @RatingsRankWeight FLOAT
AS
SELECT S.Id, S.Title, S.Rating, S.Description,
    (CAST(FT.Rank AS FLOAT) / ISNULL((NULLIF(MAX(FT.Rank) OVER(), 0)), 1) * @FTTableRankWeight              -- FTTableRank
    + (CAST(ISNULL(S.Rating, 0) AS FLOAT) / ISNULL(MAX(S.Rating) OVER(), 1)) * @RatingsRankWeight) AS Score -- RatingsRank
FROM [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[Showcases] AS S
INNER JOIN FREETEXTTABLE(
    [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[Showcases],
    (Title, Description),
    @Query
) AS FT
ON S.Id = FT.[Key]
WHERE S.IsPublished = 1
ORDER BY Score DESC
OFFSET @Offset ROWS
FETCH NEXT 50 ROWS ONLY;
GO
ALTER AUTHORIZATION ON [dbo].[SearchShowcases] TO  SCHEMA OWNER 
GO
GRANT EXECUTE ON [dbo].[SearchShowcases] TO [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] AS [dbo]
GO
/****** Object:  Trigger [dbo].[tr_fk_UserAccounts_ShowcaseCommentReports]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[tr_fk_UserAccounts_ShowcaseCommentReports]
ON [dbo].[ShowcaseCommentReports]
AFTER INSERT
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [DevelopmentHell.Hubba.Users].[dbo].[UserAccounts] WHERE Id = (SELECT ReporterId FROM inserted))
    BEGIN
        RAISERROR ('Foreign key constraint violation', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
ALTER TABLE [dbo].[ShowcaseCommentReports] ENABLE TRIGGER [tr_fk_UserAccounts_ShowcaseCommentReports]
GO
/****** Object:  Trigger [dbo].[tr_fk_UserAccounts_ShowcaseComments]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[tr_fk_UserAccounts_ShowcaseComments]
ON [dbo].[ShowcaseComments]
AFTER INSERT
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [DevelopmentHell.Hubba.Users].[dbo].[UserAccounts] WHERE Id = (SELECT CommenterId FROM inserted))
    BEGIN
        RAISERROR ('Foreign key constraint violation', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
ALTER TABLE [dbo].[ShowcaseComments] ENABLE TRIGGER [tr_fk_UserAccounts_ShowcaseComments]
GO
/****** Object:  Trigger [dbo].[tr_fk_UserAccounts_ShowcaseCommentVotes]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[tr_fk_UserAccounts_ShowcaseCommentVotes]
ON [dbo].[ShowcaseCommentVotes]
AFTER INSERT
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [DevelopmentHell.Hubba.Users].[dbo].[UserAccounts] WHERE Id = (SELECT VoterId FROM inserted))
    BEGIN
        RAISERROR ('Foreign key constraint violation', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
ALTER TABLE [dbo].[ShowcaseCommentVotes] ENABLE TRIGGER [tr_fk_UserAccounts_ShowcaseCommentVotes]
GO
/****** Object:  Trigger [dbo].[tr_fk_UserAccounts_ShowcaseReports]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[tr_fk_UserAccounts_ShowcaseReports]
ON [dbo].[ShowcaseReports]
AFTER INSERT
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [DevelopmentHell.Hubba.Users].[dbo].[UserAccounts] WHERE Id = (SELECT ReporterId FROM inserted))
    BEGIN
        RAISERROR ('Foreign key constraint violation', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
ALTER TABLE [dbo].[ShowcaseReports] ENABLE TRIGGER [tr_fk_UserAccounts_ShowcaseReports]
GO
/****** Object:  Trigger [dbo].[tr_fk_UserAccounts_Showcases]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[tr_fk_UserAccounts_Showcases]
ON [dbo].[Showcases]
AFTER INSERT
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [DevelopmentHell.Hubba.Users].[dbo].[UserAccounts] WHERE Id = (SELECT ShowcaseUserId FROM inserted))
    BEGIN
        RAISERROR ('Foreign key constraint violation', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
ALTER TABLE [dbo].[Showcases] ENABLE TRIGGER [tr_fk_UserAccounts_Showcases]
GO
/****** Object:  Trigger [dbo].[tr_fk_UserAccounts_ShowcaseVotes]    Script Date: 5/1/2023 3:51:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[tr_fk_UserAccounts_ShowcaseVotes]
ON [dbo].[ShowcaseVotes]
AFTER INSERT
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [DevelopmentHell.Hubba.Users].[dbo].[UserAccounts] WHERE Id = (SELECT UserAccountId FROM inserted))
    BEGIN
        RAISERROR ('Foreign key constraint violation', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
ALTER TABLE [dbo].[ShowcaseVotes] ENABLE TRIGGER [tr_fk_UserAccounts_ShowcaseVotes]
GO
USE [master]
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET  READ_WRITE 
GO

