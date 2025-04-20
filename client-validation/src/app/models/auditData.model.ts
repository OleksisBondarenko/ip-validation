export default interface AuditRecordModel {
  id: string;
  auditType: number,
  auditTypeString: string,
  auditData: AuditDataModel,
  timeStamp: string,
}

export interface ResponseGetListAudit
{
  data: AuditRecordModel [],
  totalCount: number,
}
export interface AuditDataModel {
  ip: string;
  ipAddress: string;
  hostName: string;
  domain: string;
}
