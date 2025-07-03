
-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Person::id Uuid
-- Person::name String from Named::name
-- Individual::cpf Cpf
-- Individual::email Email
-- Organization::doing_business_as String
-- Organization::cnpj Cnpj

-- Validation functions for type constraints
CREATE OR REPLACE FUNCTION is_email(email_value TEXT)
RETURNS BOOLEAN AS $$
BEGIN
    RETURN email_value ~ '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$' 
           AND LENGTH(email_value) >= 1 
           AND LENGTH(email_value) <= 120;
END;
$$ LANGUAGE plpgsql IMMUTABLE;

CREATE OR REPLACE FUNCTION is_cpf(cpf_value TEXT)
RETURNS BOOLEAN AS $$
BEGIN
    RETURN cpf_value ~ '^[0-9]{11}$';
END;
$$ LANGUAGE plpgsql IMMUTABLE;

CREATE OR REPLACE FUNCTION is_cnpj(cnpj_value TEXT)
RETURNS BOOLEAN AS $$
BEGIN
    RETURN cnpj_value ~ '^[0-9]{14}$';
END;
$$ LANGUAGE plpgsql IMMUTABLE;

-- Main tables
CREATE TABLE person (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    name text NOT NULL,
    type varchar(12) NOT NULL DEFAULT '' CHECK( type IN ('Individual', 'Organization') )
);

CREATE TABLE individual (
    id uuid NOT NULL,
    cpf VARCHAR(11) NOT NULL,
    email VARCHAR(120) NOT NULL,

    PRIMARY KEY(id),
    CONSTRAINT fk_individual_person FOREIGN KEY (id) REFERENCES person(id) ON DELETE CASCADE,
    CONSTRAINT chk_individual_cpf CHECK (is_cpf(cpf)),
    CONSTRAINT chk_individual_email CHECK (is_email(email)),
    CONSTRAINT uq_individual_cpf UNIQUE (cpf),
    CONSTRAINT uq_individual_email UNIQUE (email)
);

CREATE TABLE organization (
    id uuid NOT NULL,
    doing_business_as text NOT NULL,
    cnpj VARCHAR(14) NOT NULL,

    PRIMARY KEY(id),
    CONSTRAINT fk_organization_person FOREIGN KEY (id) REFERENCES person(id) ON DELETE CASCADE,
    CONSTRAINT chk_organization_cnpj CHECK (is_cnpj(cnpj)),
    CONSTRAINT uq_organization_cnpj UNIQUE (cnpj)
);

-- Indexes for performance
CREATE INDEX idx_person_name ON person(name);
