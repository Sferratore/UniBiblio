/*
    ############################################################
    #                     ETL Process Overview                 #
    ############################################################
    
    Questo file SQL definisce un processo ETL (Extract, Transform, Load) per raccogliere statistiche mensili 
    sulle prenotazioni di libri e sale in una biblioteca. L'ETL è suddiviso in tre fasi principali:

    1. **Extract (Estrazione)**:
        - Estrazione dei dati dalle tabelle di prenotazioni, ovvero 'Prenotazioni_Libri' e 'Prenotazioni_Sale'.
        - Si estraggono i conteggi delle prenotazioni per i diversi stati dei libri: 'Prenotato', 'Ritirato', 
          'Restituito', 'Cancellato'.
        - Si estraggono anche i conteggi delle prenotazioni delle sale in base agli stati: 'Prenotato', 
          'Usufruito', 'Cancellato'.
        - I dati vengono raggruppati per mese e anno.

    2. **Transform (Trasformazione)**:
        - I dati estratti vengono trasformati per calcolare statistiche mensili come il numero totale di 
          prenotazioni di libri e sale, le prenotazioni attive, completate, o cancellate.
        - Utilizzando funzioni come 'IFNULL', gestiamo eventuali mancanze di dati per un determinato mese, 
          assicurando che tutte le statistiche siano valide.
        - Vengono calcolati anche i libri e le sale più prenotati.

    3. **Load (Caricamento)**:
        - Le statistiche vengono caricate nella tabella 'Statistiche_Prenotazioni_Mensili' per permettere stampa e download dell'excel.
    
    ############################################################
    #                       Business Rules                     #
    ############################################################
    
    - Un utente può prenotare un massimo di 3 libri che non sono ancora stati ritirati.
    - Un utente può effettuare una sola prenotazione di una sala studio alla volta.

    ############################################################
    #                 Dettagli della procedura SQL              #
    ############################################################

    - La procedura 'InsertMonthlyStatistics' cancella prima i dati esistenti dalla tabella delle statistiche mensili.
    - Viene creata una tabella temporanea per inserire tutti i mesi dell'anno corrente.
    - Per ogni mese, vengono calcolate le statistiche delle prenotazioni di libri e sale, che vengono poi
      inserite nella tabella delle statistiche.
    - Le prenotazioni di libri sono basate sulla data di prenotazione, mentre le prenotazioni delle sale sono
      basate sulla 'giorno_prenotato'.
*/

-- Crea la tabella che conterrà le statistiche mensili delle prenotazioni di libri e sale
CREATE TABLE IF NOT EXISTS Statistiche_Prenotazioni_Mensili (
    id_statistica SERIAL PRIMARY KEY,  -- Chiave primaria per identificare ogni record
    mese INT,  -- Il mese delle statistiche
    anno INT,  -- L'anno delle statistiche
    totale_prenotazioni_libri INT,  -- Totale delle prenotazioni di libri per il mese
    totale_prenotazioni_sale INT,  -- Totale delle prenotazioni di sale studio per il mese
    prenotazioni_libri_prenotati INT,  -- Libri ancora prenotati ma non ritirati
    prenotazioni_libri_ritirati INT,  -- Libri che sono stati ritirati
    prenotazioni_libri_restituiti INT,  -- Libri che sono stati restituiti
    prenotazioni_libri_cancellati INT,  -- Prenotazioni di libri cancellate
    prenotazioni_sale_prenotate INT,  -- Prenotazioni di sale studio ancora attive
    prenotazioni_sale_usufruite INT,  -- Sale studio che sono state utilizzate
    prenotazioni_sale_cancellate INT,  -- Prenotazioni di sale studio cancellate
    libro_piu_prenotato VARCHAR(255),  -- Il titolo del libro più prenotato per il mese
    sala_piu_prenotata VARCHAR(255),  -- Il nome della sala studio più prenotata per il mese
    creato_il TIMESTAMP DEFAULT CURRENT_TIMESTAMP  -- Data di creazione della statistica
);

DELIMITER //

