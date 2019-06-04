using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cur2png
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string folder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "curs");
            string outFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "png");

            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);

            var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".cur") || s.EndsWith(".any"));

            int errorCount = 0;

            foreach (var file in files)
            {
                string fileNoExtension = Path.GetFileNameWithoutExtension(file);
                string fileWithExtension = Path.GetFileName(file);

                // string filePath = Path.Combine(folder, "curs", "aero_arrow.cur");
                string relPath = file.Replace(folder, "");
                string fileName = fileNoExtension + ".png";
                string outputFolder = outFolder + relPath;

                // We need to remove the file
                outputFolder = Path.GetDirectoryName(outputFolder);

                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);

                //Using LoadCursorFromFile from user32.dll, get a handle to the icon
                IntPtr hCursor = LoadCursorFromFile(file);

                // "C:\\Windows\\Cursors\\Windows Aero\\aero_busy.ani"

                //Create a Cursor object from that handle
                Cursor cursor = new Cursor(hCursor);

                //Convert that cursor into a bitmap
                try
                {
                    using (Bitmap cursorBitmap = BitmapFromCursor(cursor))
                    {
                        cursorBitmap.Save(Path.Combine(outputFolder, fileName), ImageFormat.Png);

                        //Draw that cursor bitmap directly to the form canvas
                        // e.Graphics.DrawImage(cursorBitmap, 50, 50);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message} || File: {fileWithExtension}");
                    ++errorCount;
                }
            }

            Console.WriteLine($"Succesfully created {files.Count() - errorCount} files! (Errors: {errorCount}) Press any key to exit...");
            Console.Read();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ICONINFO
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32")]
        private static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO pIconInfo);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursorFromFile(string lpFileName);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        private static Bitmap BitmapFromCursor(Cursor cur)
        {
            ICONINFO ii;
            GetIconInfo(cur.Handle, out ii);

            Bitmap bmp = Bitmap.FromHbitmap(ii.hbmColor);
            DeleteObject(ii.hbmColor);
            DeleteObject(ii.hbmMask);

            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            Bitmap dstBitmap = new Bitmap(bmData.Width, bmData.Height, bmData.Stride, PixelFormat.Format32bppArgb, bmData.Scan0);
            bmp.UnlockBits(bmData);

            return new Bitmap(dstBitmap);
        }
    }
}