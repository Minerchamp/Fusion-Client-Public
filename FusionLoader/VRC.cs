using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FusionLoader
{
    internal static class VRC
    {
        private static MethodInfo _onApplicationStart;
        private static MethodInfo _onApplicationQuit;
        private static MethodInfo _onSceneWasLoaded;
        private static MethodInfo _onSceneWasUnloaded;
        private static MethodInfo _onSceneWasInitialized;
        private static MethodInfo _onUpdate;
        private static MethodInfo _OnLateUpdate;

        private static void GetMethods()
        {
            var main = Loader.ModType;
            _onApplicationStart = main?.GetMethod("OnApplicationStart");
            _onApplicationQuit = main?.GetMethod("OnApplicationQuit");
            _onSceneWasLoaded = main?.GetMethod("OnSceneWasLoaded");
            _onSceneWasUnloaded = main?.GetMethod("OnSceneWasUnloaded");
            _onSceneWasInitialized = main?.GetMethod("OnSceneWasInitialized");
            _onUpdate = main?.GetMethod("OnUpdate");
            _OnLateUpdate = main?.GetMethod("OnLateUpdate");
        }

        public static void OnApplicationStart()
        {
            GetMethods();
            if (_onApplicationStart != null)
            {
                _onApplicationStart.Invoke(null, new object[]
                {
                    ((GuidAttribute)typeof(Loader).Assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value,
                });
            }
        }

        public static void OnApplicationQuit()
        {
            if (_onApplicationQuit != null)
            {
                _onApplicationQuit.Invoke(null, null);
            }
        }

        public static void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (_onSceneWasInitialized != null)
            {
                _onSceneWasInitialized.Invoke(null, new object[]
                {
                    buildIndex,
                    sceneName
                });
            }
        }

        public static void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (_onSceneWasLoaded != null)
            {
                _onSceneWasLoaded.Invoke(null, new object[]
                {
                    buildIndex,
                    sceneName
                });
            }
        }

        public static void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if (_onSceneWasUnloaded != null)
            {
                _onSceneWasUnloaded.Invoke(null, new object[]
                {
                    buildIndex,
                    sceneName
                });
            }
        }

        public static void OnUpdate()
        {
            if (_onUpdate != null)
            {
                _onUpdate.Invoke(null, null);
            }
        }

        public static void OnLateUpdate()
        {
            if (_OnLateUpdate != null)
            {
                _OnLateUpdate.Invoke(null, null);
            }
        }
    }
}
