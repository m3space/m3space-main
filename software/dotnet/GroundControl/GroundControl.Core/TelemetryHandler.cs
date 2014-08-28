namespace GroundControl.Core
{
    /// <summary>
    /// A telemetry data handler delegate.
    /// </summary>
    /// <param name="telemetry">the telemetry data</param>
    /// <param name="burst">true if balloon bursted, false otherwise</param>
    public delegate void TelemetryBurstHandler(TelemetryData telemetry, bool burst);

    /// <summary>
    /// A telemetry data handler delegate.
    /// </summary>
    /// <param name="telemetry">the telemetry data</param>
    public delegate void TelemetryHandler(TelemetryData telemetry);
}