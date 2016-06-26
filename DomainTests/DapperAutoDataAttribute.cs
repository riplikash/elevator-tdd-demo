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
                    fixture.Register(() => new ElevatorService(fixture.Create<List<ICallPanel>>(), fixture.Create<IElevator>(), fixture.Create<IElevatorControls>()));
                    //            fixture.Register(() => new List<callPanel>
                    //            {
                    //                new callPanel(fixture.Create<IElevatorService>(), 1, 5),
                    //                new callPanel(fixture.Create<IElevatorService>(), 2, 5),
                    //                new callPanel(fixture.Create<IElevatorService>(), 3, 5),
                    //                new callPanel(fixture.Create<IElevatorService>(), 4, 5),
                    //                new callPanel(fixture.Create<IElevatorService>(), 5, 5)
                    //            });
                    fixture.Register(() => new CallPanel(fixture.Create<IElevatorService>()));
                }
            }
        }
       
}