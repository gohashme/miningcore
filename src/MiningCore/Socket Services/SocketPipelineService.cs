using EventHandler;
using MiningCore.Socket_Services.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using WebSocketManager;

namespace MiningCore.Socket_Services
{
    public class SocketPipelineService : WebSocketHandler
    {
        private readonly SocketEventHandler socketEventHandler;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public SocketPipelineService(WebSocketConnectionManager webSocketConnectionManager, SocketEventHandler socketEventHandler) : base(webSocketConnectionManager)
        {
            Console.WriteLine(">>>>>>> Socket pipeline service has been initiated");
            this.socketEventHandler = socketEventHandler;
            this.socketEventHandler.Subscribe<PipePackage>(PackHandler);
        }

        private async void PackHandler(PipePackage obj)
        {
            await SendMessage(obj);
        }

        public override Task OnConnected(WebSocket socket)
        {
            logger.Info("Socket connection established!");

            return base.OnConnected(socket);
        }

        public override Task OnDisconnected(WebSocket socket)
        {
            return base.OnDisconnected(socket);
        }

        public async Task SendMessage(PipePackage message)
        {
            logger.Info(">>>>>>>>>>>>>>>>>>>>>>>>> " + JsonConvert.SerializeObject(new object[] { message }));


            foreach (var item in this.WebSocketConnectionManager.GetAll())
            {
                switch (message.Name)
                {
                    case "Block":
                        await InvokeClientMethodAsync(item.Key, "blocks", new object[] { message });
                        break;
                    case "Share":
                        await InvokeClientMethodAsync(item.Key, "shares", new object[] { message });
                        break;
                    case "PoolStat":
                        await InvokeClientMethodAsync(item.Key, "poolStats", new object[] { message });
                        break;
                    case "MinerStat":
                        await InvokeClientMethodAsync(item.Key, "minerStats", new object[] { message });
                        break;
                    default:
                        break;
                }
            }

           
        }

    }
}
