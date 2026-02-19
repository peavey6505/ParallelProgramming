using System.Collections.Concurrent;

namespace ConcurrentCollections
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var bag = new ConcurrentBag<int>(); // no FIFO, LIFO, or any other order
            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                var i1 = i;
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    bag.Add(i1);
                    Console.WriteLine($"Task: {Task.CurrentId} has added: {i1}");
                    int result;
                    if (bag.TryPeek(out result))
                    {
                        Console.WriteLine($"Task: {Task.CurrentId} has peeked the value: {result}");
                    }
                }));
            }


            Task.WaitAll(tasks.ToArray());

            int last;
            if(bag.TryTake(out last))
            {
                Console.WriteLine($"Took {last} from the bag");
            }

        }

        public static void AddParis()
        {
            bool success = capitals.TryAdd("France", "Paris");
            string who = Task.CurrentId.HasValue ? $"Task {Task.CurrentId.Value}" : "Main thread";
            Console.WriteLine($"{who} {(success ? "added" : "could not add")} the capital of France");
        }
        private static ConcurrentDictionary<string, string> capitals = new ConcurrentDictionary<string, string>();
        static void ConcurrentDictionaryExample()
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
        static void ConcurrentQueueExample()
        {
            var q = new ConcurrentQueue<int>();
            q.Enqueue(1);
            q.Enqueue(2);

            int result;
            if (q.TryDequeue(out result))
            {
                Console.WriteLine($"Dequeued {result}");
            }

            if (q.TryPeek(out result))
            {
                Console.WriteLine($"Front element is {result}");
            }
        }
        static void ConcurrentStackExample() {
            var stack = new ConcurrentStack<int>();
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            int result;

            if (stack.TryPeek(out result))
            {
                Console.WriteLine($"Top element is {result}");
            }

            if (stack.TryPop(out result))
            {
                Console.WriteLine($"Popped {result}");
            }

            var items = new int[5];
            if (stack.TryPopRange(items, 0, 5) > 0)
            {
                var text = string.Join(", ", items.Select(i => i.ToString()));
                Console.WriteLine($"Popped these items: {text}");
            }

        }

    }
}
