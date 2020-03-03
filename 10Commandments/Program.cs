using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace _10Commandments
{
    class Program
    {
        static string path;
        static readonly ICommandment<IEnumerable<string>>[] documentCommandments = new ICommandment<IEnumerable<string>>[]
        {
            new Commandment1(),
            new Commandment10()
        };
        static readonly ICommandment<string>[] lineCommandments = new ICommandment<string>[]
        {
            new Commandment4(),
            new Commandment6(),
            new Commandment7()
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

            Console.WriteLine("\nDone");
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
                if (item.Contains("class ") && !item.Contains("\"") && !item.Contains("//") && !item.Contains("*"))
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

    class Commandment6 : ICommandment<string>
    {
        public void Validate(string item)
        {
            if (Regex.IsMatch(item, @"public (?:static final|final static) [a-zA-Z]* [A-Z0-9_]*[a-z]+"))
                throw new CommandmentException(null, 6, item);
        }
    }

    class Commandment7 : ICommandment<string>
    {
        public void Validate(string item)
        {
            if (item.Length > 120) throw new CommandmentException(null, 7, item);
        }
    }

    class Commandment10 : ICommandment<IEnumerable<string>>
    {
        public void Validate(IEnumerable<string> items)
        {
            string all = "";

            foreach (var item in items)
                all += item;

            if (Regex.IsMatch(all, @".*catch\s*\(.* ([a-zA-Z0-9_]*)\)\s*{\s*\n*\r*(\1.printStackTrace\(\);|return;)*\s*\n*\r*(\1.printStackTrace\(\);|return;)*\s*\n*\r*}"))
                //TODO: display line number and line
                throw new CommandmentException(null, 10, "[CANNOT DISPLAY LINE]");
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
