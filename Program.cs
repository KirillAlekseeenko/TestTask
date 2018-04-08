using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Diagnostics;

namespace TestTask_Nival
{
    class MainClass
    {
        private const int NumberOfThreads = 4;

        public static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew();

            var directoryPath = args[0];
            DeserializeXML(directoryPath);

            watch.Stop();
            Console.WriteLine("\nThe execution time: {0}", watch.Elapsed);
        }

        private static void DeserializeXML(string directoryPath)
        {
            var filePaths = Directory.GetFiles(directoryPath, "*.xml");

            var pathQueue = new ConcurrentQueue<string>(filePaths);
            var calculators = new ConcurrentBag<Calculator>();

            var threads = new List<Thread>(NumberOfThreads);
            for (int i = 0; i < threads.Capacity; i++)
            {
                threads.Add(new Thread(() =>
                {
                    while (!pathQueue.IsEmpty)
                    {
                        if (pathQueue.TryDequeue(out string path))
                        {
                            Calculator calculator;

                            using (var file = new StreamReader(path))
                            {
                                XmlSerializer serializer = new XmlSerializer(typeof(Calculator));
                                calculator = (Calculator)serializer.Deserialize(file);
                                calculator.Filename = Path.GetFileName(path);
                            }

                            calculators.Add(calculator);
                        }
                        else
                            continue;
                    }
                }));
            }

            threads.ForEach(thread => thread.Start());
            threads.ForEach(thread => thread.Join());

            foreach (var calculator in calculators)
            {
                var result = calculator.Process();

                Console.WriteLine("\nFile: {0}", calculator.Filename);
                Console.WriteLine("\nLog: \n{0}", calculator.Log);
                Console.WriteLine("Result: {0}", result);
            }

            var maxCalculationsFilename = calculators.Min().Filename;

            Console.WriteLine("\n\nMax calculation file: {0}", maxCalculationsFilename);
        }

    }
}
