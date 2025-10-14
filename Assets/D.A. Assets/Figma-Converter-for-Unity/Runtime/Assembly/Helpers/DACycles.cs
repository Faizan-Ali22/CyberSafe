using DA_Assets.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DA_Assets.FCU
{
    public class DACycles
    {
        public static async Task ForEach<T>(IList<T> source, Action<T> body, float iterationTimeout = 0, int beforeWaitItersCount = 0)
        {
            if (source.IsEmpty())
            {
                return;
            }

            for (int i = 0; i < source.Count; i++)
            {
                if (i != 0 &&
                    iterationTimeout != 0 &&
                    beforeWaitItersCount != 0 &&
                    i % beforeWaitItersCount == 0)
                {
                    await Task.Delay((int)(iterationTimeout * 1000));
                }

                body.Invoke(source[i]);
            }
        }
    }
}