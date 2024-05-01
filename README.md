# Xbto Next Steps Erik Morais


## Components overview


![Blank diagram](https://github.com/ErikCQF/XBTO_Next_Stetps_Erik_Morais/assets/160326708/1c56d54d-26bc-46d0-9d7b-0e4a441aec76)


# Market Data Recorder for Deribit Instruments

The Monitor price services has only these functionalites:

-**Start**()

-**Stop**()

-**MonitorPrice**(string instrumentName)()

-**Event** : PriceChanged<Price>


```
interface IPriceMonitor {
   + Task Start(int fetchIntervalSeconds, int rateLimit, CancellationToken cancellationTokens);
   + void Stop();
   + void MonitorPrice(string instrumentName);
   + event Action<PriceDeribit> PriceChanged;
}

```

## Fetch Algorithm With Rate Limit Control

The algorithm maintains the processing rate at the maximum pace of the rate limit while respecting the trigger interval.

```csharp
Start()
{
  While (stop not requested)   
    {
       For Each Instrument To Monitor
       {
         RequestPrice();
         // calculates the processing rate.
         // if it is higher than the limit rate, it waits a calculated time to equalize the processing rate to the rate limit
         WaitRateLimit(); 
       }
       // if all have processed before the time for the next trigger, it waits for the time interval
       // otherwise starts right away
       CalculateRemainingForNextTrigger()
    }
}

```

## Dev Set Up 
To run the application locally, please make the **MaketDataAPI** as you start up project
You can use **Swagger** to add instruments to be monitored and get last prices using end points

There is a Get Last Price endpoint and a Monitor Price endpoint. Instrument name is the parameter you need to test. As you are running locally, you can see the prices being written to the Console Monitor.

## Deribit / Monitor Price / Repositories Set UP

See appSetting.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  // Deribit settings for connection. It is using test
  "DeribitOSettings": {
    "ApiKey": "your_api_key",
    "ApiSecret": "your_api_secret",
    "WsUrl": "wss://test.deribit.com/ws/api/v2"
  },

  "MonitorPriceSettings": {
    // Trigger each seconds
    "fetchIntervalSeconds": 3,
    // Max request per seconds
    "RateLimit": 10
  },

  // This version saves the data states into JSON files.
  "ConnectionStrings": {
    "InstrumentDB": "InstrumentData.json",
    "PriceDB": "PriceData.json"
  }
}
```



## FUNCTIONAL
## Implementation Guidelines:

This exercise is intended to be straight forward, but you may encounter some questions or issues - please work through these using your best judgement and document any assumptions/decisions you take. If time restrictions impact the design, please indicate what would have been done differently with more time.

Time spent on this exercise can be limited to 4 hours.

## Coding Exercise:

Using C# and .Net6 or above, implement a market data recorder for Deribit instruments and expose that market data for client consumption.

1. Using either Websockets or Rest (your choice), fetch the latest price per instrument every N seconds while respecting the rate limit. Persist the retrieved data into the data store of your choice (text file, database, etc).

2. Time permitting, create a service for clients to retrieve the pricing data you've persisted above.

## Deribit API References:

- [Deribit API v2.1.1 Documentation](https://docs.deribit.com/#deribit-api-v2-1-1)
- [Public: Get Instruments](https://docs.deribit.com/#public-get_instruments)
- [Public: Get Last Trades by Instrument](https://docs.deribit.com/#public-get_last_trades_by_instrument)



