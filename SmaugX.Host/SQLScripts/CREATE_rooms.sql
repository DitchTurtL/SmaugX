-- Table: public.rooms

-- DROP TABLE IF EXISTS public.rooms;

CREATE TABLE IF NOT EXISTS public.rooms
(
    id bigint NOT NULL DEFAULT nextval('rooms_id_seq'::regclass),
    name character varying COLLATE pg_catalog."default" NOT NULL,
    short_description character varying COLLATE pg_catalog."default",
    long_description character varying COLLATE pg_catalog."default",
    created_on timestamp with time zone NOT NULL DEFAULT now(),
    created_by bigint NOT NULL DEFAULT 0,
    CONSTRAINT rooms_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.rooms
    OWNER to smaugx;