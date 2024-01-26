using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ChatApp.Data.ValueGenerators
{
    public class LongValueGenerator : ValueGenerator
    {
        public override bool GeneratesTemporaryValues => true;
        private long currentKey = long.MinValue;

        protected override object? NextValue(EntityEntry entry)
        {
            return currentKey++;
        }
    }
}
