namespace DataSharingAndSynchronization
{
    public class BankAccount 
    {

        public int Balance { get; private set; }


        /*
        // LOCK EXAMPLE
        public object padlock = new object();
        public void Deposit(int amount)
        {
            // += NOT ATOMIC
            // op1: temp <- getBalance() + amount'
            // op2: set_Balance(temp)
            //thats why there are interruptions without locking

            lock (padlock)
            {
                Balance += amount;
            }
        }

        public void Withdraw(int amount) 
        {
            lock (padlock)
            {
                Balance -= amount;
            }
        }*/
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            var ba = new BankAccount();

            for (int i = 0; i< 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        ba.Deposit(100);
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        ba.Withdraw(100);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Final balance is {ba.Balance} ");

        }
    }
}
