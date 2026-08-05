-- Practice — Technicians Above Average Assigned Workload (Subquery reinforcement)
-- Reinforcement exercise for TICKET #037 (Correlated + Scalar Subquery)
-- Verified 2026-08-05: assigned 3 requests to Somchai Jaidee -> only Somchai
-- returned as above-average (3 vs avg 1.0 across 3 technicians)

USE YokohamaMaintenanceDB;
GO

SELECT t.Id, t.FullName,
    (SELECT COUNT(*) FROM MaintenanceRequests mr WHERE mr.TechnicianId = t.Id) AS RequestCount
FROM Technicians t
WHERE (SELECT COUNT(*) FROM MaintenanceRequests mr WHERE mr.TechnicianId = t.Id)
    >
    (SELECT AVG(CAST(RequestCount AS FLOAT))
     FROM (
         SELECT t2.Id AS TechnicianId, COUNT(mr2.Id) AS RequestCount
         FROM Technicians t2
         LEFT JOIN MaintenanceRequests mr2 ON mr2.TechnicianId = t2.Id
         GROUP BY t2.Id
     ) AS Sub2);
GO
