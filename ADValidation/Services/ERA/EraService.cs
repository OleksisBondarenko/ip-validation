using System.Data;
using System.Security.Authentication;
using Microsoft.Data.SqlClient;
using ADValidation.Models.ERA;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ADValidation.Services;

public class EraValidationService
{
    private readonly ERASettings _eraSettings;
    private readonly ILogger<EraValidationService> _logger;

    public EraValidationService(IOptions<ERASettings> eraSettings, ILogger<EraValidationService> logger)
    {
        _eraSettings = eraSettings.Value;
        _logger = logger;
    }

    public async Task<string> GetHostByIp(string ipAddress)
    {
        foreach (var connectionString in _eraSettings.EraDbConnections)
        {
            try
            {
                string? hostName = await GetHostByIP(connectionString, ipAddress);
                if (!string.IsNullOrEmpty(hostName))
                {
                    return hostName;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        throw new UnauthorizedAccessException("Provided IP address is not present in DB");
    }
    
    private async Task<string> GetHostByIP(string connectionString, string address)
    {
        try
        {
            var networkInfo = await GetNetworkInfoByIpAsync(connectionString, address);
            var computerInfo = await GetComputerInfoByUuidAsync(connectionString, networkInfo.SourceUuid);
            return computerInfo.ComputerName;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Unauthorized Access " + ex.Message);
            throw ex;
        }

    }
    

    private async Task<NetworkInfo> GetNetworkInfoByIpAsync(string connectionString, string address)
    {
        await using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand(
                "SELECT [Address], [Mac], [Occurred], [SourceUuid] FROM [tblf_network_ipaddresses_status] WHERE Address = @Address ORDER BY [Occurred] DESC", connection);
            command.Parameters.AddWithValue("@Address", address);

            await using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    Guid test = new Guid((byte[])reader["SourceUuid"]);
                    
                    return new NetworkInfo
                    {
                        Address = reader["Address"].ToString(),
                        Mac = reader["Mac"].ToString(),
                        Occurred = Convert.ToDateTime(reader["Occurred"]),
                        SourceUuid = (byte[])reader["SourceUuid"]
                    };
                }
            }
        }

        throw new UnauthorizedAccessException("Provided IP address is not present in DB");
    }

    public async Task<ComputerAggregatedData> GetComputerAggregatedData(string ipAddress)
    {
        // var tasks = _eraSettings.EraDbConnections.Select(connectionString => 
        //     GetComputerAggregatedDataSafe(connectionString, ipAddress)).ToList();

        // while (tasks.Any())
        // {
        //     var completedTask = await Task.WhenAny(tasks);
        //     tasks.Remove(completedTask);
        //
        //     var result = await completedTask;
        //     if (result != null)
        //     {
        //         return result;
        //     }
        // }

        foreach (var connectionString in _eraSettings.EraDbConnections)
        {
            try
            {
                return await GetComputerAggregatedData(connectionString, ipAddress);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        throw new UnauthorizedAccessException("Provided IP address is not present in DB");
    }

    // Helper method that catches UnauthorizedAccessException and returns null
    private async Task<ComputerAggregatedData?> GetComputerAggregatedDataSafe(string connectionString, string ipAddress)
    {
        try
        {
            return await GetComputerAggregatedData(connectionString, ipAddress);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, $"Unauthorized access on connection: {connectionString}");
            return null;
        }
    }

    private async Task<ComputerAggregatedData> GetComputerAggregatedData(string connectionString, string address)
    {
        await using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand(
                $"SELECT tbl_computers.computer_id, [computer_uuid], [computer_name], [computer_connected], [ip_address] " +
                        "FROM [tbl_computers_aggr] " + 
                        "INNER JOIN tbl_computers ON tbl_computers.computer_id = tbl_computers_aggr.computer_id " +
                        "WHERE ip_address = @ip_address " +
                        "ORDER BY [computer_connected] DESC"
                
                , connection);
            command.Parameters.AddWithValue("@ip_address", address);

            await using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new ComputerAggregatedData ()
                    {
                        IpAddress = reader["ip_address"].ToString(),
                        ComputerName = reader["computer_name"].ToString(),
                        ComputerConnected = Convert.ToDateTime(reader["computer_connected"]),
                        ComputerGUID = (byte[])reader["computer_uuid"],
                        ComputerId = int.Parse(reader["computer_id"].ToString())
                    };
                }
            }
        }

        throw new UnauthorizedAccessException("Provided IP address is not present in DB");
    }

    private async Task<ComputerInfo> GetComputerInfoByUuidAsync(string connectionString, byte[] uuid)
    {
        await using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand(
                "SELECT computer_name FROM [tbl_computers] WHERE computer_uuid = @Uuid", connection);
            command.Parameters.Add("@Uuid", SqlDbType.VarBinary).Value = uuid;

            await using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new ComputerInfo
                    {
                        ComputerUuid = uuid,
                        ComputerName = reader["computer_name"].ToString()
                    };
                }
            }
        }
        
        throw new UnauthorizedAccessException("Provided computer_uuid is not present");
    }
}