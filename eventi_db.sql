CREATE EVENT IF NOT EXISTS update_sale_status_usufruito
ON SCHEDULE EVERY 1 DAY
STARTS CURRENT_TIMESTAMP
DO
    -- Aggiorna lo stato delle prenotazioni delle sale ogni giorno
    UPDATE Prenotazioni_Sale
    SET stato = 'Usufruito'
    WHERE stato = 'Prenotato'  -- Aggiorna solo le prenotazioni che sono ancora nello stato 'Prenotato'
    AND CURDATE() > giorno_prenotato;  -- Verifica se la data corrente è maggiore del giorno prenotato