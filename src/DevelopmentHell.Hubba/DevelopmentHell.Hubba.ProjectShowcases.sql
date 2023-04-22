USE [master]
GO

alter database [DevelopmentHell.Hubba.ProjectShowcases] set single_user with rollback immediate
DROP DATABASE [DevelopmentHell.Hubba.ProjectShowcases]
GO
USE [master]
GO

/****** Object:  Database [DevelopmentHell.Hubba.ProjectShowcases]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE DATABASE [DevelopmentHell.Hubba.ProjectShowcases]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DevelopmentHell.Hubba.ProjectShowcases', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.ProjectShowcases.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DevelopmentHell.Hubba.ProjectShowcases_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.ProjectShowcases_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
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
/****** Object:  Login [NT SERVICE\Winmgmt]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [NT SERVICE\Winmgmt] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLWriter]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [NT SERVICE\SQLWriter] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY$SQLEXPRESS]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [NT SERVICE\SQLTELEMETRY$SQLEXPRESS] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [NT SERVICE\SQLTELEMETRY] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLSERVERAGENT]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [NT SERVICE\SQLSERVERAGENT] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT Service\MSSQLSERVER]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [NT Service\MSSQLSERVER] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT Service\MSSQL$SQLEXPRESS]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [NT Service\MSSQL$SQLEXPRESS] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT AUTHORITY\SYSTEM]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [GMAIN\tsuma]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [GMAIN\tsuma] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.User] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Notification]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] WITH PASSWORD=N'password', DEFAULT_DATABASE=[DevelopmentHell.Hubba.Notifications], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Logging]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.ListingProfile]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] ENABLE
GO
/****** Object:  Login [BUILTIN\Users]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [BUILTIN\Users] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyTsqlExecutionLogin##]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [##MS_PolicyTsqlExecutionLogin##] WITH PASSWORD=N'/VgTVtQ+Dr6FnSMJBPyt0swQYHFG79IhMufBJRC54kk=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [##MS_PolicyTsqlExecutionLogin##] DISABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyEventProcessingLogin##]    Script Date: 4/19/2023 11:22:13 PM ******/
CREATE LOGIN [##MS_PolicyEventProcessingLogin##] WITH PASSWORD=N'v6oSChtkmfb6FXQuO/KF12RWfSeCk5436eNnRq4aEWc=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [##MS_PolicyEventProcessingLogin##] DISABLE
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
USE [DevelopmentHell.Hubba.ProjectShowcases]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]    Script Date: 4/19/2023 11:22:13 PM ******/
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
/****** Object:  FullTextCatalog [ShowcasesCatalog]    Script Date: 4/19/2023 11:22:14 PM ******/
CREATE FULLTEXT CATALOG [ShowcasesCatalog] WITH ACCENT_SENSITIVITY = ON
AUTHORIZATION [dbo]
GO
/****** Object:  Table [dbo].[Showcases]    Script Date: 4/19/2023 11:22:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Showcases](
	[Id] [nvarchar](30) NOT NULL,
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[Showcases] TO  SCHEMA OWNER 
GO
/****** Object:  FullTextIndex     Script Date: 4/19/2023 11:22:14 PM ******/
CREATE FULLTEXT INDEX ON [dbo].[Showcases](
[Description] LANGUAGE 'English', 
[Title] LANGUAGE 'English')
KEY INDEX [PK_Showcases]ON ([ShowcasesCatalog], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)

GO
ALTER TABLE [dbo].[Showcases] ADD  CONSTRAINT [DF_Showcases_IsPublished]  DEFAULT ((0)) FOR [IsPublished]
GO
ALTER TABLE [dbo].[Showcases] ADD  CONSTRAINT [DF_Showcases_Rating]  DEFAULT ((0)) FOR [Rating]
GO
/****** Object:  StoredProcedure [dbo].[CurateShowcases]    Script Date: 4/19/2023 11:22:14 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SearchShowcases]    Script Date: 4/19/2023 11:22:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- SearchShowcases
CREATE PROCEDURE [dbo].[SearchShowcases] @Query NVARCHAR(200), @Offset INT, @FTTableRankWeight FLOAT, @RatingsRankWeight FLOAT
AS
SELECT S.Id, S.Title, S.Rating, S.Description,
    (CAST(FT.Rank AS FLOAT) / ISNULL((NULLIF(MAX(FT.Rank) OVER(), 0)), 1) * @FTTableRankWeight             -- FTTableRank
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
USE [master]
GO
ALTER DATABASE [DevelopmentHell.Hubba.ProjectShowcases] SET  READ_WRITE 
GO
