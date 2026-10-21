using CameraReader.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraReader.Interfaces
{
    internal interface ICamera
    {
        int Id { get; }
        Task RunningTask { get; }
        Task ConnectAsync();
        Task DisconnectAsync();
        Task StartAsync();
        Task StopAsync();
        //Task StartStreaming(CameraDto camera, CancellationToken cancellationToken);
        //Task StopStreaming(CameraDto camera, CancellationToken cancellationToken);
    }
}
