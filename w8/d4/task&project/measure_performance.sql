SET STATISTICS IO ON;
SET STATISTICS TIME ON;

SELECT Id, PatientId, HeartRate, OxygenLevel, RecordedAt
FROM VitalSigns
WHERE PatientId = 8
ORDER BY RecordedAt DESC;