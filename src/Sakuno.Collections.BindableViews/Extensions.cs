using System.Runtime.CompilerServices;

namespace Sakuno.Collections.BindableViews
{
    static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EnsurePositiveIndex(this int index)
        {
            if (index < 0)
                index = ~index;

            return index;
        }
    }
}
