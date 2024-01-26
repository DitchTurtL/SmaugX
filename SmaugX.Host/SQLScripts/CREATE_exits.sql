-- Table: public.exits

DO $$ 
BEGIN
    -- DROP TABLE IF EXISTS public.exits;

    IF NOT EXISTS (SELECT 1 FROM pg_sequences WHERE sequencename = 'exits_id_seq') THEN
        CREATE SEQUENCE exits_id_seq START 1;
    END IF;

    CREATE TABLE IF NOT EXISTS public.exits
    (
        id bigint NOT NULL DEFAULT nextval('exits_id_seq'::regclass),
        room_id bigint NOT NULL,
        destination_room_id bigint NOT NULL,
        name character varying COLLATE pg_catalog."default" NOT NULL,
        short_description character varying COLLATE pg_catalog."default",
        long_description character varying COLLATE pg_catalog."default",
        direction integer NOT NULL,
        one_way boolean NOT NULL DEFAULT false,
        CONSTRAINT exits_pkey PRIMARY KEY (id)
    )

    TABLESPACE pg_default;

    ALTER TABLE IF EXISTS public.exits
        OWNER to smaugx;

END $$;