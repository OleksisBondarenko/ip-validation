// using Microsoft.EntityFrameworkCore;
//
// public class AppDbContext : DbContext
// {
//     public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
//
//     public DbSet<Result1> Results1 { get; set; }
//     public DbSet<Result2> Results2 { get; set; }
// }
//
// public class Result1
// {
//     public string Address { get; set; }
//     public string Mac { get; set; }
//     public DateTime Occured { get; set; }
//     public byte[] SourceUuid { get; set; }
// }
//
// public class Result2
// {
//     public string Computer_name { get; set; }
// }