-- Vista che mostra l'elenco dei libri disponibili con i relativi dettagli e la biblioteca in cui si trovano
CREATE VIEW PrenotaLibriView AS
SELECT 
	L.id_libro,  -- ID univoco del libro
    L.titolo,  -- Titolo del libro
    L.autore,  -- Autore del libro
    L.anno_pubblicazione,  -- Anno di pubblicazione del libro
    L.isbn,  -- Codice ISBN del libro
    L.quantita_disponibile,  -- Quantità disponibile del libro nella biblioteca
    L.categoria,  -- Categoria del libro
    B.nome_biblioteca AS biblioteca  -- Nome della biblioteca dove il libro è disponibile
FROM libri L 
INNER JOIN Biblioteche B ON (L.id_biblioteca = B.id_biblioteca);  -- Associa i libri con le biblioteche


-- Vista che mostra le prenotazioni di libri effettuate dagli utenti
CREATE VIEW PrenotazioniEffettuateLibriView AS
SELECT 
    PL.id_prenotazione,  -- ID univoco della prenotazione del libro
    PL.id_utente,  -- ID dell'utente che ha effettuato la prenotazione
    PL.id_libro,  -- ID del libro prenotato
    U.email AS email_utente,  -- Email dell'utente che ha effettuato la prenotazione
    L.titolo AS titolo_libro,  -- Titolo del libro prenotato
    L.isbn,  -- ISBN del libro prenotato
    B.nome_biblioteca,  -- Nome della biblioteca in cui il libro è disponibile
    PL.data_prenotazione,  -- Data in cui è stata effettuata la prenotazione
    PL.data_ritiro,  -- Data di ritiro del libro (se applicabile)
    PL.stato  -- Stato attuale della prenotazione
FROM Prenotazioni_Libri PL 
INNER JOIN Utenti U ON (PL.id_utente = U.id_utente)  -- Unisce le prenotazioni con gli utenti
INNER JOIN Libri L ON (PL.id_libro = L.id_libro)  -- Unisce le prenotazioni con i libri
INNER JOIN Biblioteche B ON (L.id_biblioteca = B.id_biblioteca)  -- Unisce i libri con le biblioteche
WHERE PL.stato NOT IN ('Restituito', 'Cancellato');  -- Filtra le prenotazioni attive (esclude le prenotazioni restituite o cancellate)


-- Vista che mostra le prenotazioni di sale studio effettuate dagli utenti
CREATE VIEW PrenotazioniEffettuateSaleView AS
SELECT
    PS.id_prenotazione,  -- ID univoco della prenotazione della sala studio
    PS.id_utente,  -- ID dell'utente che ha effettuato la prenotazione
    U.email AS email_utente,  -- Email dell'utente che ha effettuato la prenotazione
    SS.nome_sala AS nome_sala,  -- Nome della sala studio prenotata
    PS.data_prenotazione,  -- Data in cui è stata effettuata la prenotazione
    PS.giorno_prenotato,  -- Giorno per il quale la sala studio è stata prenotata
    PS.ora_inizio,  -- Ora di inizio della prenotazione
    PS.ora_fine,  -- Ora di fine della prenotazione
    PS.stato,  -- Stato attuale della prenotazione
    B.nome_biblioteca AS biblioteca  -- Nome della biblioteca in cui si trova la sala studio
FROM Prenotazioni_Sale PS
INNER JOIN Utenti U ON PS.id_utente = U.id_utente  -- Unisce le prenotazioni con gli utenti
INNER JOIN Sale_Studio SS ON PS.id_sala = SS.id_sala  -- Unisce le prenotazioni con le sale studio
INNER JOIN Biblioteche B ON SS.id_biblioteca = B.id_biblioteca  -- Unisce le sale studio con le biblioteche
WHERE PS.stato NOT IN ('Cancellato', 'Usufruito');  -- Filtra le prenotazioni attive (esclude quelle cancellate o già usufruite)
