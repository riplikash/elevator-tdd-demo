﻿using Alexprof.AutoMoq;
using Domain;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace ApplicationServices.Tests
{
    public class DapperAutoDataAttribute : InlineAutoDataAttribute
    {
        public DapperAutoDataAttribute(params object[] values)
            : base(new AutoFixtureMoqDataAttribute(), values)
        {
        }

        private class AutoFixtureMoqDataAttribute : AutoDataAttribute
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
               
            }
        }
    }

}