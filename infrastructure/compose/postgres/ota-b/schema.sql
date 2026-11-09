-- OTA B: modelo propio basado en anuncios individuales.
-- Base: ota_b_simulator_db. Esquema: public.
-- Ejecutar una sola vez en una base nueva con:
-- psql -v ON_ERROR_STOP=1 --single-transaction -f schema.sql
-- No requiere extensiones ni migraciones de EF Core.

-- ============================================================================
-- 1. CATÁLOGOS: claves comerciales estables, independientes de los IDs de OTA A
-- ============================================================================

CREATE TABLE public.currencies (
    id serial PRIMARY KEY,
    code varchar(3) NOT NULL UNIQUE CHECK (code ~ '^[A-Z]{3}$'),
    name varchar(100) NOT NULL CHECK (length(btrim(name)) > 0),
    decimal_places smallint NOT NULL CHECK (decimal_places BETWEEN 0 AND 4)
);

CREATE TABLE public.languages (
    id serial PRIMARY KEY,
    code varchar(15) NOT NULL UNIQUE CHECK (length(btrim(code)) > 0),
    name varchar(100) NOT NULL CHECK (length(btrim(name)) > 0)
);

CREATE TABLE public.listing_categories (
    id serial PRIMARY KEY,
    code varchar(40) NOT NULL UNIQUE,
    name varchar(100) NOT NULL CHECK (length(btrim(name)) > 0)
);

CREATE TABLE public.rental_modes (
    code varchar(30) PRIMARY KEY,
    name varchar(100) NOT NULL CHECK (length(btrim(name)) > 0)
);

CREATE TABLE public.amenity_groups (
    code varchar(30) PRIMARY KEY,
    name varchar(100) NOT NULL CHECK (length(btrim(name)) > 0)
);

CREATE TABLE public.amenities (
    id serial PRIMARY KEY,
    code varchar(80) NOT NULL UNIQUE,
    group_code varchar(30) NOT NULL REFERENCES public.amenity_groups(code),
    name varchar(150) NOT NULL CHECK (length(btrim(name)) > 0)
);

CREATE TABLE public.contact_roles (
    code varchar(30) PRIMARY KEY,
    name varchar(100) NOT NULL CHECK (length(btrim(name)) > 0)
);

CREATE TABLE public.listing_statuses (
    id serial PRIMARY KEY,
    code varchar(20) NOT NULL UNIQUE,
    name varchar(100) NOT NULL CHECK (length(btrim(name)) > 0)
);

CREATE TABLE public.room_types (
    id serial PRIMARY KEY,
    code varchar(40) NOT NULL UNIQUE,
    name varchar(100) NOT NULL CHECK (length(btrim(name)) > 0)
);

CREATE TABLE public.bed_types (
    id serial PRIMARY KEY,
    code varchar(40) NOT NULL UNIQUE,
    name varchar(100) NOT NULL CHECK (length(btrim(name)) > 0)
);

CREATE TABLE public.availability_states (
    code varchar(20) PRIMARY KEY,
    name varchar(100) NOT NULL CHECK (length(btrim(name)) > 0),
    is_open boolean NOT NULL
);

-- Antes del plazo: reembolso del 100%. Después: refund_percent_after_deadline.
-- free_cancellation_days_before NULL significa que no existe un plazo gratuito.
-- Con NULL y reembolso 100%, no se impone penalización en ningún momento.
-- OTA B expresa REEMBOLSO, mientras que OTA A expresa PENALIZACIÓN.
CREATE TABLE public.cancellation_terms (
    code varchar(60) PRIMARY KEY,
    name varchar(150) NOT NULL CHECK (length(btrim(name)) > 0),
    free_cancellation_days_before integer CHECK (free_cancellation_days_before >= 0),
    refund_percent_after_deadline numeric(5,2) NOT NULL
        CHECK (refund_percent_after_deadline BETWEEN 0 AND 100)
);

CREATE TABLE public.fee_bases (
    code varchar(30) PRIMARY KEY,
    name varchar(100) NOT NULL CHECK (length(btrim(name)) > 0)
);

-- ============================================================================
-- 2. ANUNCIOS
-- Un Listing representa UNA unidad rentable concreta, con inventario 0/1.
-- No existe una jerarquía local Property -> Unit -> RatePlan.
-- ============================================================================

