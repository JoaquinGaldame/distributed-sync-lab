-- OTA A: esquema relacional.
-- Ejecutar conectado a ota_a_simulator_db.
-- La transacción se controla mediante psql --single-transaction.

-- 1. CATÁLOGOS

CREATE TABLE property_types (
    id serial PRIMARY KEY,
    code varchar(100) NOT NULL
);

CREATE TABLE unit_types (
    id serial PRIMARY KEY,
    code varchar(40) UNIQUE NOT NULL
);

CREATE TABLE facilities (
    id serial PRIMARY KEY,
    code varchar(100) UNIQUE NOT NULL,
    name varchar(255) NOT NULL
);

CREATE TABLE unit_facility_types (
    id serial PRIMARY KEY,
    code varchar(100) UNIQUE NOT NULL,
    name varchar(255) NOT NULL
);

CREATE TABLE bathroom_configurations (
    id serial PRIMARY KEY,
    code varchar(100) UNIQUE NOT NULL
);

CREATE TABLE languages (
    id serial PRIMARY KEY,
    code varchar(5) UNIQUE NOT NULL,
    name varchar(100) NOT NULL
);


CREATE TABLE currency_types (
    id serial PRIMARY KEY,
    code varchar(3) UNIQUE NOT NULL CHECK (code ~ '^[A-Z]{3}$')
);


CREATE TABLE profiles_types (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    code varchar(50) UNIQUE NOT NULL,
    name varchar(150) NOT NULL,
    required boolean NOT NULL DEFAULT false
);


-- 2. PROPIEDADES

