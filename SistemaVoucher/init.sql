-- Arquivo SQL inicial para configuração do banco
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Criar database de produção se não existir
SELECT 'CREATE DATABASE sistemavoucher'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'sistemavoucher')\gexec