CREATE TABLE public.listings (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    title varchar(200) NOT NULL CHECK (length(btrim(title)) > 0),
    category_id integer NOT NULL REFERENCES public.listing_categories(id),
    rental_mode_code varchar(30) NOT NULL REFERENCES public.rental_modes(code),
    currency_code varchar(3) NOT NULL REFERENCES public.currencies(code),
    primary_language_id integer NOT NULL REFERENCES public.languages(id),
    secondary_language_id integer REFERENCES public.languages(id),
    timezone varchar(100) NOT NULL CHECK (length(btrim(timezone)) > 0),
    status_id integer NOT NULL DEFAULT 1 REFERENCES public.listing_statuses(id),
    max_guests integer NOT NULL CHECK (max_guests > 0),
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE public.rooms (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    listing_id uuid NOT NULL REFERENCES public.listings(id) ON DELETE CASCADE,
    room_type_id integer NOT NULL REFERENCES public.room_types(id),
    name varchar(150) NOT NULL CHECK (length(btrim(name)) > 0),
    description text,
    area_m2 numeric(10,2) CHECK (area_m2 > 0),
    floor_number integer,
    is_shared boolean NOT NULL DEFAULT false,
    display_order integer NOT NULL DEFAULT 0 CHECK (display_order >= 0),
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE public.room_beds (
    room_id uuid NOT NULL REFERENCES public.rooms(id) ON DELETE CASCADE,
    bed_type_id integer NOT NULL REFERENCES public.bed_types(id),
    quantity integer NOT NULL CHECK (quantity > 0),
    PRIMARY KEY (room_id, bed_type_id)
);


CREATE TABLE public.locations (
    listing_id uuid PRIMARY KEY REFERENCES public.listings(id) ON DELETE CASCADE,
    country_code varchar(2) NOT NULL CHECK (country_code ~ '^[A-Z]{2}$'),
    region varchar(150),
    city varchar(150) NOT NULL CHECK (length(btrim(city)) > 0),
    neighbourhood varchar(150),
    address_line varchar(250),
    postal_code varchar(30),
    latitude numeric(9,6) CHECK (latitude BETWEEN -90 AND 90),
    longitude numeric(9,6) CHECK (longitude BETWEEN -180 AND 180),
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    CHECK ((latitude IS NULL) = (longitude IS NULL))
);

CREATE TABLE public.contacts (
    listing_id uuid NOT NULL REFERENCES public.listings(id) ON DELETE CASCADE,
    role_code varchar(30) NOT NULL REFERENCES public.contact_roles(code),
    display_name varchar(150),
    email varchar(254),
    phone varchar(50),
    language_code varchar(15) NOT NULL REFERENCES public.languages(code),
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (listing_id, role_code),
    CHECK (NULLIF(btrim(email), '') IS NOT NULL OR NULLIF(btrim(phone), '') IS NOT NULL)
);

CREATE TABLE public.listing_texts (
    listing_id uuid NOT NULL REFERENCES public.listings(id) ON DELETE CASCADE,
    language_id integer NOT NULL REFERENCES public.languages(id),
    summary varchar(500),
    description text NOT NULL CHECK (length(btrim(description)) > 0),
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (listing_id, language_id)
);

CREATE TABLE public.listing_amenities (
    listing_id uuid NOT NULL REFERENCES public.listings(id) ON DELETE CASCADE,
    amenity_id integer NOT NULL REFERENCES public.amenities(id),
    PRIMARY KEY (listing_id, amenity_id)
);

-- ============================================================================
-- 3. REGLAS: una configuración comercial por anuncio, sin planes tarifarios
-- ============================================================================

CREATE TABLE public.listing_house_rules (
    listing_id uuid PRIMARY KEY REFERENCES public.listings(id) ON DELETE CASCADE,
    check_in_from time NOT NULL,
    check_in_until time NOT NULL,
    check_out_from time NOT NULL,
    check_out_until time NOT NULL,
    minimum_guest_age integer NOT NULL DEFAULT 18 CHECK (minimum_guest_age >= 0),
    allows_children boolean NOT NULL DEFAULT true,
    allows_pets boolean NOT NULL DEFAULT false,
    allows_smoking boolean NOT NULL DEFAULT false,
    allows_parties boolean NOT NULL DEFAULT false,
    additional_rules text,
    updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE public.listing_booking_rules (
    listing_id uuid PRIMARY KEY REFERENCES public.listings(id) ON DELETE CASCADE,
    cancellation_term_code varchar(60) NOT NULL REFERENCES public.cancellation_terms(code),
    min_nights integer NOT NULL DEFAULT 1 CHECK (min_nights > 0),
    max_nights integer,
    updated_at timestamptz NOT NULL DEFAULT now(),
    CHECK (max_nights IS NULL OR max_nights >= min_nights)
);

-- ============================================================================
-- 4. PRECIOS: tarifa base heredable y excepciones por noche
-- Los importes monetarios son enteros en la unidad menor de la moneda del anuncio.
-- Con decimal_places = 2, 95000.00 se representa como 9500000.
-- La API no debe cambiar currency_code después de crear el anuncio.
-- ============================================================================

CREATE TABLE public.listing_pricing (
    listing_id uuid PRIMARY KEY REFERENCES public.listings(id) ON DELETE CASCADE,
    base_nightly_amount_minor bigint NOT NULL CHECK (base_nightly_amount_minor > 0),
    updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE public.listing_fees (
    listing_id uuid NOT NULL REFERENCES public.listings(id) ON DELETE CASCADE,
    code varchar(60) NOT NULL CHECK (length(btrim(code)) > 0),
    name varchar(150) NOT NULL CHECK (length(btrim(name)) > 0),
    basis_code varchar(30) NOT NULL REFERENCES public.fee_bases(code),
    amount_minor bigint,
    percent numeric(5,2),
    is_included boolean NOT NULL DEFAULT false,
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (listing_id, code),
    CHECK (
        (basis_code = 'LODGING_PERCENT'
            AND amount_minor IS NULL
            AND percent IS NOT NULL AND percent BETWEEN 0 AND 100)
        OR
        (basis_code <> 'LODGING_PERCENT'
            AND amount_minor IS NOT NULL AND amount_minor >= 0
            AND percent IS NULL)
    )
);

-- Una fila sustituye la tarifa base únicamente para esa fecha.
-- Si no existe fila, se hereda listing_pricing.base_nightly_amount_minor.
-- Si tampoco existe listing_pricing, el precio está sin configurar.
CREATE TABLE public.listing_daily_prices (
    listing_id uuid NOT NULL REFERENCES public.listings(id) ON DELETE CASCADE,
    stay_date date NOT NULL,
    amount_minor bigint NOT NULL CHECK (amount_minor > 0),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (listing_id, stay_date)
);

-- ============================================================================
-- 5. DISPONIBILIDAD: calendario independiente del de precios
-- OPEN: reservable; BLOCKED: cierre/no disponible; BOOKED: reservado.
-- No hay available_units ni capacidad de inventario compartida.
-- Ausencia de fila = UNCONFIGURED en la consulta; nunca se asume OPEN.
-- UNCONFIGURED es un estado de respuesta, no un catálogo persistible.
-- ============================================================================

CREATE TABLE public.listing_daily_availability (
    listing_id uuid NOT NULL REFERENCES public.listings(id) ON DELETE CASCADE,
    stay_date date NOT NULL,
    state_code varchar(20) NOT NULL REFERENCES public.availability_states(code),
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (listing_id, stay_date)
);

-- ============================================================================
-- 6. MARCAS DE ACTUALIZACIÓN
-- Única automatización por trigger: marcas de tiempo.
-- La integridad restante utiliza PK, FK y CHECK de la misma fila.
-- ============================================================================

CREATE FUNCTION public.touch_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at := clock_timestamp();
    RETURN NEW;
END;
$$;

DO $$
DECLARE
    target_table text;
BEGIN
    FOREACH target_table IN ARRAY ARRAY[
        'listings', 'locations', 'contacts', 'listing_texts',
        'listing_house_rules', 'listing_booking_rules', 'listing_pricing',
        'listing_fees', 'listing_daily_prices', 'listing_daily_availability'
    ]
    LOOP
        EXECUTE format(
            'CREATE TRIGGER touch_updated_at BEFORE UPDATE ON public.%I '
            'FOR EACH ROW EXECUTE FUNCTION public.touch_updated_at()',
            target_table
        );
    END LOOP;
END;
$$;


CREATE TRIGGER touch_updated_at
BEFORE UPDATE ON public.rooms
FOR EACH ROW EXECUTE FUNCTION public.touch_updated_at();


-- ============================================================================
-- 7. ÍNDICES
-- Las PK de calendarios ya cubren las consultas por anuncio y rango de fechas.
-- ============================================================================

CREATE INDEX ix_listings_status ON public.listings(status_id);
CREATE INDEX ix_rooms_listing ON public.rooms(listing_id);
CREATE INDEX ix_rooms_type ON public.rooms(room_type_id);
CREATE INDEX ix_room_beds_type ON public.room_beds(bed_type_id);
CREATE INDEX ix_listings_category ON public.listings(category_id);
CREATE INDEX ix_amenities_group ON public.amenities(group_code);
CREATE INDEX ix_listing_booking_rules_cancellation ON public.listing_booking_rules(cancellation_term_code);
CREATE INDEX ix_contacts_role ON public.contacts(role_code);
CREATE INDEX ix_listing_texts_language ON public.listing_texts(language_id);
CREATE INDEX ix_listing_amenities_amenity ON public.listing_amenities(amenity_id);
CREATE INDEX ix_listing_fees_basis ON public.listing_fees(basis_code);
CREATE INDEX ix_listing_daily_availability_state ON public.listing_daily_availability(state_code);
