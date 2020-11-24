using System;

namespace AwsAppConfigClient.Logger
{
    public class ConsoleLogger
    {
        private readonly object _lock = new object();

        public void WriteConsistently(Action action)
        {
            lock (_lock)
            {
                action();
                Console.ResetColor();
            }
        }
    }
}