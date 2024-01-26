-- Table: public.rooms

DO $$ 
BEGIN

    -- DROP TABLE IF EXISTS public.rooms;

    IF NOT EXISTS (SELECT 1 FROM pg_sequences WHERE sequencename = 'rooms_id_seq') THEN
        CREATE SEQUENCE rooms_id_seq START 1;
    END IF;


    CREATE TABLE IF NOT EXISTS public.rooms
    (
        id bigint NOT NULL DEFAULT nextval('rooms_id_seq'::regclass),
        name character varying COLLATE pg_catalog."default" NOT NULL,
        short_description character varying COLLATE pg_catalog."default",
        long_description character varying COLLATE pg_catalog."default",
        CONSTRAINT rooms_pkey PRIMARY KEY (id)
    )

    TABLESPACE pg_default;

    ALTER TABLE IF EXISTS public.rooms
        OWNER to smaugx;

END $$;