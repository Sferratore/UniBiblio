/*
    ############################################################
    #                     Database Overview                    #
    ############################################################

    Questo file SQL definisce la struttura di un database per un sistema di gestione bibliotecario.
    Il sistema permette la gestione delle prenotazioni di libri e sale studio, la gestione degli utenti, 
    e il tracciamento delle categorie di libri. 

    ############################################################
    #                 Normalizzazione e Forma Normale           #
    ############################################################

    Le tabelle sono progettate seguendo i principi di normalizzazione fino alla **Terza Forma Normale (3NF)**:

    1. **Prima Forma Normale (1NF)**: 
        - Ogni tabella ha righe uniche, e ogni colonna contiene valori atomici (non ci sono gruppi ripetuti o colonne multiple 
          per lo stesso tipo di dato).
        - Le tabelle come `Prenotazioni_Libri` e `Prenotazioni_Sale` non hanno colonne con valori multipli o ripetuti.
    
    2. **Seconda Forma Normale (2NF)**:
        - Ogni attributo non chiave dipende completamente dalla chiave primaria. 
        - Ad esempio, in `Prenotazioni_Libri`, gli attributi `data_prenotazione`, `stato`, `data_ritiro`, ecc. dipendono 
          direttamente dalla chiave primaria `id_prenotazione`, e non ci sono dipendenze parziali.
    
    3. **Terza Forma Normale (3NF)**:
        - Non ci sono dipendenze transitive tra attributi non chiave. 
        - In `Libri`, ad esempio, non ci sono attributi che dipendono transitivamente da altri attributi non chiave: tutti 
          dipendono solo da `id_libro`.
    
    ############################################################
    #                     Business Rules                       #
    ############################################################

    Le seguenti regole di business sono implementate nell'applicazione o a livello di database:

    1. **Limite di Prenotazioni Libri**:
       - Un utente può prenotare un massimo di 3 libri contemporaneamente che non sono ancora stati ritirati (stato 'Prenotato').

    2. **Limite di Prenotazioni Sale**:
       - Un utente può avere solo una prenotazione attiva di una sala studio per volta (stato 'Prenotato').

    3. **Tracciamento delle Categorie di Libri**:
       - I libri possono appartenere a una o più categorie tramite la tabella di relazione `Libri_Categorie`.

    4. **Stato delle Prenotazioni**:
       - Le prenotazioni di libri e sale devono avere uno stato predefinito:
         - `Prenotato`: Quando la prenotazione è stata effettuata.
         - `Ritirato` o `Usufruito`: Quando l'utente ha ritirato il libro o ha utilizzato la sala studio.
         - `Restituito` o `Cancellato`: Quando la prenotazione è stata completata o annullata.
    
    ############################################################
    #                        Contesto                          #
    ############################################################

    Il database è progettato per supportare un sistema di gestione di prenotazioni in una biblioteca, dove:
    - Gli utenti possono prenotare libri e sale studio.
    - I bibliotecari possono monitorare le prenotazioni, gestire le categorie di libri e ottenere statistiche di utilizzo.
    - L'applicazione si basa su MySQL come motore di database, con procedure e trigger (es. evento per definire l'uso delle sale e "chiudere" i record di prenotazione) per gestire eventi e regole specifiche 
      di business.
    - L'autenticazione degli utenti è gestita tramite il campo `is_amministratore` che differenzia gli utenti standard dagli 
      amministratori.
*/

-- Definizione delle tabelle

-- Tabella utenti per memorizzare le informazioni sugli utenti della biblioteca
CREATE TABLE Utenti (
    id_utente SERIAL PRIMARY KEY,  -- Chiave primaria
    nome VARCHAR(100) NOT NULL,  -- Nome dell'utente
    cognome VARCHAR(100) NOT NULL,  -- Cognome dell'utente
    email VARCHAR(150) UNIQUE NOT NULL,  -- Email univoca per l'autenticazione
    password_hash VARCHAR(255) NOT NULL,  -- Hash della password per l'autenticazione
    telefono VARCHAR(15),  -- Numero di telefono (facoltativo)
    is_amministratore BOOL  -- Identifica se l'utente è un amministratore (true/false)
);

