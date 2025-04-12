CREATE DATABASE voiture;
USE voiture;

-- Table des voitures
/*CREATE TABLE voiture (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(255) NOT NULL,
    accel DOUBLE NOT NULL,
    decel DOUBLE NOT NULL,
    vitesse DOUBLE DEFAULT 0
);

-- Table pour enregistrer l'historique des accélérations avec uniquement l'heure
CREATE TABLE acceleration_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    voiture_id INT,
    vitesseInitiale DOUBLE NOT NULL,
    acceleration DOUBLE NOT NULL,
    heure_acceleration TIME DEFAULT CURRENT_TIME,  -- Enregistre uniquement l'heure
    FOREIGN KEY (voiture_id) REFERENCES voiture(id)
);*/

-- Table des voitures avec précision sur les valeurs décimales
CREATE TABLE voiture (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(255) NOT NULL,
    accel DECIMAL(5,1) NOT NULL,  -- 5 chiffres au total, dont 2 après la virgule
    decel DECIMAL(5,1) NOT NULL,  -- Idem
    vitesse DECIMAL(5,1) DEFAULT 0,-- Idem
    capacite DECIMAL(5,1),
    consoMax DECIMAL(5,2)
);

-- Table pour enregistrer l'historique des accélérations
CREATE TABLE acceleration_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    voiture_id INT,
    vitesseInitiale DECIMAL(5,1) NOT NULL,  -- Limite à 2 décimales
    acceleration DECIMAL(5,1) NOT NULL,     -- Limite à 2 décimales
    heure_acceleration TIME(3) DEFAULT CURRENT_TIMESTAMP(3),
    FOREIGN KEY (voiture_id) REFERENCES voiture(id)
);

--consommation moyenne d'essence litre par 100 km 

-- INSERT INTO voiture VALUES (1,'Nissan patrol',10,10,0,25,0.08);
-- INSERT INTO voiture VALUES (2,'Nissan GTR',20,15,0,20,0.03);
-- INSERT INTO voiture VALUES (3,'Toyota supra',25,15,0,22,0.1);
-- INSERT INTO voiture VALUES (4,'208',20,25,0,80,0.1);

-- INSERT INTO acceleration_log (voiture_id,vitesseInitiale,acceleration,heure_acceleration) VALUES (1,0,10,'13:53:01');
-- INSERT INTO acceleration_log (voiture_id,vitesseInitiale,acceleration,heure_acceleration) VALUES (1,30,0,'13:53:04');
-- INSERT INTO acceleration_log (voiture_id,vitesseInitiale,acceleration,heure_acceleration) VALUES (1,30,-10,'13:53:06');
-- INSERT INTO acceleration_log (voiture_id,vitesseInitiale,acceleration,heure_acceleration) VALUES (1,10,0,'13:53:08');



-- INSERT INTO acceleration_log (voiture_id, vitesseInitiale, acceleration, heure_acceleration) 
-- VALUES 
--     (1, 0.0, 10.0, '13:53:01'),   -- Départ à 0 km/h, accélération
--     (1, 30.0, 0.0, '13:53:04'),   -- Vitesse constante à 30 km/h
--     (1, 30.0, -10.0, '13:53:06'), -- Freinage
--     (1, 10.0, 0.0, '13:53:08'),   -- Vitesse constante à 10 km/h
--     (1, 10.0, 5.0, '13:53:10'),   -- Nouvelle accélération
--     (1, 25.0, 0.0, '13:53:13');   -- Vitesse constante à 25 km/h 50 10s 5 s  20 5s


-- accel

INSERT INTO voiture (nom, accel, decel, vitesse, capacite, consoMax) VALUES ('alea1', 50, 20,0,1,0.05);  
