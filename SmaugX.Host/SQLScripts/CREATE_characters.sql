-- Table: public.characters

-- DROP TABLE IF EXISTS public.characters;

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