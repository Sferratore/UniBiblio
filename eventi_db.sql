SHOW VARIABLES LIKE 'event_scheduler';
SET GLOBAL event_scheduler = ON;
SHOW EVENTS;
 
DELIMITER //  -- Vi è un curioso bug col delimiter, devi eseguirlo solo col delimiter superiore anche se va scritto con tutti e due.

CREATE EVENT IF NOT EXISTS `AutoDeleteBookReservations`
ON SCHEDULE EVERY 1 DAY STARTS CURRENT_TIMESTAMP
DO 
BEGIN
    DELETE FROM Prenotazioni_Libri
    WHERE DATEDIFF(CURRENT_DATE, data_prenotazione) > 7 AND data_ritiro IS NULL;
END;

DELIMITER ;


DELIMITER //

CREATE EVENT IF NOT EXISTS `AutoDeleteRoomReservations`
ON SCHEDULE EVERY 1 DAY STARTS CURRENT_TIMESTAMP
DO 
BEGIN
    DELETE FROM Prenotazioni_Sale
    WHERE giorno_prenotato < CURRENT_DATE;
END;

DELIMITER ;