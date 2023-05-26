CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE wells (
    id UUID NOT NULL DEFAULT uuid_generate_v4(),
    area VARCHAR(128) NOT NULL,
    field VARCHAR (128) NOT NULL,
    uwi VARCHAR(128) UNIQUE NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT pkey_well PRIMARY KEY(id)
);

CREATE TABLE tapers (
    id UUID NOT NULL DEFAULT uuid_generate_v4(),
    well_id UUID NOT NULL,
    type VARCHAR(128) NOT NULL,
    length NUMERIC NOT NULL,
    diameter NUMERIC NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT pkey_taper PRIMARY KEY(id),
    CONSTRAINT fkey_well FOREIGN KEY (well_id) REFERENCES wells(id)
);


INSERT INTO wells (id, area, field, uwi) VALUES
    ('e109f248-6724-4729-b498-fe873372e1b4','Area 1', 'Field 1', 'A1F1-1'),
    ('0f81016b-9d71-4218-a2b8-df9f2615c07b','Area 2', 'Field 1', 'A2F1-2'),
    ('59a67321-ff55-49ef-b357-ea5cc6b9e6fb','Area 1', 'Field 2', 'A1F2-3');

INSERT INTO tapers (well_id, type, length, diameter) VALUES 
    ('e109f248-6724-4729-b498-fe873372e1b4', 'Polished Rod', 50, 1.25),
    ('e109f248-6724-4729-b498-fe873372e1b4', 'Steel', 2300, 1),
    ('e109f248-6724-4729-b498-fe873372e1b4', 'Steel', 25, 0.75),
    ('0f81016b-9d71-4218-a2b8-df9f2615c07b', 'Polished Rod', 50, 1),
    ('0f81016b-9d71-4218-a2b8-df9f2615c07b', 'Fiberglass', 1020, 1),
    ('0f81016b-9d71-4218-a2b8-df9f2615c07b', 'Fiberglass', 30, 0.75),
    ('59a67321-ff55-49ef-b357-ea5cc6b9e6fb', 'Polished Rod', 50, 1.25),
    ('59a67321-ff55-49ef-b357-ea5cc6b9e6fb', 'Steel', 2000, 1),
    ('59a67321-ff55-49ef-b357-ea5cc6b9e6fb', 'Steel', 5, 0.75);
    
    