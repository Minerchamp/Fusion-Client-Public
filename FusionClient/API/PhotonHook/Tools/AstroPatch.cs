namespace FusionClient
{
    #region Imports

    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Text;
    using FC;
    using FCConsole;
    using FusionClient.Core;
    using HarmonyLib;
    using MelonLoader;

    #endregion Imports

    internal class FusionPatch
    {

        private Type type { get; set; }
        private string PatchIdentifier { get; } = "Patches";
        internal MethodInfo TargetMethod_MethodInfo { get; set; }

        internal MethodBase TargetMethod_MethodBase { get; set; }
        internal string HarmonyInstanceID { get; set; }
        internal HarmonyMethod Prefix { get; set; }
        internal HarmonyMethod PostFix { get; set; }
        internal HarmonyMethod Transpiler { get; set; }
        internal HarmonyMethod Finalizer { get; set; }

        internal HarmonyMethod IlManipulator { get; set; }
        internal Harmony Instance { get; set; }
        private bool HasThrownException { get; set; } = false;
        private bool ShowErrorOnConsole { get; set; } = true;
        private bool isActivePatch { get; set; } = false;
        private bool isMethodInfoPatch { get; set; } = false;

        internal string TargetPath_MethodInfo => $"{TargetMethod_MethodInfo?.DeclaringType?.FullName}.{TargetMethod_MethodInfo?.Name}";
        internal string TargetPath_base => $"{TargetMethod_MethodInfo?.DeclaringType?.FullName}.{TargetMethod_MethodBase.Name}";
        internal string PatchType
        {
            get
            {
                StringBuilder patchtype = new StringBuilder();
                if (PostFix != null)
                {
                    string patch = $"PostFix Patch : {PostFix.method?.DeclaringType?.FullName}.{PostFix.method?.Name} ";
                    if (patchtype.Length != 0)
                    {
                        patchtype.AppendLine(patch);
                    }
                    else
                    {
                        patchtype.Append(patch);
                    }
                }

                if (Prefix != null)
                {
                    string patch = $"Prefix Patch : {Prefix.method?.DeclaringType?.FullName}.{Prefix.method?.Name} ";
                    if (patchtype.Length != 0)
                    {
                        patchtype.AppendLine(patch);
                    }
                    else
                    {
                        patchtype.Append(patch);
                    }
                }

                if (Transpiler != null)
                {
                    string patch = $"Transpiler Patch : {Transpiler.method?.DeclaringType?.FullName}.{Transpiler.method?.Name} ";
                    if (patchtype.Length != 0)
                    {
                        patchtype.AppendLine(patch);
                    }
                    else
                    {
                        patchtype.Append(patch);
                    }
                }

                if (Finalizer != null)
                {
                    string patch = $"Finalizer Patch : {Finalizer.method?.DeclaringType?.FullName}.{Finalizer.method?.Name} ";
                    if (patchtype.Length != 0)
                    {
                        patchtype.AppendLine(patch);
                    }
                    else
                    {
                        patchtype.Append(patch);
                    }
                }

                if (IlManipulator != null)
                {
                    string patch = $"IlManipulator Patch : {IlManipulator.method?.DeclaringType?.FullName}.{IlManipulator.method?.Name} ";
                    if (patchtype.Length != 0)
                    {
                        patchtype.AppendLine(patch);
                    }
                    else
                    {
                        patchtype.Append(patch);
                    }
                }

                if (patchtype.Length == 0)
                {
                    return "Failed to Read Patch.";
                }

                return patchtype.ToString();
            }
        }

        internal FusionPatch(MethodInfo TargetMethod, HarmonyMethod Prefix = null, HarmonyMethod PostFix = null, HarmonyMethod Transpiler = null, HarmonyMethod Finalizer = null, HarmonyMethod ILmanipulator = null, bool showErrorOnConsole = true)
        {
            if (TargetMethod == null || (Prefix == null && PostFix == null && Transpiler == null && Finalizer == null && ILmanipulator == null))
            {
                StringBuilder FailureReason = new StringBuilder();
                if (Prefix == null)
                {
                    string reason = "Prefix Method is null";
                    if (FailureReason.Length != 0)
                    {
                        FailureReason.AppendLine(reason);
                    }
                    else
                    {
                        FailureReason.Append(reason);
                    }
                }

                if (PostFix == null)
                {
                    string reason = "PostFix Method is null";
                    if (FailureReason.Length != 0)
                    {
                        FailureReason.AppendLine(reason);
                    }
                    else
                    {
                        FailureReason.Append(reason);
                    }
                }

                if (Transpiler == null)
                {
                    string reason = "Transpiler Method is null";
                    if (FailureReason.Length != 0)
                    {
                        FailureReason.AppendLine(reason);
                    }
                    else
                    {
                        FailureReason.Append(reason);
                    }
                }

                if (Finalizer == null)
                {
                    string reason = "Finalizer Method is null";
                    if (FailureReason.Length != 0)
                    {
                        FailureReason.AppendLine(reason);
                    }
                    else
                    {
                        FailureReason.Append(reason);
                    }
                }

                if (ILmanipulator == null)
                {
                    string reason = "ILmanipulator Method is null";
                    if (FailureReason.Length != 0)
                    {
                        FailureReason.AppendLine(reason);
                    }
                    else
                    {
                        FailureReason.Append(reason);
                    }
                }

                if (TargetMethod != null)
                {
                    Logs.Log($"[{PatchIdentifier}] TargetMethod is NULL", ConsoleColor.Red);
                }
                //else
                //{
                //    if (FC.Utils.Config.Main.IsDeveloper)
                //    {
                //        Logs.Log($"[{PatchIdentifier}] Failed to Patch {TargetMethod.DeclaringType?.FullName}.{TargetMethod?.Name} because {FailureReason}.", ConsoleColor.Red);
                //    }
                //    else
                //    {
                //        Logs.Log($"[{PatchIdentifier}] Failed to Patch {TargetMethod.Name}", ConsoleColor.Red);
                //    }
                //}

                return;
            }

            this.TargetMethod_MethodInfo = TargetMethod;
            this.Prefix = Prefix;
            this.PostFix = PostFix;
            this.Transpiler = Transpiler;
            this.Finalizer = Finalizer;
            this.IlManipulator = ILmanipulator;
            this.ShowErrorOnConsole = showErrorOnConsole;
            this.HarmonyInstanceID = $"{PatchIdentifier}: {TargetPath_MethodInfo}, {PatchType}";
            this.isMethodInfoPatch = true;
            this.Instance = new Harmony(HarmonyInstanceID);
            this.DoPatch_info(this);
        }

        internal FusionPatch(MethodBase TargetMethod, HarmonyMethod Prefix = null, HarmonyMethod PostFix = null, HarmonyMethod Transpiler = null, HarmonyMethod Finalizer = null, HarmonyMethod ILmanipulator = null, bool showErrorOnConsole = true)
        {
            if (TargetMethod == null || (Prefix == null && PostFix == null && Transpiler == null && Finalizer == null && ILmanipulator == null))
            {
                StringBuilder FailureReason = new StringBuilder();
                if (Prefix == null)
                {
                    string reason = "Prefix Method is null";
                    if (FailureReason.Length != 0)
                    {
                        FailureReason.AppendLine(reason);
                    }
                    else
                    {
                        FailureReason.Append(reason);
                    }
                }

                if (PostFix == null)
                {
                    string reason = "PostFix Method is null";
                    if (FailureReason.Length != 0)
                    {
                        FailureReason.AppendLine(reason);
                    }
                    else
                    {
                        FailureReason.Append(reason);
                    }
                }

                if (Transpiler == null)
                {
                    string reason = "Transpiler Method is null";
                    if (FailureReason.Length != 0)
                    {
                        FailureReason.AppendLine(reason);
                    }
                    else
                    {
                        FailureReason.Append(reason);
                    }
                }

                if (Finalizer == null)
                {
                    string reason = "Finalizer Method is null";
                    if (FailureReason.Length != 0)
                    {
                        FailureReason.AppendLine(reason);
                    }
                    else
                    {
                        FailureReason.Append(reason);
                    }
                }

                if (ILmanipulator == null)
                {
                    string reason = "ILmanipulator Method is null";
                    if (FailureReason.Length != 0)
                    {
                        FailureReason.AppendLine(reason);
                    }
                    else
                    {
                        FailureReason.Append(reason);
                    }
                }

                if (TargetMethod != null)
                {
                    Logs.Log($"[{PatchIdentifier}] TargetMethod is NULL", ConsoleColor.Red);
                }
                //else
                //{
                //    if (FC.Utils.Config.Main.IsDeveloper)
                //    {
                //        Logs.Log($"[{PatchIdentifier}] Failed to Patch {TargetMethod.DeclaringType?.FullName}.{TargetMethod?.Name} because {FailureReason}.", ConsoleColor.Red);
                //    }
                //    else
                //    {
                //        Logs.Log($"[{PatchIdentifier}] Failed to Patch {TargetMethod.Name}", ConsoleColor.Red);
                //    }
                //}

                return;
            }

            this.TargetMethod_MethodBase = TargetMethod;
            this.Prefix = Prefix;
            this.PostFix = PostFix;
            this.Transpiler = Transpiler;
            this.Finalizer = Finalizer;
            this.IlManipulator = ILmanipulator;
            this.ShowErrorOnConsole = showErrorOnConsole;
            this.HarmonyInstanceID = $"{PatchIdentifier}: {TargetPath_MethodInfo}, {PatchType}";
            this.isMethodInfoPatch = false;
            this.Instance = new Harmony(HarmonyInstanceID);
            DoPatch_base(this);
        }

        private void DoPatch_info(FusionPatch patch)
        {
            try
            {
                patch.Instance.Patch(patch.TargetMethod_MethodInfo, patch.Prefix, patch.PostFix, patch.Transpiler, patch.Finalizer, patch.IlManipulator);
            }
            catch (Exception e)
            {
                HasThrownException = true;
                if (ShowErrorOnConsole)
                {
                    Logs.Log($"{e}", ConsoleColor.Red);
                }
            }
            finally
            {
                if (!HasThrownException)
                {
                    isActivePatch = true;
                    //if (FC.Utils.Config.Main.IsDeveloper)
                    //{
                    //    Logs.Log($"[{patch.PatchIdentifier}] Patched {patch.TargetPath_MethodInfo} | with {patch.PatchType}", ConsoleColor.Green);
                    //}
                    //else
                    //{
                        Logs.Log($"[{patch.PatchIdentifier}] Successfully patched {patch.TargetMethod_MethodInfo?.Name}", ConsoleColor.Green);
                    //}
                }
                else
                {
                    isActivePatch = false;
                    //if (FC.Utils.Config.Main.IsDeveloper)
                    //{
                    //    Logs.Log($"[{patch.PatchIdentifier}] Failed At {patch.TargetPath_MethodInfo} | with {patch.PatchType}", ConsoleColor.Red);
                    //}
                    //else
                    //{
                        Logs.Log($"[{patch.PatchIdentifier}] Failed At {patch.TargetMethod_MethodInfo?.Name}", ConsoleColor.Red);
                    //}
                }
            }
        }
        private void DoPatch_base(FusionPatch patch)
        {
            try
            {
                patch.Instance.Patch(patch.TargetMethod_MethodBase, patch.Prefix, patch.PostFix, patch.Transpiler, patch.Finalizer, patch.IlManipulator);
            }
            catch (Exception e)
            {
                HasThrownException = true;
                if (ShowErrorOnConsole)
                {
                    Logs.Log($"{e}", ConsoleColor.Red);
                }
            }
            finally
            {
                if (!HasThrownException)
                {
                    isActivePatch = true;
                    //if (FC.Utils.Config.Main.IsDeveloper)
                    //{
                    //    Logs.Log($"[{patch.PatchIdentifier}] Patched {patch.TargetPath_base} | with {patch.PatchType}", ConsoleColor.Green);
                    //}
                    //else
                    //{
                        Logs.Log($"[{patch.PatchIdentifier}] Successfully patched {patch.TargetMethod_MethodBase?.Name}", ConsoleColor.Green);
                    //}
                }
                else
                {
                    isActivePatch = false;
                    //if (FC.Utils.Config.Main.IsDeveloper)
                    //{
                    //    Logs.Log($"[{patch.PatchIdentifier}] Failed At {patch.TargetPath_base} | with {patch.PatchType}", ConsoleColor.Red);
                    //}
                    //else
                    //{
                        Logs.Log($"[{patch.PatchIdentifier}] Failed At {patch.TargetMethod_MethodBase?.Name}", ConsoleColor.Red);
                    //}

                }
            }
        }

        internal void Unpatch()
        {
            if (isActivePatch)
            {
                Instance.UnpatchSelf();
                if (!isMethodInfoPatch)
                {
                    //if (FC.Utils.Config.Main.IsDeveloper)
                    //{
                    //    Logs.Log($"[{this.PatchIdentifier}] Removed Patch from {this.TargetPath_base} , Unlinked Method : {this.PatchType}", ConsoleColor.Green);
                    //}
                    //else
                    //{
                        Logs.Log($"[{this.PatchIdentifier}] Removed Patch from {this.TargetMethod_MethodBase?.Name}", ConsoleColor.Green);
                    //}
                }
                else
                {
                    //if (FC.Utils.Config.Main.IsDeveloper)
                    //{
                    //    Logs.Log($"[{this.PatchIdentifier}] Removed Patch from {this.TargetPath_MethodInfo} , Unlinked Method : {this.PatchType}", ConsoleColor.Green);
                    //}
                    //else
                    //{
                        Logs.Log($"[{this.PatchIdentifier}] Removed Patch from {this.TargetMethod_MethodInfo?.Name}", ConsoleColor.Green);
                    //}

                }
                isActivePatch = false;
            }

        }

        internal void Patch()
        {
            if (!isActivePatch)
            {
                if (isMethodInfoPatch)
                {
                    DoPatch_info(this);
                }
                else
                {
                    DoPatch_base(this);
                }
            }
        }
    }
}