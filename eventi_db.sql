CREATE EVENT IF NOT EXISTS update_sale_status_usufruito
ON SCHEDULE EVERY 1 DAY
STARTS CURRENT_TIMESTAMP
DO
    UPDATE Prenotazioni_Sale
    SET stato = 'Usufruito'
    WHERE stato = 'Prenotato'  -- Only update records that are still in 'Prenotato' state
    AND CURDATE() > giorno_prenotato;