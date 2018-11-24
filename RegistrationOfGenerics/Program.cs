using System;
using System.Collections.Generic;
using Autofac;

namespace RegistrationOfGenerics
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // IList<T> --> List<T>
            // IList<int> --> List<int>
            builder.RegisterGeneric(typeof(List<>)).As(typeof(IList<>)); // we register open generic type

            IContainer container = builder.Build();

            var myList = container.Resolve<IList<int>>();  // when we resolve IList<int> we get List<int>
            Console.WriteLine(myList.GetType());
        }
    }
}
