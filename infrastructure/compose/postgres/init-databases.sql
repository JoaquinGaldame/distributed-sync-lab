CREATE USER pms_user WITH PASSWORD 'pms_password';
CREATE DATABASE pms_db OWNER pms_user;

CREATE USER orchestrator_user WITH PASSWORD 'orchestrator_password';
CREATE DATABASE orchestrator_db OWNER orchestrator_user;

CREATE USER ota_replace_service_user WITH PASSWORD 'ota_replace_service_password';
CREATE DATABASE ota_replace_service_db OWNER ota_replace_service_user;

CREATE USER ota_replace_simulator_user WITH PASSWORD 'ota_replace_simulator_password';
CREATE DATABASE ota_replace_simulator_db OWNER ota_replace_simulator_user;