/*
    ############################################################
    #                     Database Overview                    #
    ############################################################

    Questo file SQL definisce la struttura di un database per un sistema di gestione bibliotecario.
    Il sistema permette la gestione delle prenotazioni di libri e sale studio, la gestione degli utenti, 
    e il tracciamento delle categorie di libri. 
    
	############################################################
    #                       Business Rules                     #
    ############################################################
    
    - Un utente può prenotare un massimo di 3 libri che non sono ancora stati ritirati.
    - Un utente può effettuare una sola prenotazione di una sala studio alla volta.

*/

-- Tabella utenti per memorizzare le informazioni sugli utenti della biblioteca
CREATE TABLE Utenti (
    id_utente BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,  -- Chiave primaria
    nome VARCHAR(100) NOT NULL,  -- Nome dell'utente
    cognome VARCHAR(100) NOT NULL,  -- Cognome dell'utente
    email VARCHAR(150) UNIQUE NOT NULL,  -- Email univoca per l'autenticazione
    password_hash VARCHAR(255) NOT NULL,  -- Hash della password per l'autenticazione
    telefono VARCHAR(15),  -- Numero di telefono (facoltativo)
    is_amministratore BOOL  -- Identifica se l'utente è un amministratore (true/false)
);

-- Tabella delle biblioteche per registrare le diverse sedi e i loro dettagli
CREATE TABLE Biblioteche (
    id_biblioteca BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,  -- Chiave primaria
    nome_biblioteca VARCHAR(100) NOT NULL,  -- Nome della biblioteca
    indirizzo VARCHAR(255) NOT NULL,  -- Indirizzo della biblioteca
    telefono VARCHAR(15),  -- Numero di telefono della biblioteca (facoltativo)
    orari_apertura VARCHAR(50)  -- Orari di apertura della biblioteca
);

-- Tabella dei libri disponibili nelle biblioteche
CREATE TABLE Libri (
    id_libro BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,  -- Chiave primaria per identificare un libro
    titolo VARCHAR(255) NOT NULL,  -- Titolo del libro
    autore VARCHAR(255) NOT NULL,  -- Autore del libro
    anno_pubblicazione INT,  -- Anno di pubblicazione
    isbn VARCHAR(13) UNIQUE NOT NULL,  -- Codice ISBN univoco per il libro
    id_biblioteca BIGINT UNSIGNED,  -- Relazione con la biblioteca
    quantita_disponibile INT DEFAULT 0,  -- Numero di copie disponibili
    categoria VARCHAR(100),  -- Categoria del libro (facoltativo)
    CONSTRAINT fk_libri_biblioteca FOREIGN KEY (id_biblioteca) REFERENCES Biblioteche(id_biblioteca) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Tabella delle sale studio disponibili nelle biblioteche
CREATE TABLE Sale_Studio (
    id_sala BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,  -- Chiave primaria per identificare una sala studio
    nome_sala VARCHAR(100) NOT NULL,  -- Nome della sala studio
    id_biblioteca BIGINT UNSIGNED,  -- Relazione con la biblioteca
    capienza INT NOT NULL,  -- Capacità massima della sala studio
    disponibilita BOOLEAN DEFAULT TRUE,  -- Indica se la sala è disponibile per la prenotazione
    CONSTRAINT fk_sala_biblioteca FOREIGN KEY (id_biblioteca) REFERENCES Biblioteche(id_biblioteca) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Tabella per registrare le prenotazioni di libri effettuate dagli utenti
CREATE TABLE Prenotazioni_Libri (
    id_prenotazione BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,  -- Chiave primaria per la prenotazione
    id_utente BIGINT UNSIGNED,  -- Relazione con l'utente che ha effettuato la prenotazione
    id_libro BIGINT UNSIGNED,  -- Relazione con il libro prenotato
    data_prenotazione DATE NOT NULL,  -- Data in cui è stata effettuata la prenotazione
    data_ritiro DATE,  -- Data in cui il libro è stato ritirato (se applicabile)
    data_restituzione DATE,  -- Data in cui il libro è stato restituito (se applicabile)
    stato ENUM('Prenotato', 'Ritirato', 'Restituito', 'Cancellato') NOT NULL,  -- Stato attuale della prenotazione
    CONSTRAINT fk_prenotazioni_libri_utente FOREIGN KEY (id_utente) REFERENCES Utenti(id_utente) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_prenotazioni_libri_libro FOREIGN KEY (id_libro) REFERENCES Libri(id_libro) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Tabella per registrare le prenotazioni delle sale studio effettuate dagli utenti
CREATE TABLE Prenotazioni_Sale (
    id_prenotazione BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,  -- Chiave primaria per la prenotazione
    id_utente BIGINT UNSIGNED,  -- Relazione con l'utente che ha effettuato la prenotazione
    id_sala BIGINT UNSIGNED,  -- Relazione con la sala prenotata
    data_prenotazione DATE NOT NULL,  -- Data in cui è stata effettuata la prenotazione
    giorno_prenotato DATE NOT NULL,  -- Giorno per il quale è stata prenotata la sala
    ora_inizio TIME NOT NULL,  -- Ora di inizio della prenotazione
    ora_fine TIME NOT NULL,  -- Ora di fine della prenotazione
    stato ENUM('Prenotato', 'Usufruito', 'Cancellato') NOT NULL,  -- Stato attuale della prenotazione
    CONSTRAINT fk_prenotazioni_sale_utente FOREIGN KEY (id_utente) REFERENCES Utenti(id_utente) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_prenotazioni_sale_sala FOREIGN KEY (id_sala) REFERENCES Sale_Studio(id_sala) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Tabella delle categorie per classificare i libri
CREATE TABLE Categorie (
    id_categoria BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,  -- Chiave primaria per la categoria
    nome_categoria VARCHAR(100) UNIQUE NOT NULL  -- Nome univoco della categoria
);

-- Tabella di relazione molti-a-molti tra Libri e Categorie
CREATE TABLE Libri_Categorie (
    id_libro BIGINT UNSIGNED,  -- Relazione con il libro
    id_categoria BIGINT UNSIGNED,  -- Relazione con la categoria
    PRIMARY KEY (id_libro, id_categoria),  -- La chiave primaria composta garantisce che non ci siano duplicati
    CONSTRAINT fk_libri_categorie_libro FOREIGN KEY (id_libro) REFERENCES Libri(id_libro) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_libri_categorie_categoria FOREIGN KEY (id_categoria) REFERENCES Categorie(id_categoria) ON DELETE CASCADE ON UPDATE CASCADE
);
