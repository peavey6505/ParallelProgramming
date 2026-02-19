using System.Collections.Concurrent;

namespace ConcurrentCollections
{
    internal class Program
    {
        private static ConcurrentDictionary<string,string> capitals = new ConcurrentDictionary<string, string>();

        public static void AddParis()
        {
            bool success = capitals.TryAdd("France", "Paris");
            string who = Task.CurrentId.HasValue ? $"Task {Task.CurrentId.Value}" : "Main thread";
            Console.WriteLine($"{who} {(success ? "added" : "could not add")} the capital of France");
        }
        static void Main(string[] args)
        {
            Task.Factory.StartNew(() => AddParis()).Wait();
            AddParis();

            //capitals["Russia"] = "Leningrad";
            capitals.AddOrUpdate("Russia", "Moscow", (k, old) => old + "---> Moscow"); //if not exists, add Moscow, if exists, update with old value + Moscow
            Console.WriteLine($"The capital of Russia is {capitals["Russia"]}");

            //capitals["Sweden"] = "Uppsala";
            var capOfSweden = capitals.GetOrAdd("Sweden", "Stockholm");
            Console.WriteLine($"The capital of Sweden is {capOfSweden}");

            const string toRemove = "Russia";
            string removed;
            var didRemove = capitals.TryRemove(toRemove, out removed);
            if (didRemove)
            {
                Console.WriteLine($"Just removed {removed}");
            }
            else
            {
                Console.WriteLine($"Couldn't remove {toRemove}");
            }

            
        }
    }
}
