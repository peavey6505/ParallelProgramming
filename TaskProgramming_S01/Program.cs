namespace TaskProgramming_S01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSourceExample();
            CompositeCancellationTokensExample();
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
