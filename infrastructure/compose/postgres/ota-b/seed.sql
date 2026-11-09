-- Catálogos propios de OTA B. No carga anuncios ni copia datos del PMS.
-- Ejecutar después de schema.sql con ON_ERROR_STOP=1 --single-transaction.

INSERT INTO public.currencies (code, name, decimal_places) VALUES
    ('ARS', 'Peso argentino', 2),
    ('USD', 'Dólar estadounidense', 2),
    ('EUR', 'Euro', 2),
    ('BRL', 'Real brasileño', 2),
    ('CLP', 'Peso chileno', 0),
    ('JPY', 'Yen japonés', 0);

INSERT INTO public.languages (code, name) VALUES
    ('es', 'Español'),
    ('es-AR', 'Español de Argentina'),
    ('en-US', 'Inglés de Estados Unidos'),
    ('en-GB', 'Inglés del Reino Unido'),
    ('pt-BR', 'Portugués de Brasil'),
    ('fr', 'Francés'),
    ('de', 'Alemán'),
    ('it', 'Italiano');

INSERT INTO public.listing_categories (code, name) VALUES
    ('HOUSE', 'Casa'),
    ('APARTMENT', 'Departamento'),
    ('CABIN', 'Cabaña'),
    ('VILLA', 'Villa'),
    ('GUEST_SUITE', 'Suite de huéspedes'),
    ('HOTEL_ROOM', 'Habitación de hotel'),
    ('OTHER', 'Otro alojamiento');

INSERT INTO public.rental_modes (code, name) VALUES
    ('ENTIRE_PLACE', 'Alojamiento completo'),
    ('PRIVATE_SPACE', 'Espacio privado'),
    ('SHARED_SPACE', 'Espacio compartido');

INSERT INTO public.amenity_groups (code, name) VALUES
    ('CONNECTIVITY', 'Conectividad'),
    ('KITCHEN', 'Cocina'),
    ('CLIMATE', 'Climatización'),
    ('COMFORT', 'Comodidad'),
    ('OUTDOOR', 'Exterior'),
    ('RECREATION', 'Recreación'),
    ('PARKING', 'Estacionamiento'),
    ('SAFETY', 'Seguridad'),
    ('ACCESSIBILITY', 'Accesibilidad'),
    ('SERVICES', 'Servicios');

INSERT INTO public.amenities (code, group_code, name) VALUES
    ('wifi', 'CONNECTIVITY', 'Internet inalámbrico'),
    ('wired-internet', 'CONNECTIVITY', 'Internet por cable'),
    ('workspace', 'CONNECTIVITY', 'Espacio de trabajo'),
    ('kitchen', 'KITCHEN', 'Acceso a cocina'),
    ('private-kitchen', 'KITCHEN', 'Cocina privada'),
    ('shared-kitchen', 'KITCHEN', 'Cocina compartida'),
    ('kitchenette', 'KITCHEN', 'Kitchenette'),
    ('refrigerator', 'KITCHEN', 'Heladera'),
    ('microwave', 'KITCHEN', 'Microondas'),
    ('oven', 'KITCHEN', 'Horno'),
    ('stove', 'KITCHEN', 'Anafe'),
    ('dishwasher', 'KITCHEN', 'Lavavajillas'),
    ('coffee-maker', 'KITCHEN', 'Cafetera'),
    ('kettle', 'KITCHEN', 'Pava eléctrica'),
    ('cookware', 'KITCHEN', 'Utensilios de cocina'),
    ('dining-table', 'KITCHEN', 'Mesa de comedor'),
    ('air-conditioning', 'CLIMATE', 'Aire acondicionado'),
    ('heating', 'CLIMATE', 'Calefacción'),
    ('ceiling-fan', 'CLIMATE', 'Ventilador de techo'),
    ('indoor-fireplace', 'CLIMATE', 'Chimenea interior'),
    ('tv', 'COMFORT', 'Televisión'),
    ('washer', 'COMFORT', 'Lavarropas'),
    ('dryer', 'COMFORT', 'Secadora'),
    ('iron', 'COMFORT', 'Plancha'),
    ('hair-dryer', 'COMFORT', 'Secador de pelo'),
    ('towels', 'COMFORT', 'Toallas'),
    ('bed-linen', 'COMFORT', 'Ropa de cama'),
    ('crib', 'COMFORT', 'Cuna'),
    ('high-chair', 'COMFORT', 'Silla para bebé'),
    ('private-entrance', 'COMFORT', 'Entrada privada'),
    ('balcony', 'OUTDOOR', 'Balcón'),
    ('terrace', 'OUTDOOR', 'Terraza'),
    ('garden', 'OUTDOOR', 'Jardín'),
    ('bbq', 'OUTDOOR', 'Parrilla'),
    ('outdoor-furniture', 'OUTDOOR', 'Muebles de exterior'),
    ('pool-access', 'RECREATION', 'Acceso a piscina'),
    ('private-pool', 'RECREATION', 'Piscina privada'),
    ('shared-pool', 'RECREATION', 'Piscina compartida'),
    ('hot-tub', 'RECREATION', 'Hidromasaje'),
    ('gym-access', 'RECREATION', 'Acceso a gimnasio'),
    ('board-games', 'RECREATION', 'Juegos de mesa'),
    ('parking', 'PARKING', 'Estacionamiento disponible'),
    ('free-parking', 'PARKING', 'Estacionamiento gratuito'),
    ('paid-parking', 'PARKING', 'Estacionamiento de pago'),
    ('smoke-alarm', 'SAFETY', 'Detector de humo'),
    ('carbon-monoxide-alarm', 'SAFETY', 'Detector de monóxido de carbono'),
    ('fire-extinguisher', 'SAFETY', 'Extintor'),
    ('first-aid-kit', 'SAFETY', 'Botiquín'),
    ('step-free-entry', 'ACCESSIBILITY', 'Entrada sin escalones'),
    ('elevator', 'ACCESSIBILITY', 'Ascensor'),
    ('wheelchair-access', 'ACCESSIBILITY', 'Acceso para silla de ruedas'),
    ('reception', 'SERVICES', 'Recepción'),
    ('luggage-storage', 'SERVICES', 'Guardaequipaje'),
    ('airport-transfer', 'SERVICES', 'Traslado al aeropuerto'),
    ('breakfast', 'SERVICES', 'Desayuno disponible');

