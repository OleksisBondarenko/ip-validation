import {Environment} from "./environment.interface";

export const environment: Environment = {
  production: true,
  apiUrl: "http://localhost:5001",
  featureFlag: false,
  filterConfig: [
    {
      type: 'selectMany',
      key: 'auditType',
      label: 'Тип помилки',
      options: [
        { value: '0', label: 'Ok' },  // Ok
        { value: '1', label: 'Загальна помилка' }, // NotFound
        { value: '2', label: 'Не знайдено в домені' }, // NotFoundDomain
        { value: '3', label: 'Не знайдено в Eset' }, // NotFoundEset
        { value: '4', label: 'Відсутній агент Eset' }, // NotVlaidEsetTimespan
        { value: '10', label: 'Відсутнє з\'єднання з БД' }, // NoAccessToDB
      ]
    },
    {
      type: 'text',
      key: 'resourceName',
      label: 'Назва ресурсу',
    },
    {
      type: 'date',
      key: 'timestamp',
      label: 'Проміжок часу'
    },
    {
      type: 'text',
      key: 'ipAddress',
      label: 'Ip адреса'
    },
    {
      type: 'selectMany',
      key: 'domain',
      label: 'Домен',
      options: [
        { value: 'dpsu.dsk', label: 'DPSU' },
        { value: 'test.com', label: 'SED' },
        { value: "", label: "Усі"}
      ]
    }
  ]
}
