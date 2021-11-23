using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NETCoreServer.InternalLogic
{
    /// <summary>
    /// Clase desde la que se controlan las operaciones ciclicas del servidor.
    /// </summary>
    public sealed class MainDaemon
    {
        /// <summary>
        /// Tiempo de margen para que arranque la aplicación y empezar a operar.
        /// </summary>
        private const int StartAppMargin = 5000;

        /// <summary>
        /// Tiempo de margen entre ejecución y ejecución del bucle.
        /// </summary>
        private const int DaemonSleepTime = 1000;
        private static Thread daemonThread = null;
        private static GamesListManteinance gamesListManteinance;

        public static void StartMainDaemon()
        {
            if (daemonThread == null)
            {
                daemonThread = new Thread(new ThreadStart(MainDaemonLoop));
                daemonThread.Start();
            }
        }

        /// <summary>
        /// This method stops the aplication, this is maked from deamon Abort catch.
        /// </summary>
        public static void StopApplication()
        {
            daemonThread.Abort();
        }

        private static void MainDaemonLoop()
        {
            try
            {
                Thread.Sleep(StartAppMargin);
                gamesListManteinance = new GamesListManteinance();

                while (true)
                {
                    Thread.Sleep(DaemonSleepTime);

                    gamesListManteinance.CleanInactiveGames();
                }
            }
            catch (ThreadInterruptedException)
            {
                // TODO: Definir aquí lógica de cara a cuando se quiera pausar el demonio.
            }
            catch (ThreadAbortException)
            {
                // TODO: Definir aquí lógica a realizar cuando se quiera cerrar la aplicación.
                Environment.Exit(0);
            }
        }
    }
}
