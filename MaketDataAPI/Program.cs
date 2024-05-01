using XbtoMarketData.DataRepository.Instrument;
using XbtoMarketData.DataRepository.Price;
using XbtoMarketData.DataSource;
using XbtoMarketData.DataSource.Instrument;
using XbtoMarketData.DataSource.Price;
using XbtoMarketData.Service.Data;
using XbtoMarketData.Service.Monitor;
using XbtoMarketData.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



//** DATA SOURCE Deribit
// Register the DeribitOSettings from appsettings.json
builder.Services.Configure<DeribitOSettings>(builder.Configuration.GetSection("DeribitOSettings"));

// Register the InstrumentDeribitDataSource as a singleton service
builder.Services.AddSingleton<IInstrumentDataSource, InstrumentDeribitDataSource>();

// Register the PriceDeribitDataSource as a singleton service
builder.Services.AddSingleton<IPriceDeribitDataSource, PriceDeribitDataSource>();
builder.Services.AddSingleton<IDateProvider, DateProvider>();


//** REPOSITORY Data File
// Get the file path from the configuration
// Get the connection strings section from the configuration
var connectionStringsSection = builder.Configuration.GetSection("ConnectionStrings");

// Read individual connection strings
string? instrumentDBConnectionString = connectionStringsSection["InstrumentDB"];
string? priceDBConnectionString = connectionStringsSection["PriceDB"];

// Register the FileInstrumentRepo as a singleton service
builder.Services.AddSingleton<IInstrumentRepo>(_ => new FileInstrumentRepo(instrumentDBConnectionString));
builder.Services.AddSingleton<IPriceRepo>(_ => new FilePriceRepo(instrumentDBConnectionString));


//** Price MOnitor Settings

builder.Services.AddSingleton<PriceMonitor>();

builder.Services.AddSingleton<IPriceMonitor, PriceMonitor>();

builder.Services.AddSingleton<IPriceService, PriceService>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Run the PriceMonitor background task
var priceMonitor = app.Services.GetRequiredService<IPriceMonitor>();
var cancellationToken = app.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping;


// Read individual settings
var monitorPriceSettings = builder.Configuration.GetSection("MonitorPriceSettings");
int fetchIntervalSeconds = monitorPriceSettings.GetValue<int>("FetchIntervalSeconds");
int rateLimit = monitorPriceSettings.GetValue<int>("RateLimit");

//******************************************
// Start the PriceMonitor background task
var task = priceMonitor.Start(fetchIntervalSeconds: fetchIntervalSeconds, rateLimit: rateLimit, cancellationToken);
//******************************************

// You can see in the console the worker pooling last changes
if (app.Environment.IsDevelopment())
{
    priceMonitor.PriceChanged += (price) =>
    {
        Console.WriteLine($"Instrument: {price?.InstrumentName} = >  price: {price?.Price}");
    };
}





app.Run();


