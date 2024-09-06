CREATE TABLE IF NOT EXISTS Statistiche_Prenotazioni_Mensili (
    id_statistica SERIAL PRIMARY KEY,
    mese INT,
    anno INT,
    totale_prenotazioni_libri INT,
    totale_prenotazioni_sale INT,
    prenotazioni_libri_completate INT,
    prenotazioni_libri_attive INT,
    prenotazioni_sale_confermate INT,
    libro_piu_prenotato VARCHAR(255),
    sala_piu_prenotata VARCHAR(255),
    creato_il TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


-- Inserisci le statistiche mensili combinate per libri e sale nella nuova tabella
INSERT INTO Statistiche_Prenotazioni_Mensili (
    mese,
    anno,
    totale_prenotazioni_libri,
    totale_prenotazioni_sale,
    prenotazioni_libri_completate,
    prenotazioni_libri_attive,
    prenotazioni_sale_confermate,
    libro_piu_prenotato,
    sala_piu_prenotata
)
SELECT 
    B.mese, 
    B.anno,
    B.totale_prenotazioni_libri,
    R.totale_prenotazioni_sale,
    B.prenotazioni_libri_completate,
    B.prenotazioni_libri_attive,
    R.prenotazioni_sale_confermate,
    B.libro_piu_prenotato,
    R.sala_piu_prenotata
FROM (
    -- Statistiche prenotazioni libri
    SELECT 
        MONTH(PL.data_prenotazione) AS mese,
        YEAR(PL.data_prenotazione) AS anno,
        COUNT(*) AS totale_prenotazioni_libri,
        SUM(CASE WHEN PL.stato = 'Completato' THEN 1 ELSE 0 END) AS prenotazioni_libri_completate,
        SUM(CASE WHEN PL.stato = 'Prenotato' THEN 1 ELSE 0 END) AS prenotazioni_libri_attive,
        MAX(L.titolo) AS libro_piu_prenotato
    FROM Prenotazioni_Libri PL
    INNER JOIN Libri L ON PL.id_libro = L.id_libro
    WHERE YEAR(PL.data_prenotazione) = YEAR(CURDATE())
    GROUP BY mese, anno
) AS B
JOIN (
    -- Statistiche prenotazioni sale
    SELECT 
        MONTH(PS.data_prenotazione) AS mese,
        YEAR(PS.data_prenotazione) AS anno,
        COUNT(*) AS totale_prenotazioni_sale,
        SUM(CASE WHEN PS.stato = 'Confermato' THEN 1 ELSE 0 END) AS prenotazioni_sale_confermate,
        MAX(SS.nome_sala) AS sala_piu_prenotata
    FROM Prenotazioni_Sale PS
    INNER JOIN Sale_Studio SS ON PS.id_sala = SS.id_sala
    WHERE YEAR(PS.data_prenotazione) = YEAR(CURDATE())
    GROUP BY mese, anno
) AS R
ON B.mese = R.mese AND B.anno = R.anno;




DELIMITER //

CREATE PROCEDURE InsertMonthlyStatistics()
BEGIN
	DELETE FROM Statistiche_Prenotazioni_Mensili;
    -- Inserisci le statistiche mensili combinate per libri e sale nella nuova tabella
    INSERT INTO Statistiche_Prenotazioni_Mensili (
        mese,
        anno,
        totale_prenotazioni_libri,
        totale_prenotazioni_sale,
        prenotazioni_libri_completate,
        prenotazioni_libri_attive,
        prenotazioni_sale_confermate,
        libro_piu_prenotato,
        sala_piu_prenotata
    )
    SELECT 
        B.mese, 
        B.anno,
        B.totale_prenotazioni_libri,
        R.totale_prenotazioni_sale,
        B.prenotazioni_libri_completate,
        B.prenotazioni_libri_attive,
        R.prenotazioni_sale_confermate,
        B.libro_piu_prenotato,
        R.sala_piu_prenotata
    FROM (
        -- Statistiche prenotazioni libri
        SELECT 
            MONTH(PL.data_prenotazione) AS mese,
            YEAR(PL.data_prenotazione) AS anno,
            COUNT(*) AS totale_prenotazioni_libri,
            SUM(CASE WHEN PL.stato = 'Completato' THEN 1 ELSE 0 END) AS prenotazioni_libri_completate,
            SUM(CASE WHEN PL.stato = 'Prenotato' THEN 1 ELSE 0 END) AS prenotazioni_libri_attive,
            MAX(L.titolo) AS libro_piu_prenotato
        FROM Prenotazioni_Libri PL
        INNER JOIN Libri L ON PL.id_libro = L.id_libro
        WHERE YEAR(PL.data_prenotazione) = YEAR(CURDATE())
        GROUP BY mese, anno
    ) AS B
    JOIN (
        -- Statistiche prenotazioni sale
        SELECT 
            MONTH(PS.data_prenotazione) AS mese,
            YEAR(PS.data_prenotazione) AS anno,
            COUNT(*) AS totale_prenotazioni_sale,
            SUM(CASE WHEN PS.stato = 'Confermato' THEN 1 ELSE 0 END) AS prenotazioni_sale_confermate,
            MAX(SS.nome_sala) AS sala_piu_prenotata
        FROM Prenotazioni_Sale PS
        INNER JOIN Sale_Studio SS ON PS.id_sala = SS.id_sala
        WHERE YEAR(PS.data_prenotazione) = YEAR(CURDATE())
        GROUP BY mese, anno
    ) AS R
    ON B.mese = R.mese AND B.anno = R.anno;
END //

DELIMITER ;