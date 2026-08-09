-- TICKET #038 — Technician Leaderboard (Window Function: ROW_NUMBER / RANK / DENSE_RANK)
-- Status stored as int-backed enum: Pending=0, InProgress=1, Completed=2.
-- Verified 2026-08-09: all technicians tied at 0 completed requests, which cleanly
-- demonstrates the tie-break difference between the three ranking functions
-- (ROW_NUMBER -> 1,2,3 / RANK -> 1,1,3 / DENSE_RANK -> 1,1,2).

USE YokohamaMaintenanceDB;
GO

WITH TechnicianCompletedCounts AS (
    SELECT
        t.Id,
        t.FullName,
        COUNT(mr.Id) AS CompletedCount
    FROM Technicians t
    LEFT JOIN MaintenanceRequests mr
        ON mr.TechnicianId = t.Id AND mr.Status = 2
    GROUP BY t.Id, t.FullName
)
SELECT
    FullName,
    CompletedCount,
    ROW_NUMBER() OVER (ORDER BY CompletedCount DESC) AS Rank,
    RANK() OVER (ORDER BY CompletedCount DESC) AS RankSkip,
    DENSE_RANK() OVER (ORDER BY CompletedCount DESC) AS RankDense
FROM TechnicianCompletedCounts;
GO
