-- TICKET #039 — Window Function: PARTITION BY (rank Technicians within their own Specialization)
-- Builds on TICKET #038 (ROW_NUMBER/RANK/DENSE_RANK) — adds PARTITION BY to reset ranking per group.
-- Verified 2026-08-10: with PARTITION BY, all 3 (distinct Specialization each) rank 1,1,1.
-- Without PARTITION BY (tested by commenting it out), same tied data ranks 1,2,3 sequentially
-- across the whole table — proves PARTITION BY controls grouping, ORDER BY controls sort order.

USE YokohamaMaintenanceDB;
GO

WITH TechnicianCompletedCounts AS (
    SELECT
        t.Id,
        t.FullName,
        t.Specialization,
        COUNT(mr.Id) AS CompletedCount
    FROM Technicians t
    LEFT JOIN MaintenanceRequests mr
        ON mr.TechnicianId = t.Id AND mr.Status = 2
    GROUP BY t.Id, t.FullName, t.Specialization
)
SELECT
    FullName,
    Specialization,
    CompletedCount,
    ROW_NUMBER() OVER (PARTITION BY Specialization ORDER BY CompletedCount DESC) AS RankInGroup
FROM TechnicianCompletedCounts;
GO
