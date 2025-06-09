export interface ValidationDto {
  auditCode: string;
  errorMessage: string;
  data: ValidationDataDto
}

export interface ValidationDataDto {
  domain: string;
  hostname: string;
  ipAddress: string;
  message: string;
  resourceName: string;
  userName: string;
}
