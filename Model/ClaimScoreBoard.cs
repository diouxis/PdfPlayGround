using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfPlayGround.Model
{
    using Attributes;
    using Helper;
    using Contract;
    using PdfPlayGround.UI;

    public class ClaimScoreBoard
    {
        public int BoardId { get; set; } = 1;
        public string Title { get; set; }
        public string InsurerHeader { get; set; }
        public string InsurerLogo { get; set; }
        public List<ClaimScoreGroup> ScoreGroups { get; set; }
        public List<ClaimScoreTable> Tables { get; set; }

        public ClaimScoreBoard(int id)
        {
            BoardId = id;
            InsurerLogo = "Allianz.jpg";
            InsurerHeader = "Allianz Claim Operations";
            Title = "Builder Performance Summary";
            ScoreGroups = new List<ClaimScoreGroup>
            {
                new ClaimScoreGroup
                {
                    Name = "Cost",
                    Icon = "account_balance",
                    Orientation = Orientation.Vertical,
                    Items = new List<ClaimScoreItem>
                    {
                        new ClaimScoreItem
                        {
                            Name = "$0-$10k",
                            Color = "Blue",
                            Value = 4168.32,
                            Unit = DataUnit.Currency
                        },
                        new ClaimScoreItem
                        {
                            Name = "$10k-$20k",
                            Color = "Blue",
                            Value = 13994.14,
                            Unit = DataUnit.Currency
                        }
                    }
                },
                new ClaimScoreGroup
                {
                    Name = "Timeliness",
                    Icon = "event_note",
                    Orientation = Orientation.Horizontal,
                    Items = new List<ClaimScoreItem>
                    {
                        new ClaimScoreItem
                        {
                            Name = "Scoping",
                            Color = "Grey",
                            Value = 7.51,
                            Unit = DataUnit.Number
                        },
                        new ClaimScoreItem
                        {
                            Name = "Quoting",
                            Color = "Grey",
                            Value = 1.988,
                            Unit = DataUnit.Number
                        },
                        new ClaimScoreItem
                        {
                            Name = "Repair",
                            Color = "Grey",
                            Value = 23.62,
                            Unit = DataUnit.Number
                        }
                    }
                },
                new ClaimScoreGroup
                {
                    Name = "Quality",
                    Icon = "brightness_low",
                    Orientation = Orientation.Vertical,
                    Items = new List<ClaimScoreItem>
                    {
                        new ClaimScoreItem
                        {
                            Name = "Star Rating",
                            Color = "LightBlue",
                            Value = 4.16,
                            Unit = DataUnit.Number
                        },
                        new ClaimScoreItem
                        {
                            Name = "Variations",
                            Color = "LightBlue",
                            Value = 0.13,
                            Unit = DataUnit.Percentage
                        }
                    }
                },
                new ClaimScoreGroup
                {
                    Name = "Overall",
                    Icon = "deck",
                    Orientation = Orientation.Horizontal,
                    Items = new List<ClaimScoreItem>
                    {
                        new ClaimScoreItem
                        {
                            Name = "Rating",
                            Color = "DarkBlue",
                            Unit = DataUnit.Number
                        }
                    }
                }
            };
            Tables = new List<ClaimScoreTable>
            {
                new ClaimScoreTable("Comparison", ScoreGroups.SelectMany(x => x.Items))
            };
        }
    }

    public class ClaimScoreGroup
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public Orientation Orientation { get; set; }
        public List<ClaimScoreItem> Items { get; set; } = new List<ClaimScoreItem>();
    }

    public class ClaimScoreTable
    {
        public string Name { get; set; }
        public List<ClaimScoreRow> Rows { get; set; }

        public ClaimScoreTable(string name, IEnumerable<ClaimScoreItem> items)
        {
            Name = name;
            Rows = new List<ClaimScoreRow>
            {
                new ClaimScoreRow(1, "AJG", items),
                new ClaimScoreRow(2, "BBS", items),
                new ClaimScoreRow(3, "TBS", items),
                new ClaimScoreRow(4, "AWP", items)
            };
        }
    }

    public class ClaimScoreRow
    {
        public int RowId { get; set; }
        public string Name { get; set; }
        public List<ClaimScoreItem> Fields { get; set; } = new List<ClaimScoreItem>();

        public ClaimScoreRow(int id, string name, IEnumerable<ClaimScoreItem> fields)
        {
            RowId = id;
            Name = name;
            Fields.AddRange(fields.Select(x => new ClaimScoreItem
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
