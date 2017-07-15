using Autofac;
using NumeralCalculator.Worker;
using System;
using System.Collections.Generic;

namespace ClientConsole
{
    public class Runner : IStartable
    {
        private readonly IProcessor processor;
        private readonly IEnumerable<string> contents;

        public Runner(
            IProcessor processor,
            IEnumerable<string> contents)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }
            
            this.processor = processor;
            this.contents = contents;
        }

        public void Start()
        {
            IEnumerable<string> responses = this.processor.Process(this.contents);
            
            foreach(string response in responses)
            {
                Console.WriteLine(response);
            }
        }
    }
}