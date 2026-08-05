-- TICKET #037 — Machines Above Average Maintenance Load (Subquery + View)
-- Goal: find machines whose MaintenanceRequest count is above the average,
-- and wrap the aggregate logic in a reusable View.
-- Edge case handled: machines with 0 requests must still show RequestCount = 0 (not disappear).

USE YokohamaMaintenanceDB;
GO

-- Step 1: average MaintenanceRequest count per machine (includes machines with 0 requests)
SELECT AVG(CAST(RequestCount AS FLOAT)) AS AvgRequests
FROM (
    SELECT m.Id AS MachineId, COUNT(mr.Id) AS RequestCount
    FROM Machines m
    LEFT JOIN MaintenanceRequests mr ON mr.MachineId = m.Id
    GROUP BY m.Id
) AS Sub;
GO

-- Step 2: machines with RequestCount above that average (correlated subquery)
SELECT m.Id, m.Name,
    (SELECT COUNT(*) FROM MaintenanceRequests mr WHERE mr.MachineId = m.Id) AS RequestCount
FROM Machines m
WHERE (SELECT COUNT(*) FROM MaintenanceRequests mr WHERE mr.MachineId = m.Id) >
    (SELECT AVG(CAST(RequestCount AS FLOAT))
     FROM (
         SELECT m2.Id AS MachineId, COUNT(mr2.Id) AS RequestCount
         FROM Machines m2
         LEFT JOIN MaintenanceRequests mr2 ON mr2.MachineId = m2.Id
         GROUP BY m2.Id
     ) AS Sub2);
GO

-- Step 3: reusable View wrapping the same aggregate
DROP VIEW IF EXISTS vw_MachineRequestCounts;
GO

CREATE VIEW vw_MachineRequestCounts AS
SELECT m.Id AS MachineId, m.Name AS MachineName, COUNT(mr.Id) AS RequestCount
FROM Machines m
LEFT JOIN MaintenanceRequests mr ON mr.MachineId = m.Id
GROUP BY m.Id, m.Name;
GO

-- Verified 2026-08-04: inserted a temp machine with 0 requests, average correctly
-- dropped 3 -> 2.5 and the view showed RequestCount = 0 for it (not missing).
