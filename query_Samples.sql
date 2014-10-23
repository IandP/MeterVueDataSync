
SELECT m.DisplayName,
		md.MeterGUID,
		md.TimeStamp,
		md.DataValue
FROM MeterData md
INNER JOIN Meter m ON m.MeterGUID = md.MeterGUID
WHERE TimeStamp BETWEEN '1 jul 2014' AND '1 Aug 2014'



SELECT m.MeterGUID,
		m.DisplayName,
		AVG(md.DataValue) * DATEDIFF(hour,'1 jul 2014','1 Aug 2014') AS Volume
FROM MeterData md
INNER JOIN Meter m ON m.MeterGUID = md.MeterGUID
WHERE TimeStamp BETWEEN '1 jul 2014' AND '1 Aug 2014'
GROUP BY m.MeterGUID,m.DisplayName
		