-- Tüm randevuların AppointmentEndDate'ini düzelt
-- Buffer süresi için WorkingHour'dan BufferDuration al

UPDATE A
SET A.AppointmentEndDate = DATEADD(MINUTE, 
    CAST(A.Duration AS INT) + ISNULL(W.BufferDuration, 10), 
    A.AppointmentDate)
FROM Appointments A
LEFT JOIN WorkingHours W ON W.PsychologistId = A.PsychologistId 
    AND W.DayOfWeek = CASE DATEPART(WEEKDAY, A.AppointmentDate)
        WHEN 1 THEN 7  -- Sunday
        WHEN 2 THEN 1  -- Monday
        WHEN 3 THEN 2  -- Tuesday
        WHEN 4 THEN 3  -- Wednesday
        WHEN 5 THEN 4  -- Thursday
        WHEN 6 THEN 5  -- Friday
        WHEN 7 THEN 6  -- Saturday
    END
WHERE A.IsDeleted = 0
  AND A.Status != 3; -- Cancelled olmayan randevular

-- Kontrol için
SELECT 
    A.Id,
    A.AppointmentDate,
    A.Duration,
    ISNULL(W.BufferDuration, 10) as BufferDuration,
    A.AppointmentEndDate,
    DATEADD(MINUTE, CAST(A.Duration AS INT) + ISNULL(W.BufferDuration, 10), A.AppointmentDate) as CalculatedEndDate
FROM Appointments A
LEFT JOIN WorkingHours W ON W.PsychologistId = A.PsychologistId 
    AND W.DayOfWeek = CASE DATEPART(WEEKDAY, A.AppointmentDate)
        WHEN 1 THEN 7  -- Sunday
        WHEN 2 THEN 1  -- Monday
        WHEN 3 THEN 2  -- Tuesday
        WHEN 4 THEN 3  -- Wednesday
        WHEN 5 THEN 4  -- Thursday
        WHEN 6 THEN 5  -- Friday
        WHEN 7 THEN 6  -- Saturday
    END
WHERE A.IsDeleted = 0
ORDER BY A.AppointmentDate DESC;
