﻿using EventHandler;
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
            logger.Info("Broadcasting " + message.Name + " data");

            switch (message.Name)
            {
                case "Block":
                    await InvokeClientMethodToAllAsync("blocks", message.Data);
                    break;
                // case "Share":
                //     await InvokeClientMethodToAllAsync("shares", message.Data);
                //     break;
                case "PoolStat":
                    await InvokeClientMethodToAllAsync("poolStats", message.Data);
                    break;
                case "MinerStat":
                    await InvokeClientMethodToAllAsync("minerStats", message.Data);
                    break;
                default:
                    break;
            }
        }

    }
}
