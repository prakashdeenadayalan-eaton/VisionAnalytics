using CameraReader;
using CameraReader.Contracts;
using CameraReader.Implementations;
using CameraReader.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var runningCameras = new ConcurrentDictionary<int, ICamera>();

        Console.WriteLine("Camera Reader Application (polling every 10 seconds)");
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:8069/"); // Replace with your API base address
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var cameraApiClient = new CameraApiClient(httpClient);

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Declare pollingTask so shutdown handlers can reference it
        Task pollingTask = Task.CompletedTask;

        // Handle Ctrl+C in console (graceful shutdown)
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            Console.WriteLine("Cancellation requested (Ctrl+C).");
            eventArgs.Cancel = true; // prevent immediate process termination
            try
            {
                cts.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // already disposed, ignore
            }
        };

        // Handle process exit (e.g., console window close). Keep this short — blocking time is limited.
        AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
        {
            Console.WriteLine("Process exit detected; requesting shutdown...");
            try
            {
                cts.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // already disposed, ignore
            }

            try
            {
                // Attempt to wait a short time for the polling loop to finish cleanup.
                pollingTask?.Wait(TimeSpan.FromSeconds(5));
            }
            catch
            {
                // ignored - process is exiting
            }
        };

        // Start the polling loop task
        pollingTask = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    List<CameraDto>? cameras = await cameraApiClient.GetCamerasAsync(analyticsEnabled: true).ConfigureAwait(false);
                    cameras ??= new List<CameraDto>();

                    if (cameras.Count == 0)
                    {
                        Console.WriteLine("No cameras retrieved.");
                    }
                    else
                    {
                        Console.WriteLine($"Retrieved {cameras.Count} cameras:");
                        foreach (var camera in cameras)
                        {
                            if (runningCameras.ContainsKey(camera.Id))
                            {
                                Console.WriteLine($"Camera {camera.Id} is already running.");
                            }
                            else
                            {
                                Console.WriteLine($"- {camera.Name} (ID: {camera.Id}) starting.");
                                ICamera cameraSource = new SimulatorCameraSource(camera);

                                // Start asynchronously and do not await (fire-and-forget)
                                _ = cameraSource.StartAsync();

                                runningCameras[camera.Id] = cameraSource;
                            }
                        }
                    }

                    // Determine cameras that were running but are no longer returned by the API (analytics disabled)
                    var cameraIds = cameras.Select(c => c.Id).ToHashSet();
                    var runningIds = runningCameras.Keys.ToList();

                    foreach (var runningId in runningIds)
                    {
                        if (!cameraIds.Contains(runningId))
                        {
                            Console.WriteLine($"Camera {runningId} has analytics disabled. Stopping.");
                            if (runningCameras.TryRemove(runningId, out var camToStop))
                            {
                                try
                                {
                                    await camToStop.StopAsync().ConfigureAwait(false);
                                }
                                catch (Exception stopEx)
                                {
                                    Console.WriteLine($"Error stopping camera {runningId}: {stopEx.Message}");
                                }
                            }
                        }
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    Console.WriteLine($"HTTP error retrieving cameras: {httpEx.Message}");
                }
                catch (OperationCanceledException)
                {
                    // Cancellation requested - break loop gracefully
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving cameras: {ex.Message}");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Cancellation requested while delaying - exit loop
                    break;
                }
            }
        }, token);

        Console.WriteLine("Press Enter to stop the application...");
        Console.ReadLine();

        // Request cancellation and wait for the polling loop to finish
        cts.Cancel();
        try
        {
            await pollingTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // expected on cancellation
        }

        // Stop any remaining running cameras
        var remainingIds = runningCameras.Keys.ToList();
        foreach (var id in remainingIds)
        {
            if (runningCameras.TryRemove(id, out var cam))
            {
                try
                {
                    Console.WriteLine($"Stopping camera {id} on shutdown.");
                    await cam.StopAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error stopping camera {id} during shutdown: {ex.Message}");
                }
            }
        }

        Console.WriteLine("Application stopped.");
    }
}