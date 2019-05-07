// The original idea and the implementation are borrowed from  https://github.com/NuGetPackageExplorer/NuGetPackageExplorer
// Credits to Oren Novotny

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GitCommands;
using GitExtUtils.GitUI;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace GitUI.Infrastructure.Telemetry
{
    public static class DiagnosticsClient
    {
        private static bool _initialized;
        private static TelemetryClient _client;

        private static bool Enabled => _initialized && AppSettings.TelemetryEnabled;

        public static void Initialize(bool isDirty)
        {
            TelemetryConfiguration.Active.TelemetryInitializers.Add(new AppInfoTelemetryInitializer(isDirty));

            _initialized = true;

            Application.ApplicationExit += (s, e) =>
            {
                TrackEvent("AppExit");
                OnExit();
            };

            _client = new TelemetryClient();

            TrackEvent("Monitors", AttachMonitorInformation());
        }

        public static void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            if (!Enabled)
            {
                return;
            }

            _client.TrackEvent(eventName, properties, metrics);
        }

        public static void TrackTrace(string evt)
        {
            if (!Enabled)
            {
                return;
            }

            _client.TrackTrace(evt);
        }

        public static void Notify(Exception exception)
        {
            if (!Enabled)
            {
                return;
            }

            _client.TrackException(exception);
        }

        public static void TrackPageView(string pageName)
        {
            if (!Enabled)
            {
                return;
            }

            _client.TrackPageView(pageName);
        }

        private static Dictionary<string, string> AttachMonitorInformation()
        {
            var properties = new Dictionary<string, string>();
            properties["Count"] = Screen.AllScreens.Length.ToString();
            properties["Primary DPI"] = DpiUtil.DpiX.ToString();
            properties["Primary scaling"] = $"{(DpiUtil.ScaleX == 1 ? "no" : $"{Math.Round(DpiUtil.ScaleX * 100)}%")} scaling";

            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                var key = Screen.AllScreens[i].Primary ? "Primary" : $"Secondary {i}";

                var bounds = Screen.AllScreens[i].Bounds;
                properties[$"{key} resolution"] = $"{bounds.Width}x{bounds.Height}";
            }

            return properties;
        }

        private static void OnExit()
        {
            if (!Enabled)
            {
                return;
            }

            _client.Flush();

            // Allow time for flushing:
            System.Threading.Thread.Sleep(1000);
        }
    }
}
