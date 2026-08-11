-- TICKET #040 — T-SQL TRY/CATCH + TRANSACTION (safe insert with rollback)
-- Bridges from C# try/catch (2026-06-03) — same idea, different shape:
-- BEGIN TRY/END TRY + BEGIN CATCH/END CATCH instead of { }, and CATCH always
-- catches everything (no typed exceptions like C#) — inspect ERROR_NUMBER()/
-- ERROR_MESSAGE() inside CATCH to find out what actually broke.
--
-- Verified 2026-08-11:
--   - Fake MachineId (9999, no such row in Machines) -> Error 547 FK violation,
--     caught cleanly, ROLLBACK confirmed with 0 leftover rows.
--   - Real MachineId (363) -> no error, COMMIT confirmed with a real persisted row.
--   - Also hit + fixed along the way: Error 515 NOT NULL on Title/Description/
--     Priority/CreatedAt (MaintenanceRequests requires all of these; CreatedAt's
--     `= DateTime.Now` C# default is NOT a DB-level default, so raw SQL bypassing
--     EF Core must supply it explicitly via GETDATE()).

USE YokohamaMaintenanceDB;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO MaintenanceRequests (MachineId, TechnicianId, Status, Title, Description, Priority, CreatedAt)
    VALUES (363, 1, 0, 'Test Ticket', 'Verify COMMIT path - valid MachineId', 'Medium', GETDATE());
    -- Swap MachineId to a non-existent Id (e.g. 9999) to trigger the FK-violation / ROLLBACK path instead.

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
    ROLLBACK TRANSACTION;
END CATCH
GO
