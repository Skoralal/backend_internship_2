using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Fuse8.BackendInternship.Domain;

namespace Fuse8.Benchmark
{
    [MemoryDiagnoser(displayGenColumns: true)]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class AccountProcessorBenchmark
    {
        private AccountProcessor _processor;
        private BankAccount _bankAccount;

        [GlobalSetup]
        public void Setup()
        {
            _processor = new AccountProcessor();
            _bankAccount = new BankAccount
            {
                TotalAmount = 10000,
                LastOperation = new BankOperation
                {
                    TotalAmount = 50000,
                    OperationInfo0 = 142000,
                    OperationInfo1 = 243600,
                    OperationInfo2 = 308400
                },
                PreviousOperation = new BankOperation
                {
                    TotalAmount = 30000,
                    OperationInfo0 = 135244,
                    OperationInfo1 = 784525,
                    OperationInfo2 = 357321
                }
            };
        }

        [Benchmark]
        public decimal Base()
        {
            return _processor.Calculate(_bankAccount);
        }

        [Benchmark]
        public decimal Optimized()
        {
            return _processor.CalculatePerformed(ref _bankAccount);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<AccountProcessorBenchmark>();
        }
    }
}