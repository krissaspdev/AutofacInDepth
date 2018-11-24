using System;
using Autofac;
using Autofac.Core;

namespace PropertyAndMethodInjection
{
    public class Parent
    {
        public override string ToString()
        {
            return "I'm your father";
        }
    }

    public class Child
    {
        public string Name { get; set; }
        public Parent Parent { get; set; }

        public void SetParent(Parent parent)
        {
            Parent = parent;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            var builder = new ContainerBuilder();
            builder.RegisterType<Parent>();

            // 1. resolving properties using PropertyAutowired
            //builder.RegisterType<Child>().PropertiesAutowired();

            //2. setting property manually
            //builder.RegisterType<Child>()
            //    .WithProperty("Parent", new Parent());

            //3. setting everything manually using component context
            //builder.Register(c =>
            //{
            //    var child = new Child();
            //    child.SetParent(c.Resolve<Parent>());
            //    return child;
            //});

            //4. using Activating Event Handler
            builder.RegisterType<Child>()
                .OnActivated((IActivatedEventArgs<Child> e) =>
                {
                    var p = e.Context.Resolve<Parent>(); // resolving
                    e.Instance.SetParent(p); // setting property
                });

            var conteiner = builder.Build();
            var parent = conteiner.Resolve<Child>().Parent;

            Console.WriteLine(parent);
        }
    }
}