INSERT INTO public.contact_roles (code, name) VALUES
    ('HOST', 'Contacto principal'),
    ('BOOKING', 'Reservas'),
    ('BILLING', 'Facturación'),
    ('CALENDAR', 'Disponibilidad'),
    ('AGENCY', 'Central de reservas');

INSERT INTO public.listing_statuses (id, code, name) VALUES
    (1,'DRAFT', 'Borrador'),
    (2,'LIVE', 'Publicado'),
    (3, 'HIDDEN', 'Oculto'),
    (4, 'ARCHIVED', 'Archivado');

INSERT INTO public.room_types (code, name) VALUES
    ('BEDROOM', 'Dormitorio'),
    ('BATHROOM', 'Baño'),
    ('LIVING_ROOM', 'Sala de estar'),
    ('KITCHEN', 'Cocina'),
    ('DINING_ROOM', 'Comedor'),
    ('STUDIO', 'Ambiente integrado'),
    ('WORKSPACE', 'Espacio de trabajo'),
    ('LAUNDRY', 'Lavadero'),
    ('OTHER', 'Otro espacio');

INSERT INTO public.bed_types (code, name) VALUES
    ('SINGLE', 'Cama individual'),
    ('DOUBLE', 'Cama doble'),
    ('QUEEN', 'Cama queen'),
    ('KING', 'Cama king'),
    ('BUNK_BED', 'Litera'),
    ('SOFA_BED', 'Sofá cama'),
    ('CRIB', 'Cuna'),
    ('OTHER', 'Otro tipo de cama');

INSERT INTO public.availability_states (code, name, is_open) VALUES
    ('OPEN', 'Reservable', true),
    ('BLOCKED', 'Cerrado o no disponible', false),
    ('BOOKED', 'Reservado', false);

