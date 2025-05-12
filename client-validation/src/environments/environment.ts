import {Environment} from "./environment.interface";

export const environment: Environment = {
  production: false,
  apiUrl: "http://localhost:5001", // change when deploy.
  featureFlag: true,
  filterConfig: [
    {
      type: 'selectMany',
      key: 'auditType',
      label: 'Audit Type',
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
      type: 'text',
      key: 'hostname',
      label: 'Назва АП в антивірусі',
    },
    {
      type: 'selectMany',
      key: 'domain',
      label: 'Домен',
      options: [
        { value: 'localhost', label: 'Localhost' },
        { value: 'test.com', label: 'Test Domain' },
        { value: "", label: "Усі"}
      ]
    }
  ]
}
