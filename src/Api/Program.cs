using System.Reflection;
using ECommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

/*─────────────────────────────  Serilog Configuration ─────────────────────────────*/
builder.Host.UseSerilog((ctx, lc) => lc
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        theme: ConsoleTheme.Colorful,  
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}",
        applyThemeToRedirectedOutput: true,
        standardErrorFromLevel: LogEventLevel.Error)
    .WriteTo.File("logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}{NewLine}",
        retainedFileCountLimit: 7,
        shared: true)
    .WriteTo.Debug(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"));

/*───────────────────────────  Infrastructure  ───────────────────────*/
builder.WebHost.ConfigureKestrel(opts => {
    opts.ListenAnyIP(443);
});

/*─────────────────────────────  Services  ───────────────────────────*/
builder.Services.AddControllers();

builder.Services.AddApiVersioning(o => {
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
});

builder.Services.AddDbContext<ECommerceDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(opts => {
    opts.AddPolicy("CorsPolicy", p =>
        p.WithOrigins("http://localhost:3000")  // React app
         .AllowAnyMethod()
         .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts => {
    opts.SwaggerDoc("v1", new OpenApiInfo {
        Title = "ECommerce API",
        Version = "v1",
        Description = "API para gerenciamento de clientes, produtos e vendas"
    });
});

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddMediatR(typeof(Program).Assembly);

var app = builder.Build();

/*───────────────────────────  Logging Endpoint  ──────────────────────*/
app.MapGet("/log-samples", () => {
    Log.Verbose("Detailed trace information");
    Log.Debug("Debug data for development");
    Log.Information("API request processed");
    Log.Warning("Potential configuration issue detected");
    Log.Error("Failed to process request");
    Log.Fatal("Critical system failure (simulated)");

    Log.Information("System Status | Host: {HostName} | Memory: {MemoryUsage} MB | Threads: {ThreadCount}", 
        Environment.MachineName, 
        Math.Round(GC.GetTotalMemory(false) / 1024.0 / 1024.0, 2),
        ThreadPool.ThreadCount);

    try {
        throw new InvalidOperationException("Simulated business rule violation") {
            Data = { ["OrderId"] = 12345, ["UserId"] = "test-user" }
        };
    }
    catch (Exception ex) {
        Log.Error(ex, "Order processing failed | Order: {OrderId} | User: {UserId}",
            ex.Data["OrderId"], 
            ex.Data["UserId"]);
    }

    return Results.Ok(new {
        Message = "Log samples generated successfully",
        Instructions = "Check console output or log files",
        LogFiles = new[] {
            "logs/log-.txt (rolling daily)",
            "logs/system-status.log"
        },
        TestTime = DateTime.UtcNow.ToString("O")
    });
});

/*───────────────────────────  Middleware Pipeline  ──────────────────────*/
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(options => {
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("RemoteIp", httpContext.Connection.RemoteIpAddress);
    };
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

/*───────────────────────────  Database Migration  ──────────────────────*/
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
    db.Database.Migrate();
    Log.Information("Database migrations applied successfully");
}

app.Run();