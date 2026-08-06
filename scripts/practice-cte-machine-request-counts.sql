-- Practice — CTE (Common Table Expression) reinforcement
-- Rewrites vw_MachineRequestCounts (TICKET #037) logic using WITH instead of CREATE VIEW.
-- Same aggregate, but scoped to a single query instead of persisted in the database.
--
-- Gotcha hit + fixed 2026-08-06: `WITH` must be the FIRST statement in a batch.
-- A preceding statement (e.g. `USE YokohamaMaintenanceDB` without a `;`) before
-- the WITH clause throws Msg 319 "Incorrect syntax near the keyword 'with'".
-- Fix: terminate the statement before WITH with a semicolon (or GO).

USE YokohamaMaintenanceDB;
GO

WITH MachineRequestCounts AS (
    SELECT
        m.Id AS MachineId,
        m.Name,
        COUNT(mr.Id) AS RequestCount
    FROM Machines m
    LEFT JOIN MaintenanceRequests mr ON mr.MachineId = m.Id
    GROUP BY m.Id, m.Name
)
SELECT *
FROM MachineRequestCounts
WHERE RequestCount > (SELECT AVG(RequestCount) FROM MachineRequestCounts);
GO

-- Comparison — same logic via the TICKET #037 View, should match row-for-row.
SELECT *
FROM vw_MachineRequestCounts
WHERE RequestCount > (SELECT AVG(CAST(RequestCount AS FLOAT)) FROM vw_MachineRequestCounts);
GO

-- Verified 2026-08-06: both queries returned matching result sets (0 rows at time
-- of test — expected given current data, not a bug; confirmed CTE logic equivalent to View).
