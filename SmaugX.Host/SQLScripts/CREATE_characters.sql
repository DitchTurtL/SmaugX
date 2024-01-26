-- Table: public.characters

-- DROP TABLE IF EXISTS public.characters;

CREATE TABLE IF NOT EXISTS public.characters
(
    id bigint NOT NULL DEFAULT nextval('characters_id_seq'::regclass),
    user_id bigint NOT NULL DEFAULT nextval('characters_user_id_seq'::regclass),
    name character varying COLLATE pg_catalog."default" NOT NULL,
    current_room_id bigint,
    permissions integer NOT NULL DEFAULT 1,
    CONSTRAINT characters_pkey PRIMARY KEY (id),
    CONSTRAINT current_room_id_fk FOREIGN KEY (current_room_id)
        REFERENCES public.rooms (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    CONSTRAINT user_id_fk FOREIGN KEY (user_id)
        REFERENCES public.users (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.characters
    OWNER to smaugx;