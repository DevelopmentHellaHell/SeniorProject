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

/****** Object:  Database [DevelopmentHell.Hubba.Users]    Script Date: 5/1/2023 3:52:02 PM ******/
CREATE DATABASE [DevelopmentHell.Hubba.Users]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DevelopmentHell.Hubba.Accounts', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.Accounts.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DevelopmentHell.Hubba.Accounts_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.Accounts_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
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
/****** Object:  Login [NT SERVICE\Winmgmt]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [NT SERVICE\Winmgmt] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLWriter]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [NT SERVICE\SQLWriter] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [NT SERVICE\SQLTELEMETRY] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLSERVERAGENT]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [NT SERVICE\SQLSERVERAGENT] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT Service\MSSQLSERVER]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [NT Service\MSSQLSERVER] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT AUTHORITY\SYSTEM]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [distributor_admin]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [distributor_admin] WITH PASSWORD=N'ttYLGVjyeECkQawUPekQhGEys+tGBK7ye+92PBzTFRY=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [distributor_admin] DISABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.User] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Notification]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] WITH PASSWORD=N'password', DEFAULT_DATABASE=[DevelopmentHell.Hubba.Notifications], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Notification] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Logging]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.ListingProfile]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.ListingProfile] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] ENABLE
GO
/****** Object:  Login [BUILTIN\Users]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [BUILTIN\Users] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [BRYANS-LAPTOP\bryan]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [BRYANS-LAPTOP\bryan] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyTsqlExecutionLogin##]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [##MS_PolicyTsqlExecutionLogin##] WITH PASSWORD=N'x53jrNvfiqDZG70qghMHHI0QDxRNdjQjaEv7u4MJXYo=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [##MS_PolicyTsqlExecutionLogin##] DISABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyEventProcessingLogin##]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE LOGIN [##MS_PolicyEventProcessingLogin##] WITH PASSWORD=N'QKJ867iVSzraQlcZrSd+CZQVGV3KK/u30FmmeU+hj4c=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [##MS_PolicyEventProcessingLogin##] DISABLE
GO
ALTER AUTHORIZATION ON DATABASE::[DevelopmentHell.Hubba.Users] TO [BRYANS-LAPTOP\bryan]
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
USE [DevelopmentHell.Hubba.Users]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.User] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile]    Script Date: 5/1/2023 3:52:03 PM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.User]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.User]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.ProjectShowcase]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile]
GO
GRANT CONNECT TO [DevelopmentHell.Hubba.SqlUser.CollaboratorProfile] AS [dbo]
GO
GRANT CONNECT TO [DevelopmentHell.Hubba.SqlUser.ProjectShowcase] AS [dbo]
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
/****** Object:  Table [dbo].[RecoveryRequests]    Script Date: 5/1/2023 3:52:03 PM ******/
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
/****** Object:  Table [dbo].[UserAccounts]    Script Date: 5/1/2023 3:52:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAccounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](320) NOT NULL,
	[PasswordHash] [varchar](256) NOT NULL,
	[PasswordSalt] [varchar](256) NOT NULL,
	[LoginAttempts] [int] NOT NULL,
	[FailureTime] [datetime] NULL,
	[Disabled] [bit] NOT NULL,
	[Role] [varchar](32) NOT NULL,
	[CellPhoneProvider] [int] NULL,
	[CellPhoneNumber] [varchar](11) NULL,
	[FirstName] [varchar](25) NULL,
	[LastName] [varchar](25) NULL,
 CONSTRAINT [PK_UserAccounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[UserAccounts] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[UserLogins]    Script Date: 5/1/2023 3:52:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogins](
	[Id] [int] NOT NULL,
	[IPAddress] [varchar](16) NOT NULL,
 CONSTRAINT [PK_UserLogins] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[IPAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[UserLogins] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[UserNames]    Script Date: 5/1/2023 3:52:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserNames](
	[UserAccountId] [int] NOT NULL,
	[FirstName] [varchar](256) NULL,
	[LastName] [varchar](256) NULL,
	[UserName] [varchar](50) NULL,
 CONSTRAINT [PK_UserData] PRIMARY KEY CLUSTERED 
(
	[UserAccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[UserNames] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[UserOTPs]    Script Date: 5/1/2023 3:52:03 PM ******/
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
/****** Object:  Trigger [dbo].[tr_UserAccounts_CascadingDelete]    Script Date: 5/1/2023 3:52:03 PM ******/
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

