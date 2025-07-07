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
    RETURNS BOOLEAN AS
$$
BEGIN
    RETURN email_value ~ '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$'
        AND LENGTH(email_value) >= 1
        AND LENGTH(email_value) <= 120;
END;
$$ LANGUAGE plpgsql IMMUTABLE;

CREATE OR REPLACE FUNCTION is_cpf(cpf_value TEXT)
    RETURNS BOOLEAN AS
$$
BEGIN
    RETURN cpf_value ~ '^[0-9]{11}$';
END;
$$ LANGUAGE plpgsql IMMUTABLE;

CREATE OR REPLACE FUNCTION is_cnpj(cnpj_value TEXT)
    RETURNS BOOLEAN AS
$$
BEGIN
    RETURN cnpj_value ~ '^[0-9]{14}$';
END;
$$ LANGUAGE plpgsql IMMUTABLE;

CREATE OR REPLACE FUNCTION verify_person_type_ck1(person_type VARCHAR(12))
    RETURNS BOOLEAN AS
$$
BEGIN
    RETURN person_type IN ('Individual', 'Organization');
END;
$$ LANGUAGE plpgsql IMMUTABLE;

-- Main tables
CREATE TABLE person
(
    id   uuid PRIMARY KEY     DEFAULT uuid_generate_v4(),
    name text        NOT NULL,
    type varchar(12) NOT NULL DEFAULT '' CHECK ( verify_person_type_ck1(type) )
);

CREATE TABLE individual
(
    id    uuid         NOT NULL,
    cpf   VARCHAR(11)  NOT NULL,
    email VARCHAR(120) NOT NULL,

    PRIMARY KEY (id),
    CONSTRAINT fk_individual_person FOREIGN KEY (id) REFERENCES person (id) ON DELETE CASCADE,
    CONSTRAINT chk_individual_cpf CHECK (is_cpf(cpf)),
    CONSTRAINT chk_individual_email CHECK (is_email(email)),
    CONSTRAINT uq_individual_cpf UNIQUE (cpf),
    CONSTRAINT uq_individual_email UNIQUE (email)
);

CREATE TABLE organization
(
    id                uuid        NOT NULL,
    doing_business_as text        NOT NULL,
    cnpj              VARCHAR(14) NOT NULL,

    PRIMARY KEY (id),
    CONSTRAINT fk_organization_person FOREIGN KEY (id) REFERENCES person (id) ON DELETE CASCADE,
    CONSTRAINT chk_organization_cnpj CHECK (is_cnpj(cnpj)),
    CONSTRAINT uq_organization_cnpj UNIQUE (cnpj)
);

-- Indexes for performance
CREATE INDEX idx_person_name ON person (name);

CREATE OR REPLACE FUNCTION person_service_add_individual(
    p_name text,
    p_cpf text,
    p_email text,
    p_id uuid DEFAULT uuid_generate_v4()
)
    RETURNS TABLE
            (
                id    uuid,
                name  text,
                type  varchar(12),
                cpf   varchar(11),
                email varchar(120)
            )
AS
$$
BEGIN
    -- Insert into person table and return the result
    RETURN QUERY
        WITH
            person_insert AS (
                                 INSERT INTO person (id, name, type)
                                     VALUES (p_id, p_name, 'Individual')
                                     RETURNING person.id AS person_id,
                                         person.name AS person_name,
                                         person.type AS person_type
                             ),
            individual_insert AS (
                                 INSERT INTO individual (id, cpf, email)
                                     SELECT person_id, p_cpf, p_email
                                     FROM person_insert
                                     RETURNING individual.id AS individual_id,
                                         individual.cpf AS individual_cpf,
                                         individual.email AS individual_email
                             )
        SELECT pi.person_id        as id,
               pi.person_name      as name,
               pi.person_type      as type,
               ii.individual_cpf   as cpf,
               ii.individual_email as email
        FROM person_insert pi
                 JOIN individual_insert ii ON pi.person_id = ii.individual_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION person_service_add_organization(
    p_name text,
    p_doing_business_as text,
    p_cnpj text,
    p_id uuid DEFAULT uuid_generate_v4()
)
    RETURNS TABLE
            (
                id                uuid,
                name              text,
                type              varchar(12),
                doing_business_as text,
                cnpj              varchar(14)
            )
AS
$$
BEGIN
    -- Insert into person table and return the result
    RETURN QUERY
        WITH
            person_insert AS (
                                 INSERT INTO person (id, name, type)
                                     VALUES (p_id, p_name, 'Organization')
                                     RETURNING person.id AS person_id,
                                         person.name AS person_name,
                                         person.type AS person_type
                             ),
            organization_insert AS (
                                 INSERT INTO organization (id, doing_business_as, cnpj)
                                     SELECT person_id, p_doing_business_as, p_cnpj
                                     FROM person_insert
                                     RETURNING organization.id AS organization_id,
                                         organization.doing_business_as AS organization_doing_business_as,
                                         organization.cnpj AS organization_cnpj
                             )
        SELECT pi.person_id                      as id,
               pi.person_name                    as name,
               pi.person_type                    as type,
               oi.organization_doing_business_as as doing_business_as,
               oi.organization_cnpj              as cnpj
        FROM person_insert pi
                 JOIN organization_insert oi ON pi.person_id = oi.organization_id;
END;
$$ LANGUAGE plpgsql;
