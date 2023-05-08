USE [master]
GO

alter database [DevelopmentHell.Hubba.ListingProfiles] set single_user with rollback immediate
DROP DATABASE [DevelopmentHell.Hubba.ListingProfiles]
GO
USE [master]
GO

/****** Object:  Database [DevelopmentHell.Hubba.ListingProfiles]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE DATABASE [DevelopmentHell.Hubba.ListingProfiles]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Test', FILENAME = N'/var/opt/mssql/data/DevelopmentHell.Hubba.ListingProfiles.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Test_log', FILENAME = N'/var/opt/mssql/data/DevelopmentHell.Hubba.ListingProfiles_log.ldf' , SIZE = 139264KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
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
/****** Object:  Login [NT SERVICE\Winmgmt]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [NT SERVICE\Winmgmt] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLWriter]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [NT SERVICE\SQLWriter] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [NT SERVICE\SQLTELEMETRY] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLSERVERAGENT]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [NT SERVICE\SQLSERVERAGENT] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT Service\MSSQLSERVER]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [NT Service\MSSQLSERVER] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT AUTHORITY\SYSTEM]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [distributor_admin]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [distributor_admin] WITH PASSWORD=N'7daU2C31CN6OG4VaRgZYDUILl0okdPJQUe6JjBZELCs=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [distributor_admin] DISABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.User] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Scheduling]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Scheduling] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Scheduling] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Notification]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] WITH PASSWORD=N'password', DEFAULT_DATABASE=[DevelopmentHell.Hubba.Notifications], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Logging]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.ListingProfile]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] ENABLE
GO
/****** Object:  Login [BUILTIN\Users]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [BUILTIN\Users] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [BRYANS-DESKTOP\bryan]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [BRYANS-DESKTOP\bryan] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyTsqlExecutionLogin##]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [##MS_PolicyTsqlExecutionLogin##] WITH PASSWORD=N'RHOnVuE93vbnP9ZWMK6rjkbYMFR38zmu03zLnXJg2nk=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [##MS_PolicyTsqlExecutionLogin##] DISABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyEventProcessingLogin##]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE LOGIN [##MS_PolicyEventProcessingLogin##] WITH PASSWORD=N'aHTPmNUM3Ziejr7BEDRFTJBjMmNL+WP0fJZgMoLo+zI=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [##MS_PolicyEventProcessingLogin##] DISABLE
GO
ALTER AUTHORIZATION ON DATABASE::[DevelopmentHell.Hubba.ListingProfiles] TO [BRYANS-DESKTOP\bryan]
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
ALTER SERVER ROLE [sysadmin] ADD MEMBER [BRYANS-DESKTOP\bryan]
GO
USE [DevelopmentHell.Hubba.ListingProfiles]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.Scheduling]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.Scheduling] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.Scheduling] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.ListingProfile]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.ListingProfile] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.Scheduling]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.Scheduling]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.ListingProfile]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.ListingProfile]
GO
GRANT CONNECT TO [DevelopmentHell.Hubba.SqlUser.ListingProfile] AS [dbo]
GO
GRANT CONNECT TO [DevelopmentHell.Hubba.SqlUser.Scheduling] AS [dbo]
GO
GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [public] AS [dbo]
GO
GRANT VIEW ANY COLUMN MASTER KEY DEFINITION TO [public] AS [dbo]
GO
/****** Object:  FullTextCatalog [ListingsCatalog]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE FULLTEXT CATALOG [ListingsCatalog] WITH ACCENT_SENSITIVITY = OFF
AUTHORIZATION [dbo]
GO
/****** Object:  Table [dbo].[BookedTimeFrames]    Script Date: 5/8/2023 1:19:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookedTimeFrames](
	[BookingId] [int] NOT NULL,
	[ListingId] [int] NOT NULL,
	[AvailabilityId] [int] NOT NULL,
	[StartDateTime] [datetime] NOT NULL,
	[EndDateTime] [datetime] NOT NULL,
 CONSTRAINT [UK_BookedTimeFrames] UNIQUE NONCLUSTERED 
(
	[ListingId] ASC,
	[AvailabilityId] ASC,
	[StartDateTime] ASC,
	[EndDateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[BookedTimeFrames] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[Bookings]    Script Date: 5/8/2023 1:19:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bookings](
	[BookingId] [int] IDENTITY(1,1) NOT NULL,
	[ListingId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[FullPrice] [money] NOT NULL,
	[BookingStatusId] [int] NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastEditUser] [int] NOT NULL,
 CONSTRAINT [PK_Bookings] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[Bookings] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[BookingStatuses]    Script Date: 5/8/2023 1:19:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingStatuses](
	[BookingStatusId] [int] IDENTITY(1,1) NOT NULL,
	[BookingStatus] [varchar](50) NOT NULL,
 CONSTRAINT [PK_BookingStatuses] PRIMARY KEY CLUSTERED 
(
	[BookingStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[BookingStatuses] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[ListingAvailabilities]    Script Date: 5/8/2023 1:19:34 AM ******/
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
/****** Object:  Table [dbo].[ListingHistory]    Script Date: 5/8/2023 1:19:34 AM ******/
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
/****** Object:  Table [dbo].[ListingRatings]    Script Date: 5/8/2023 1:19:34 AM ******/
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
/****** Object:  Table [dbo].[Listings]    Script Date: 5/8/2023 1:19:34 AM ******/
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
/****** Object:  Index [UK01]    Script Date: 5/8/2023 1:19:34 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UK01] ON [dbo].[Listings]
(
	[Title] ASC,
	[OwnerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  FullTextIndex     Script Date: 5/8/2023 1:19:34 AM ******/
