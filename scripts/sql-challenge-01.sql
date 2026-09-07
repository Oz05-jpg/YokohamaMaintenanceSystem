-- SQL Challenge #1 (2026-09-07)
-- Requirement: หา Technician ที่รับงาน (MaintenanceRequest) มากกว่าค่าเฉลี่ยจำนวนงานของทั้งทีม
-- ครั้งแรกของ SQL/Algorithm Challenge track — blank-page, ไม่มี skeleton ให้ตั้งแต่ต้น
-- Spaced repetition: ทำซ้ำไม่ดูเฉลยนี้ที่ 10 ก.ย. / 14 ก.ย. / 21 ก.ย. 2026

USE YokohamaMaintenanceDB;
GO

-- Main query
SELECT t.FullName, COUNT(m.TechnicianId) AS JobCount
FROM Technicians t
LEFT JOIN MaintenanceRequests m ON t.Id = m.TechnicianId
GROUP BY t.FullName
HAVING COUNT(m.TechnicianId) > (
    SELECT AVG(sub.JobCount)
    FROM (
        SELECT COUNT(m2.TechnicianId) AS JobCount
        FROM Technicians t2
        LEFT JOIN MaintenanceRequests m2 ON t2.Id = m2.TechnicianId
        GROUP BY t2.FullName
    ) AS sub
);

-- Test data used to verify (ปรับ Id range ตามข้อมูลจริงในเครื่องก่อนรัน — เช็คด้วย
-- SELECT Id FROM MaintenanceRequests ORDER BY Id; ก่อนเสมอ อย่าสมมติ range เอง)
-- ตัวอย่างที่ใช้ตอน verify (Id จริงตอนนั้นคือ 1222-1236):
-- UPDATE MaintenanceRequests SET TechnicianId = 1 WHERE Id BETWEEN 1222 AND 1231; -- 10 งาน
-- UPDATE MaintenanceRequests SET TechnicianId = 2 WHERE Id BETWEEN 1232 AND 1234; -- 3 งาน
-- UPDATE MaintenanceRequests SET TechnicianId = 3 WHERE Id = 1235;                -- 1 งาน
-- ผลที่ควรได้: เฉลี่ย = 14/3 ≈ 4.67 -> มีแค่ Technician ที่มี 10 งานโผล่มา
