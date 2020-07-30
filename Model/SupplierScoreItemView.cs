using System;
using System.Collections.Generic;
using System.Text;

namespace PdfPlayGround.Model
{
    using Contract;

    public class SupplierScoreItemView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public DataUnit Unit { get; set; }
        public double? Value { get; set; }
        public int? Ranking { get; set; }
    }
}
