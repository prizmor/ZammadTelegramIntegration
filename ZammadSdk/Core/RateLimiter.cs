using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Zammad.Sdk.Core;

/// <summary>
/// Provides adaptive rate limiting for the Zammad API.
/// </summary>
internal sealed class RateLimiter
{
    private readonly RateLimitingOptions _options;
    private readonly ConcurrentQueue<DateTimeOffset> _requestTimestamps = new();
    private double _backoffSeconds;

    public RateLimiter(RateLimitingOptions options)
    {
        _options = options;
        _backoffSeconds = 0;
    }

    public async ValueTask ThrottleAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return;
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            Trim();
            if (_requestTimestamps.Count < _options.RequestsPerWindow)
            {
                _requestTimestamps.Enqueue(DateTimeOffset.UtcNow);
                return;
            }

            var oldest = PeekOldest();
            var delay = oldest + _options.Window - DateTimeOffset.UtcNow;
            if (_backoffSeconds > 0)
            {
                delay += TimeSpan.FromSeconds(_backoffSeconds);
            }

            if (delay <= TimeSpan.Zero)
            {
                Trim();
                continue;
            }

            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }
    }

    public void Register429()
    {
        if (!_options.Enabled)
        {
            return;
        }

        _backoffSeconds = Math.Min(
            Math.Max(_backoffSeconds * _options.BackoffMultiplier, 1),
            _options.Window.TotalSeconds);
    }

    public void ResetBackoff()
    {
        _backoffSeconds = 0;
    }

    private void Trim()
    {
        while (_requestTimestamps.TryPeek(out var timestamp))
        {
            if (DateTimeOffset.UtcNow - timestamp < _options.Window)
            {
                break;
            }

            _requestTimestamps.TryDequeue(out _);
        }
    }

    private DateTimeOffset PeekOldest()
    {
        _requestTimestamps.TryPeek(out var oldest);
        return oldest;
    }
}
