using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CommandLine;

namespace cur2png
{
    /// <summary>
    /// The Program class
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The Options class (used for CLI)
        /// </summary>
        public class Options
        {
            /// <summary>
            /// Gets or sets the target folder.
            /// </summary>
            /// <value>
            /// The target folder.
            /// </value>
            [Option('f', "folder", Required = true, HelpText = "Set target folder for .")]
            public string TargetFolder { get; set; }
        }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    if (string.IsNullOrEmpty(o.TargetFolder))
                    {
                        throw new Exception("Invalid target folder specified.\nPress any key to exit...");
                    }
                    else
                    {
                        Execute(o.TargetFolder);
                    }
                });

            Console.Read();
        }

        /// <summary>
        /// Executes the app to process the specified folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        private static void Execute(string folder)
        {
            // string folder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "curs");

            // Create folder next to the target folder
            string outFolder = Path.Combine(Path.GetDirectoryName(folder), "png");

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

                // Example of folder: "C:\\Windows\\Cursors\\Windows Aero\\aero_busy.ani"

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

            Console.WriteLine(
                $"Succesfully created {files.Count() - errorCount} files! (Errors: {errorCount}) Press any key to exit...");
        }

        /// <summary>
        /// The ICONINFO folder
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct ICONINFO
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        /// <summary>
        /// Gets the icon information.
        /// </summary>
        /// <param name="hIcon">The h icon.</param>
        /// <param name="pIconInfo">The p icon information.</param>
        /// <returns></returns>
        [DllImport("user32")]
        private static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO pIconInfo);

        /// <summary>
        /// Loads the cursor from file.
        /// </summary>
        /// <param name="lpFileName">Name of the lp file.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursorFromFile(string lpFileName);

        /// <summary>
        /// Deletes the object.
        /// </summary>
        /// <param name="hObject">The h object.</param>
        /// <returns></returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Creates a bitmap from cursor.
        /// </summary>
        /// <param name="cur">The current.</param>
        /// <returns></returns>
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