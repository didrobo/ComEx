using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ComEx.Helpers
{
    public class Log
    {
        private static Log myClsLog = new Log();
        private static string nombre = "";
        private static string transaccion = "error";
        static TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
        private static DateTime fechaActual;

        public static Log GetInstancia()
        {
            fechaActual = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
            nombre = "Log-";
            return myClsLog;
        }

        public void EscribirLog(string mensaje, string tipo, string origenLog, string usuario = "Usuario")
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string dir = Path.GetDirectoryName(path);
                dir += @"\Log\" + fechaActual.ToString("yyyyMMdd");

                if (!Directory.Exists(dir))
                {
                    BorrarLogs();
                    Directory.CreateDirectory(dir); // inside the if statement
                }

                using (StreamWriter sw = new StreamWriter(dir + @"\" + nombre + tipo + ".txt", true))
                {
                    sw.WriteLine(usuario + " - " + origenLog + " - " + TimeZoneInfo.ConvertTime(DateTime.Now, timezone).ToString() + " - " + mensaje);
                    sw.Flush();
                }
            }
            catch (Exception ex) { }
        }

        private static void BorrarLogs()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string dir = Path.GetDirectoryName(path);
                dir += @"\Log";
                foreach (string carpeta in Directory.GetDirectories(dir))
                {
                    DateTime fechaCarpeta = Directory.GetCreationTime(carpeta);
                    int dias = (int)(DateTime.Now - fechaCarpeta).TotalDays;

                    if (dias >= Properties.Settings.Default.diasEliminarLog)
                    {
                        Directory.Delete(carpeta, true);
                    }
                }
            }
            catch (Exception ex) { }
        }
    }
}