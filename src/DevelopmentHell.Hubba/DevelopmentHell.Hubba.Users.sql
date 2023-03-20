﻿USE [master]
GO

alter database [DevelopmentHell.Hubba.Users] set single_user with rollback immediate
DROP DATABASE [DevelopmentHell.Hubba.Users]
GO
USE [master]
GO

ALTER TABLE [dbo].[UserOTPs] DROP CONSTRAINT [FK_UserOTPs_UserAccounts]
GO
ALTER TABLE [dbo].[UserNames] DROP CONSTRAINT [FK_UserData_UserAccounts]
GO
ALTER TABLE [dbo].[UserLogins] DROP CONSTRAINT [FK_UserLogins_UserAccounts]
GO
ALTER TABLE [dbo].[RecoveryRequests] DROP CONSTRAINT [FK_RecoveryRequests_UserAccounts]
GO
/****** Object:  Index [IX_UserData]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP INDEX [IX_UserData] ON [dbo].[UserNames]
GO
/****** Object:  Table [dbo].[UserOTPs]    Script Date: 3/20/2023 9:23:14 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOTPs]') AND type in (N'U'))
DROP TABLE [dbo].[UserOTPs]
GO
/****** Object:  Table [dbo].[UserNames]    Script Date: 3/20/2023 9:23:14 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserNames]') AND type in (N'U'))
DROP TABLE [dbo].[UserNames]
GO
/****** Object:  Table [dbo].[UserLogins]    Script Date: 3/20/2023 9:23:14 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserLogins]') AND type in (N'U'))
DROP TABLE [dbo].[UserLogins]
GO
/****** Object:  Table [dbo].[UserAccounts]    Script Date: 3/20/2023 9:23:14 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserAccounts]') AND type in (N'U'))
DROP TABLE [dbo].[UserAccounts]
GO
/****** Object:  Table [dbo].[RecoveryRequests]    Script Date: 3/20/2023 9:23:14 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RecoveryRequests]') AND type in (N'U'))
DROP TABLE [dbo].[RecoveryRequests]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP USER [DevelopmentHell.Hubba.SqlUser.User]
GO
USE [master]
GO
/****** Object:  Login [##MS_PolicyEventProcessingLogin##]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [##MS_PolicyEventProcessingLogin##]
GO
/****** Object:  Login [##MS_PolicyTsqlExecutionLogin##]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [##MS_PolicyTsqlExecutionLogin##]
GO
/****** Object:  Login [BUILTIN\Users]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [BUILTIN\Users]
GO
																						   
								  
  
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Logging]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [DevelopmentHell.Hubba.SqlUser.Logging]
GO
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [DevelopmentHell.Hubba.SqlUser.User]
GO
/****** Object:  Login [GMAIN\tsuma]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [GMAIN\tsuma]
GO
/****** Object:  Login [NT AUTHORITY\SYSTEM]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [NT AUTHORITY\SYSTEM]
GO
/****** Object:  Login [NT Service\MSSQL$SQLEXPRESS]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [NT Service\MSSQL$SQLEXPRESS]
GO
/****** Object:  Login [NT Service\MSSQLSERVER]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [NT Service\MSSQLSERVER]
GO
/****** Object:  Login [NT SERVICE\SQLSERVERAGENT]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [NT SERVICE\SQLSERVERAGENT]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [NT SERVICE\SQLTELEMETRY]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY$SQLEXPRESS]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [NT SERVICE\SQLTELEMETRY$SQLEXPRESS]
GO
/****** Object:  Login [NT SERVICE\SQLWriter]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [NT SERVICE\SQLWriter]
GO
/****** Object:  Login [NT SERVICE\Winmgmt]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP LOGIN [NT SERVICE\Winmgmt]
GO
/****** Object:  Database [DevelopmentHell.Hubba.Users]    Script Date: 3/20/2023 9:23:14 AM ******/
DROP DATABASE [DevelopmentHell.Hubba.Users]
GO
/****** Object:  Database [DevelopmentHell.Hubba.Users]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE DATABASE [DevelopmentHell.Hubba.Users]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DevelopmentHell.Hubba.Accounts', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.Accounts.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DevelopmentHell.Hubba.Accounts_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DevelopmentHell.Hubba.Accounts_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
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
/****** Object:  Login [NT SERVICE\Winmgmt]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [NT SERVICE\Winmgmt] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLWriter]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [NT SERVICE\SQLWriter] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY$SQLEXPRESS]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [NT SERVICE\SQLTELEMETRY$SQLEXPRESS] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLTELEMETRY]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [NT SERVICE\SQLTELEMETRY] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT SERVICE\SQLSERVERAGENT]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [NT SERVICE\SQLSERVERAGENT] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT Service\MSSQLSERVER]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [NT Service\MSSQLSERVER] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT Service\MSSQL$SQLEXPRESS]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [NT Service\MSSQL$SQLEXPRESS] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [NT AUTHORITY\SYSTEM]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/****** Object:  Login [GMAIN\tsuma]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [GMAIN\tsuma] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH PASSWORD=N'xp6Wp/rfs6DEdUXD3NMUCf+RnoFr7UEnpzMV5Yr0gEs=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.User] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [DevelopmentHell.Hubba.SqlUser.Logging]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] WITH PASSWORD=N'5Pg0K16nKfedqVrZkEv/4ym+AoKiLEPw0Y7pKjd+2Xw=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
  
														  
GO
ALTER LOGIN [DevelopmentHell.Hubba.SqlUser.Logging] ENABLE
																											   
GO
/****** Object:  Login [BUILTIN\Users]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [BUILTIN\Users] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyTsqlExecutionLogin##]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [##MS_PolicyTsqlExecutionLogin##] WITH PASSWORD=N'VXEZN1gVf+WpHptk9xxzefyRYBRx8W8Yz9h3+XRVmlA=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [##MS_PolicyTsqlExecutionLogin##] ENABLE
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [##MS_PolicyEventProcessingLogin##]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE LOGIN [##MS_PolicyEventProcessingLogin##] WITH PASSWORD=N'U3k1WLxeNEc4BaeBLeNf+6nfsBZaKAEcgQMtYzqQT/s=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [##MS_PolicyEventProcessingLogin##] ENABLE
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
USE [DevelopmentHell.Hubba.Users]
GO
/****** Object:  User [DevelopmentHell.Hubba.SqlUser.User]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE USER [DevelopmentHell.Hubba.SqlUser.User] FOR LOGIN [DevelopmentHell.Hubba.SqlUser.User] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.User]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [DevelopmentHell.Hubba.SqlUser.User]
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
/****** Object:  Table [dbo].[RecoveryRequests]    Script Date: 3/20/2023 9:23:14 AM ******/
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
/****** Object:  Table [dbo].[UserAccounts]    Script Date: 3/20/2023 9:23:14 AM ******/
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
								
									  
 CONSTRAINT [PK_UserAccounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[UserAccounts] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[UserLogins]    Script Date: 3/20/2023 9:23:14 AM ******/
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
/****** Object:  Table [dbo].[UserNames]    Script Date: 3/20/2023 9:23:14 AM ******/
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
/****** Object:  Table [dbo].[UserOTPs]    Script Date: 3/20/2023 9:23:14 AM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserData]    Script Date: 3/20/2023 9:23:14 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserData] ON [dbo].[UserNames]
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
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
ALTER TABLE [dbo].[UserNames]  WITH CHECK ADD  CONSTRAINT [FK_UserData_UserAccounts] FOREIGN KEY([UserAccountId])
REFERENCES [dbo].[UserAccounts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserNames] CHECK CONSTRAINT [FK_UserData_UserAccounts]
GO
ALTER TABLE [dbo].[UserOTPs]  WITH CHECK ADD  CONSTRAINT [FK_UserOTPs_UserAccounts] FOREIGN KEY([UserAccountId])
REFERENCES [dbo].[UserAccounts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserOTPs] CHECK CONSTRAINT [FK_UserOTPs_UserAccounts]
GO
USE [master]
GO
ALTER DATABASE [DevelopmentHell.Hubba.Users] SET  READ_WRITE 
GO
