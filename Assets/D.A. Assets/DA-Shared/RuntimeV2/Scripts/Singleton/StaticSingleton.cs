using System;

namespace DA_Assets.Singleton
{
    public class StaticSingleton<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Activator.CreateInstance<T>();
                }

                return instance;
            }
        }
    }
}