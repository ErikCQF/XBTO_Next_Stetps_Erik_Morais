﻿
using Microsoft.Extensions.Options;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace XbtoMarketData.DataSource.Price
{
    public class PriceDeribitDataSource : IPriceDeribitDataSource
    {
        private readonly IOptions<DeribitOSettings> deribitOSettings;

        public PriceDeribitDataSource(IOptions<DeribitOSettings> deribitOSettings)
        {
            this.deribitOSettings = deribitOSettings;
        }

        public async Task<PriceDeribit?> GetLastPrice(string instrumentName)
        {

            var msg = new
            {
                jsonrpc = "2.0",
                id = 9267,
                method = "public/get_last_trades_by_instrument",
                @params = new
                {
                    instrument_name = instrumentName,
                    count = 1
                }
            };

            using (var client = new ClientWebSocket())
            {
                // Connect to WebSocket
                await client.ConnectAsync(new Uri(deribitOSettings.Value.WsUrl), CancellationToken.None);

                // Convert message to JSON and send it
                var jsonMsg = JsonSerializer.Serialize(msg);
                var bytes = Encoding.UTF8.GetBytes(jsonMsg);
                await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);

                // Wait for response
                var buffer = new byte[1024];
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                // Process response
                var response = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("Received from server: " + response);

                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }

                PriceDeribittResponse? instrumentResponse = JsonSerializer.Deserialize<PriceDeribittResponse?>(response);

                return instrumentResponse?.Result?.Trades?.FirstOrDefault() ?? null;
            }
        }


    }
}