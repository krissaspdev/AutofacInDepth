using System;
using Autofac;

namespace LambdaExpressionComponents
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

        public Engine(ILog log, int id)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.id = id;
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

            builder.Register((IComponentContext c) => new Engine(c.Resolve<ILog>(), 123)); // we specify which constructor to call by lambda
            // we still using container (access the container through component context) and login interface is taken from component
            // second parameter is harcoded

            //builder.RegisterType<Engine>();
            builder.RegisterType<Car>();

            IContainer container = builder.Build();

            var car = container.Resolve<Car>();
            car.Go();
        }
    }

}
