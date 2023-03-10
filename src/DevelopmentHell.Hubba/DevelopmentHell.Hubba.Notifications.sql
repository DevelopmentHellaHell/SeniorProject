USE [master]
GO

alter database [DevelopmentHell.Hubba.Notifications] set single_user with rollback immediate
DROP DATABASE [DevelopmentHell.Hubba.Notifications]
GO
USE [master]
GO

/****** Object:  Database [DevelopmentHell.Hubba.Notifications]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE DATABASE [DevelopmentHell.Hubba.Notifications]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DevelopmentHell.Hubba.Notifications', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.Notifications.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DevelopmentHell.Hubba.Notifications_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.Notifications_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DevelopmentHell.Hubba.Notifications].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET ARITHABORT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET RECOVERY FULL 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET  MULTI_USER 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'DevelopmentHell.Hubba.Notifications', N'ON'
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET QUERY_STORE = ON
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
/****** Object:  Login [ZOJIRUSHI\Kevin]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE LOGIN [ZOJIRUSHI\Kevin] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\Winmgmt]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE LOGIN [NT SERVICE\Winmgmt] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLWriter]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE LOGIN [NT SERVICE\SQLWriter] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE LOGIN [NT SERVICE\SQLTELEMETRY] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLSERVERAGENT]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE LOGIN [NT SERVICE\SQLSERVERAGENT] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT Service\MSSQLSERVER]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE LOGIN [NT Service\MSSQLSERVER] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT AUTHORITY\SYSTEM]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.User] DISABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Notification]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] WITH PASSWORD=N'password', DEFAULT_DATABASE=[DevelopmentHell.Hubba.Notifications], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] DISABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Logging]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] DISABLE
GO
/****** Object:  Login [BUILTIN\Users]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE LOGIN [BUILTIN\Users] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyTsqlExecutionLogin##]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE LOGIN [##MS_PolicyTsqlExecutionLogin##] WITH PASSWORD=N'VF5EXEMg0+u/qRDOhef1eSBYlUat4bVG2qhYFskIAdw=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [##MS_PolicyTsqlExecutionLogin##] DISABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyEventProcessingLogin##]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE LOGIN [##MS_PolicyEventProcessingLogin##] WITH PASSWORD=N'LL6GFoxTSQtX1s5Nc/8ff9jwM2xWCKTZv6VzxWZjwH4=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [##MS_PolicyEventProcessingLogin##] DISABLE
GO
ALTER AUTHORIZATION ON DATABASE::[DevelopmentHell.Hubba.Notifications] TO [ZOJIRUSHI\Kevin]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [ZOJIRUSHI\Kevin]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\Winmgmt]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\SQLWriter]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT SERVICE\SQLSERVERAGENT]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [NT Service\MSSQLSERVER]
GO
USE [DevelopmentHell.Hubba.Notifications]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.Notifications]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.Notifications] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.Notification]    Script Date: 3/10/2023 12:30:08 AM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.Notification] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.Notifications]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.Notifications]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.Notification]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.Notification]
GO
GRANT CONNECT TO [DevelopmentHell.Hubba.SqlUser.Notification] AS [dbo]
GO
GRANT CONNECT TO [DevelopmentHell.Hubba.SqlUser.Notifications] AS [dbo]
GO
GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [public] AS [dbo]
GO
GRANT VIEW ANY COLUMN MASTER KEY DEFINITION TO [public] AS [dbo]
GO
/****** Object:  Table [dbo].[NotificationSettings]    Script Date: 3/10/2023 12:30:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotificationSettings](
	[UserId] [int] NOT NULL,
	[SiteNotifications] [bit] NOT NULL,
	[EmailNotifications] [bit] NOT NULL,
	[TextNotifications] [bit] NOT NULL,
	[TypeScheduling] [bit] NOT NULL,
	[TypeWorkspace] [bit] NOT NULL,
	[TypeProjectShowcase] [bit] NOT NULL,
	[TypeOther] [bit] NOT NULL,
 CONSTRAINT [PK_NotificationSettings] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[NotificationSettings] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[UserNotifications]    Script Date: 3/10/2023 12:30:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserNotifications](
	[UserId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[Tag] [int] NOT NULL,
	[Message] [varchar](256) NOT NULL,
	[Hide] [bit] NOT NULL,
	[NotificationId] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_UserNotifications] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[DateCreated] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[UserNotifications] TO  SCHEMA OWNER 
GO
USE [master]
GO
ALTER DATABASE [DevelopmentHell.Hubba.Notifications] SET  READ_WRITE 
GO

