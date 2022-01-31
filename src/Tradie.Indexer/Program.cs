// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Runtime.InteropServices;
using Tradie.Indexer;

Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

long startBytes = currentProcess.WorkingSet64;

var test = new IndexTest();
//test.searchByBruteForce();
test.searchByBlocks();

GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

currentProcess = System.Diagnostics.Process.GetCurrentProcess();
long endBytes = currentProcess.WorkingSet64;

test.printResults();

Console.WriteLine($"Started off with using {startBytes / (1024 * 1024)}MB, ended with {endBytes / (1024*1024)}MB.");