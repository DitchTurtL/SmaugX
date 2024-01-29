-- Table: public.characters

DO $$ 
BEGIN

    -- DROP TABLE IF EXISTS public.characters;

    IF NOT EXISTS (SELECT 1 FROM pg_sequences WHERE sequencename = 'characters_id_seq') THEN
        CREATE SEQUENCE characters_id_seq START 1;
    END IF;

    CREATE TABLE IF NOT EXISTS public.characters
    (
        id bigint NOT NULL DEFAULT nextval('characters_id_seq'::regclass),
        user_id bigint NOT NULL,
        name character varying COLLATE pg_catalog."default",
        current_room_id bigint DEFAULT 1,
        permissions integer DEFAULT 1,
        CONSTRAINT characters_pkey PRIMARY KEY (id)
    )

    TABLESPACE pg_default;

    ALTER TABLE IF EXISTS public.characters
        OWNER to smaugx;

END $$;