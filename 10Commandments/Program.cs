using System;
using System.Collections.Generic;
using System.IO;

namespace _10Commandments
{
    class Program
    {
        static string path;

        static void Main(string[] args)
        {
            if (args.Length != 0) path = args[0];
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                do Console.WriteLine("Please, specify a valid file path to check:"); while (string.IsNullOrWhiteSpace(path = Console.ReadLine()) || !File.Exists(path));

            Commandment1 commandment1 = new Commandment1();
            try
            {
                commandment1.Validate(File.ReadAllLines(path));
            }
            catch (CommandmentException e)
            {
                Console.Error.WriteLine(e);
            }
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

    class CommandmentException : Exception
    {
        public readonly int LineNumber;
        public readonly int CommandmentNumber;
        public readonly string Line;

        public CommandmentException(int lineNumber, int commandmentNumber, string line)
        {
            LineNumber = lineNumber;
            CommandmentNumber = commandmentNumber;
            Line = line;
        }

        public override string ToString()
        {
            return $"\nCommandment {CommandmentNumber} disrespected at line {LineNumber}:\n{Line}";
        }
    }

    interface ICommandment<T>
    {
        void Validate(T item);
    }
}
