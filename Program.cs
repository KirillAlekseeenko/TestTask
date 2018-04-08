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

            if(args.Length == 0)
            {
                Console.WriteLine("Parameter 'directoryPath' is required");
                return;
            }

            var directoryPath = args[0];
            if (Directory.Exists(directoryPath))
                DeserializeXML(directoryPath);
            else
                Console.WriteLine("directory was not found");

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
                            var file = new StreamReader(path);

                            try
                            {
                                XmlSerializer serializer = new XmlSerializer(typeof(Calculator));
                                calculator = (Calculator)serializer.Deserialize(file);
                                calculator.Filename = Path.GetFileName(path);
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine("Critical error in {0}: {1}", Path.GetFileName(path), ex.InnerException.Message);
                                return;
                            }
                            finally
                            {
                                file.Close();
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

            var maxCalculationsFilename = calculators.Min()?.Filename;
            if (maxCalculationsFilename == null)
                Console.WriteLine("\n\nThere are no valid xml files");
            else
                Console.WriteLine("\n\nMax number of calculations in: {0}", maxCalculationsFilename);

        }

    }
}
