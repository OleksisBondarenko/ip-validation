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
    private const int SQL_TIMEOUT_SECONDS = 2;
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
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, $"Timeout is over {SQL_TIMEOUT_SECONDS} seconds when tried to connect to {connectionString}");
            return null;
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
    // Create a cancellation token that will cancel after 5 seconds
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(SQL_TIMEOUT_SECONDS));
    
    try
    {
        await using var connection = new SqlConnection(eraDbConnectionString);
        
        await connection.OpenAsync(cts.Token);
        
        var command = new SqlCommand(
            @"SELECT tbl_computers.computer_id, tbl_computers.computer_uuid, 
                     tbl_computers.computer_name, tbl_computers_aggr.computer_connected, 
                     tbl_computers_aggr.ip_address 
              FROM [tbl_computers_aggr] 
              INNER JOIN tbl_computers ON tbl_computers.computer_id = tbl_computers_aggr.computer_id 
              WHERE ip_address = @ip_address 
              ORDER BY [computer_connected] DESC", 
            connection);
            
        command.Parameters.AddWithValue("@ip_address", ipAddress);
        
        // Set command timeout (in seconds)
        command.CommandTimeout = SQL_TIMEOUT_SECONDS;

        await using var reader = await command.ExecuteReaderAsync(cts.Token);
        
        if (await reader.ReadAsync(cts.Token))
        {
            return new EraComputerInfo()
            {
                IpAddress = reader["ip_address"].ToString(),
                ComputerName = reader["computer_name"].ToString(),
                ComputerConnected = Convert.ToDateTime(reader["computer_connected"]),
                ComputerGUID = (byte[])reader["computer_uuid"],
                ComputerId = int.Parse(reader["computer_id"].ToString())
            };
        } 
        
        return new EraComputerInfo();
    }
    catch (TaskCanceledException)
    {
        throw new TimeoutException("Database operation timed out after 5 seconds");
    }
    catch (SqlException ex) when (ex.Number == -2) // -2 is SQL timeout error code
    {
        throw new TimeoutException("Database command timed out", ex);
    }

    throw new UnauthorizedAccessException($"Provided IP address is not present in DB {ipAddress}");
}
}