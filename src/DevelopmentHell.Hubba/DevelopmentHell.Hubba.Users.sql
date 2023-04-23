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

alter database [DevelopmentHell.Hubba.Users] set single_user with rollback immediate
DROP DATABASE [DevelopmentHell.Hubba.Users]
GO
USE [master]
GO
/****** Object:  Database [DevelopmentHell.Hubba.Users]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE DATABASE [DevelopmentHell.Hubba.Users]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DevelopmentHell.Hubba.Accounts', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.Accounts.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DevelopmentHell.Hubba.Accounts_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.Accounts_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 COLLATE SQL_Latin1_General_CP1_CI_AS
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DevelopmentHell.Hubba.Users].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET ARITHABORT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET RECOVERY FULL 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET  MULTI_USER 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'DevelopmentHell.Hubba.Users', N'ON'
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET QUERY_STORE = OFF
GO
/****** Object:  Login [NT SERVICE\Winmgmt]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [NT SERVICE\Winmgmt] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLWriter]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [NT SERVICE\SQLWriter] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY$SQLEXPRESS]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [NT SERVICE\SQLTELEMETRY$SQLEXPRESS] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [NT SERVICE\SQLTELEMETRY] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLSERVERAGENT]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [NT SERVICE\SQLSERVERAGENT] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT Service\MSSQLSERVER]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [NT Service\MSSQLSERVER] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT Service\MSSQL$SQLEXPRESS]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [NT Service\MSSQL$SQLEXPRESS] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT AUTHORITY\SYSTEM]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [GMAIN\tsuma]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [GMAIN\tsuma] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [distributor_admin]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [distributor_admin] WITH PASSWORD=N'PyiPPooDFYCKGEp7vN8Ok3f4xMhozzxhZ4xOhvKtqgc=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH PASSWORD=N'uRQjGJV3HGjrXgaIUtnWbn5CP/VCDvjqmUda0D1m2vw=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] WITH PASSWORD=N'ZnonZ6Yw0E6pp6Q0eMo4QeqFYQuljyvIRohn0xGsmdM=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Notification]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] WITH PASSWORD=N'dej7ZFNhHH2922K3zwphk6Oh+auyncvQFcumMsenZwE=', DEFAULT_DATABASE=[DevelopmentHell.Hubba.Notifications], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Logging]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] WITH PASSWORD=N'SgurjvFAXugFc+lDAdTjSJdZq2ixCMXWoQhxiAhhV+k=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.ListingProfile]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] WITH PASSWORD=N'HAltLsce93N6tSzNi/2rhG9cVp6DF1OkwtZMXMVEvNs=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
/****** Object:  Login [BUILTIN\Users]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [BUILTIN\Users] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyTsqlExecutionLogin##]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [##MS_PolicyTsqlExecutionLogin##] WITH PASSWORD=N'TiAeF9iUSJYZyi4MmF2JtdFeXzs0AidDULv6gc1L3Vg=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyEventProcessingLogin##]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE LOGIN [##MS_PolicyEventProcessingLogin##] WITH PASSWORD=N'DeWNXzK6+kXqlmHi8n/NVXxdYNHkLpbsuqD4OvHcp94=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER AUTHORIZATION ON DATABASE::[DevelopmentHell.Hubba.Users] TO [GMAIN\tsuma]
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
USE [DevelopmentHell.Hubba.Users]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.User] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [cdc]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE USER [cdc] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[cdc]
GO
/****** Object:  DatabaseRole [MStran_PAL_role]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE ROLE [MStran_PAL_role]
GO
/****** Object:  DatabaseRole [MSReplPAL_5_1]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE ROLE [MSReplPAL_5_1]
GO
ALTER AUTHORIZATION ON ROLE::[MStran_PAL_role] TO [dbo]
GO
ALTER AUTHORIZATION ON ROLE::[MSReplPAL_5_1] TO [dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.User]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.User]
GO
ALTER ROLE [MStran_PAL_role] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]
GO
ALTER ROLE [MSReplPAL_5_1] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]
GO
ALTER ROLE [db_owner] ADD MEMBER [cdc]
GO
ALTER ROLE [MStran_PAL_role] ADD MEMBER [MSReplPAL_5_1]
GO
GRANT CONNECT TO [cdc] AS [dbo]
GO
GRANT CONNECT TO [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] AS [dbo]
GO
GRANT CONNECT REPLICATION TO [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] AS [dbo]
GO
GRANT CONNECT TO [DevelopmentHell.Hubba.SqlUser.User] AS [dbo]
GO
GRANT DELETE TO [DevelopmentHell.Hubba.SqlUser.User] AS [dbo]
GO
GRANT INSERT TO [DevelopmentHell.Hubba.SqlUser.User] AS [dbo]
GO
GRANT SELECT TO [DevelopmentHell.Hubba.SqlUser.User] AS [dbo]
GO
GRANT UPDATE TO [DevelopmentHell.Hubba.SqlUser.User] AS [dbo]
GO
GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [public] AS [dbo]
GO
GRANT VIEW ANY COLUMN MASTER KEY DEFINITION TO [public] AS [dbo]
GO
/****** Object:  Schema [cdc]    Script Date: 4/22/2023 11:57:23 PM ******/
CREATE SCHEMA [cdc] AUTHORIZATION [cdc]
GO
/****** Object:  Table [dbo].[RecoveryRequests]    Script Date: 4/22/2023 11:57:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecoveryRequests](
	[Id] [int] NOT NULL,
	[RequestTime] [datetime] NOT NULL,
 CONSTRAINT [PK_RecoveryRequests_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[RecoveryRequests] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[UserAccounts]    Script Date: 4/22/2023 11:57:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAccounts](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Email] [varchar](320) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[PasswordHash] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[PasswordSalt] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LoginAttempts] [int] NOT NULL,
	[FailureTime] [datetime] NULL,
	[Disabled] [bit] NOT NULL,
	[Role] [varchar](32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[CellPhoneProvider] [int] NULL,
	[CellPhoneNumber] [varchar](11) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_UserAccounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[UserAccounts] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[UserLogins]    Script Date: 4/22/2023 11:57:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogins](
	[Id] [int] NOT NULL,
	[IPAddress] [varchar](16) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_UserLogins] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[IPAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[UserLogins] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[UserNames]    Script Date: 4/22/2023 11:57:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserNames](
	[UserAccountId] [int] NOT NULL,
	[FirstName] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LastName] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[UserName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_UserData] PRIMARY KEY CLUSTERED 
(
	[UserAccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[UserNames] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[UserOTPs]    Script Date: 4/22/2023 11:57:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserOTPs](
	[UserAccountId] [int] NOT NULL,
	[Expiration] [datetime] NULL,
	[Passphrase] [binary](16) NULL,
 CONSTRAINT [PK_UserOTPs] PRIMARY KEY CLUSTERED 
(
	[UserAccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[UserOTPs] TO  SCHEMA OWNER 
GO
ALTER TABLE [dbo].[RecoveryRequests]  WITH CHECK ADD  CONSTRAINT [FK_RecoveryRequests_UserAccounts] FOREIGN KEY([Id])
REFERENCES [dbo].[UserAccounts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RecoveryRequests] CHECK CONSTRAINT [FK_RecoveryRequests_UserAccounts]
GO
ALTER TABLE [dbo].[UserLogins]  WITH CHECK ADD  CONSTRAINT [FK_UserLogins_UserAccounts] FOREIGN KEY([Id])
REFERENCES [dbo].[UserAccounts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserLogins] CHECK CONSTRAINT [FK_UserLogins_UserAccounts]
GO
ALTER TABLE [dbo].[UserOTPs]  WITH CHECK ADD  CONSTRAINT [FK_UserOTPs_UserAccounts] FOREIGN KEY([UserAccountId])
REFERENCES [dbo].[UserAccounts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserOTPs] CHECK CONSTRAINT [FK_UserOTPs_UserAccounts]
GO
ALTER TABLE [dbo].[UserNames]  WITH CHECK ADD  CONSTRAINT [FK_UserNames_UserAccounts] FOREIGN KEY([UserAccountId])
REFERENCES [dbo].[UserAccounts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserOTPs] CHECK CONSTRAINT [FK_UserNames_UserAccounts]
GO
/****** Object:  Trigger [dbo].[tr_UserAccounts_CascadingDelete]    Script Date: 4/22/2023 11:57:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[tr_UserAccounts_CascadingDelete]
ON [dbo].[UserAccounts]
AFTER DELETE
AS
BEGIN
	DELETE FROM [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[ShowcaseCommentReports] WHERE [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[ShowcaseCommentReports].ReporterId IN (SELECT Id FROM deleted);
	DELETE FROM [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[ShowcaseComments] WHERE [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[ShowcaseComments].CommenterId IN (SELECT Id FROM deleted);
	DELETE FROM [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[ShowcaseCommentVotes] WHERE [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[ShowcaseCommentVotes].VoterId IN (SELECT Id FROM deleted);
	DELETE FROM [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[ShowcaseReports] WHERE [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[ShowcaseReports].ReporterId IN (SELECT Id FROM deleted);
	DELETE FROM [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[Showcases] WHERE [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[Showcases].ShowcaseUserId IN (SELECT Id FROM deleted);
	DELETE FROM [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[ShowcaseVotes] WHERE [DevelopmentHell.Hubba.ProjectShowcases].[dbo].[ShowcaseVotes].UserAccountId IN (SELECT Id FROM deleted);
END;
GO
ALTER TABLE [dbo].[UserAccounts] ENABLE TRIGGER [tr_UserAccounts_CascadingDelete]
GO
USE [master]
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET  READ_WRITE 
GO
