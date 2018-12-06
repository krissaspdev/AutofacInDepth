using System;
using Autofac;
using Autofac.Core;

namespace LifetimeEventsCodeStartup
{
    public class Parent
    {
        public override string ToString()
        {
            return "I am your father";
        }
    }

    public class Child
    {
        public string Name { get; set; }
        public Parent Parent { get; set; }


        public Child()
        {
            Console.WriteLine("Child being created");
        }

        public void SetParent(Parent parent)
        {
            Parent = parent;
        }

        public override string ToString()
        {
            return "Hi there";
        }
    }

    public class BadChild : Child
    {
        public override string ToString()
        {
            return "I hate you";
        }
    }

    public class MyClass : IStartable
    {
        public MyClass()
        {
            Console.WriteLine("MyClass ctor");
        }

        public void Start()
        {
            Console.WriteLine("Container being build");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MyClass>()
                .AsSelf()
                .As<IStartable>()
                .SingleInstance();

            builder.RegisterType<Parent>();
            builder.RegisterType<Child>()
                .OnActivating((IActivatingEventArgs<Child> a) =>
                {
                    Console.WriteLine("Child activating");
                    a.Instance.Parent = a.Context.Resolve<Parent>();
                    //a.ReplaceInstance(new BadChild()); // you can overdrive type with subtype if needed
                })
                .OnActivated(a =>
                {
                    Console.WriteLine("Child activated");
                })
                .OnRelease(a =>
                {
                    Console.WriteLine("Child about to be removed");
                });

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var child = scope.Resolve<Child>();
                var parent = child.Parent;

                Console.WriteLine(child);
                Console.WriteLine(parent);
            }
        }
    }
}
