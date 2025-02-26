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

    private async Task<ComputersResult> GetComputerInfoByUuidAsync(string connectionString, byte[] uuid)
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
                    return new ComputersResult
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