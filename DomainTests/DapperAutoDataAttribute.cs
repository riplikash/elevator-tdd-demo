using System;
using System.Collections.Generic;
using Alexprof.AutoMoq;
using Domain;
using Moq;
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

        private class ProjectCustomizations : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.RepeatCount = 5;
                fixture.Customize(new MultipleCustomization())
                    .Customize(new AutoConfiguredMoqCustomization());
                fixture.Register(() =>
                {
                    var service = new ElevatorService(fixture.Create<IElevator>());
                    var panels = fixture.Create<List<CallPanel>>();
                    foreach (var panel in panels)
                    {
                        service.RegisterCallPanel(panel);
                    }
                    return service;
                }
                    );
                fixture.Register(() =>
                {
                    var list = new List<Mock<ICallPanel>>();
                    for (var i = 1; i < 6; i++)
                    {
                        var panel = fixture.Create<Mock<ICallPanel>>();
                        panel.Setup(x => x.Floor).Returns(i);
                        list.Add(panel);
                    }
                    return list;
                });
            }
        }
    }
}