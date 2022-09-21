using SyncsContext;

class Program
{
  
       async static Task Main(String[] args)
        {
            CancellationToken token = new CancellationToken();  
            var cesp = new SingleThreadSynchronizationContext(token);
             cesp.Post(TestAsyncMethod, null);
        }
    
        async static void TestAsyncMethod(object obj)
        {
        Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
        await Task.Run(() => Console.WriteLine(Thread.CurrentThread.ManagedThreadId));
        Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
        }
}
