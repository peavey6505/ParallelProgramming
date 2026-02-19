using System.Threading;
using System.Threading.Tasks;

namespace DataSharingAndSynchronization
{
    public class BankAccount 
    {
        private int balance;

        public int Balance { get => balance; private set => balance = value; }
        public void Deposit(int amount)
        {
           balance += amount;

        }

        public void Withdraw(int amount)
        {
           balance -= amount;
        }




        // INTERLOCKED EXAMPLE
        //public void Deposit(int amount)
        //{
        //    Interlocked.Add(ref balance, amount);
        //    //Interlocked.MemoryBarrier(); // ensures avoid reordering

        //}

        //public void Withdraw(int amount)
        //{
        //    Interlocked.Add(ref balance, -amount);
        //}


        /*// LOCK EXAMPLE
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

            //LockRecursion(5);
            //SpinLockExample();

        }
        static SpinLock sl = new SpinLock(true);

        static void LockRecursion(int x) // example why do not do it
        {
            bool lockTaken = false;

            try
            {
                sl.Enter(ref lockTaken);
            } 
            catch(LockRecursionException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (lockTaken)
                {
                    Console.WriteLine($"Took a lock x = {x}");
                    LockRecursion(x - 1);
                    sl.Exit();
                }
                else
                {
                    Console.WriteLine($"Failed to take a lock, x= {x}");
                }
            }
        }
        static void SpinLockExample()
        {
            var tasks = new List<Task>();
            var ba = new BankAccount();

            SpinLock spinLock = new SpinLock();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        var lockTaken = false; // variable for confirmation if the lock is taken or not
                        try
                        {
                            spinLock.Enter(ref lockTaken); // if lock is not taken, it will wait and try again until it can take the lock
                            ba.Deposit(100);
                        }
                        finally
                        {
                            if (lockTaken) // if the lock is taken, then we can release it, otherwise we should not call Exit() because it will throw an exception
                            {
                                spinLock.Exit(); // release the lock
                            }
                        }
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        var lockTaken = false;
                        try
                        {
                            spinLock.Enter(ref lockTaken);
                            ba.Withdraw(100);
                        }
                        finally
                        {
                            if (lockTaken)
                            {
                                spinLock.Exit();
                            }
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Final balance is {ba.Balance} ");
        }
    }
}
