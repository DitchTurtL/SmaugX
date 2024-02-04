-- Table: public.items

-- DROP TABLE IF EXISTS public.items;

CREATE TABLE IF NOT EXISTS public.items
(
    id bigint NOT NULL DEFAULT nextval('items_id_seq'::regclass),
    name character varying COLLATE pg_catalog."default",
    short_description character varying COLLATE pg_catalog."default",
    long_description character varying COLLATE pg_catalog."default",
    weight integer,
    can_carry boolean,
    cost integer,
    CONSTRAINT items_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.items
    OWNER to smaugx;