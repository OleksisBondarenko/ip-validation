export interface ValidationDto {
auditCode: string;
message: string;
body: ValidationDataDto
}

export interface ValidationDataDto {
  domain: string;
  hostname: string;
  ipAddress: string;
  message: string;
  resourceName: string;
  userName: string;
}
