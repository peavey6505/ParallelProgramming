namespace TaskProgramming_S01
{
    internal class Program
    {
        static void Main(string[] args)
        {
           
            
        }

        static void RunExamples() {

            CancellationTokenSourceExample();
            CompositeCancellationTokensExample();
            WaitForTimeToPassExample();
        }

        static void WaitForTimeToPassExample()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var t = new Task(() =>
            {
                //Thread.Sleep(); // oddaje wątek do puli wątków, ale nie jest to zalecane, bo wątek jest blokowany i nie może być wykorzystany do innych zadań
                //SpinWait.SpinUntil(); // nie blokuje wątku, ale zużywa CPU, więc jest to dobre rozwiązanie dla krótkich operacji, zużywa CPU, nie robi context switching
                Console.WriteLine("Press anything. 5secs to disarm");
                bool cancelled = token.WaitHandle.WaitOne(5000); // czeka na sygnał anulowania lub upływ czasu
                Console.WriteLine(cancelled ? "Disarmed" : "Boom!");

            }, token);
            t.Start();

            Console.ReadKey();
            cts.Cancel();
        }

        static void CompositeCancellationTokensExample()
        {
            var planned = new CancellationTokenSource();
            var preventative = new CancellationTokenSource();
            var emergency = new CancellationTokenSource();

            var paranoid = CancellationTokenSource.CreateLinkedTokenSource(planned.Token, preventative.Token, emergency.Token);

            Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (true)
                {
                    paranoid.Token.ThrowIfCancellationRequested();
                    Console.WriteLine($"{i++}\t");
                    Thread.Sleep(1000);
                }
            }, paranoid.Token);

            Console.ReadKey();
            emergency.Cancel(); // or any other token can be cancelled to stop the task
        }

        static void CancellationTokenSourceExample()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            token.Register(() => // delegate executed when cancellation is requested
            {
                Console.WriteLine("Cancellation requested.");
            });

            var t = new Task(() =>
            {
                int i = 0;
                while (true)
                {
                    //if(token.IsCancellationRequested)
                    //{
                    //    //Console.WriteLine("Task is cancelled."); NOT RECOMMENDED
                    //    //break;

                    //    throw new OperationCanceledException(); 
                    //}

                    token.ThrowIfCancellationRequested(); // RECOMMENDED
                    Console.WriteLine(i++);
                }
            }, token);
            t.Start();


            Task.Factory.StartNew(() =>
            {
                token.WaitHandle.WaitOne(); // Waits until the cancellation is requested
                Console.WriteLine("Cancellation requested. Task is cancelled.");
            }, token);
            Console.ReadKey();
            cts.Cancel();
        }
    }
}
