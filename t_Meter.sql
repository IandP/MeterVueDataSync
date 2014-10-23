/*
---------------------------------------------------------------------------------------------------
DATE: 23/10/2014	WHO: JW
---------------------------------------------------------------------------------------------------
TITLE: Meter
---------------------------------------------------------------------------------------------------
DESCRIPTION:


TODO:

---------------------------------------------------------------------------------------------------
*/

if exists (select * from sysobjects where id = object_id('dbo.Meter') and sysstat & 0xf = 3)
	drop table dbo.Meter
GO

SET ANSI_NULLS ON 
GO

CREATE TABLE dbo.Meter
(
	MeterGUID uniqueidentifier NOT NULL,
	DisplayName nvarchar(50) NOT NULL,
	CONSTRAINT PK_Meter PRIMARY KEY (MeterGUID)
)
GO

-- add permissions here
-- GRANT SELECT ON Meter TO 

GO

SET ANSI_NULLS ON 
GO