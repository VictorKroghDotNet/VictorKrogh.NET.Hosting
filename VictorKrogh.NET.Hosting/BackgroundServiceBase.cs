﻿using Microsoft.Extensions.Logging;
using VictorKrogh.NET.Disposable;

namespace VictorKrogh.NET.Hosting;

public abstract class BackgroundServiceBase<TBackgroundService> : DisposableObject, IBackgroundService
    where TBackgroundService : IBackgroundService
{
    public BackgroundServiceBase(ILogger<TBackgroundService> logger)
    {
        Logger = logger;
    }

    protected ILogger<TBackgroundService> Logger { get; }

    public async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation($"{typeof(TBackgroundService).Name} started executing.");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{typeof(TBackgroundService).Name} failed to perform work");
            }

            await DelayAsync(cancellationToken);
        }

        Logger.LogInformation($"{typeof(TBackgroundService).Name} ended executing.");
    }

    protected abstract Task ExecuteAsync(CancellationToken cancellationToken);

    protected virtual async Task DelayAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
    }
}