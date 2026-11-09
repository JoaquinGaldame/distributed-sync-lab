CREATE USER rental_management_user WITH PASSWORD 'rental_management_password';
CREATE DATABASE rental_management OWNER rental_management_user;

CREATE USER ota_replace_service_user WITH PASSWORD 'ota_replace_service_password';
CREATE DATABASE ota_replace_service_db OWNER ota_replace_service_user;

CREATE USER ota_replace_simulator_user WITH PASSWORD 'ota_replace_simulator_password';
CREATE DATABASE ota_replace_simulator_db OWNER ota_replace_simulator_user;

-- OTAS SIMULADAS

CREATE USER ota_a_simulator_user WITH PASSWORD 'ota_a_simulator_password';
CREATE DATABASE ota_a_simulator_db OWNER ota_a_simulator_user;

CREATE USER ota_b_simulator_user WITH PASSWORD 'ota_b_simulator_password';
CREATE DATABASE ota_b_simulator_db OWNER ota_b_simulator_user;