﻿using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Infrastructure
{
    public class DemoElevator : IElevator
    {
        public Task MoveUpAsync()
        {
            Thread.Sleep(3000);
            return Task.FromResult(0);
        }

        public Task MoveDownAsync()
        {
            Thread.Sleep(3000);
            return Task.FromResult(0);
        }
    }
}