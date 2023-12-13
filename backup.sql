--
-- PostgreSQL database dump
--

-- Dumped from database version 15.4
-- Dumped by pg_dump version 15.4

-- Started on 2023-12-09 23:11:32

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 225 (class 1255 OID 24643)
-- Name: getdeviceusersinfo(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.getdeviceusersinfo() RETURNS TABLE(device_name character varying, login character varying, is_admin boolean)
    LANGUAGE plpgsql
    AS $$
BEGIN
	RETURN QUERY
	SELECT 
		public.device.name as device_name,
		public.user.login as login,
		public.user.is_admin as is_admin
	FROM 
		public.device
	JOIN 
		public.device_to_user 
		on public.device.device_id = public.device_to_user.device_id
	JOIN 
		public.user 
		on public.device_to_user.user_id = public.user.user_id; 
END;
$$;


ALTER FUNCTION public.getdeviceusersinfo() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 222 (class 1259 OID 24631)
-- Name: channel; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.channel (
    channel_id integer NOT NULL,
    phone character varying NOT NULL,
    operator character varying NOT NULL,
    tariff character varying NOT NULL
);


ALTER TABLE public.channel OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 24630)
-- Name: channel_channel_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.channel ALTER COLUMN channel_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.channel_channel_id_seq
    START WITH 0
    INCREMENT BY 1
    MINVALUE 0
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 214 (class 1259 OID 16399)
-- Name: data; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.data (
    id integer NOT NULL,
    temp integer NOT NULL,
    humidity integer NOT NULL,
    power boolean NOT NULL,
    people boolean NOT NULL,
    smoke boolean NOT NULL,
    get_time timestamp without time zone NOT NULL,
    device_id integer NOT NULL
);


ALTER TABLE public.data OWNER TO postgres;

--
-- TOC entry 215 (class 1259 OID 24592)
-- Name: data_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.data ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.data_id_seq
    START WITH 2
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 217 (class 1259 OID 24595)
-- Name: device; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.device (
    device_id integer NOT NULL,
    name character varying NOT NULL,
    channel_id integer
);


ALTER TABLE public.device OWNER TO postgres;

--
-- TOC entry 216 (class 1259 OID 24594)
-- Name: device_device_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.device ALTER COLUMN device_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.device_device_id_seq
    START WITH 0
    INCREMENT BY 1
    MINVALUE 0
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 220 (class 1259 OID 24615)
-- Name: device_to_user; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.device_to_user (
    user_id integer NOT NULL,
    device_id integer NOT NULL,
    info character varying
);


ALTER TABLE public.device_to_user OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 24646)
-- Name: transactions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.transactions (
    transaction_id integer NOT NULL,
    sender_id integer NOT NULL,
    receiver_id integer NOT NULL,
    amount numeric(10,2) NOT NULL,
    transaction_date timestamp without time zone NOT NULL
);


ALTER TABLE public.transactions OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 24645)
-- Name: transactions_transaction_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.transactions_transaction_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.transactions_transaction_id_seq OWNER TO postgres;

--
-- TOC entry 3375 (class 0 OID 0)
-- Dependencies: 223
-- Name: transactions_transaction_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.transactions_transaction_id_seq OWNED BY public.transactions.transaction_id;


--
-- TOC entry 219 (class 1259 OID 24603)
-- Name: user; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."user" (
    user_id integer NOT NULL,
    login character varying NOT NULL,
    password character varying NOT NULL,
    is_admin boolean NOT NULL
);


ALTER TABLE public."user" OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 24602)
-- Name: user_user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."user" ALTER COLUMN user_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.user_user_id_seq
    START WITH 0
    INCREMENT BY 1
    MINVALUE 0
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 3198 (class 2604 OID 24649)
-- Name: transactions transaction_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transactions ALTER COLUMN transaction_id SET DEFAULT nextval('public.transactions_transaction_id_seq'::regclass);


--
-- TOC entry 3367 (class 0 OID 24631)
-- Dependencies: 222
-- Data for Name: channel; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.channel (channel_id, phone, operator, tariff) FROM stdin;
1	+7xxxxxxxxxx	beeline	the best
2	+7yyyyyyyyyy	mts	the worst
\.


--
-- TOC entry 3359 (class 0 OID 16399)
-- Dependencies: 214
-- Data for Name: data; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.data (id, temp, humidity, power, people, smoke, get_time, device_id) FROM stdin;
2	25	54	t	f	f	2023-09-19 15:57:05.633242	0
4	25	54	f	f	f	2023-09-19 16:04:50.730407	0
8	25	54	f	f	t	2023-09-20 02:24:28.265647	0
9	25	54	f	f	t	2023-09-20 02:25:10.347983	0
11	25	54	f	f	t	2023-11-09 11:09:21.292475	0
14	25	54	f	f	t	2023-11-27 16:26:04.537406	0
3	25	54	f	f	t	2023-09-19 16:02:38.997125	0
5	25	54	t	f	f	2023-09-19 16:06:35.686218	0
6	25	54	f	t	t	2023-09-20 02:19:22.105164	0
7	25	54	t	f	f	2023-09-20 02:24:10.095547	0
10	25	54	t	t	f	2023-10-31 14:38:50.218746	0
13	25	54	t	f	t	2023-11-14 23:56:17.464297	0
\.


