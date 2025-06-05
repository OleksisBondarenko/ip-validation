
export interface AuditTypeInfo {
  type: number;
  name: string;
  status: AuditTypeStatus;
  displayName: string;
  isAllow: boolean;
}

export enum AuditTypeStatus {
  Ok = 0,
  Alert = 1,
  Danger = 2
}

