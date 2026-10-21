using CameraReader.Contracts;
using CameraReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;
namespace CameraReader.Implementations
{
    internal class SimulatorCameraSource : ICamera
    {

        private Task? _runningTask;
        private CancellationTokenSource? _cancellationTokenSource;
        private CameraDto _camera;

        private int _frameCount;
        public int FrameCount => Volatile.Read(ref _frameCount);
        public Task RunningTask => _runningTask;

        public int Id => _camera.Id;

        public SimulatorCameraSource(CameraDto camera)
        {
            _camera = camera;
        }

        public Task StartAsync()
        {
            if (_runningTask != null && !_runningTask.IsCompleted) return Task.CompletedTask;
            _cancellationTokenSource = new CancellationTokenSource();
            _runningTask = PerformOperationsAsync(_cancellationTokenSource.Token);
            // Observe exceptions to avoid unobserved task exceptions
            _runningTask.ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    Console.WriteLine($"Camera {_camera.Name} (ID: {_camera.Id}) encountered an error: {t.Exception.GetBaseException()}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);

            // Fire-and-forget start: return immediately
            return Task.CompletedTask;
        }
        public async Task PerformOperationsAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Performing operations with camera {_camera.Name} (ID: {_camera.Id})...");

            try
            {
                await ConnectAsync();

                while (!cancellationToken.IsCancellationRequested)
                {
                    Interlocked.Increment(ref _frameCount);
                    Console.WriteLine($"Captured frame {FrameCount} from camera {_camera.Name} (ID: {_camera.Id}). - {DateTime.UtcNow.ToString("mm:ss")}");
                    var frame = GenerateFrame();
                    var message = new Frame
                    {
                        CameraId = _camera.Id,
                        FrameId = FrameCount,
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                        Data = frame
                    };
                    await Task.Delay(TimeSpan.FromMilliseconds(125), cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
            finally
            {
                await DisconnectAsync();
            }

            Console.WriteLine($"Completed operations with camera {_camera.Name} (ID: {_camera.Id}).");
        }



        //public async Task StartStreaming(
        //    CameraDto camera,
        //    CancellationToken cancellationToken)
        //{
        //    long frameId = 0;

        //    while (!cancellationToken.IsCancellationRequested)
        //    {
        //        var frame = GenerateFrame();

        //        var message = new Frame
        //        {
        //            CameraId = camera.Id,
        //            FrameId = frameId++,
        //            Timestamp = DateTimeOffset.UtcNow
        //                .ToUnixTimeMilliseconds(),
        //            Data = frame
        //        };

        //        // Publish to analytics

        //        await Task.Delay(
        //            TimeSpan.FromMilliseconds(125),
        //            cancellationToken);
        //    }
        //}

        private byte[] GenerateFrame()
        {
            return null;
        }

        public async Task StopAsync()
        {
            if (_cancellationTokenSource == null) return;
            _cancellationTokenSource.Cancel();
            try { if (_runningTask != null) await _runningTask; }
            catch (OperationCanceledException) { }
            finally { _cancellationTokenSource.Dispose(); _cancellationTokenSource = null; _runningTask = null; }
        }

        public async Task ConnectAsync()
        {
            // Simulate connecting to the camera asynchronously
            Console.WriteLine($"Connecting to camera {_camera.Name} (ID: {_camera.Id})...");
            await Task.Delay(1000);
            Console.WriteLine($"Connected to camera {_camera.Name} (ID: {_camera.Id}).");
        }
        public async Task DisconnectAsync()
        {
            // Simulate disconnecting from the camera asynchronously
            Console.WriteLine($"Disconnecting from camera {_camera.Name} (ID: {_camera.Id})...");
            await Task.Delay(1000);
            Console.WriteLine($"Disconnected from camera {_camera.Name} (ID: {_camera.Id}).");
        }
    }
}
