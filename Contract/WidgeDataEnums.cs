using System;
using System.Collections.Generic;
using System.Text;

namespace PdfPlayGround.Contract
{
    public enum DataSense : byte
    {
        Neutral = 0,
        Commendatory = 1,
        Derogatory = 2
    }

    public enum DataUnit : byte
    {
        Number = 1,
        Currency = 2,
        Percentage = 3
    }

    public static class DataPresentHelper
    {
        public static string ToString(this double val, DataUnit unit) => unit switch
        {
            DataUnit.Number => val.ToString("n2"),
            DataUnit.Currency => val.ToString("c2"),
            DataUnit.Percentage => val.ToString("p2"),
            _ => val.ToString()
        };
    }
}