INSERT INTO public.cancellation_terms (code, name, free_cancellation_days_before, refund_percent_after_deadline) VALUES
    ('NO_REFUND', 'Sin reembolso', NULL, 0),
    ('HALF_REFUND', 'Reembolso del 50% desde la contratación', NULL, 50),
    ('FULL_REFUND', 'Reembolso del 100% sin penalización', NULL, 100),
    ('FREE_1D_THEN_REFUND_0', 'Gratuita hasta 1 días; después reembolso del 0%', 1, 0),
    ('FREE_1D_THEN_REFUND_50', 'Gratuita hasta 1 días; después reembolso del 50%', 1, 50),
    ('FREE_2D_THEN_REFUND_0', 'Gratuita hasta 2 días; después reembolso del 0%', 2, 0),
    ('FREE_2D_THEN_REFUND_50', 'Gratuita hasta 2 días; después reembolso del 50%', 2, 50),
    ('FREE_3D_THEN_REFUND_0', 'Gratuita hasta 3 días; después reembolso del 0%', 3, 0),
    ('FREE_3D_THEN_REFUND_50', 'Gratuita hasta 3 días; después reembolso del 50%', 3, 50),
    ('FREE_4D_THEN_REFUND_0', 'Gratuita hasta 4 días; después reembolso del 0%', 4, 0),
    ('FREE_4D_THEN_REFUND_50', 'Gratuita hasta 4 días; después reembolso del 50%', 4, 50),
    ('FREE_5D_THEN_REFUND_0', 'Gratuita hasta 5 días; después reembolso del 0%', 5, 0),
    ('FREE_5D_THEN_REFUND_50', 'Gratuita hasta 5 días; después reembolso del 50%', 5, 50),
    ('FREE_6D_THEN_REFUND_0', 'Gratuita hasta 6 días; después reembolso del 0%', 6, 0),
    ('FREE_6D_THEN_REFUND_50', 'Gratuita hasta 6 días; después reembolso del 50%', 6, 50),
    ('FREE_7D_THEN_REFUND_0', 'Gratuita hasta 7 días; después reembolso del 0%', 7, 0),
    ('FREE_7D_THEN_REFUND_50', 'Gratuita hasta 7 días; después reembolso del 50%', 7, 50),
    ('FREE_8D_THEN_REFUND_0', 'Gratuita hasta 8 días; después reembolso del 0%', 8, 0),
    ('FREE_8D_THEN_REFUND_50', 'Gratuita hasta 8 días; después reembolso del 50%', 8, 50),
    ('FREE_10D_THEN_REFUND_0', 'Gratuita hasta 10 días; después reembolso del 0%', 10, 0),
    ('FREE_10D_THEN_REFUND_50', 'Gratuita hasta 10 días; después reembolso del 50%', 10, 50),
    ('FREE_14D_THEN_REFUND_0', 'Gratuita hasta 14 días; después reembolso del 0%', 14, 0),
    ('FREE_14D_THEN_REFUND_50', 'Gratuita hasta 14 días; después reembolso del 50%', 14, 50),
    ('FREE_15D_THEN_REFUND_0', 'Gratuita hasta 15 días; después reembolso del 0%', 15, 0),
    ('FREE_15D_THEN_REFUND_50', 'Gratuita hasta 15 días; después reembolso del 50%', 15, 50),
    ('FREE_20D_THEN_REFUND_0', 'Gratuita hasta 20 días; después reembolso del 0%', 20, 0),
    ('FREE_20D_THEN_REFUND_50', 'Gratuita hasta 20 días; después reembolso del 50%', 20, 50),
    ('FREE_21D_THEN_REFUND_0', 'Gratuita hasta 21 días; después reembolso del 0%', 21, 0),
    ('FREE_21D_THEN_REFUND_50', 'Gratuita hasta 21 días; después reembolso del 50%', 21, 50),
    ('FREE_28D_THEN_REFUND_0', 'Gratuita hasta 28 días; después reembolso del 0%', 28, 0),
    ('FREE_28D_THEN_REFUND_50', 'Gratuita hasta 28 días; después reembolso del 50%', 28, 50),
    ('FREE_30D_THEN_REFUND_0', 'Gratuita hasta 30 días; después reembolso del 0%', 30, 0),
    ('FREE_30D_THEN_REFUND_50', 'Gratuita hasta 30 días; después reembolso del 50%', 30, 50),
    ('FREE_31D_THEN_REFUND_0', 'Gratuita hasta 31 días; después reembolso del 0%', 31, 0),
    ('FREE_31D_THEN_REFUND_50', 'Gratuita hasta 31 días; después reembolso del 50%', 31, 50),
    ('FREE_42D_THEN_REFUND_0', 'Gratuita hasta 42 días; después reembolso del 0%', 42, 0),
    ('FREE_42D_THEN_REFUND_50', 'Gratuita hasta 42 días; después reembolso del 50%', 42, 50),
    ('FREE_45D_THEN_REFUND_0', 'Gratuita hasta 45 días; después reembolso del 0%', 45, 0),
    ('FREE_45D_THEN_REFUND_50', 'Gratuita hasta 45 días; después reembolso del 50%', 45, 50),
    ('FREE_60D_THEN_REFUND_0', 'Gratuita hasta 60 días; después reembolso del 0%', 60, 0),
    ('FREE_60D_THEN_REFUND_50', 'Gratuita hasta 60 días; después reembolso del 50%', 60, 50),
    ('FREE_90D_THEN_REFUND_0', 'Gratuita hasta 90 días; después reembolso del 0%', 90, 0),
    ('FREE_90D_THEN_REFUND_50', 'Gratuita hasta 90 días; después reembolso del 50%', 90, 50);

INSERT INTO public.fee_bases (code, name) VALUES
    ('STAY', 'Por estancia'),
    ('NIGHT', 'Por noche'),
    ('GUEST_NIGHT', 'Por huésped y noche'),
    ('LODGING_PERCENT', 'Porcentaje sobre alojamiento, excluidos otros cargos');
