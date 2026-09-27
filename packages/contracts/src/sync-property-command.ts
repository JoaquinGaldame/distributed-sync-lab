export interface PropertyState {
  name: string;
  price: number;
}

export type SyncTarget =
  | "ota-replace"
  | "ota-async";

export interface SyncPropertyCommand {
  experimentId?: string;
  jobId: string;

  propertyId: string;
  version: number;

  target: SyncTarget;

  idempotencyKey: string;

  desiredState: PropertyState;
}