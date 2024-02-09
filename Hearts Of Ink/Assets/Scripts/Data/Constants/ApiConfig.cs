namespace Assets.Scripts.Data.Constants
{
    class ApiConfig
    {
        // Local
        public const string LobbyHOIServerUrl = "http://192.168.1.142:5000/";
        public const string IngameServerUrl = "http://192.168.1.142:7001/";
        public const string LoggingServerUrl = "https://192.168.1.142:44356/";

        public const string SignalRHUBName = "signalrhoi";

        /// <summary>
        /// Delay in miliseconds between calls to server for update states.
        /// </summary>
        public const int DelayBetweenStateUpdates = 50;
    }
}
