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
}
