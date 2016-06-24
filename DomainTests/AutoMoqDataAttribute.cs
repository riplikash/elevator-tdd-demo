using Alexprof.AutoMoq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace DomainTests
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new MyCustomizations()))
        {
        }
    }
}