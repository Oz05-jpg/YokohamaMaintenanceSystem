-- TICKET #041 — PIVOT: Machine Request Status Report
-- Reshapes long-format (one row per request) into wide-format (one row per
-- Machine, Status values as columns) — same idea as an Excel Pivot Table.
--
-- Bonus round: extended to LEFT JOIN Machines so machines with zero requests
-- still appear (same lesson as TICKET #037/038/039). Current data happens to
-- have exactly 5 machines == 5 machines with requests, so the LEFT JOIN isn't
-- visibly different in this dataset (same situation as TICKET #039's
-- PARTITION BY demo) — but it's still the correct guard for future data.
--
-- Debug notes 2026-08-12:
--   - `Machines` also has a column literally named `Status` (Running/Stopped/
--     Under Maintenance) distinct from `MaintenanceRequests.Status` (the
--     RequestStatus enum) — unqualified `Status` after the JOIN throws
--     "Ambiguous column name" (a LOUD bug). Must qualify as `mr.Status`.
--   - Unqualified `MachineId` does NOT error (only MaintenanceRequests has
--     that literal column name) but silently resolves to `mr.MachineId`,
--     which is NULL for unmatched LEFT JOIN rows — a QUIET bug. Must use
--     `m.Id AS MachineId` explicitly instead.

USE YokohamaMaintenanceDB;
GO

SELECT MachineId, MachineName, [Pending], [InProgress], [Completed]
FROM (
    SELECT
        m.Id AS MachineId,
        m.Name AS MachineName,
        CASE mr.Status
            WHEN 0 THEN 'Pending'
            WHEN 1 THEN 'InProgress'
            WHEN 2 THEN 'Completed'
        END AS StatusLabel
    FROM Machines m
    LEFT JOIN MaintenanceRequests mr ON mr.MachineId = m.Id
) AS SourceData
PIVOT (
    COUNT(StatusLabel)
    FOR StatusLabel IN ([Pending], [InProgress], [Completed])
) AS PivotTable;
GO
