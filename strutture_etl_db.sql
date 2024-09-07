CREATE TABLE IF NOT EXISTS Statistiche_Prenotazioni_Mensili (
    id_statistica SERIAL PRIMARY KEY,
    mese INT,
    anno INT,
    totale_prenotazioni_libri INT,
    totale_prenotazioni_sale INT,
    prenotazioni_libri_prenotati INT,
    prenotazioni_libri_ritirati INT,
    prenotazioni_libri_restituiti INT,
    prenotazioni_libri_cancellati INT,
    prenotazioni_sale_prenotate INT,
    prenotazioni_sale_usufruite INT,
    prenotazioni_sale_cancellate INT,
    libro_piu_prenotato VARCHAR(255),
    sala_piu_prenotata VARCHAR(255),
    creato_il TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

DELIMITER //

CREATE PROCEDURE InsertMonthlyStatistics()
BEGIN
    DECLARE m INT DEFAULT 1;
    DECLARE y INT DEFAULT YEAR(CURDATE());

    -- Delete existing statistics
    DELETE FROM Statistiche_Prenotazioni_Mensili;

    -- Step 1: Create a temporary table that holds all months of the year
    CREATE TEMPORARY TABLE IF NOT EXISTS MonthTable (
        mese INT
    );

    -- Step 2: Insert all months from 1 to 12 into the temporary table
    TRUNCATE MonthTable;  -- Clear the table before inserting data
    SET m = 1;
    WHILE m <= 12 DO
        INSERT INTO MonthTable (mese) VALUES (m);
        SET m = m + 1;
    END WHILE;

    -- Step 3: Insert statistics for each month using the month table as the base
    INSERT INTO Statistiche_Prenotazioni_Mensili (
        mese,
        anno,
        totale_prenotazioni_libri,
        totale_prenotazioni_sale,
        prenotazioni_libri_prenotati,
        prenotazioni_libri_ritirati,
        prenotazioni_libri_restituiti,
        prenotazioni_libri_cancellati,
        prenotazioni_sale_prenotate,
        prenotazioni_sale_usufruite,
        prenotazioni_sale_cancellate,
        libro_piu_prenotato,
        sala_piu_prenotata
    )
    SELECT 
        M.mese, 
        y AS anno,
        IFNULL(B.totale_prenotazioni_libri, 0) AS totale_prenotazioni_libri,
        IFNULL(R.totale_prenotazioni_sale, 0) AS totale_prenotazioni_sale,
        IFNULL(B.prenotazioni_libri_prenotati, 0) AS prenotazioni_libri_prenotati,
        IFNULL(B.prenotazioni_libri_ritirati, 0) AS prenotazioni_libri_ritirati,
        IFNULL(B.prenotazioni_libri_restituiti, 0) AS prenotazioni_libri_restituiti,
        IFNULL(B.prenotazioni_libri_cancellati, 0) AS prenotazioni_libri_cancellati,
        IFNULL(R.prenotazioni_sale_prenotate, 0) AS prenotazioni_sale_prenotate,
        IFNULL(R.prenotazioni_sale_usufruite, 0) AS prenotazioni_sale_usufruite,
        IFNULL(R.prenotazioni_sale_cancellate, 0) AS prenotazioni_sale_cancellate,
        IFNULL(B.libro_piu_prenotato, 'N/A') AS libro_piu_prenotato,
        IFNULL(R.sala_piu_prenotata, 'N/A') AS sala_piu_prenotata
    FROM MonthTable M
    LEFT JOIN (
        -- Statistiche prenotazioni libri per il mese corrente
        SELECT 
            MONTH(PL.data_prenotazione) AS mese,
            COUNT(*) AS totale_prenotazioni_libri,
            SUM(CASE WHEN PL.stato = 'Prenotato' THEN 1 ELSE 0 END) AS prenotazioni_libri_prenotati,
            SUM(CASE WHEN PL.stato = 'Ritirato' THEN 1 ELSE 0 END) AS prenotazioni_libri_ritirati,
            SUM(CASE WHEN PL.stato = 'Restituito' THEN 1 ELSE 0 END) AS prenotazioni_libri_restituiti,
            SUM(CASE WHEN PL.stato = 'Cancellato' THEN 1 ELSE 0 END) AS prenotazioni_libri_cancellati,
            MAX(L.titolo) AS libro_piu_prenotato
        FROM Prenotazioni_Libri PL
        LEFT JOIN Libri L ON PL.id_libro = L.id_libro
        WHERE YEAR(PL.data_prenotazione) = y
        GROUP BY mese
    ) AS B ON M.mese = B.mese
    LEFT JOIN (
        -- Statistiche prenotazioni sale basate su giorno_prenotato
        SELECT 
            MONTH(PS.giorno_prenotato) AS mese,
            COUNT(*) AS totale_prenotazioni_sale,
            SUM(CASE WHEN PS.stato = 'Prenotato' THEN 1 ELSE 0 END) AS prenotazioni_sale_prenotate,
            SUM(CASE WHEN PS.stato = 'Usufruito' THEN 1 ELSE 0 END) AS prenotazioni_sale_usufruite,
            SUM(CASE WHEN PS.stato = 'Cancellato' THEN 1 ELSE 0 END) AS prenotazioni_sale_cancellate,
            MAX(SS.nome_sala) AS sala_piu_prenotata
        FROM Prenotazioni_Sale PS
        LEFT JOIN Sale_Studio SS ON PS.id_sala = SS.id_sala
        WHERE YEAR(PS.giorno_prenotato) = y
        GROUP BY mese
    ) AS R ON M.mese = R.mese;

    -- Clean up the temporary table
    DROP TEMPORARY TABLE IF EXISTS MonthTable;
END //

DELIMITER ;

