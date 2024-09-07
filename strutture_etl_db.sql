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
    DELETE FROM Statistiche_Prenotazioni_Mensili;
    
    -- Inserisci le statistiche mensili combinate per libri e sale nella nuova tabella
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
        B.mese, 
        B.anno,
        B.totale_prenotazioni_libri,
        R.totale_prenotazioni_sale,
        B.prenotazioni_libri_prenotati,
        B.prenotazioni_libri_ritirati,
        B.prenotazioni_libri_restituiti,
        B.prenotazioni_libri_cancellati,
        R.prenotazioni_sale_prenotate,
        R.prenotazioni_sale_usufruite,
        R.prenotazioni_sale_cancellate,
        B.libro_piu_prenotato,
        R.sala_piu_prenotata
    FROM (
        -- Statistiche prenotazioni libri
        SELECT 
            MONTH(PL.data_prenotazione) AS mese,
            YEAR(PL.data_prenotazione) AS anno,
            COUNT(*) AS totale_prenotazioni_libri,
            SUM(CASE WHEN PL.stato = 'Prenotato' THEN 1 ELSE 0 END) AS prenotazioni_libri_prenotati,
            SUM(CASE WHEN PL.stato = 'Ritirato' THEN 1 ELSE 0 END) AS prenotazioni_libri_ritirati,
            SUM(CASE WHEN PL.stato = 'Restituito' THEN 1 ELSE 0 END) AS prenotazioni_libri_restituiti,
            SUM(CASE WHEN PL.stato = 'Cancellato' THEN 1 ELSE 0 END) AS prenotazioni_libri_cancellati,
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
            SUM(CASE WHEN PS.stato = 'Prenotato' THEN 1 ELSE 0 END) AS prenotazioni_sale_prenotate,
            SUM(CASE WHEN PS.stato = 'Usufruito' THEN 1 ELSE 0 END) AS prenotazioni_sale_usufruite,
            SUM(CASE WHEN PS.stato = 'Cancellato' THEN 1 ELSE 0 END) AS prenotazioni_sale_cancellate,
            MAX(SS.nome_sala) AS sala_piu_prenotata
        FROM Prenotazioni_Sale PS
        INNER JOIN Sale_Studio SS ON PS.id_sala = SS.id_sala
        WHERE YEAR(PS.data_prenotazione) = YEAR(CURDATE())
        GROUP BY mese, anno
    ) AS R
    ON B.mese = R.mese AND B.anno = R.anno;
END //

DELIMITER ;