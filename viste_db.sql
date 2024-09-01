CREATE VIEW PrenotaLibriView AS
SELECT 
	L.id_libro, 
    L.titolo, 
    L.autore, 
    L.anno_pubblicazione, 
    L.isbn, 
    L.quantita_disponibile, 
    L.categoria, 
    B.nome_biblioteca AS biblioteca
FROM libri L 
INNER JOIN Biblioteche B ON (L.id_biblioteca = B.id_biblioteca);

CREATE VIEW PrenotazioniEffettuateLibriView AS
SELECT 
	PL.id_prenotazione, 
    PL.id_utente, 
    PL.id_libro, U.email AS email_utente , 
    L.titolo AS titolo_libro, 
    L.isbn, 
    PL.data_prenotazione, 
    PL.data_ritiro, 
    PL.stato
FROM Prenotazioni_Libri PL 
INNER JOIN Utenti U ON (PL.id_utente = U.id_utente) 
INNER JOIN Libri L ON (PL.id_libro = L.id_libro);

CREATE VIEW PrenotaSaleView AS
SELECT
    SS.id_sala,
    SS.nome_sala,
    B.nome_biblioteca AS biblioteca,
    SS.capienza,
    SS.disponibilita,
    B.indirizzo AS indirizzo_biblioteca,
    (SS.capienza - COALESCE(PS.prenotazioni_attive, 0)) AS posti_disponibili
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
        stato = 'Confermato'
    GROUP BY
        id_sala
) PS ON SS.id_sala = PS.id_sala;

CREATE VIEW PrenotazioniEffettuateSaleView AS
SELECT
    PS.id_prenotazione,
    PS.id_utente,
    U.email AS email_utente,
    SS.nome_sala AS nome_sala,
    PS.data_prenotazione,
    PS.giorno_prenotato,
    PS.ora_inizio,
    PS.ora_fine,
    PS.stato,
    B.nome_biblioteca AS biblioteca
FROM Prenotazioni_Sale PS
INNER JOIN Utenti U ON PS.id_utente = U.id_utente
INNER JOIN Sale_Studio SS ON PS.id_sala = SS.id_sala
INNER JOIN Biblioteche B ON SS.id_biblioteca = B.id_biblioteca