-- Procedura per inserire le statistiche mensili combinate di prenotazioni libri e sale
CREATE PROCEDURE InsertMonthlyStatistics()
BEGIN
    DECLARE m INT DEFAULT 1;  -- Variabile per i mesi, inizia da gennaio (mese 1)
    DECLARE y INT DEFAULT YEAR(CURDATE());  -- Variabile per l'anno corrente

    -- Fase di Load: Cancella le statistiche esistenti dalla tabella Statistiche_Prenotazioni_Mensili
    DELETE FROM Statistiche_Prenotazioni_Mensili;

    -- Step 1: Creazione di una tabella temporanea per inserire tutti i mesi dell'anno
    CREATE TEMPORARY TABLE IF NOT EXISTS MonthTable (
        mese INT  -- Memorizza i mesi dell'anno
    );

    -- Step 2: Inserimento dei mesi da 1 a 12 nella tabella temporanea
    TRUNCATE MonthTable;  -- Pulisce la tabella temporanea prima di inserire i dati
    SET m = 1;
    WHILE m <= 12 DO
        INSERT INTO MonthTable (mese) VALUES (m);  -- Inserisce i mesi da gennaio a dicembre
        SET m = m + 1;
    END WHILE;

    -- Step 3: Estrazione e trasformazione dei dati e inserimento delle statistiche mensili
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
        IFNULL(B.totale_prenotazioni_libri, 0) AS totale_prenotazioni_libri,  -- Calcola il totale delle prenotazioni di libri
        IFNULL(R.totale_prenotazioni_sale, 0) AS totale_prenotazioni_sale,  -- Calcola il totale delle prenotazioni di sale
        IFNULL(B.prenotazioni_libri_prenotati, 0) AS prenotazioni_libri_prenotati,  -- Prenotazioni di libri attive
        IFNULL(B.prenotazioni_libri_ritirati, 0) AS prenotazioni_libri_ritirati,  -- Libri ritirati
        IFNULL(B.prenotazioni_libri_restituiti, 0) AS prenotazioni_libri_restituiti,  -- Libri restituiti
        IFNULL(B.prenotazioni_libri_cancellati, 0) AS prenotazioni_libri_cancellati,  -- Prenotazioni di libri cancellate
        IFNULL(R.prenotazioni_sale_prenotate, 0) AS prenotazioni_sale_prenotate,  -- Sale studio ancora prenotate
        IFNULL(R.prenotazioni_sale_usufruite, 0) AS prenotazioni_sale_usufruite,  -- Sale studio usufruite
        IFNULL(R.prenotazioni_sale_cancellate, 0) AS prenotazioni_sale_cancellate,  -- Sale studio cancellate
        IFNULL(B.libro_piu_prenotato, 'N/A') AS libro_piu_prenotato,  -- Libro più prenotato del mese
        IFNULL(R.sala_piu_prenotata, 'N/A') AS sala_piu_prenotata  -- Sala studio più prenotata del mese
    FROM MonthTable M
    LEFT JOIN (
        -- Statistiche delle prenotazioni libri per il mese corrente
        SELECT 
            MONTH(PL.data_prenotazione) AS mese,
            COUNT(*) AS totale_prenotazioni_libri,
            SUM(CASE WHEN PL.stato = 'Prenotato' THEN 1 ELSE 0 END) AS prenotazioni_libri_prenotati,
            SUM(CASE WHEN PL.stato = 'Ritirato' THEN 1 ELSE 0 END) AS prenotazioni_libri_ritirati,
            SUM(CASE WHEN PL.stato = 'Restituito' THEN 1 ELSE 0 END) AS prenotazioni_libri_restituiti,
            SUM(CASE WHEN PL.stato = 'Cancellato' THEN 1 ELSE 0 END) AS prenotazioni_libri_cancellati,
            MAX(L.titolo) AS libro_piu_prenotato  -- Trova il libro più prenotato del mese
        FROM Prenotazioni_Libri PL
        LEFT JOIN Libri L ON PL.id_libro = L.id_libro
        WHERE YEAR(PL.data_prenotazione) = y
        GROUP BY mese
    ) AS B ON M.mese = B.mese
    LEFT JOIN (
        -- Statistiche delle prenotazioni sale basate su giorno_prenotato
        SELECT 
            MONTH(PS.giorno_prenotato) AS mese,
            COUNT(*) AS totale_prenotazioni_sale,
            SUM(CASE WHEN PS.stato = 'Prenotato' THEN 1 ELSE 0 END) AS prenotazioni_sale_prenotate,
            SUM(CASE WHEN PS.stato = 'Usufruito' THEN 1 ELSE 0 END) AS prenotazioni_sale_usufruite,
            SUM(CASE WHEN PS.stato = 'Cancellato' THEN 1 ELSE 0 END) AS prenotazioni_sale_cancellate,
            MAX(SS.nome_sala) AS sala_piu_prenotata  -- Trova la sala più prenotata del mese
        FROM Prenotazioni_Sale PS
        LEFT JOIN Sale_Studio SS ON PS.id_sala = SS.id_sala
        WHERE YEAR(PS.giorno_prenotato) = y
        GROUP BY mese
    ) AS R ON M.mese = R.mese;

    -- Step 4: Pulizia della tabella temporanea
    DROP TEMPORARY TABLE IF EXISTS MonthTable;
END //

DELIMITER ;
