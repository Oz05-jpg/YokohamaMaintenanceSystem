-- Practice — Technicians Above Average Workload (CTE reinforcement)
-- Rewrites practice-technician-subquery.sql (08-05) using WITH instead of nested subquery.
-- Verified 2026-08-09: matches subquery version row-for-row (Somchai Jaidee, RequestCount 3).

USE YokohamaMaintenanceDB;
GO

WITH TechnicianRequestCounts AS (
    SELECT
        t.Id AS TechnicianId,
        t.FullName,
        COUNT(mr.Id) AS RequestCount
    FROM Technicians t
    LEFT JOIN MaintenanceRequests mr ON mr.TechnicianId = t.Id
    GROUP BY t.Id, t.FullName
)
SELECT * FROM TechnicianRequestCounts
WHERE RequestCount > (SELECT AVG(CAST(RequestCount AS FLOAT)) FROM TechnicianRequestCounts);
GO
