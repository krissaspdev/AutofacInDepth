using System;
using Autofac;

namespace ConstructorChoise
{
    public interface ILog
    {
        void Write(string message);
    }

    public interface IConsole
    {

    }

    public class ConsoleLog : ILog, IConsole
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class EmailLog : ILog
    {
        private const string adminEmail = "admin@foo.com";
        public void Write(string message)
        {
            Console.WriteLine($"Email sent to {adminEmail} : {message}");
        }
    }

    public class Engine
    {
        private ILog log;
        private int id;

        public Engine(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.id = new Random().Next();
        }

        public void Ahead(int power)
        {
            log.Write($"Engine [{id}] ahead {power}");
        }
    }

    public class Car
    {
        private Engine engine;
        private ILog log;

        public Car(Engine engine, ILog log)
        {
            this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Car(Engine engine)
        {
            this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.log = new EmailLog();
        }

        public void Go()
        {
            engine.Ahead(100);
            log.Write("Car going forward...");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<EmailLog>().As<ILog>();
            builder.RegisterType<ConsoleLog>().As<ILog>();
            builder.RegisterType<Engine>();
            builder.RegisterType<Car>()
                .UsingConstructor(typeof(Engine)); // please use constructor, that has one argument and the type of argument is Engine
            // this only affects for the type Car, type Engine is still using default ConsoleLog because it uses dependency injection container

            IContainer container = builder.Build();

            var car = container.Resolve<Car>();
            car.Go();
        }
    }

}
