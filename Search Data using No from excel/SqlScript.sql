USE [master]
GO
/****** Object:  Database [uploadingfile]    Script Date: 12/5/2023 5:24:27 PM ******/
CREATE DATABASE [uploadingfile]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'uploadingfile', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\uploadingfile.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'uploadingfile_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\uploadingfile_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [uploadingfile] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [uploadingfile].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [uploadingfile] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [uploadingfile] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [uploadingfile] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [uploadingfile] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [uploadingfile] SET ARITHABORT OFF 
GO
ALTER DATABASE [uploadingfile] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [uploadingfile] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [uploadingfile] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [uploadingfile] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [uploadingfile] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [uploadingfile] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [uploadingfile] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [uploadingfile] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [uploadingfile] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [uploadingfile] SET  ENABLE_BROKER 
GO
ALTER DATABASE [uploadingfile] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [uploadingfile] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [uploadingfile] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [uploadingfile] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [uploadingfile] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [uploadingfile] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [uploadingfile] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [uploadingfile] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [uploadingfile] SET  MULTI_USER 
GO
ALTER DATABASE [uploadingfile] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [uploadingfile] SET DB_CHAINING OFF 
GO
ALTER DATABASE [uploadingfile] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [uploadingfile] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [uploadingfile] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [uploadingfile] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [uploadingfile] SET QUERY_STORE = OFF
GO
USE [uploadingfile]
GO
/****** Object:  Table [dbo].[MainTable]    Script Date: 12/5/2023 5:24:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MainTable](
	[ProposalNumber] [varchar](50) NULL,
	[Status] [varchar](50) NULL,
	[SubStatus] [varchar](50) NULL,
	[CollectionType] [varchar](50) NULL,
	[IMDCode] [varchar](50) NULL,
	[Policyholder] [varchar](100) NULL,
	[PremiumPayerApplicable] [varchar](50) NULL,
	[Premium] [varchar](50) NULL,
	[PayerID] [varchar](50) NULL,
	[TotalTaxes] [varchar](50) NULL,
	[TotalPremiumDue] [varchar](50) NULL,
	[CollectionNumber] [varchar](50) NULL,
	[CollectionDate] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[temptable]    Script Date: 12/5/2023 5:24:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[temptable](
	[ProposalNumber] [varchar](50) NULL,
	[Status] [varchar](50) NULL,
	[SubStatus] [varchar](50) NULL,
	[CollectionType] [varchar](50) NULL,
	[IMDCode] [varchar](50) NULL,
	[Policyholder] [varchar](100) NULL,
	[PremiumPayerApplicable] [varchar](50) NULL,
	[Premium] [varchar](50) NULL,
	[PayerID] [varchar](50) NULL,
	[TotalTaxes] [varchar](50) NULL,
	[TotalPremiumDue] [varchar](50) NULL,
	[CollectionNumber] [varchar](50) NULL,
	[CollectionDate] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[InsertMatchingProposalDetails]    Script Date: 12/5/2023 5:24:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--use uploadingfile
--SELECT *
--INTO temptable
--FROM Maintable
--WHERE 1 = 0
--select * from Maintable
--select * from temptable
--truncate table temptable
--CREATE PROCEDURE InsertMatchingProposalDetails
--AS
--BEGIN
--    INSERT INTO temptable (ProposalNumber, Status, SubStatus, CollectionType, IMDCode, PolicyHolder, PremiumPayerApplicable, PayerID, Premium, TotalTaxes, TotalPremiumDue, CollectionNumber, CollectionDate)
--    SELECT t.ProposalNumber, m.Status, m.SubStatus, m.CollectionType, m.IMDCode, m.PolicyHolder, m.PremiumPayerApplicable, m.PayerID, m.Premium, m.TotalTaxes, m.TotalPremiumDue, m.CollectionNumber, m.CollectionDate
--    FROM temptable t
--     JOIN Maintable m ON t.ProposalNumber = m.ProposalNumber
--END

CREATE PROCEDURE [dbo].[InsertMatchingProposalDetails]
AS
BEGIN
    UPDATE temptable
    SET temptable.Status = m.Status,
        temptable.SubStatus = m.SubStatus,
        temptable.CollectionType = m.CollectionType,
        temptable.IMDCode = m.IMDCode,
        temptable.PolicyHolder = m.PolicyHolder,
        temptable.PremiumPayerApplicable = m.PremiumPayerApplicable,
        temptable.PayerID = m.PayerID,
        temptable.Premium = m.Premium,
        temptable.TotalTaxes = m.TotalTaxes,
        temptable.TotalPremiumDue = m.TotalPremiumDue,
        temptable.CollectionNumber = m.CollectionNumber,
        temptable.CollectionDate = m.CollectionDate
    FROM temptable t
    JOIN Maintable m ON t.ProposalNumber = m.ProposalNumber;
END





GO
USE [master]
GO
ALTER DATABASE [uploadingfile] SET  READ_WRITE 
GO
