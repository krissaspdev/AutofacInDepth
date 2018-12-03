using System;
using Autofac;

namespace InstanceScope
{
    public interface ILog : IDisposable
    {
        void Write(string message);
    }

    public class ConsoleLog : ILog
    {
        public ConsoleLog()
        {
            Console.WriteLine($"Console log created at {DateTime.Now.Ticks}");
        }
        public void Write(string message)
        {
            Console.WriteLine(message);
        }

        public void Dispose()
        {
            Console.WriteLine("Console logger no longer required");
        }
    }


    public class Engine
    {
        public Engine()
        {
            Console.WriteLine("Engine is built");
        }
    }

    public class Car
    {
        private int id;

        public Car()
        {
            id = new Random().Next(5);
            Console.WriteLine($"Created car with id: {id}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleLog>().As<ILog>(); // default is InstancePerDependency()
            builder.RegisterType<Engine>().InstancePerLifetimeScope(); // one instance per scope
            builder.RegisterType<Car>().SingleInstance(); // single instance

            var container = builder.Build();



            using (var scope = container.BeginLifetimeScope())
            {
                var log = scope.Resolve<ILog>(); // <-- we use scope, not container !!!!
                log.Write("Testing!");
            }

            using (var scope = container.BeginLifetimeScope())
            {

                Console.WriteLine("");
                Console.WriteLine("Trying to resolve single instance twice:");
                scope.Resolve<Car>();
                scope.Resolve<Car>();
            }


            Console.WriteLine("");
            Console.WriteLine("We have only one instance per lifetime scope:");

            using (var scope1 = container.BeginLifetimeScope())
            {
                for (int i = 0; i < 3; i++)
                {
                    scope1.Resolve<Engine>();
                }
            }

            using (var scope2 = container.BeginLifetimeScope())
            {
                for (int i = 0; i < 3; i++)
                {
                    scope2.Resolve<Engine>();
                }
            }
        }
    }
}
