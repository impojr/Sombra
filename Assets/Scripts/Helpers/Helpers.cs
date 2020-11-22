using System;

namespace Assets.Scripts.Helpers
{
    public static class Helpers
    {
        public static void NullChecker(object obj, string errorMsg)
        {
            if (obj == null)
                throw new NullReferenceException(errorMsg);
        }
    }
}
