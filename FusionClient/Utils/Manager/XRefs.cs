using System.Linq;
using System.Reflection;
using Il2CppSystem;
using Photon.Realtime;
using UnhollowerRuntimeLib.XrefScans;

namespace FusionClient.Modules
{
    internal static class XRefs
    {
        private static MethodInfo _OnMenuOpened;
        private static MethodInfo _OnMenuClosed;
        public static MethodInfo _OnPhotonPlayerJoined;
        public static MethodInfo _OnPhotonPlayerLeft;

        internal static bool CheckUsed(MethodBase methodBase, string methodName)
        {
            try
            {
                return XrefScanner.UsedBy(methodBase).Where(instance => instance.TryResolve() != null && instance.TryResolve().Name.Contains(methodName)).Any();
            }
            catch { }

            return false;
        }

        internal static MethodInfo OnMenuOpened
        {
            get
            {
                if (_OnMenuOpened == null)
                {
                    _OnMenuOpened = (from m in typeof(QuickMenu).GetMethods()
                                     where m.Name.StartsWith("Method_Private_Void_")
                                     where m.Name.Length <= 22
                                     where (from s in (from x in XrefScanner.XrefScan(m)
                                                       where x.Type == 0
                                                       select x).Select(delegate (XrefInstance x)
                                                       {
                                                           Object @object = x.ReadAsObject();
                                                           if (@object == null)
                                                           {
                                                               return null;
                                                           }
                                                           return @object.ToString();
                                                       })
                                            where s.StartsWith("Mic")
                                            select s).Count() == 3
                                     select m).FirstOrDefault();
                }
                return _OnMenuOpened;
            }
        }

        internal static MethodInfo OnMenuClosed
        {
            get
            {
                if (_OnMenuClosed == null)
                {
                    _OnMenuClosed = (from m in typeof(QuickMenu).GetMethods()
                                     where m.Name.StartsWith("Method_Public_Void_Boolean_")
                                     where m.Name.Length <= 29
                                     orderby XrefScanner.XrefScan(m).Count(x => x.Type == (XrefType)1)
                                     select m).ElementAt(3);
                }
                return _OnMenuClosed;
            }
        }

        internal static MethodInfo OnPhotonPlayerJoinMethod
        {
            get
            {
                if (_OnPhotonPlayerJoined == null)
                {
                    _OnPhotonPlayerJoined = typeof(VRCFlowNetworkManager).GetMethods().Single(delegate (MethodInfo it)
                    {
                        if (it.ReturnType == typeof(void) && it.GetParameters().Length == 1 && it.GetParameters()[0].ParameterType == typeof(Player))
                        {
                            return XrefScanner.XrefScan(it).Any(jt =>
                            {
                                if (jt.Type == XrefType.Global)
                                {
                                    Object @object = jt.ReadAsObject();
                                    if (@object != null)
                                    {
                                        if (@object.ToString().Contains("Enter"))
                                        {
                                            _OnPhotonPlayerJoined = it;
                                            return true;
                                        }
                                    }
                                    return false;
                                }
                                return false;
                            });
                        }
                        return false;
                    });
                }
                return _OnPhotonPlayerJoined;
            }
        }

        internal static MethodInfo OnPhotonPlayerLeftMethod
        {
            get
            {
                if (_OnPhotonPlayerLeft == null)
                {
                    _OnPhotonPlayerLeft = typeof(VRCFlowNetworkManager).GetMethods().Single(delegate (MethodInfo it)
                    {
                        if (it.ReturnType == typeof(void) && it.GetParameters().Length == 1 && it.GetParameters()[0].ParameterType == typeof(Player))
                        {
                            return XrefScanner.XrefScan(it).Any(jt =>
                            {
                                if (jt.Type == XrefType.Global
                                )
                                {
                                    Object @object = jt.ReadAsObject();
                                    if (@object != null)
                                    {
                                        if (@object.ToString().Contains("Left"))
                                        {
                                            _OnPhotonPlayerLeft = it;
                                            return true;
                                        }
                                    }
                                    return false;
                                }
                                return false;
                            });
                        }
                        return false;
                    });
                }
                return _OnPhotonPlayerLeft;
            }
        }
    }
}