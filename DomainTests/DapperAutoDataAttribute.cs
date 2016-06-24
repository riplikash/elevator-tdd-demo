using System;
using System.Collections.Generic;
using Alexprof.AutoMoq;
using Domain;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace DomainTests
{
        public class DapperAutoDataAttribute : InlineAutoDataAttribute
        {
            public DapperAutoDataAttribute(params object[] values)
                : base(new AutoFixtureMoqDataAttribute(), values)
            {
            }
    
            public class AutoFixtureMoqDataAttribute : AutoDataAttribute
            {
                public AutoFixtureMoqDataAttribute()
                    : base(new Fixture()
                        .Customize(new ProjectCustomizations())
                        //.Customize(new ConstructorCustomization(typeof (ExampleController), new GreedyConstructorQuery()))                
                        )
                {
                }
            }
            internal class ProjectCustomizations : ICustomization
            {
                public void Customize(IFixture fixture)
                {
    
                    fixture.RepeatCount = 5;
                    fixture.Customize(new MultipleCustomization())
                        .Customize(new AutoConfiguredMoqCustomization());
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
       
}