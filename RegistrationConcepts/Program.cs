using System;
using Autofac;

namespace RegistrationConcepts
{
    public interface ILog
    {
        void Write(string message);
    }

    public class ConsoleLog: ILog
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
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
            builder.RegisterType<ConsoleLog>().As<ILog>();
            builder.RegisterType<Engine>();
            builder.RegisterType<Car>();

            IContainer container = builder.Build();

            var car = container.Resolve<Car>();
            car.Go();
        }
    }
}
