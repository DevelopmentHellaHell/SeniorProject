USE [master]
GO

alter database [DevelopmentHell.Hubba.ListingProfiles] set single_user with rollback immediate
DROP DATABASE [DevelopmentHell.Hubba.ListingProfiles]
GO
USE [master]
GO

/****** Object:  Database [DevelopmentHell.Hubba.ListingProfiles]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE DATABASE [DevelopmentHell.Hubba.ListingProfiles]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Test', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.ListingProfiles.mdf.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Test_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.ListingProfiles_log.ldf' , SIZE = 139264KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DevelopmentHell.Hubba.ListingProfiles].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET ARITHABORT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET RECOVERY FULL 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET  MULTI_USER 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'DevelopmentHell.Hubba.ListingProfiles', N'ON'
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET QUERY_STORE = ON
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
/****** Object:  Login [NT SERVICE\Winmgmt]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [NT SERVICE\Winmgmt] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLWriter]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [NT SERVICE\SQLWriter] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [NT SERVICE\SQLTELEMETRY] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLSERVERAGENT]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [NT SERVICE\SQLSERVERAGENT] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT Service\MSSQLSERVER]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [NT Service\MSSQLSERVER] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT AUTHORITY\SYSTEM]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.User] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Notification]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] WITH PASSWORD=N'password', DEFAULT_DATABASE=[DevelopmentHell.Hubba.Notifications], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Logging]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.ListingProfile]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] ENABLE
GO
/****** Object:  Login [BUILTIN\Users]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [BUILTIN\Users] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [BRYANS-LAPTOP\bryan]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [BRYANS-LAPTOP\bryan] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyTsqlExecutionLogin##]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [##MS_PolicyTsqlExecutionLogin##] WITH PASSWORD=N'oEVGbv6jc0BTcfffVoHqfrUMl6ZJXXLhJdWBFLX8dFQ=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [##MS_PolicyTsqlExecutionLogin##] DISABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyEventProcessingLogin##]    Script Date: 4/18/2023 9:25:18 PM ******/
CREATE LOGIN [##MS_PolicyEventProcessingLogin##] WITH PASSWORD=N'byDPosud09IPIONEO4G2nwjpFputRmXK6r6IEG9eDAg=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [##MS_PolicyEventProcessingLogin##] DISABLE
GO
ALTER AUTHORIZATION ON DATABASE::[DevelopmentHell.Hubba.ListingProfiles] TO [BRYANS-LAPTOP\bryan]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\Winmgmt]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\SQLWriter]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\SQLSERVERAGENT]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT Service\MSSQLSERVER]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [BRYANS-LAPTOP\bryan]
GO
USE [DevelopmentHell.Hubba.ListingProfiles]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.ListingProfile]    Script Date: 4/18/2023 9:25:19 PM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.ListingProfile] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.ListingProfile]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.ListingProfile]
GO
GRANT CONNECT TO [DevelopmentHell.Hubba.SqlUser.ListingProfile] AS [dbo]
GO
GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [public] AS [dbo]
GO
GRANT VIEW ANY COLUMN MASTER KEY DEFINITION TO [public] AS [dbo]
GO
/****** Object:  FullTextCatalog [ListingsCatalog]    Script Date: 4/18/2023 9:25:19 PM ******/
CREATE FULLTEXT CATALOG [ListingsCatalog] WITH ACCENT_SENSITIVITY = OFF
AUTHORIZATION [dbo]
GO
/****** Object:  Table [dbo].[ListingAvailabilities]    Script Date: 4/18/2023 9:25:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ListingAvailabilities](
	[ListingId] [int] NOT NULL,
	[AvailabilityId] [int] IDENTITY(1,1) NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ListingAvailabilities] PRIMARY KEY CLUSTERED 
(
	[ListingId] ASC,
	[AvailabilityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[ListingAvailabilities] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[ListingHistory]    Script Date: 4/18/2023 9:25:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ListingHistory](
	[ListingId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_ListingHistory] PRIMARY KEY CLUSTERED 
(
	[ListingId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[ListingHistory] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[ListingRatings]    Script Date: 4/18/2023 9:25:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ListingRatings](
	[ListingId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Rating] [int] NOT NULL,
	[Comment] [nvarchar](250) NULL,
	[Anonymous] [bit] NOT NULL,
	[LastEdited] [datetime] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ListingRatings] PRIMARY KEY CLUSTERED 
(
	[ListingId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[ListingRatings] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[Listings]    Script Date: 4/18/2023 9:25:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Listings](
	[ListingId] [int] IDENTITY(1,1) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[Price] [money] NULL,
	[Location] [nvarchar](200) NULL,
	[LastEdited] [datetime] NOT NULL,
	[Published] [bit] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Listings] PRIMARY KEY CLUSTERED 
(
	[ListingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[Listings] TO  SCHEMA OWNER 
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK01]    Script Date: 4/18/2023 9:25:19 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UK01] ON [dbo].[Listings]
(
	[Title] ASC,
	[OwnerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  FullTextIndex     Script Date: 4/18/2023 9:25:19 PM ******/
