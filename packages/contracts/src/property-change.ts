export interface PropertyChangedEvent {
  eventId: string;
  eventType: "PropertyChanged";
  propertyId: string;
  version: number;
  occurredAt: string;
}