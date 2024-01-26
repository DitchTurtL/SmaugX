-- Table: public.rooms

-- DROP TABLE IF EXISTS public.rooms;

CREATE TABLE IF NOT EXISTS public.rooms
(
    id bigint NOT NULL DEFAULT nextval('rooms_id_seq'::regclass),
    reference_id character varying COLLATE pg_catalog."default" NOT NULL,
    name character varying COLLATE pg_catalog."default" NOT NULL,
    short_description character varying COLLATE pg_catalog."default",
    long_description character varying COLLATE pg_catalog."default",
    CONSTRAINT rooms_pkey PRIMARY KEY (id),
    CONSTRAINT unique_reference_id UNIQUE (reference_id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.rooms
    OWNER to smaugx;