-- Table: public.users

DO $$ 
BEGIN
    -- DROP TABLE IF EXISTS public.users;
    
    IF NOT EXISTS (SELECT 1 FROM pg_sequences WHERE sequencename = 'users_id_seq') THEN
        CREATE SEQUENCE users_id_seq START 1;
    END IF;

CREATE TABLE IF NOT EXISTS public.users
(
    id bigint NOT NULL DEFAULT nextval('users_id_seq'::regclass),
    name character varying COLLATE pg_catalog."default" NOT NULL,
    email character varying COLLATE pg_catalog."default" NOT NULL,
    password character varying COLLATE pg_catalog."default" NOT NULL,
    permissions integer NOT NULL DEFAULT 1,
    CONSTRAINT users_pkey PRIMARY KEY (id)
)

    TABLESPACE pg_default;

    ALTER TABLE IF EXISTS public.users
        OWNER to smaugx;

    
END $$;