-- we don't know how to generate root <with-no-name> (class Root) :(

grant connect on database :: era_db to dbo
go

grant view any column encryption key definition, view any column master key definition on database :: era_db to [public]
go

create table tbl_computers
(
    ID            int identity
        primary key,
    computer_uuid varbinary(16) not null,
    computer_name nvarchar(255) not null
)
    go

create table tblf_network_ipaddresses_status
(
    ID         int identity
        primary key,
    Address    nvarchar(100) not null,
    MAC        nvarchar(100) not null,
    Occurred   datetime      not null,
    SourceUuid varbinary(16) not null
)
    go

use master
go

grant connect sql to ##MS_AgentSigningCertificate##
go

use era_db
go

use master
go

grant connect sql to ##MS_PolicyEventProcessingLogin##
go

use era_db
go

use master
go

grant control server, view any definition to ##MS_PolicySigningCertificate##
go

use era_db
go

use master
go

grant connect sql, view any definition, view server state to ##MS_PolicyTsqlExecutionLogin##
go

use era_db
go

use master
go

grant authenticate server to ##MS_SQLAuthenticatorCertificate##
go

use era_db
go

use master
go

grant authenticate server, view any definition, view server state to ##MS_SQLReplicationSigningCertificate##
go

use era_db
go

use master
go

grant view any definition to ##MS_SQLResourceSigningCertificate##
go

use era_db
go

use master
go

grant view any definition to ##MS_SmoExtendedSigningCertificate##
go

use era_db
go

use master
go

grant connect sql to [AD1\admin]
go

use era_db
go

use master
go

grant connect sql to [BUILTIN\Users]
go

use era_db
go

use master
go

grant alter any availability group, connect sql, view server state to [NT AUTHORITY\SYSTEM]
go

use era_db
go

use master
go

grant alter any event session, connect any database, connect sql, view any definition, view server state to [NT SERVICE\SQLTELEMETRY$SQLEXPRESS]
go

use era_db
go

use master
go

grant connect sql to [NT SERVICE\SQLWriter]
go

use era_db
go

use master
go

grant connect sql to [NT SERVICE\Winmgmt]
go

use era_db
go

use master
go

grant connect sql to [NT Service\MSSQL$SQLEXPRESS]
go

use era_db
go

use master
go

grant connect sql to admin
go

use era_db
go

use master
go

grant view any database to [public]
go

use era_db
go

use master
go

grant connect sql to sa
go

use era_db
go

