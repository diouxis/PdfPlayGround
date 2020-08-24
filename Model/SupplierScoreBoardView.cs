using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfPlayGround.Model
{
    //using Attributes;
    //using Helper;
    using Contract;
    using iTextSharp.text.html;
    using PdfPlayGround.UI;

    public class SupplierScoreBoardView
    {
        public int BoardId { get; set; } = 1;
        public string Title { get; set; }
        public string InsurerHeader { get; set; }
        public string InsurerLogo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<SupplierScoreGroupView> ScoreGroups { get; set; }
        public List<SupplierScoreTable> Tables { get; set; }

        public SupplierScoreBoardView(int id)
        {
            BoardId = id;
            //InsurerLogo = "https://company-resources-edt.s3-ap-southeast-2.amazonaws.com/allianz/company_logo.png";
            InsurerLogo = "https://www.endataclaims.com/NewWebsite/Images/ENDataTransparent.png";
            InsurerHeader = "Allianz Claim Operations";
            Title = "Builder Performance Summary";
            DateFrom = new DateTime(2020, 01, 01);
            DateTo = new DateTime(2020, 07, 30);
            ScoreGroups = new List<SupplierScoreGroupView>
            {
                new SupplierScoreGroupView
                {
                    GroupDescription = @"<p style='text-align: center'><strong>Definition&nbsp;</strong></p>
<p>Sum of Total Cost divided by count of Authorised + Variations within specified date range<br />- A claim might be included in the AVG or TOTAL only with the variation value approved (i.e. without the scope approved value)<br />- i.e. &ldquo;Authorised&rdquo; * &ldquo;Avg Total Cost&rdquo; will not always be equal to &ldquo;Total Cost&rdquo;</p>
<p><em>Note: Claims with a status of 'Claim Lodged in Error' are excluded from this measure</em></p>
<p>Sum of Total Cost divided by count of Authorised + Variations within specified date range<br />- A claim might be included in the AVG or TOTAL only with the variation value approved (i.e. without the scope approved value)<br />- i.e. &ldquo;Authorised&rdquo; * &ldquo;Avg Total Cost&rdquo; will not always be equal to &ldquo;Total Cost&rdquo;</p>
<p><em>Note: Claims with a status of 'Claim Lodged in Error' are excluded from this measure</em></p>
<p>Sum of Total Cost divided by count of Authorised + Variations within specified date range<br />- A claim might be included in the AVG or TOTAL only with the variation value approved (i.e. without the scope approved value)<br />- i.e. &ldquo;Authorised&rdquo; * &ldquo;Avg Total Cost&rdquo; will not always be equal to &ldquo;Total Cost&rdquo;</p>
<p><em>Note: Claims with a status of 'Claim Lodged in Error' are excluded from this measure</em></p>",
                    Name = "Cost",
                    Icon = "\\ue84f",
                    Orientation = Orientation.Vertical,
                    ItemValues = new List<SupplierScoreItemView>
                    {
                        new SupplierScoreItemView
                        {
                            Name = "$0-$10k",
                            Color = "#2b60e3",
                            Value = 4168.32,
                            Unit = DataUnit.Currency
                        },
                        new SupplierScoreItemView
                        {
                            Name = "$10k-$20k",
                            Color = "#2b60e3",
                            Value = 13994.14,
                            Unit = DataUnit.Currency
                        },
                        new SupplierScoreItemView
                        {
                            Name = "$20k-$25k",
                            Color = "#2b60e3",
                            Value = 23456.45,
                            Unit = DataUnit.Currency
                        }
                    }
                },
                new SupplierScoreGroupView
                {
                    GroupDescription = @"<table style= border: 1px solid black><tbody><tr><td width=‘301’><p><strong>Metric</strong></p></td><td width=‘301’><p><strong>Weighting</strong></p></td></tr><tr><td width=‘301’><p><strong>Cost  0-$10K &ndash; EOL</strong></p></td><td width=‘301’><p>8%</p></td></tr><tr><td idth=‘301’><p><strong>Cost - $0-$10K - Natural Hazard</strong></p></td><td width=‘301’><p>4%</p></td></tr><tr><td width=‘301’><p><strong>Cost - $0-$10K - Working oss</strong></p></td><td width=‘301’><p>8%</p></td></tr><tr><td width=‘301’><p><strong>Cost - $10-$20K &ndash; EOL</strong></p></td><td width=‘301’><p>8%</p></td></tr><tr><td width=‘301’><p><strong>Cost - $10-$20K - Natural azard</strong></p></td><td width=‘301’><p>4%</p></td></tr><tr><td width=‘301’><p><strong>Cost - $10-$20K - Working Loss</strong></p></td><td width=‘301’><p>8%</p></td></tr><tr><td width=‘301’><p><strong>Timeliness &ndash; coping</strong></p></td><td width=‘301’><p>10%</p></td></tr><tr><td width=‘301’><p><strong>Timeliness &ndash; Quoting</strong></p></td><td width=‘301’><p>10%</p></td></tr><tr><td width=‘301’><p><strong>Timeliness &ndash; epair</strong></p></td><td width=‘301’><p>10%</p></td></tr><tr><td width=‘301’><p><strong>Quality &ndash; Customer Rating</strong></p></td><td width=‘301’><p>20%</p></td></tr><tr><td width=‘301’><p><strong>Quality &ndash; ariations</strong></p></td><td width=‘301’><p>10%</p></td></tr><tr><td width=‘301’><p><strong>Total</strong></p></td><td width=‘301’><p>100%</p></td></tr></tbody></table>",
                    Name = "Timeliness",
                    Icon = "\\ue616",
                    Orientation = Orientation.Horizontal,
                    ItemValues = new List<SupplierScoreItemView>
                    {
                        new SupplierScoreItemView
                        {
                            Name = "Scoping",
                            Color = "#7e828c",
                            Value = 7.51,
                            Unit = DataUnit.Number
                        },
                        new SupplierScoreItemView
                        {
                            Name = "Quoting",
                            Color = "#7e828c",
                            Value = 1.988,
                            Unit = DataUnit.Number
                        },
                        new SupplierScoreItemView
                        {
                            Name = "Repair",
                            Color = "#7e828c",
                            Value = 23.62,
                            Unit = DataUnit.Number
                        }
                    }
                },
                new SupplierScoreGroupView
                {
                    GroupDescription = @"<p><strong>Definition&nbsp;</strong></p>
<p>Sum of Total Cost divided by count of Authorised + Variations within specified date range<br />- A claim might be included in the AVG or TOTAL only with the variation value approved (i.e. without the scope approved value)<br />- i.e. &ldquo;Authorised&rdquo; * &ldquo;Avg Total Cost&rdquo; will not always be equal to &ldquo;Total Cost&rdquo;</p>
<p><em>Note: Claims with a status of 'Claim Lodged in Error' are excluded from this measure</em></p>
<p>Sum of Total Cost divided by count of Authorised + Variations within specified date range<br />- A claim might be included in the AVG or TOTAL only with the variation value approved (i.e. without the scope approved value)<br />- i.e. &ldquo;Authorised&rdquo; * &ldquo;Avg Total Cost&rdquo; will not always be equal to &ldquo;Total Cost&rdquo;</p>
<p><em>Note: Claims with a status of 'Claim Lodged in Error' are excluded from this measure</em></p>
<p>Sum of Total Cost divided by count of Authorised + Variations within specified date range<br />- A claim might be included in the AVG or TOTAL only with the variation value approved (i.e. without the scope approved value)<br />- i.e. &ldquo;Authorised&rdquo; * &ldquo;Avg Total Cost&rdquo; will not always be equal to &ldquo;Total Cost&rdquo;</p>
<p><em>Note: Claims with a status of 'Claim Lodged in Error' are excluded from this measure</em></p>",
                    Name = "Quality",
                    Icon = "\\ue1ad",
                    Orientation = Orientation.Vertical,
                    ItemValues = new List<SupplierScoreItemView>
                    {
                        new SupplierScoreItemView
                        {
                            Name = "Star Rating",
                            Color = "#547ae3",
                            Value = null,
                            Unit = DataUnit.Number
                        },
                        new SupplierScoreItemView
                        {
                            Name = "Variations",
                            Color = "#547ae3",
                            Value = 0.13,
                            Unit = DataUnit.Percentage
                        }
                    }
                },
                new SupplierScoreGroupView
                {
                    GroupDescription = @"<p><strong>Definition&nbsp;</strong></p>
<p>Sum of Total Cost divided by count of Authorised + Variations within specified date range<br />- A claim might be included in the AVG or TOTAL only with the variation value approved (i.e. without the scope approved value)<br />- i.e. &ldquo;Authorised&rdquo; * &ldquo;Avg Total Cost&rdquo; will not always be equal to &ldquo;Total Cost&rdquo;</p>
<p><em>Note: Claims with a status of 'Claim Lodged in Error' are excluded from this measure</em></p>
<p>Sum of Total Cost divided by count of Authorised + Variations within specified date range<br />- A claim might be included in the AVG or TOTAL only with the variation value approved (i.e. without the scope approved value)<br />- i.e. &ldquo;Authorised&rdquo; * &ldquo;Avg Total Cost&rdquo; will not always be equal to &ldquo;Total Cost&rdquo;</p>
<p><em>Note: Claims with a status of 'Claim Lodged in Error' are excluded from this measure</em></p>",
                    Name = "Overall",
                    Icon = "\\ue30c",
                    Orientation = Orientation.Horizontal,
                    ItemValues = new List<SupplierScoreItemView>
                    {
                        new SupplierScoreItemView
                        {
                            Name = "Rating",
                            Color = "#042787",
                            Unit = DataUnit.Number
                        }
                    }
                },
                new SupplierScoreGroupView
                {
                    GroupDescription = @"<table><tr><th>Metric</th><th>Weighting</th></tr><tr><td>Cost&ndash;$0&ndash;$10K&ndash;EOL</td><td>8%</td></tr><tr><td>Cost&ndash;$0&ndash;$10K&ndash;Natural Hazard</td><td>4%</td></tr><tr><td>Cost&ndash;$0-$10K&ndash;Working Loss</td><td>8%</td></tr><tr><td>Cost&ndash;$10$&ndash;20K&ndash;EOL</td><td>8%</td></tr><tr><td>Cost&ndash;$10-$20K&ndash;Natural Hazard</td><td>4%</td></tr><tr><td>Cost&ndash;$10-$20K&ndash;Working Loss</td><td>8%</td></tr><tr><td>Timeliness&ndash;Scoping</td><td>10%</td></tr><tr><td>Timeliness&ndash;Quoting</td><td>10%</td></tr><tr><td>Timeliness&ndash;Repair</td><td>10%</td></tr><tr><td>Quality&ndash;Customer Rating</td><td>20%</td></tr><tr><td>Quality&ndash;Variations</td><td>10%</td></tr><tr><td>Total</td><td>100%</td></tr></table>",
                    Name = "Cost 1",
                    Icon = "\\ue31c",
                    Orientation = Orientation.Vertical,
                    ItemValues = new List<SupplierScoreItemView>
                    {
                        new SupplierScoreItemView
                        {
                            Name = "Star Rating",
                            Color = "#547ae3",
                            Value = 4.16,
                            Unit = DataUnit.Number
                        },
                        new SupplierScoreItemView
                        {
                            Name = "Variations",
                            Color = "#547ae3",
                            Value = 0.13,
                            Unit = DataUnit.Percentage
                        },
                        new SupplierScoreItemView
                        {
                            Name = "Variations",
                            Color = "#547ae3",
                            Value = 0.13,
                            Unit = DataUnit.Percentage
                        }
                    }
                },
                //new SupplierScoreGroupView
                //{
                //    Name = "Cost2",
                //    Icon = "\\ue30c",
                //    Orientation = Orientation.Vertical,
                //    ItemValues = new List<SupplierScoreItemView>
                //    {
                //        new SupplierScoreItemView
                //        {
                //            Name = "Star Rating",
                //            Color = "#547ae3",
                //            Value = 4.16,
                //            Unit = DataUnit.Number
                //        },
                //        new SupplierScoreItemView
                //        {
                //            Name = "Variations",
                //            Color = "#547ae3",
                //            Value = 0.13,
                //            Unit = DataUnit.Percentage
                //        }
                //    }
                //},
                //new SupplierScoreGroupView
                //{
                //    Name = "Cost 3",
                //    Icon = "brightness_low",
                //    Orientation = Orientation.Vertical,
                //    ItemValues = new List<SupplierScoreItemView>
                //    {
                //        new SupplierScoreItemView
                //        {
                //            Name = "Star Rating",
                //            Color = "#547ae3",
                //            Value = 4.16,
                //            Unit = DataUnit.Number
                //        },
                //        new SupplierScoreItemView
                //        {
                //            Name = "Variations",
                //            Color = "#547ae3",
                //            Value = 0.13,
                //            Unit = DataUnit.Percentage
                //        }
                //    }
                //},
            };
            Tables = new List<SupplierScoreTable>
            {
                new SupplierScoreTable("Comparison", ScoreGroups.SelectMany(x => x.ItemValues))
            };
        }
    }

    public class SupplierScoreGroupView
    {
        public string GroupDescription { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public Orientation Orientation { get; set; }
        public List<SupplierScoreItemView> ItemValues { get; set; } = new List<SupplierScoreItemView>();
    }

    public class SupplierScoreTable
    {
        public string Name { get; set; }
        public List<SupplierScoreRow> Rows { get; set; }

        public SupplierScoreTable(string name, IEnumerable<SupplierScoreItemView> items)
        {
            Name = name;
            Rows = new List<SupplierScoreRow>
            {
                new SupplierScoreRow(1, "AJG", items),
                new SupplierScoreRow(2, "BBS", items),
                new SupplierScoreRow(3, "TBS", items),
                new SupplierScoreRow(4, "AWP", items)
            };
        }
    }

    public class SupplierScoreRow
    {
        public int RowId { get; set; }
        public string Name { get; set; }
        public List<SupplierScoreItemView> Fields { get; set; } = new List<SupplierScoreItemView>();

        public SupplierScoreRow(int id, string name, IEnumerable<SupplierScoreItemView> fields)
        {
            RowId = id;
            Name = name;
            Fields.AddRange(fields.Select(x => new SupplierScoreItemView
            {
                Id = RowId,
                Name = x.Name,
                Color = x.Color,
                Unit = x.Unit,
                Value = x.Value,
                Ranking = 1
            }));
        }
    }

}
