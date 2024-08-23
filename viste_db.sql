CREATE VIEW PrenotaLibriView AS
SELECT L.id_libro, L.titolo, L.autore, L.anno_pubblicazione, L.isbn, L.quantita_disponibile, L.categoria, B.nome_biblioteca AS biblioteca
FROM libri L INNER JOIN Biblioteche B ON (L.id_biblioteca = B.id_biblioteca)