--
-- TOC entry 3362 (class 0 OID 24595)
-- Dependencies: 217
-- Data for Name: device; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.device (device_id, name, channel_id) FROM stdin;
0	Main device	1
1	Future device	2
\.


--
-- TOC entry 3365 (class 0 OID 24615)
-- Dependencies: 220
-- Data for Name: device_to_user; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.device_to_user (user_id, device_id, info) FROM stdin;
22	0	\N
\.


--
-- TOC entry 3369 (class 0 OID 24646)
-- Dependencies: 224
-- Data for Name: transactions; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.transactions (transaction_id, sender_id, receiver_id, amount, transaction_date) FROM stdin;
1	1	2	100.00	2023-09-20 01:31:01.141672
2	1	2	100.00	2023-09-20 01:31:15.292898
3	1	2	100.00	2023-09-20 01:31:39.415862
4	1	2	100.00	2023-09-20 01:34:39.776838
5	1	2	100.00	2023-09-20 01:36:18.523269
\.


--
-- TOC entry 3364 (class 0 OID 24603)
-- Dependencies: 219
-- Data for Name: user; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."user" (user_id, login, password, is_admin) FROM stdin;
1	admin	8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918	t
60	user	changeme	f
22	test	9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08	f
\.


--
-- TOC entry 3376 (class 0 OID 0)
-- Dependencies: 221
-- Name: channel_channel_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.channel_channel_id_seq', 2, true);


--
-- TOC entry 3377 (class 0 OID 0)
-- Dependencies: 215
-- Name: data_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.data_id_seq', 14, true);


--
-- TOC entry 3378 (class 0 OID 0)
-- Dependencies: 216
-- Name: device_device_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.device_device_id_seq', 1, true);


--
-- TOC entry 3379 (class 0 OID 0)
-- Dependencies: 223
-- Name: transactions_transaction_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.transactions_transaction_id_seq', 5, true);


--
-- TOC entry 3380 (class 0 OID 0)
-- Dependencies: 218
-- Name: user_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_user_id_seq', 60, true);


--
-- TOC entry 3210 (class 2606 OID 24637)
-- Name: channel channel_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.channel
    ADD CONSTRAINT channel_pkey PRIMARY KEY (channel_id);


--
-- TOC entry 3204 (class 2606 OID 24731)
-- Name: user constraintLogin; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT "constraintLogin" UNIQUE (login);


--
-- TOC entry 3200 (class 2606 OID 16403)
-- Name: data data_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.data
    ADD CONSTRAINT data_pkey PRIMARY KEY (id);


--
-- TOC entry 3202 (class 2606 OID 24601)
-- Name: device device_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.device
    ADD CONSTRAINT device_pkey PRIMARY KEY (device_id);


--
-- TOC entry 3208 (class 2606 OID 24619)
-- Name: device_to_user pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.device_to_user
    ADD CONSTRAINT pk PRIMARY KEY (user_id, device_id);


--
-- TOC entry 3212 (class 2606 OID 24651)
-- Name: transactions transactions_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transactions
    ADD CONSTRAINT transactions_pkey PRIMARY KEY (transaction_id);


--
-- TOC entry 3206 (class 2606 OID 24609)
-- Name: user user_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT user_pkey PRIMARY KEY (user_id);


--
-- TOC entry 3214 (class 2606 OID 24638)
-- Name: device channel_in_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.device
    ADD CONSTRAINT channel_in_fkey FOREIGN KEY (channel_id) REFERENCES public.channel(channel_id) NOT VALID;


--
-- TOC entry 3215 (class 2606 OID 24625)
-- Name: device_to_user device_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.device_to_user
    ADD CONSTRAINT device_id_fkey FOREIGN KEY (device_id) REFERENCES public.device(device_id);


--
-- TOC entry 3213 (class 2606 OID 24610)
-- Name: data device_id_pkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.data
    ADD CONSTRAINT device_id_pkey FOREIGN KEY (device_id) REFERENCES public.device(device_id) NOT VALID;


--
-- TOC entry 3216 (class 2606 OID 24620)
-- Name: device_to_user user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.device_to_user
    ADD CONSTRAINT user_id_fkey FOREIGN KEY (user_id) REFERENCES public."user"(user_id);


-- Completed on 2023-12-09 23:11:33

--
-- PostgreSQL database dump complete
--

