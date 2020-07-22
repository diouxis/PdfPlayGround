using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PdfPlayGround.Model
{
    public class ScoreGroups
    {
        public List<Card> Cards { get; set; }
    }

    public class ClaimScoreTable
    {
        public List<Card> Cards { get; set; }
    }
}
