﻿using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using NetRegex = System.Text.RegularExpressions.Regex;

using IronRure;

namespace Alice
{
    class Program
    {
        static void Main(string[] args)
        {
            var exePath = Assembly.GetEntryAssembly().Location;;
            var exeFolder = Path.GetDirectoryName(exePath);
            var path = Path.Combine(exeFolder, AliceFilename);
            var text = File.ReadAllText(path, Encoding.UTF8);

            BenchRegex("legged", @"\b(\w+)-legged", text);
            BenchRegex("alice", @"Alice", text);
            BenchRegex("numbers", @"\d+", text);
            BenchRegex("email", @"\w+@\w+.\w+", text);
            BenchRegex("quotes", "\"[^\"]+\"", text);
            BenchRegex("quote_said", "\"[^\"]+\"\\s+said", text);
            BenchRegex("section", @"(\*\s+){4}\*", text);
            BenchRegex("repeated_negation", @"[a-q][^u-z]{13}x", text);
            BenchRegex("ing_suffix", @"[a-zA-Z]+ing", text);
            BenchRegex("name_alt", @"Alice|Adventure", text);
            BenchRegex("name_alt2", @"Alice|Hatter|Cheshire|Dinah", text);
            BenchRegex("nomatch_uncommon", @"zqj", text);
            BenchRegex("nomatch_common", @"aei", text);
            BenchRegex("common", "(?i)the", text);
            BenchRegex("dotplus", ".+", text);
            BenchRegex("dotplus_nl", "(?s).+", text);
            BenchRegex("alice_hattter", "Alice.{0,25}Hatter|Hatter.{0,25}Alice", text);

            // IronRure iteration doesn't handle 0-length matches yet
            // BenchRegex("dotstar", ".*", text);

            Console.WriteLine("Benchmark completed");
        }

        /// <summary>
        ///   Benchmark the execution of a given regex of the given
        ///   text. Compiles the regex using both the .NET engine and
        ///   Rure outside of the benchmark and then benches finding
        ///   all the matches in the text.
        /// </summary>
        private static void BenchRegex(string name, string pattern, string text)
        {
            var rure = new Regex(pattern);
            var net = new NetRegex(pattern);
            var bytes = Encoding.UTF8.GetBytes(text);

            Console.WriteLine("{0} ({1})", name, pattern);

            var results = new List<BenchResult>();
            
            // by doing the search this way we have to convert all 172k into
            // a .NET string for each step in the search. Surprisingly this
            // is _still_ faster than .NET in some cases.
            results.Add(Bench($"rure::{name}", () => {
                    var match = rure.Find(text);
                    while (match.Matched)
                    {
                        match = rure.Find(text, match.End);
                    }
                }));
            results.Add(Bench($"byts::{name}", () => {
                    var match = rure.Find(bytes);
                    while (match.Matched)
                    {
                        match = rure.Find(bytes, match.End);
                    }
                }));
            results.Add(Bench($".net::{name}", () => {
                    var match = net.Match(text);
                    while (match.Success)
                    {
                        match = match.NextMatch();
                    }
                }));

            foreach (var r in results)
                Console.WriteLine(r);
        }

        /// <summary>
        ///   Benchmark an Action. Executes the action repeatedly and
        ///   returns a results object detailing the outcome.
        /// </summary>
        private static BenchResult Bench(string name, Action benchee)
        {
            var sw = new Stopwatch();
            var resultTicks = new List<long>(4);
            for (int i = 0; i < 4; i++)
            {
                sw.Start();
                benchee();
                sw.Stop();
                resultTicks.Add(sw.ElapsedTicks);
                sw.Reset();
            }

            return new BenchResult(name, resultTicks);
        }

        private static string AliceFilename = "Alice's_Adventures_in_Wonderland.txt";
    }
}
