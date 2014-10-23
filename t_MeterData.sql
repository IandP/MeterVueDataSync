/*
---------------------------------------------------------------------------------------------------
DATE: 23/10/2014	WHO: JW
---------------------------------------------------------------------------------------------------
TITLE: MeterData
---------------------------------------------------------------------------------------------------
DESCRIPTION:


TODO:

---------------------------------------------------------------------------------------------------
*/

if exists (select * from sysobjects where id = object_id('dbo.MeterData') and sysstat & 0xf = 3)
	drop table dbo.MeterData
GO

SET ANSI_NULLS ON 
GO

CREATE TABLE dbo.MeterData
(
	MeterGUID uniqueidentifier NOT NULL,
	DataType varchar(50) NOT NULL,
	TimeStamp datetime NOT NULL,
	DataValue float NOT NULL,
	CONSTRAINT PK_MeterData PRIMARY KEY (MeterGUID,DataType,TimeStamp),
	CONSTRAINT FK_MeterData_Meter FOREIGN KEY (MeterGUID) REFERENCES dbo.Meter (MeterGUID)
)
GO

-- add permissions here
-- GRANT SELECT ON MeterData TO 

GO

SET ANSI_NULLS ON 
GO