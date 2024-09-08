DELIMITER //
-- La procedura restituisce record riguardanti le sale disponibili con un numero di posti disponibili calcolato in base a seconda delle prenotazioni già esistenti.
CREATE PROCEDURE GetAvailableSeatsByDate(IN ReservationDate DATE)
BEGIN
    -- Seleziona le sale studio disponibili in base alla data di prenotazione fornita
    SELECT
        SS.id_sala AS IdSala,  -- ID della sala
        SS.nome_sala AS NomeSala,  -- Nome della sala
        B.nome_biblioteca AS Biblioteca,  -- Nome della biblioteca in cui si trova la sala
        SS.capienza as Capienza,  -- Capacità totale della sala
        SS.disponibilita as Disponibilita,  -- Disponibilità della sala (1 = disponibile)
        B.indirizzo AS IndirizzoBiblioteca,  -- Indirizzo della biblioteca
        (SS.capienza - IFNULL(PS.prenotazioni_attive, 0)) AS PostiDisponibili  -- Calcola i posti disponibili sottraendo le prenotazioni attive
    FROM
        Sale_Studio SS  -- La tabella delle sale studio
    JOIN
        Biblioteche B ON SS.id_biblioteca = B.id_biblioteca  -- Unisci le sale con le biblioteche
    LEFT JOIN (
        -- Sottoquery per ottenere il numero di prenotazioni attive per ciascuna sala nella data specificata
        SELECT
            id_sala,
            COUNT(*) AS prenotazioni_attive  -- Conta le prenotazioni attive (stato 'Prenotato') per sala
        FROM
            Prenotazioni_Sale
        WHERE
            stato = 'Prenotato' AND DATE(giorno_prenotato) = ReservationDate  -- Filtra per prenotazioni attive nella data specificata
        GROUP BY
            id_sala
    ) PS ON SS.id_sala = PS.id_sala  -- Unisci con la tabella delle sale studio
    WHERE SS.disponibilita = 1  -- Considera solo le sale che sono disponibili
    HAVING PostiDisponibili > 0;  -- Mostra solo le sale che hanno posti disponibili
END //

DELIMITER ;