CREATE FULLTEXT INDEX ON [dbo].[Listings](
[Description] LANGUAGE 'English', 
[Location] LANGUAGE 'English', 
[Title] LANGUAGE 'English')
KEY INDEX [PK_Listings]ON ([ListingsCatalog], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)

GO
ALTER TABLE [dbo].[ListingAvailabilities]  WITH CHECK ADD  CONSTRAINT [FK_ListingAvailabilities_Listings] FOREIGN KEY([ListingId])
REFERENCES [dbo].[Listings] ([ListingId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ListingAvailabilities] CHECK CONSTRAINT [FK_ListingAvailabilities_Listings]
GO
ALTER TABLE [dbo].[ListingHistory]  WITH CHECK ADD  CONSTRAINT [FK_ListingHistory_Listings] FOREIGN KEY([ListingId])
REFERENCES [dbo].[Listings] ([ListingId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ListingHistory] CHECK CONSTRAINT [FK_ListingHistory_Listings]
GO
ALTER TABLE [dbo].[ListingRatings]  WITH CHECK ADD  CONSTRAINT [FK_ListingRatings_ListingHistory] FOREIGN KEY([ListingId], [UserId])
REFERENCES [dbo].[ListingHistory] ([ListingId], [UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ListingRatings] CHECK CONSTRAINT [FK_ListingRatings_ListingHistory]
GO
/****** Object:  StoredProcedure [dbo].[CurateListings]    Script Date: 4/18/2023 9:25:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CurateListings] @Offset INT
AS
SELECT L.ListingId, L.Title, L.Location, L.Price, R.AvgRatings, R.TotalRatings,
    (((ISNULL(R.AvgRatings, 0) / 5) * 0.5)                                                                 -- RatingsRank
    + (CAST(ISNULL(R.TotalRatings, 0) AS FLOAT) / ISNULL(MAX(R.TotalRatings) OVER(), 1)) * 0.5) AS Score -- RatingsCountRank
FROM [DevelopmentHell.Hubba.ListingProfiles].[dbo].[Listings] AS L
LEFT JOIN (
    SELECT ListingId, AVG(CAST(Rating AS FLOAT)) as AvgRatings, COUNT(Rating) as TotalRatings
    FROM [DevelopmentHell.Hubba.ListingProfiles].[dbo].[ListingRatings]
    GROUP BY ListingId
) as R
ON L.ListingId = R.ListingId
WHERE L.Published = 1
ORDER BY Score DESC
OFFSET @Offset ROWS
FETCH NEXT 50 ROWS ONLY;
GO
ALTER AUTHORIZATION ON [dbo].[CurateListings] TO  SCHEMA OWNER 
GO
GRANT EXECUTE ON [dbo].[CurateListings] TO [DevelopmentHell.Hubba.SqlUser.ListingProfile] AS [dbo]
GO
/****** Object:  StoredProcedure [dbo].[SearchListings]    Script Date: 4/18/2023 9:25:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- SearchListings
CREATE PROCEDURE [dbo].[SearchListings] @Query NVARCHAR(200), @Offset INT, @FTTableRankWeight FLOAT, @RatingsRankWeight FLOAT, @RatingsCountRankWeight FLOAT
AS
SELECT L.ListingId, L.Title, L.Location, L.Price, R.AvgRatings, R.TotalRatings,
    (CAST(FT.Rank AS FLOAT) / ISNULL((NULLIF(MAX(FT.Rank) OVER(), 0)), 1) * @FTTableRankWeight                               -- FTTableRank
    + ((ISNULL(R.AvgRatings, 0) / 5) * @RatingsRankWeight)                                                                   -- RatingsRank
    + (CAST(ISNULL(R.TotalRatings, 0) AS FLOAT) / ISNULL(MAX(R.TotalRatings) OVER(), 1)) * @RatingsCountRankWeight) AS Score -- RatingsCountRank
FROM [DevelopmentHell.Hubba.ListingProfiles].[dbo].[Listings] AS L
INNER JOIN FREETEXTTABLE(
    [DevelopmentHell.Hubba.ListingProfiles].[dbo].[Listings],
    (Title, Description, Location),
    @Query
) AS FT
ON L.ListingId = FT.[Key]
LEFT JOIN (
    SELECT ListingId, AVG(CAST(Rating AS FLOAT)) as AvgRatings, COUNT(Rating) as TotalRatings
    FROM [DevelopmentHell.Hubba.ListingProfiles].[dbo].[ListingRatings]
    GROUP BY ListingId
) as R
ON L.ListingId = R.ListingId
WHERE L.Published = 1
ORDER BY Score DESC
OFFSET @Offset ROWS
FETCH NEXT 50 ROWS ONLY;
GO
ALTER AUTHORIZATION ON [dbo].[SearchListings] TO  SCHEMA OWNER 
GO
GRANT EXECUTE ON [dbo].[SearchListings] TO [DevelopmentHell.Hubba.SqlUser.ListingProfile] AS [dbo]
GO
USE [master]
GO
ALTER DATABASE [DevelopmentHell.Hubba.ListingProfiles] SET  READ_WRITE 
GO

