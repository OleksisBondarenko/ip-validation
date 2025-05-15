-- Create the database
CREATE DATABASE era_db;
GO

USE era_db;
GO

-- Table: tblf_network_ipaddresses_status
CREATE TABLE tblf_network_ipaddresses_status (
                                                 Address NVARCHAR(50) NOT NULL,
                                                 Mac NVARCHAR(50) NOT NULL,
                                                 Occurred DATETIME NOT NULL,
                                                 SourceUuid VARBINARY(16) NOT NULL, -- UUID in binary form
                                                 PRIMARY KEY (Address, Occurred)
);

-- Table: tbl_computers
CREATE TABLE tbl_computers (
                               computer_id INT IDENTITY(1,1) PRIMARY KEY,
                               computer_uuid VARBINARY(16) NOT NULL UNIQUE,
                               computer_name NVARCHAR(255) NOT NULL
);

-- Table: tbl_computers_aggr
CREATE TABLE tbl_computers_aggr (
                                    computer_id INT NOT NULL,
                                    computer_connected DATETIME NOT NULL,
                                    ip_address NVARCHAR(50) NOT NULL,
                                    PRIMARY KEY (computer_id, computer_connected),
                                    FOREIGN KEY (computer_id) REFERENCES tbl_computers(computer_id)
);
GO

-- Insert test data for multiple computers
DECLARE @computer1_uuid VARBINARY(16) = CONVERT(VARBINARY(16), NEWID());
DECLARE @computer2_uuid VARBINARY(16) = CONVERT(VARBINARY(16), NEWID());
DECLARE @computer3_uuid VARBINARY(16) = CONVERT(VARBINARY(16), NEWID());

-- Insert computers
INSERT INTO tbl_computers (computer_uuid, computer_name) VALUES (@computer1_uuid, 'COMPUTER-01.ad1.org');
INSERT INTO tbl_computers (computer_uuid, computer_name) VALUES (@computer2_uuid, 'COMPUTER-02');
INSERT INTO tbl_computers (computer_uuid, computer_name) VALUES (@computer3_uuid, 'WS-ADMIN-03');
GO

-- Insert aggregated data for computers
INSERT INTO tbl_computers_aggr (computer_id, computer_connected, ip_address)
VALUES (1, DATEADD(DAY, -1, GETDATE()), '127.0.0.1');

INSERT INTO tbl_computers_aggr (computer_id, computer_connected, ip_address)
VALUES (1, GETDATE(), '192.168.1.101');

INSERT INTO tbl_computers_aggr (computer_id, computer_connected, ip_address)
VALUES (2, DATEADD(HOUR, -2, GETDATE()), '10.0.0.15');

INSERT INTO tbl_computers_aggr (computer_id, computer_connected, ip_address)
VALUES (3, GETDATE(), '172.16.10.20');
GO

-- Insert network IP address statuses
INSERT INTO tblf_network_ipaddresses_status (Address, Mac, Occurred, SourceUuid)
VALUES (
    '127.0.0.1',
    '00-14-22-01-23-45',
    DATEADD(DAY, -1, GETDATE()),
    (SELECT computer_uuid FROM tbl_computers WHERE computer_id = 1)
);

INSERT INTO tblf_network_ipaddresses_status (Address, Mac, Occurred, SourceUuid)
VALUES (
           '192.168.1.101',
           '00-14-22-01-23-45',
           GETDATE(),
           (SELECT computer_uuid FROM tbl_computers WHERE computer_id = 1)
       );

INSERT INTO tblf_network_ipaddresses_status (Address, Mac, Occurred, SourceUuid)
VALUES (
           '10.0.0.15',
           '00-1A-2B-3C-4D-5E',
           DATEADD(HOUR, -2, GETDATE()),
           (SELECT computer_uuid FROM tbl_computers WHERE computer_id = 2)
       );

INSERT INTO tblf_network_ipaddresses_status (Address, Mac, Occurred, SourceUuid)
VALUES (
           '172.16.10.20',
           '00-1C-42-AF-12-34',
           GETDATE(),
           (SELECT computer_uuid FROM tbl_computers WHERE computer_id = 3)
       );