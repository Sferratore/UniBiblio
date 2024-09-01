DELIMITER //

CREATE PROCEDURE GetAvailableSeatsByDate(IN ReservationDate DATE)
BEGIN
    SELECT
        SS.id_sala AS IdSala,
        SS.nome_sala AS NomeSala,
        B.nome_biblioteca AS Biblioteca,
        SS.capienza as Capienza,
        SS.disponibilita as Disponibilita,
        B.indirizzo AS IndirizzoBiblioteca,
        (SS.capienza - IFNULL(PS.prenotazioni_attive, 0)) AS PostiDisponibili
    FROM
        Sale_Studio SS
    JOIN
        Biblioteche B ON SS.id_biblioteca = B.id_biblioteca
    LEFT JOIN (
        SELECT
            id_sala,
            COUNT(*) AS prenotazioni_attive
        FROM
            Prenotazioni_Sale
        WHERE
            stato = 'Confermato' AND DATE(data_prenotazione) = ReservationDate
        GROUP BY
            id_sala
    ) PS ON SS.id_sala = PS.id_sala
    WHERE Disponibilita = 1;
END //

DELIMITER ;