CREATE TABLE properties (
    id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    external_code varchar(60) NOT NULL UNIQUE,
    name varchar(200) NOT NULL CHECK (length(trim(name)) > 0),
    property_type_id integer NOT NULL REFERENCES property_types(id),
    currency_id integer NOT NULL REFERENCES currency_types(id),
    primary_language integer NOT NULL REFERENCES languages(id),
    timezone varchar(100) NOT NULL,
    status varchar(20) NOT NULL DEFAULT 'draft' CHECK (status IN ('draft', 'active', 'inactive')),
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE property_locations (
    property_id bigint PRIMARY KEY REFERENCES properties(id) ON DELETE CASCADE,
    country_code varchar(2) NOT NULL CHECK (country_code ~ '^[A-Z]{2}$'),
    region varchar(150),
    city varchar(150) NOT NULL,
    neighbourhood varchar(150),
    address_line varchar(250),
    postal_code varchar(30),
    latitude numeric(9,6),
    longitude numeric(9,6),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_at timestamptz NOT NULL DEFAULT now(),
    CHECK (latitude BETWEEN -90 AND 90),
    CHECK (longitude BETWEEN -180 AND 180),
    CHECK (
        (latitude IS NULL AND longitude IS NULL)
        OR (latitude IS NOT NULL AND longitude IS NOT NULL)
    )
);


CREATE TABLE property_contacts (
    id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    property_id bigint NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    profile_type_id integer NOT NULL REFERENCES profiles_types(id),
    name varchar(150),
    email varchar(254),
    phone varchar(50),
    language_id integer NOT NULL REFERENCES languages(id),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_at timestamptz NOT NULL DEFAULT now(),
    UNIQUE (property_id, profile_type_id),
    CHECK (
        NULLIF(trim(email), '') IS NOT NULL
        OR NULLIF(trim(phone), '') IS NOT NULL
    )
);

CREATE TABLE property_descriptions (
    property_id bigint NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    language_id integer NOT NULL REFERENCES languages(id),
    description text NOT NULL CHECK (length(trim(description)) > 0),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (property_id, language_id)
);


CREATE TABLE property_facilities (
    property_id bigint NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    facility_code varchar(100) NOT NULL REFERENCES facilities(code),
    PRIMARY KEY (property_id, facility_code)
);


-- 3. UNIDADES

CREATE TABLE units (
    id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    property_id bigint NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    external_code varchar(60) NOT NULL,
    name varchar(200) NOT NULL CHECK (length(trim(name)) > 0),
    unit_type_id integer NOT NULL REFERENCES unit_types(id),
    total_units integer NOT NULL CHECK (total_units > 0),
    maximum_occupancy integer NOT NULL CHECK (maximum_occupancy > 0),
    maximum_adults integer NOT NULL CHECK (maximum_adults > 0),
    maximum_children integer NOT NULL DEFAULT 0 CHECK (maximum_children >= 0),
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    UNIQUE (property_id, id),
    UNIQUE (property_id, external_code),
    CHECK (maximum_adults <= maximum_occupancy),
    CHECK (maximum_children <= maximum_occupancy)
);

CREATE TABLE unit_bathrooms (
    id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    unit_id bigint NOT NULL REFERENCES units(id) ON DELETE CASCADE,
    bathroom_configuration_id integer NOT NULL REFERENCES bathroom_configurations(id),
    scope varchar(20) NOT NULL CHECK (scope IN ('PRIVATE', 'SHARED')),
    quantity integer NOT NULL DEFAULT 1 CHECK (quantity > 0),
    UNIQUE (unit_id, bathroom_configuration_id, scope)
);

CREATE TABLE unit_facilities (
    unit_id bigint NOT NULL REFERENCES units(id) ON DELETE CASCADE,
    unit_facility_type_id integer NOT NULL REFERENCES unit_facility_types(id),
    PRIMARY KEY (unit_id, unit_facility_type_id)
);


-- 4. POLÍTICAS

CREATE TABLE stay_policies (
    property_id bigint PRIMARY KEY REFERENCES properties(id) ON DELETE CASCADE,
    check_in_from time NOT NULL,
    check_in_until time NOT NULL,
    check_out_from time NOT NULL,
    check_out_until time NOT NULL,
    minimum_guest_age integer NOT NULL DEFAULT 18 CHECK (minimum_guest_age >= 0),
    children_allowed boolean NOT NULL DEFAULT true,
    pets_allowed boolean NOT NULL DEFAULT false,
    smoking_allowed boolean NOT NULL DEFAULT false,
    parties_allowed boolean NOT NULL DEFAULT false,
    updated_at timestamptz NOT NULL DEFAULT now()
);

-- Si free_until_days_before es NULL, no existe plazo gratuito.
-- Fuera del plazo gratuito se aplica penalty_percent.

CREATE TABLE cancellation_policies (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    code varchar(60) NOT NULL UNIQUE,
    name varchar(150) NOT NULL CHECK (length(trim(name)) > 0),
    free_until_days_before integer CHECK (free_until_days_before >= 0),
    penalty_percent numeric(5,2) NOT NULL CHECK (penalty_percent BETWEEN 0 AND 100)
);

-- 5. CARGOS
-- Los importes fijos utilizan la moneda de la propiedad.
-- percentage expresa un porcentaje sobre el alojamiento,
-- excluyendo otros cargos.

CREATE TABLE property_charges (
    id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    property_id bigint NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    code varchar(60) NOT NULL,
    name varchar(150) NOT NULL,
    calculation varchar(30) NOT NULL CHECK (
        calculation IN (
            'per_stay',
            'per_night',
            'per_person_per_night',
            'percentage'
        )
    ),
    amount numeric(12,2) NOT NULL CHECK (amount >= 0),
    included_in_price boolean NOT NULL DEFAULT false,
    UNIQUE (property_id, code),
    CHECK (calculation <> 'percentage' OR amount <= 100)
);


-- 6. PLANES TARIFARIOS
-- La unidad y la política deben pertenecer a la misma propiedad.

CREATE TABLE rate_plans (
    id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    property_id bigint NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    unit_id bigint NOT NULL,
    external_code varchar(60) NOT NULL,
    name varchar(150) NOT NULL CHECK (length(trim(name)) > 0),
    cancellation_policy_id integer NOT NULL REFERENCES cancellation_policies(id),
    minimum_nights integer NOT NULL DEFAULT 1 CHECK (minimum_nights > 0),
    maximum_nights integer,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    FOREIGN KEY (property_id, unit_id) REFERENCES units(property_id, id) ON DELETE CASCADE,
    UNIQUE (property_id, external_code),
    CHECK (
        maximum_nights IS NULL
        OR maximum_nights >= minimum_nights
    )
);

-- 7. INVENTARIO DIARIO
-- Se comparte entre todos los planes tarifarios de la unidad.
-- Ausencia de registro significa inventario no configurado.

CREATE TABLE unit_inventory (
    unit_id bigint NOT NULL REFERENCES units(id) ON DELETE CASCADE,
    stay_date date NOT NULL,
    available_units integer NOT NULL CHECK (available_units >= 0),
    closed boolean NOT NULL DEFAULT false,
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (unit_id, stay_date)
);

-- 8. PRECIOS Y RESTRICCIONES DIARIOS
-- Precio por noche de la unidad completa, en la moneda de la propiedad.
-- Ausencia de registro significa tarifa no configurada.
-- Un override NULL utiliza la restricción del plan.

CREATE TABLE rate_calendar (
    rate_plan_id bigint NOT NULL REFERENCES rate_plans(id) ON DELETE CASCADE,
    stay_date date NOT NULL,
    nightly_price numeric(12,2) NOT NULL CHECK (nightly_price > 0),
    closed boolean NOT NULL DEFAULT false,
    minimum_nights_override integer CHECK (minimum_nights_override > 0),
    maximum_nights_override integer CHECK (maximum_nights_override > 0),
    closed_to_arrival boolean NOT NULL DEFAULT false,
    closed_to_departure boolean NOT NULL DEFAULT false,
    updated_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (rate_plan_id, stay_date),
    CHECK (
        minimum_nights_override IS NULL
        OR maximum_nights_override IS NULL
        OR maximum_nights_override >= minimum_nights_override
    )
);

-- 9. RESTRICCIÓN DE CAPACIDAD
-- Serializa cambios de inventario sobre la misma unidad.

CREATE FUNCTION validate_inventory_capacity()
RETURNS trigger
LANGUAGE plpgsql
AS $$
DECLARE
    capacity integer;
BEGIN
    SELECT total_units INTO capacity
    FROM units
    WHERE id = NEW.unit_id
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Unidad inexistente: %', NEW.unit_id;
    END IF;

    IF NEW.available_units > capacity THEN
        RAISE EXCEPTION
            'Inventario % supera capacidad % de unidad %',
            NEW.available_units, capacity, NEW.unit_id;
    END IF;

    RETURN NEW;
END;
$$;

CREATE TRIGGER validate_inventory_capacity
BEFORE INSERT OR UPDATE ON unit_inventory
FOR EACH ROW EXECUTE FUNCTION validate_inventory_capacity();



CREATE FUNCTION validate_unit_capacity_change()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    IF NEW.total_units < OLD.total_units AND EXISTS (
        SELECT 1 FROM unit_inventory
        WHERE unit_id = NEW.id
          AND available_units > NEW.total_units
    ) THEN
        RAISE EXCEPTION
            'La capacidad propuesta contradice el inventario de unidad %',
            NEW.id;
    END IF;

    RETURN NEW;
END;
$$;

CREATE TRIGGER validate_unit_capacity_change
BEFORE UPDATE OF total_units ON units
FOR EACH ROW EXECUTE FUNCTION validate_unit_capacity_change();


-- Valida las restricciones diarias contra su plan tarifario.

CREATE FUNCTION validate_rate_calendar_stay_limits()
RETURNS trigger
LANGUAGE plpgsql
AS $$
DECLARE
    plan_min integer;
    plan_max integer;
    effective_min integer;
    effective_max integer;
BEGIN
    SELECT minimum_nights, maximum_nights
    INTO plan_min, plan_max
    FROM rate_plans
    WHERE id = NEW.rate_plan_id
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION
            'Plan tarifario inexistente: %', NEW.rate_plan_id;
    END IF;

    effective_min :=
        COALESCE(NEW.minimum_nights_override, plan_min);

    effective_max :=
        COALESCE(NEW.maximum_nights_override, plan_max);

    IF effective_max IS NOT NULL
       AND effective_max < effective_min THEN
        RAISE EXCEPTION
            'Restricciones inválidas para plan %, fecha %: mínimo %, máximo %',
            NEW.rate_plan_id,
            NEW.stay_date,
            effective_min,
            effective_max;
    END IF;

    RETURN NEW;
END;
$$;

CREATE TRIGGER validate_rate_calendar_stay_limits
BEFORE INSERT OR UPDATE ON rate_calendar
FOR EACH ROW
EXECUTE FUNCTION validate_rate_calendar_stay_limits();


-- Impide modificar un plan dejando restricciones diarias inválidas.

CREATE FUNCTION validate_rate_plan_stay_limits()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM rate_calendar AS calendar
        WHERE calendar.rate_plan_id = NEW.id
          AND COALESCE(
                calendar.maximum_nights_override,
                NEW.maximum_nights
              ) IS NOT NULL
          AND COALESCE(
                calendar.maximum_nights_override,
                NEW.maximum_nights
              ) < COALESCE(
                calendar.minimum_nights_override,
                NEW.minimum_nights
              )
    ) THEN
        RAISE EXCEPTION
            'Las restricciones propuestas contradicen el calendario del plan %',
            NEW.id;
    END IF;

    RETURN NEW;
