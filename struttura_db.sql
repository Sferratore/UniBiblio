CREATE TABLE Utenti (
    id_utente SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    cognome VARCHAR(100) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    telefono VARCHAR(15),
    id_ruolo INT REFERENCES Ruoli(id_ruolo)
);

CREATE TABLE Amministratori (
    id_admin SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    cognome VARCHAR(100) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    id_biblioteca INT REFERENCES Biblioteche(id_biblioteca)
);

CREATE TABLE Biblioteche (
    id_biblioteca SERIAL PRIMARY KEY,
    nome_biblioteca VARCHAR(100) NOT NULL,
    indirizzo VARCHAR(255) NOT NULL,
    telefono VARCHAR(15),
    orari_apertura VARCHAR(50)
);

CREATE TABLE Libri (
    id_libro SERIAL PRIMARY KEY,
    titolo VARCHAR(255) NOT NULL,
    autore VARCHAR(255) NOT NULL,
    anno_pubblicazione INT,
    isbn VARCHAR(13) UNIQUE NOT NULL,
    id_biblioteca INT REFERENCES Biblioteche(id_biblioteca),
    quantita_disponibile INT DEFAULT 0,
    categoria VARCHAR(100) 
);

CREATE TABLE Sale_Studio (
    id_sala SERIAL PRIMARY KEY,
    nome_sala VARCHAR(100) NOT NULL,
    id_biblioteca INT REFERENCES Biblioteche(id_biblioteca),
    capienza INT NOT NULL,
    disponibilita BOOLEAN DEFAULT TRUE 
);

CREATE TABLE Prenotazioni_Libri (
    id_prenotazione SERIAL PRIMARY KEY,
    id_utente INT REFERENCES Utenti(id_utente),
    id_libro INT REFERENCES Libri(id_libro),
    data_prenotazione DATE NOT NULL,
    data_ritiro DATE,
    data_restituzione DATE,
    stato VARCHAR(50) NOT NULL 
);

CREATE TABLE Prenotazioni_Sale (
    id_prenotazione SERIAL PRIMARY KEY,
    id_utente INT REFERENCES Utenti(id_utente),
    id_sala INT REFERENCES Sale_Studio(id_sala),
    data_prenotazione DATE NOT NULL,
    ora_inizio TIME NOT NULL,
    ora_fine TIME NOT NULL,
    stato VARCHAR(50) NOT NULL 
);

CREATE TABLE Categorie (
    id_categoria SERIAL PRIMARY KEY,
    nome_categoria VARCHAR(100) UNIQUE NOT NULL
);

CREATE TABLE Libri_Categorie (
    id_libro INT REFERENCES Libri(id_libro),
    id_categoria INT REFERENCES Categorie(id_categoria),
    PRIMARY KEY (id_libro, id_categoria)
);

CREATE TABLE Ruoli (
    id_ruolo SERIAL PRIMARY KEY,
    nome_ruolo VARCHAR(50) UNIQUE NOT NULL
);