-- Tabella delle biblioteche per registrare le diverse sedi e i loro dettagli
CREATE TABLE Biblioteche (
    id_biblioteca SERIAL PRIMARY KEY,  -- Chiave primaria
    nome_biblioteca VARCHAR(100) NOT NULL,  -- Nome della biblioteca
    indirizzo VARCHAR(255) NOT NULL,  -- Indirizzo della biblioteca
    telefono VARCHAR(15),  -- Numero di telefono della biblioteca (facoltativo)
    orari_apertura VARCHAR(50)  -- Orari di apertura della biblioteca
);

-- Tabella dei libri disponibili nelle biblioteche
CREATE TABLE Libri (
    id_libro SERIAL PRIMARY KEY,  -- Chiave primaria per identificare un libro
    titolo VARCHAR(255) NOT NULL,  -- Titolo del libro
    autore VARCHAR(255) NOT NULL,  -- Autore del libro
    anno_pubblicazione INT,  -- Anno di pubblicazione
    isbn VARCHAR(13) UNIQUE NOT NULL,  -- Codice ISBN univoco per il libro
    id_biblioteca INT REFERENCES Biblioteche(id_biblioteca),  -- Relazione con la biblioteca in cui è disponibile il libro
    quantita_disponibile INT DEFAULT 0,  -- Numero di copie disponibili
    categoria VARCHAR(100)  -- Categoria del libro (facoltativo)
);

-- Tabella delle sale studio disponibili nelle biblioteche
CREATE TABLE Sale_Studio (
    id_sala SERIAL PRIMARY KEY,  -- Chiave primaria per identificare una sala studio
    nome_sala VARCHAR(100) NOT NULL,  -- Nome della sala studio
    id_biblioteca INT REFERENCES Biblioteche(id_biblioteca),  -- Relazione con la biblioteca in cui si trova la sala
    capienza INT NOT NULL,  -- Capacità massima della sala studio
    disponibilita BOOLEAN DEFAULT TRUE  -- Indica se la sala è disponibile per la prenotazione
);

-- Tabella per registrare le prenotazioni di libri effettuate dagli utenti
CREATE TABLE Prenotazioni_Libri (
    id_prenotazione SERIAL PRIMARY KEY,  -- Chiave primaria per la prenotazione
    id_utente INT REFERENCES Utenti(id_utente),  -- Relazione con l'utente che ha effettuato la prenotazione
    id_libro INT REFERENCES Libri(id_libro),  -- Relazione con il libro prenotato
    data_prenotazione DATE NOT NULL,  -- Data in cui è stata effettuata la prenotazione
    data_ritiro DATE,  -- Data in cui il libro è stato ritirato (se applicabile)
    data_restituzione DATE,  -- Data in cui il libro è stato restituito (se applicabile)
    stato ENUM('Prenotato', 'Ritirato', 'Restituito', 'Cancellato') NOT NULL  -- Stato attuale della prenotazione
);

-- Tabella per registrare le prenotazioni delle sale studio effettuate dagli utenti
CREATE TABLE Prenotazioni_Sale (
    id_prenotazione SERIAL PRIMARY KEY,  -- Chiave primaria per la prenotazione
    id_utente INT REFERENCES Utenti(id_utente),  -- Relazione con l'utente che ha effettuato la prenotazione
    id_sala INT REFERENCES Sale_Studio(id_sala),  -- Relazione con la sala prenotata
    data_prenotazione DATE NOT NULL,  -- Data in cui è stata effettuata la prenotazione
    giorno_prenotato DATE NOT NULL,  -- Giorno per il quale è stata prenotata la sala
    ora_inizio TIME NOT NULL,  -- Ora di inizio della prenotazione
    ora_fine TIME NOT NULL,  -- Ora di fine della prenotazione
    stato ENUM('Prenotato', 'Usufruito', 'Cancellato') NOT NULL  -- Stato attuale della prenotazione
);

-- Tabella delle categorie per classificare i libri
CREATE TABLE Categorie (
    id_categoria SERIAL PRIMARY KEY,  -- Chiave primaria per la categoria
    nome_categoria VARCHAR(100) UNIQUE NOT NULL  -- Nome univoco della categoria
);

-- Tabella di relazione molti-a-molti tra Libri e Categorie
CREATE TABLE Libri_Categorie (
    id_libro INT REFERENCES Libri(id_libro),  -- Relazione con il libro
    id_categoria INT REFERENCES Categorie(id_categoria),  -- Relazione con la categoria
    PRIMARY KEY (id_libro, id_categoria)  -- La chiave primaria composta garantisce che non ci siano duplicati
);
