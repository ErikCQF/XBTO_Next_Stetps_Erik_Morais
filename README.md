# Xbto Next Steps ErikMorais

# Market Data Recorder for Deribit Instruments

## Overview

This project implements a market data recorder for Deribit instruments using C# and .NET 6. It fetches the latest price per instrument at regular intervals while respecting the rate limit provided by the Deribit API. The retrieved data is persisted into a data store of choice.

## Implementation Details

1. **Data Fetching**: The latest price per instrument is fetched from Deribit using either Websockets or REST, depending on the chosen approach.

2. **Rate Limiting**: The application respects the rate limit specified by the Deribit API to avoid exceeding the allowed request frequency.

3. **Data Persistence**: The retrieved market data is persisted into a data store, such as a text file or database, to ensure it is available for later retrieval.

4. **Service Endpoint (Optional)**: Time permitting, a service endpoint is provided to allow clients to retrieve the persisted pricing data.

## Deribit API References

- [Deribit API v2.1.1 Documentation](https://docs.deribit.com/#deribit-api-v2-1-1)
- [Public: Get Instruments](https://docs.deribit.com/#public-get_instruments)
- [Public: Get Last Trades by Instrument](https://docs.deribit.com/#public-get_last_trades_by_instrument)

## Time Restrictions

Due to time restrictions, the following considerations were made:

- **Choice of Data Store**: For simplicity, a text file or simple database (e.g., SQLite) may be chosen for data persistence. More robust solutions like a relational database or cloud storage could be considered with more time.
- **Service Endpoint Implementation**: The service endpoint for clients to retrieve pricing data may be implemented using ASP.NET Core Web API with appropriate controller endpoints.

## Assumptions and Decisions

- **Data Persistence**: Since the exercise does not specify a preferred data store, a simple text file or lightweight database solution will be used for persistence.
- **Rate Limit Handling**: The application will handle rate limiting by respecting the rate limit specified by the Deribit API and implementing appropriate throttling mechanisms to avoid exceeding the allowed request frequency.

## Future Improvements

With more time, the following improvements could be made:

- **Enhanced Data Store**: Implementing a more robust data store solution, such as a relational database or cloud storage, for better scalability and performance.
- **Service Endpoint Enhancements**: Adding authentication, pagination, filtering, and sorting capabilities to the service endpoint for improved client interaction.
- **Error Handling and Logging**: Implementing comprehensive error handling and logging mechanisms to capture and report errors effectively.

## Time Spent

The exercise was completed within the allotted 4 hours timeframe. However, additional time may be required for further enhancements and optimizations.


## Dev Set Up 
To run the application locally, please make the **MaketDataAPI** as you start up project


## Documentation

The Price Monitor implements and algoritm that flollo
2. **Rate Limiting**: The application respects the rate limit specified by the Deribit API to avoid exceeding the allowed request frequency.




