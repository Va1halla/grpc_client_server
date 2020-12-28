-- Drop table

-- DROP TABLE public.persons;

CREATE TABLE public.persons (
	id int4 NOT NULL GENERATED ALWAYS AS IDENTITY,
	first_name varchar(50) NOT NULL,
	last_name varchar(50) NOT NULL,
	age int4 NOT NULL,
	write_date timestamp NULL DEFAULT CURRENT_TIMESTAMP,
	CONSTRAINT persons_pk PRIMARY KEY (id)
);
CREATE INDEX persons_id_idx ON public.persons USING btree (id);

-- Permissions

ALTER TABLE public.persons OWNER TO postgres;
GRANT INSERT, SELECT, UPDATE, DELETE, TRUNCATE ON TABLE public.persons TO postgres;
