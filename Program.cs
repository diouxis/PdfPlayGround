using System;
using System.Diagnostics;

namespace PdfPlayGround
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new PdfTest();
            var file = a.GeneratePdf();

            string pathToAcroRd32 = 
                Environment.GetEnvironmentVariable("ProgramFiles") 
                + (Environment.Is64BitOperatingSystem ? @" (x86)\" : @"\") 
                + @"Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
            ProcessStartInfo adobeInfo = new ProcessStartInfo(pathToAcroRd32, file);
            Process.Start(adobeInfo);
        }
    }
}
