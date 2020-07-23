using System;
using System.Diagnostics;
using System.Linq;

namespace PdfPlayGround
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = null;
            using (var request = new Model.DataResolver())
            {
                var claimJob = request.GetFormData("Q2xhaW1Kb2I6MjE1Mzk4").Result;

                var a = new PdfTest(claimJob);
                file = a.GeneratePdf();
            }
            //var score = new Model.ClaimScoreBoard(1);
            //var a = new PdfSupplierScorecard(score);
            //file = a.GeneratePdf();

            string pathToAcroRd32 = 
                Environment.GetEnvironmentVariable("ProgramFiles") 
                + (Environment.Is64BitOperatingSystem ? @" (x86)\" : @"\") 
                + @"Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
            ProcessStartInfo adobeInfo = new ProcessStartInfo(pathToAcroRd32, file);
            Process.Start(adobeInfo);
        }
    }
}
