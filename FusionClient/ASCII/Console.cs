using System.Reflection;

namespace FCConsole
{
    using System;
    using System.Drawing;
    using System.Linq;
    using NativeConsole = System.Console;
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    public static class Console
    {
        public static bool HasInitialized { get; private set; }

        public static Color Default_FrontColor { get; set; } = Color.White;

        internal static void Write(string v, ConsoleColor yellow)
        {
            throw new NotImplementedException();
        }

        public static Color Default_BackColor { get; set; } = Color.Empty;

        public static void WriteFiglet(FigletFont font, string msg)
        {
            Figlet figlet = new Figlet(font);
            Write(figlet.ToAscii(msg).ToString());
        }

        public static void WriteFigletWithGradient(FigletFont font, string msg, Color frontColor1, Color frontColor2)
        {
            Figlet figlet = new Figlet(font);
            WriteGradient(figlet.ToAscii(msg).ToString(), frontColor1, frontColor2);
        }

        public static void Write(string msg, Color? frontColor = null, Color? backColor = null)
        {
            PrivateWrite(msg, frontColor, backColor);
        }

        public static void WriteLine(string msg, Color? frontColor = null, Color? backColor = null)
        {
            PrivateWrite(msg + Environment.NewLine, frontColor, backColor);
        }

        public static void WriteGradient(string msg, Color frontColor1, Color frontColor2)
        {
            char[] chars = msg.ToCharArray();

            int charCount = chars.Count();

            for (int i = 0; i < charCount; i++)
            {
                PrivateWrite(chars[i].ToString(), GetGradientColor(frontColor1, frontColor2, i, charCount));
            }
        }

        public static void WriteGradientLine(string msg, Color frontColor1, Color frontColor2)
        {
            WriteGradient(msg + Environment.NewLine, frontColor1, frontColor2);
        }

        private static Color GetGradientColor(Color color1, Color color2, int index, int count)
        {
            int rMin = color1.R;
            int rMax = color2.R;

            int gMin = color1.G;
            int gMax = color2.G;

            int bMin = color1.B;
            int bMax = color2.B;

            int rAverage = rMin + ((rMax - rMin) * index / count);
            int gAverage = gMin + ((gMax - gMin) * index / count);
            int bAverage = bMin + ((bMax - bMin) * index / count);

            return Color.FromArgb(rAverage, gAverage, bAverage);
        }

        private static void PrivateWrite(string msg, Color? frontColor = null, Color? backColor = null)
        {
            FirstRunCheck();
            string backSequence = string.Empty;

            string frontSequence = frontColor.HasValue
                ? VirtualTerminalSequences.ForegroundRgb((Color)frontColor)
                : VirtualTerminalSequences.ForegroundRgb(Default_FrontColor);

            if (!backColor.HasValue && Default_BackColor.IsEmpty)
            {
                backSequence = VirtualTerminalSequences.RestoreBackground;
            }

            NativeConsole.Write(frontSequence);
            NativeConsole.Write(backSequence);

            NativeConsole.Write(msg);
        }

        private static void Initialize()
        {
            IntPtr handle = NativeMethods.GetStdHandle(-11);
            NativeMethods.GetConsoleMode(handle, out int mode);
            NativeMethods.SetConsoleMode(handle, mode | 0x4);
            Default_BackColor = NativeConsole.BackgroundColor.ToDrawingColor();
        }

        private static void FirstRunCheck()
        {
            
            if (!HasInitialized)
            {
                Initialize();
                HasInitialized = true;
            }
        }
    }
}