END;
$$;

CREATE TRIGGER validate_rate_plan_stay_limits
BEFORE UPDATE OF minimum_nights, maximum_nights ON rate_plans
FOR EACH ROW
EXECUTE FUNCTION validate_rate_plan_stay_limits();



-- 10. MARCAS DE ACTUALIZACIÓN

CREATE FUNCTION touch_updated_at()
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
        'properties',
        'property_locations',
        'property_contacts',
        'property_descriptions',
        'units',
        'stay_policies',
        'rate_plans',
        'unit_inventory',
        'rate_calendar'
    ]
    LOOP
        EXECUTE format(
            'CREATE TRIGGER touch_updated_at
             BEFORE UPDATE ON %I
             FOR EACH ROW EXECUTE FUNCTION touch_updated_at()',
            target_table
        );
    END LOOP;
END;
$$;



-- 11. ÍNDICES

CREATE INDEX ix_units_property ON units(property_id);

CREATE INDEX ix_rate_plans_unit ON rate_plans(unit_id);

CREATE INDEX ix_rate_plans_cancellation_policy
    ON rate_plans(property_id, cancellation_policy_id);

CREATE INDEX ix_inventory_date ON unit_inventory(stay_date);

CREATE INDEX ix_rate_calendar_date ON rate_calendar(stay_date);
