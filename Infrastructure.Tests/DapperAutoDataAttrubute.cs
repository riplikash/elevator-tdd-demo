using Alexprof.AutoMoq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Infrastructure.Tests
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
            }
        }
    }

}