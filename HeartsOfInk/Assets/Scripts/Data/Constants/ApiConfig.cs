namespace Assets.Scripts.Data.Constants
{
    class ApiConfig
    {
        // Local
        //public const string LobbyHOIServerUrl = "http://localhost:5000/";
        //public const string IngameServerUrl = "http://localhost:7001/";
        //public const string LoggingServerUrl = "http://localhost:44356/";

        // Red Local
        //public const string LobbyHOIServerUrl = "http://192.168.1.134:5000/";
        public const string IngameServerUrl = "http://190.92.134.72:7001/";
        //public const string LoggingServerUrl = "http://192.168.1.134:44356/";

        // Preproducción
        public const string LobbyHOIServerUrl = "http://190.92.134.72:5000/";
        //public const string IngameServerUrl = "http://190.92.134.72:7001/";
        public const string LoggingServerUrl = "http://190.92.134.72:44356/";

        public const string SignalRHUBName = "signalrhoi";

        /// <summary>
        /// Delay in seconds between calls to server for update states.
        /// </summary>
        public const float DelayBetweenStateUpdates = 0.010f;
    }
}