CREATE FULLTEXT INDEX ON [dbo].[Listings](
[Description] LANGUAGE 'English', 
[Location] LANGUAGE 'English', 
[Title] LANGUAGE 'English')
KEY INDEX [PK_Listings]ON ([ListingsCatalog], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)

GO
ALTER TABLE [dbo].[BookedTimeFrames]  WITH CHECK ADD  CONSTRAINT [FK_BookedTimeFrames_Bookings] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([BookingId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BookedTimeFrames] CHECK CONSTRAINT [FK_BookedTimeFrames_Bookings]
GO
ALTER TABLE [dbo].[BookedTimeFrames]  WITH CHECK ADD  CONSTRAINT [FK_BookedTimeFrames_ListingAvailabilities] FOREIGN KEY([ListingId], [AvailabilityId])
REFERENCES [dbo].[ListingAvailabilities] ([ListingId], [AvailabilityId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BookedTimeFrames] CHECK CONSTRAINT [FK_BookedTimeFrames_ListingAvailabilities]
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
/****** Object:  StoredProcedure [dbo].[CurateListings]    Script Date: 5/8/2023 1:19:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CurateListings] @Offset INT
AS
SELECT L.ListingId, L.Title, L.Location, L.Price, ISNULL(R.AvgRatings, 0) as AvgRatings, ISNULL(R.TotalRatings, 0) as TotalRatings,
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
FETCH NEXT 5 ROWS ONLY;
GO
ALTER AUTHORIZATION ON [dbo].[CurateListings] TO  SCHEMA OWNER 
GO
GRANT EXECUTE ON [dbo].[CurateListings] TO [DevelopmentHell.Hubba.SqlUser.ListingProfile] AS [dbo]
GO
/****** Object:  StoredProcedure [dbo].[GetListingRatingUsernames]    Script Date: 5/8/2023 1:19:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetListingRatingUsernames] @listingId int
AS
SELECT ListingRatings.UserId, UserAccounts.Email
FROM [DevelopmentHell.Hubba.Users].[dbo].[UserAccounts] INNER JOIN [DevelopmentHell.Hubba.ListingProfiles].[dbo].[ListingRatings] ON ListingRatings.UserId = UserAccounts.Id
WHERE ListingId = @listingId
GO
ALTER AUTHORIZATION ON [dbo].[GetListingRatingUsernames] TO  SCHEMA OWNER 
GO
/****** Object:  StoredProcedure [dbo].[GetOwnerAverageRatings]    Script Date: 5/8/2023 1:19:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetOwnerAverageRatings] @OwnerId INT
AS
SELECT ListingRatings.ListingId, AVG(CAST(Rating AS FLOAT)) as AvgRating
FROM ListingRatings 
LEFT JOIN Listings ON Listings.ListingId = ListingRatings.ListingId
WHERE OwnerId = @ownerId
GROUP BY ListingRatings.ListingId
GO
ALTER AUTHORIZATION ON [dbo].[GetOwnerAverageRatings] TO  SCHEMA OWNER 
GO
GRANT EXECUTE ON [dbo].[GetOwnerAverageRatings] TO [DevelopmentHell.Hubba.SqlUser.ListingProfile] AS [dbo]
GO
/****** Object:  StoredProcedure [dbo].[SearchBookingHistory]    Script Date: 5/8/2023 1:19:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SearchBookingHistory] @Query NVARCHAR(200), @UserId INT
AS
SELECT L.OwnerId, L.ListingId, L.Title, B.UserId, B.FullPrice, L.Location, CAST(FT.Rank AS FLOAT) / ISNULL((NULLIF(MAX(FT.Rank) OVER(), 0)), 1) AS Score
FROM [DevelopmentHell.Hubba.ListingProfiles].[dbo].[Listings] AS L
INNER JOIN FREETEXTTABLE(
    [DevelopmentHell.Hubba.ListingProfiles].[dbo].[Listings],
    (Title, Description, Location),
    @Query
) AS FT
ON L.ListingId = FT.[Key]
LEFT JOIN (
    SELECT ListingId, UserId, FullPrice
    FROM [DevelopmentHell.Hubba.ListingProfiles].[dbo].[Bookings]
) as B
ON L.ListingId = B.ListingId
WHERE B.UserId = @UserId
ORDER BY Score DESC
GO
ALTER AUTHORIZATION ON [dbo].[SearchBookingHistory] TO  SCHEMA OWNER 
GO
GRANT EXECUTE ON [dbo].[SearchBookingHistory] TO [DevelopmentHell.Hubba.SqlUser.ListingProfile] AS [dbo]
GO
/****** Object:  StoredProcedure [dbo].[SearchListings]    Script Date: 5/8/2023 1:19:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- SearchListings
CREATE PROCEDURE [dbo].[SearchListings] @Query NVARCHAR(200), @Offset INT, @FTTableRankWeight FLOAT, @RatingsRankWeight FLOAT, @RatingsCountRankWeight FLOAT
AS
SELECT L.ListingId, L.Title, L.Location, L.Price, ISNULL(R.AvgRatings, 0) as AvgRatings, ISNULL(R.TotalRatings, 0) as TotalRatings,
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

