-- Table: public.items

-- DROP TABLE IF EXISTS public.items;
IF NOT EXISTS (SELECT 1 FROM pg_sequences WHERE sequencename = 'items_id_seq') THEN
    CREATE SEQUENCE items_id_seq START 1;
END IF;

CREATE TABLE IF NOT EXISTS public.items
(
    id bigint NOT NULL DEFAULT nextval('items_id_seq'::regclass),
    name character varying COLLATE pg_catalog."default",
    short_description character varying COLLATE pg_catalog."default",
    long_description character varying COLLATE pg_catalog."default",
    weight integer,
    can_carry boolean,
    cost integer DEFAULT 1,
    slot integer DEFAULT 0,
    CONSTRAINT items_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.items
    OWNER to smaugx;