// standalone, Unity-independent connection-quality algorithm & enum.
// don't need to use this directly, it's built into Mirror's NetworkClient.
using UnityEngine;

namespace Mirror
{
    public enum ConnectionQuality : byte
    {
        EXCELLENT, // ideal experience for high level competitors
        GOOD, // very playable for everyone but high level competitors
        FAIR, // very noticeable latency, not very enjoyable anymore
        POOR, // unplayable
        ESTIMATING, // still estimating
    }

    // provide different heuristics for users to choose from.
    // simple heuristics to get started.
    // this will be iterated on over time based on user feedback.
    public static class ConnectionQualityHeuristics
    {
        // convenience extension to color code Connection Quality
        public static Color ColorCode(this ConnectionQuality quality)
        {
            return quality switch
            {
                ConnectionQuality.EXCELLENT => Color.green,
                ConnectionQuality.GOOD => Color.yellow,
                ConnectionQuality.FAIR => new Color(1.0f, 0.647f, 0.0f),
                ConnectionQuality.POOR => Color.red,
                _ => Color.gray,
            };
        }

        // straight forward estimation
        //   rtt: average round trip time in seconds.
        //   jitter: average latency variance.
        public static ConnectionQuality Simple(double rtt, double jitter)
        {
            return rtt <= 0.100 && jitter <= 0.10
                ? ConnectionQuality.EXCELLENT
                : rtt <= 0.200 && jitter <= 0.20
                    ? ConnectionQuality.GOOD
                    : rtt <= 0.400 && jitter <= 0.50
                        ? ConnectionQuality.FAIR
                        : ConnectionQuality.POOR;
        }

        // snapshot interpolation based estimation.
        // snap. interp. adjusts buffer time based on connection quality.
        // based on this, we can measure how far away we are from the ideal.
        // the returned quality will always directly correlate with gameplay.
        // => requires SnapshotInterpolation dynamicAdjustment to be enabled!
        public static ConnectionQuality Pragmatic(double targetBufferTime, double currentBufferTime)
        {
            // buffer time is set by the game developer.
            // estimating in multiples is a great way to be game independent.
            // for example, a fast paced shooter and a slow paced RTS will both
            // have poor connection if the multiplier is >10.
            double multiplier = currentBufferTime / targetBufferTime;

            // empirically measured with Tanks demo + LatencySimulation.
            // it's not obvious to estimate on paper.
            if (multiplier <= 1.15)
            {
                return ConnectionQuality.EXCELLENT;
            }

            if (multiplier <= 1.25)
            {
                return ConnectionQuality.GOOD;
            }

            if (multiplier <= 1.50)
            {
                return ConnectionQuality.FAIR;
            }

            // anything else is poor
            return ConnectionQuality.POOR;
        }
    }
}
