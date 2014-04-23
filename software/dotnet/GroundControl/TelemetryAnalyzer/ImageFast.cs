using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TelemetryAnalyzer
{
    public class ImageFast
    {
        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int GdipLoadImageFromFile(string filename, out IntPtr image);

        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int GdiplusStartup(out IntPtr token, ref StartupInput input, out StartupOutput output);

        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int GdiplusShutdown(IntPtr token);

        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int GdipGetImageType(IntPtr image, out GdipImageTypeEnum type);

        private static IntPtr gdipToken = IntPtr.Zero;

        static ImageFast()
        {
#if DEBUG
            Console.WriteLine("Initializing GDI+");
#endif
            if (gdipToken == IntPtr.Zero)
            {
                StartupInput input = StartupInput.GetDefaultStartupInput();
                StartupOutput output;

                int status = GdiplusStartup(out gdipToken, ref input, out output);
#if DEBUG
                if (status == 0)
                    Console.WriteLine("Initializing GDI+ completed successfully");
#endif
                if (status == 0)
                    AppDomain.CurrentDomain.ProcessExit += new EventHandler(Cleanup_Gdiplus);
            }
        }

        private static void Cleanup_Gdiplus(object sender, EventArgs e)
        {
#if DEBUG
            Console.WriteLine("GDI+ shutdown entered through ProcessExit event");
#endif
            if (gdipToken != IntPtr.Zero)
                GdiplusShutdown(gdipToken);

#if DEBUG
            Console.WriteLine("GDI+ shutdown completed");
#endif
        }

        private static Type bmpType = typeof(System.Drawing.Bitmap);
        private static Type emfType = typeof(System.Drawing.Imaging.Metafile);

        public static Image FromFile(string filename)
        {
            filename = Path.GetFullPath(filename);
            IntPtr loadingImage = IntPtr.Zero;

            // We are not using ICM at all, fudge that, this should be FAAAAAST!
            if (GdipLoadImageFromFile(filename, out loadingImage) != 0)
            {
                throw new Exception("GDI+ threw a status error code.");
            }

            GdipImageTypeEnum imageType;
            if (GdipGetImageType(loadingImage, out imageType) != 0)
            {
                throw new Exception("GDI+ couldn't get the image type");
            }

            switch (imageType)
            {
                case GdipImageTypeEnum.Bitmap:
                    return (Bitmap)bmpType.InvokeMember("FromGDIplus", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { loadingImage });
                case GdipImageTypeEnum.Metafile:
                    return (Metafile)emfType.InvokeMember("FromGDIplus", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { loadingImage });
            }

            throw new Exception("Couldn't convert underlying GDI+ object to managed object");
        }

        private ImageFast() { }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct StartupInput
    {
        public int GdiplusVersion;
        public IntPtr DebugEventCallback;
        public bool SuppressBackgroundThread;
        public bool SuppressExternalCodecs;

        public static StartupInput GetDefaultStartupInput()
        {
            StartupInput result = new StartupInput();
            result.GdiplusVersion = 1;
            result.SuppressBackgroundThread = false;
            result.SuppressExternalCodecs = false;
            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct StartupOutput
    {
        public IntPtr Hook;
        public IntPtr Unhook;
    }

    internal enum GdipImageTypeEnum
    {
        Unknown = 0,
        Bitmap = 1,
        Metafile = 2
    }
}
