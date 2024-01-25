-- Table: public.exits

-- DROP TABLE IF EXISTS public.exits;

CREATE TABLE IF NOT EXISTS public.exits
(
    id bigint NOT NULL DEFAULT nextval('exits_id_seq'::regclass),
    room_id bigint NOT NULL,
    destination_room_id bigint NOT NULL,
    reference_id character varying COLLATE pg_catalog."default" NOT NULL,
    room_reference_id character varying COLLATE pg_catalog."default" NOT NULL,
    destination_room_reference_id character varying COLLATE pg_catalog."default" NOT NULL,
    name character varying COLLATE pg_catalog."default" NOT NULL,
    short_description character varying COLLATE pg_catalog."default",
    long_description character varying COLLATE pg_catalog."default",
    direction integer NOT NULL,
    CONSTRAINT exits_pkey PRIMARY KEY (id),
    CONSTRAINT dest_room_id_fk FOREIGN KEY (destination_room_id)
        REFERENCES public.rooms (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    CONSTRAINT room_id_fk FOREIGN KEY (room_id)
        REFERENCES public.rooms (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.exits
    OWNER to smaugx;