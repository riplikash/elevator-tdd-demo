using System;
using System.Collections.Generic;
using Domain;
using Ploeh.AutoFixture;

namespace DomainTests
{
    public class MyCustomizations : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.RepeatCount = 5;
            fixture.Register(() => new ElevatorService(5, fixture.Create<List<IExternalCallInterface>>(), fixture.Create<IElevator>(), fixture.Create<IElevatorInteriorInterface>()));
//            fixture.Register(() => new List<ExternalCallInterface>
//            {
//                new ExternalCallInterface(fixture.Create<IElevatorService>(), 1, 5),
//                new ExternalCallInterface(fixture.Create<IElevatorService>(), 2, 5),
//                new ExternalCallInterface(fixture.Create<IElevatorService>(), 3, 5),
//                new ExternalCallInterface(fixture.Create<IElevatorService>(), 4, 5),
//                new ExternalCallInterface(fixture.Create<IElevatorService>(), 5, 5)
//            });
            fixture.Register(() => new ExternalCallInterface(fixture.Create<IElevatorService>(), new Random().Next(1, 5), 5));
        }
    }
}