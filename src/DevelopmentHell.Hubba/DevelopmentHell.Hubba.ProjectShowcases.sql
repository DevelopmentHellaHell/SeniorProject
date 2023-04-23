USE [master]
GO

alter database [DevelopmentHell.Hubba.ProjectShowcases] set single_user with rollback immediate
DROP DATABASE [DevelopmentHell.Hubba.ProjectShowcases]
GO

USE [master]
GO
CREATE DATABASE [DevelopmentHell.Hubba.ProjectShowcases]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DevelopmentHell.Hubba.ProjectShowcases', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.ProjectShowcases.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DevelopmentHell.Hubba.ProjectShowcases_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.ProjectShowcases_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 COLLATE SQL_Latin1_General_CP1_CI_AS
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
CREATE LOGIN [NT SERVICE\Winmgmt] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
CREATE LOGIN [NT SERVICE\SQLWriter] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
CREATE LOGIN [NT SERVICE\SQLTELEMETRY$SQLEXPRESS] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
CREATE LOGIN [NT SERVICE\SQLTELEMETRY] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
CREATE LOGIN [NT SERVICE\SQLSERVERAGENT] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
CREATE LOGIN [NT Service\MSSQLSERVER] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
CREATE LOGIN [NT Service\MSSQL$SQLEXPRESS] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
CREATE LOGIN [GMAIN\tsuma] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
CREATE LOGIN [distributor_admin] WITH PASSWORD=N'ivSEtnTjBR4wc5nKNpVKv2VK68A/nPv7+IqIoo3POYU=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
/* For security reasons the login is created disabled and with a random password. */
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH PASSWORD=N'DghDlOCavr/USZOiWuWNcTWe7GPk8laQdJ0CXR+0T5U=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
/* For security reasons the login is created disabled and with a random password. */
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] WITH PASSWORD=N'xXis+dQ/pPePs0y+iwfZPK5++sB4eLnjfD/I6dqoaps=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
/* For security reasons the login is created disabled and with a random password. */
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] WITH PASSWORD=N'dhF3BP2vEKdnkaxi6ZBVVvfHvb0HbhXNGe1oDxAD6Ss=', DEFAULT_DATABASE=[DevelopmentHell.Hubba.Notifications], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
/* For security reasons the login is created disabled and with a random password. */
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] WITH PASSWORD=N'M6g+dGncCSp9/3IbhrkzojzgYH92NPf0pFjjpbBVqgY=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
/* For security reasons the login is created disabled and with a random password. */
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] WITH PASSWORD=N'hjvNj9LhpL7PzyyHZxj2MN6WXqf7pFIgTCw9EgEure4=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
CREATE LOGIN [BUILTIN\Users] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
CREATE LOGIN [##MS_PolicyTsqlExecutionLogin##] WITH PASSWORD=N'q7s0+K3YRt9rQqfIciAoET+SahlsHALyvVh8eXxOBC8=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
/* For security reasons the login is created disabled and with a random password. */
CREATE LOGIN [##MS_PolicyEventProcessingLogin##] WITH PASSWORD=N'AkxT7y8UVCLGN/VHg88eScuaFSUr4ckzzdKMhxUnyL8=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER AUTHORIZATION ON DATABASE::[DevelopmentHell.Hubba.ProjectShowcases] TO [GMAIN\tsuma]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\Winmgmt]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\SQLWriter]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\SQLSERVERAGENT]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT Service\MSSQLSERVER]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT Service\MSSQL$SQLEXPRESS]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [GMAIN\tsuma]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [distributor_admin]
GO
USE [DevelopmentHell.Hubba.ProjectShowcases]
GO
CREATE USER [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]
GO
GRANT CONNECT TO [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] AS [dbo]
GO
GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [public] AS [dbo]
GO
GRANT VIEW ANY COLUMN MASTER KEY DEFINITION TO [public] AS [dbo]
GO
CREATE FULLTEXT CATALOG [ShowcasesCatalog] WITH ACCENT_SENSITIVITY = ON
AUTHORIZATION [dbo]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShowcaseCommentReports](
	[CommentId] [bigint] NOT NULL,
	[ReporterId] [int] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[IsResolved] [bit] NOT NULL,
	[Reason] [nvarchar](250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[ShowcaseCommentReports] TO  SCHEMA OWNER 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShowcaseComments](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CommenterId] [int] NOT NULL,
	[ShowcaseId] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[Text] [nvarchar](250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Rating] [float] NOT NULL,
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
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShowcaseReports](
	[ShowcaseId] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ReporterId] [int] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[IsResolved] [bit] NOT NULL,
	[Reason] [nvarchar](250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ShowcaseReports] PRIMARY KEY CLUSTERED 
(
	[ShowcaseId] ASC,
	[ReporterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[ShowcaseReports] TO  SCHEMA OWNER 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Showcases](
	[Id] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ShowcaseUserId] [int] NOT NULL,
	[ListingId] [int] NULL,
	[Title] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [nvarchar](3000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[IsPublished] [bit] NOT NULL,
	[Rating] [float] NOT NULL,
	[PublishTimestamp] [datetime] NULL,
	[EditTimestamp] [datetime] NULL,
 CONSTRAINT [PK_Showcases] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[Showcases] TO  SCHEMA OWNER 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShowcaseVotes](
	[UserAccountId] [int] NOT NULL,
	[ShowcaseId] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ShowcaseVotes] PRIMARY KEY CLUSTERED 
(
	[UserAccountId] ASC,
	[ShowcaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[ShowcaseVotes] TO  SCHEMA OWNER 
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
FETCH NEXT 50 ROWS ONLY;
GO
ALTER AUTHORIZATION ON [dbo].[CurateShowcases] TO  SCHEMA OWNER 
GO
GRANT EXECUTE ON [dbo].[CurateShowcases] TO [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] AS [dbo]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- SearchShowcases
CREATE PROCEDURE [dbo].[SearchShowcases] @Query NVARCHAR(200), @Offset INT, @FTTableRankWeight FLOAT, @RatingsRankWeight FLOAT
AS
SELECT S.Id, S.Title, S.Rating
    (CAST(FT.Rank AS FLOAT) / ISNULL((NULLIF(MAX(FT.Rank) OVER(), 0)), 1) * @FTTableRankWeight             -- FTTableRank
    + (CAST(ISNULL(Ratings, 0) AS FLOAT) / ISNULL(MAX(Ratings) OVER(), 1)) * @RatingsRankWeight) AS Score -- RatingsRank
FROM [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[Showcases] AS S
INNER JOIN FREETEXTTABLE(
    [DevelopmentHell.Hubba.ShowcaseProfiles].[dbo].[Showcases],
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
USE [master]
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET  READ_WRITE 
GO
