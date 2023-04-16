using iRacingBusinessLayer.Models;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace iRacingBusinessLayer
{
    public class IOData
    {
        public static void SerielizeSchedule(Schedule schedule, string filename)
        {
            if(File.Exists(filename))
            {
                File.Delete(filename);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, schedule);
            stream.Close();
        }

        public static Schedule DeserielizeSchedule(string filename)
        {
            if (!File.Exists(filename)) return null;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Schedule schedule = (Schedule)formatter.Deserialize(stream);
            stream.Close();
            return schedule;
        }

        public static async Task SerializeJsonAsync<T>(T t, string filename)
        {
            await Task.Run(() => {
                string json = ExportConverter.SerializeJson(t);
                StringToFile(json, filename);
            });
        }

        public static T DeserializeJson<T>(string filename)
        {
            if (!File.Exists(filename)) throw new Exception("No existe fichero");
            string jsonlines = File.ReadAllText(filename);
            return ExportConverter.DeserializeJson<T>(jsonlines);
        }

        public static async Task SerializeCsvAsync<T>(T t, string filename)
        {
            await Task.Run(() => {
                List<string> lines = ExportConverter.SerializeToCSV<T>(t);
                File.WriteAllText(filename, string.Join(Environment.NewLine, lines));
            });
        }

        public static async Task SerializeExcelAsync(List<Serie> series, string filename)
        {
            await Task.Run(() => {
                ExportConverter.SerializeExcel(series, filename);
            });
        }

        /*
            public static void SerializeScheduleExcel(List<Serie> series, string filename, bool isRepeatedTracksCheck, List<IRacingContent> tracks)
            {
                ExportConverter.SerializeScheduleExcel(series, filename, isRepeatedTracksCheck, tracks);
            }
         */

        public static async Task SerializeScheduleExcelAsync(List<Serie> series, string filename, bool isRepeatedTracksCheck, List<IRacingContent> tracks)
        {
            await Task.Run(() => {
                ExportConverter.SerializeScheduleExcel(series, filename, isRepeatedTracksCheck, tracks);
            });
        }

        public static void StringToFile(string line, string filename)
        {
            if (File.Exists(filename)) File.Delete(filename);
            File.WriteAllText(filename, string.Join(Environment.NewLine, line));
        }

        public static void LinesToFile(List<string> lines, string filename)
        {
            if (File.Exists(filename)) File.Delete(filename);
            File.WriteAllText(filename, string.Join(Environment.NewLine, lines));
        }

        public static List<string> LoadLinesFromFile(string filename)
        {
            return File.ReadAllText(filename).Split(Environment.NewLine).ToList();
        }
    }
}
