using System;

namespace IL.Game
{
    public class Singleton<T> where T : new()
    {
        protected static T mInstance;

        public static T Instance
        {
            get
            {
                if (Singleton<T>.mInstance == null)
                {
                    Singleton<T>.mInstance = Activator.CreateInstance<T>();
                }
                return Singleton<T>.mInstance;
            }
        }

        public static bool Exists
        {
            get
            {
                return Singleton<T>.mInstance != null;
            }
        }

        protected Singleton()
        {
        }

        public static void Init()
        {
            if (Singleton<T>.mInstance == null)
            {
                Singleton<T>.mInstance = Activator.CreateInstance<T>();
            }
        }
    }
}