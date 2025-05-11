using System.Data;
using System.Net;
using System.Security.Authentication;
using Microsoft.Data.SqlClient;
using ADValidation.Models.ERA;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ADValidation.Services;

public class EraService
{
    // private readonly ERASettings _eraSettings;
    private readonly ILogger<EraService> _logger;

    public EraService(IOptions<ERASettings> eraSettings, ILogger<EraService> logger)
    {
        // _eraSettings = eraSettings.Value;
        _logger = logger;
    }
  
    // public async Task<EraComputerInfo> GetComputerInfo(string ipAddress)
    // {
    //     var tasks = _eraSettings.EraDbConnectionStrings.Select(connectionStringKV =>
    //         GetComputerAggregatedDataSafe(connectionStringKV.Value, ipAddress)).ToList();
    //
    //     while (tasks.Any())
    //     {
    //         var completedTask = await Task.WhenAny(tasks);
    //         tasks.Remove(completedTask);
    //     
    //         var result = await completedTask;
    //         if (result != null)
    //         {
    //             return result;
    //         }
    //     }
    //
    //     throw new UnauthorizedAccessException("Provided IP address is not present in DB");
    // }

    // Helper method that catches UnauthorizedAccessException and returns null
    public async Task<EraComputerInfo?> GetComputerInfoSafe(string connectionString, string ipAddress)
    {
        try
        {
            return await GetComputerInfo(connectionString, ipAddress);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, $"Unauthorized access on connection: {connectionString}");
            return null;
        }
        catch (SqlException ex)
        {    _logger.LogError(ex, $"SqlException: {connectionString}");
            return null;
        }
    }

    private async Task<EraComputerInfo> GetComputerInfo(string eraDbConnectionString, string ipAddress)
    {
        await using (var connection = new SqlConnection(eraDbConnectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand(
                $"SELECT tbl_computers.computer_id, tbl_computers.computer_uuid, tbl_computers.computer_name, tbl_computers_aggr.computer_connected, tbl_computers_aggr.ip_address " +
                        "FROM [tbl_computers_aggr] " + 
                        "INNER JOIN tbl_computers ON tbl_computers.computer_id = tbl_computers_aggr.computer_id " +
                        "WHERE ip_address = @ip_address " +
                        "ORDER BY [computer_connected] DESC"
                
                , connection);
            command.Parameters.AddWithValue("@ip_address", ipAddress);

            await using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new EraComputerInfo ()
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

        throw new UnauthorizedAccessException($"Provided IP address is not present in DB {ipAddress}");
    }
}