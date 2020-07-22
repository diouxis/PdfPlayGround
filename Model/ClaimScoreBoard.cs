using System;
using System.Collections.Generic;
using System.Text;

namespace PdfPlayGround.Model
{
    public class ClaimScoreBoardForm
    {
        public string Id { get; set; }
        public string InsurerHeader { get; set; }
        public string InsurerLogo { get; set; }
        public string Title { get; set; }

        public ScoreGroups ScoreGroups { get; set; }

        public ClaimScoreTable ClaimScoreTable { get; set; }
    }
}
