using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfPlayGround.Model
{
    //using Attributes;
    //using Helper;
    using Contract;
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
            InsurerLogo = "https://company-resources-edt.s3-ap-southeast-2.amazonaws.com/allianz/company_logo.png";
            InsurerHeader = "Allianz Claim Operations";
            Title = "Builder Performance Summary";
            DateFrom = new DateTime(2020, 01, 01);
            DateTo = new DateTime(2020, 07, 30);
            ScoreGroups = new List<SupplierScoreGroupView>
            {
                new SupplierScoreGroupView
                {
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
