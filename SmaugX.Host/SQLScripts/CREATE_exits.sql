-- Table: public.exits

-- DROP TABLE IF EXISTS public.exits;

CREATE TABLE IF NOT EXISTS public.exits
(
    id bigint NOT NULL DEFAULT nextval('exits_id_seq'::regclass),
    room_id bigint NOT NULL,
    destination_room_id bigint NOT NULL,
    direction integer NOT NULL,
    created_on timestamp with time zone NOT NULL DEFAULT now(),
    created_by bigint NOT NULL DEFAULT 0,
    CONSTRAINT exits_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.exits
    OWNER to smaugx;