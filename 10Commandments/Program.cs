using System;
using System.Collections.Generic;
using System.IO;

namespace _10Commandments
{
    class Program
    {
        static string path;
        static readonly ICommandment<IEnumerable<string>>[] documentCommandments = new ICommandment<IEnumerable<string>>[]
        {
            new Commandment1()
        };
        static readonly ICommandment<string>[] lineCommandments = new ICommandment<string>[]
        {
            new Commandment4()
        };


        static void Main(string[] args)
        {
            if (args.Length != 0) path = args[0];
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                do Console.WriteLine("Please, specify a valid file path to check:"); while (string.IsNullOrWhiteSpace(path = Console.ReadLine()) || !File.Exists(path));

            var lines = File.ReadAllLines(path);
            foreach (var c in documentCommandments)
            {
                try
                {
                    c.Validate(lines);
                }
                catch (CommandmentException e)
                {
                    Console.Error.WriteLine(e);
                }
            }

            foreach (var line in lines)
            {
                foreach (var c in lineCommandments)
                {
                    try
                    {
                        c.Validate(line);
                    }
                    catch (CommandmentException e)
                    {
                        Console.Error.WriteLine(e);
                    }
                }
            }

            Console.ReadLine();
        }
    }

    class Commandment1 : ICommandment<IEnumerable<string>>
    {
        public void Validate(IEnumerable<string> items)
        {
            string lastItem = "";
            int i = 0;
            foreach (var item in items)
            {
                i++;
                if (item.Contains("class ") && !item.Contains("\"") && !item.Contains("*"))
                {
                    if (!(lastItem.Contains("//") || lastItem.Contains("*/")))
                    {
                        throw new CommandmentException(i, 1, item);
                    }
                }
                lastItem = item;
            }
        }
    }

    class Commandment4 : ICommandment<string>
    {
        public void Validate(string item)
        {
            if (item.Contains("\t")) throw new CommandmentException(null, 4, item);
        }
    }

    class CommandmentException : Exception
    {
        public readonly int? LineNumber;
        public readonly int CommandmentNumber;
        public readonly string Line;

        public CommandmentException(int? lineNumber, int commandmentNumber, string line)
        {
            LineNumber = lineNumber;
            CommandmentNumber = commandmentNumber;
            Line = line;
        }

        public override string ToString()
        {
            return $"\nCommandment {CommandmentNumber} disrespected" + (LineNumber != null ? $" at line {LineNumber}" : "") + $":\n{Line}";
        }
    }

    interface ICommandment<T>
    {
        void Validate(T item);
    }
}
