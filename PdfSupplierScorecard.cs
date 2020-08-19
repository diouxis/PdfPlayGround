﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PdfPlayGround
{
    using UI;
    using Model;
    using PdfPlayGround.Contract;
    using System.IO;
    using iTextSharp.text.html;

    public class PdfSupplierScorecard : PdfBase
    {
        protected readonly SupplierScoreBoardView Source;

        //private int BorderId => Source.BoardId;
        private string InsurerHeader => Source.InsurerHeader;
        private string InsurerLogo => Source.InsurerLogo;
        private string BorderTitle => Source.Title;
        //private DateTime DateFrom => Source.DateFrom;
        //private DateTime DateTo => Source.DateTo;

        Dictionary<string, string> iconUnicode = new Dictionary<string, string>();

        public PdfSupplierScorecard(SupplierScoreBoardView claimJob)
        {
            Source = claimJob;
            PageMargin = new Margin(5, 5, 20, 20);
            PageInfo = new Rectangle(PageSize.A4.Rotate());
        }

        protected override void InitialPdf()
        {
            PdfPageEvent = new ENDataClassicHeader(this);
        }

        protected override void WriteDocument()
        {
            base.WriteDocument();
            Doc.AddAuthor("ENData");
            Doc.AddCreator("ENData");
            Doc.AddKeywords("This is an report of Supplier Scorecard");
            Doc.AddSubject("Supplier Scorecard");
            Doc.AddTitle(System.DateTime.Now.ToString("dd/MM/yyyy").Replace("-", " ") + "Supplier Scorecard Report");
            //Pdf Start 

            //first table 
            PdfPTable startTable = new PdfPTable(7);
            startTable.TotalWidth = PageContentWidth;
            startTable.LockedWidth = true;
            startTable.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell startTableLeftCell = new PdfPCell();
            startTableLeftCell.Colspan = 1;
            startTableLeftCell.Border = 0;
            startTableLeftCell.MinimumHeight = 150f;
            startTableLeftCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            var logoImg = Image.GetInstance(new Uri(InsurerLogo));
            logoImg.ScalePercent(30f);
            logoImg.Alignment = Element.ALIGN_CENTER;
            startTableLeftCell.AddElement(logoImg);
            startTable.AddCell(startTableLeftCell);

            PdfPCell startTableRightCell = new PdfPCell();
            startTableRightCell.Colspan = 6;
            startTableRightCell.BackgroundColor = new BaseColor(0, 0, 51);
            startTableRightCell.Border = 0;
            startTableRightCell.MinimumHeight = 150f;

            PdfPTable startTableRightCellTable = new PdfPTable(1);
            startTableRightCellTable.DefaultCell.Border = Rectangle.NO_BORDER;
            startTableRightCellTable.AddCell(new Phrase(InsurerHeader, new Font(Font.BOLD, 15f, Font.BOLD, BaseColor.White)));
            startTableRightCellTable.AddCell(new Phrase(BorderTitle, new Font(Font.BOLD, 20f, Font.BOLD, BaseColor.White)));
            startTableRightCell.AddElement(startTableRightCellTable);
            startTable.AddCell(startTableRightCell);

            startTable.AddCell(startTableRightCellTable);

            Doc.Add(startTable);


            //datetime range 
            PdfPTable dateTimeTable = new PdfPTable(1);
            dateTimeTable.TotalWidth = PageContentWidth;
            dateTimeTable.LockedWidth = true;
            dateTimeTable.DefaultCell.Border = Rectangle.NO_BORDER;
            dateTimeTable.SpacingBefore = 5f;

            Phrase dateFromPhrase = new Phrase
            {
                new Chunk("Date Range: ", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)),
                new Chunk($"{Source.DateFrom:dd/MM/yyyy} - {Source.DateTo:dd/MM/yyyy}")
            };

            PdfPCell dateFromCell = new PdfPCell();
            dateFromCell.Border = 0;
            dateFromCell.AddElement(dateFromPhrase);

            dateTimeTable.AddCell(dateFromCell);
            Doc.Add(dateTimeTable);

            //second table 
            int columnNumOfscoreGroupTable = Source.ScoreGroups.Count();
            PdfPTable scoreGroupTable = new PdfPTable(columnNumOfscoreGroupTable);
            scoreGroupTable.TotalWidth = PageContentWidth;
            scoreGroupTable.LockedWidth = true;
            scoreGroupTable.DefaultCell.Border = Rectangle.NO_BORDER;
            scoreGroupTable.SpacingBefore = 10f;

            foreach (SupplierScoreGroupView item in Source.ScoreGroups)
            {
                PdfPCell scoreGroupCell = new PdfPCell();
                scoreGroupCell.Colspan = 1;
                scoreGroupCell.Border = 0;
                PdfPTable scoreGroupTb = generateBaseTable(item, columnNumOfscoreGroupTable);
                scoreGroupCell.AddElement(scoreGroupTb);
                scoreGroupTable.AddCell(scoreGroupCell);
            }
            scoreGroupTable.CompleteRow();
            Doc.Add(scoreGroupTable);

            //third table
            int compareTableColumnNum = Source.Tables.FirstOrDefault().Rows.FirstOrDefault().Fields.Count() + 1;
            PdfPTable compareTable = new PdfPTable(compareTableColumnNum);
            compareTable.TotalWidth = PageContentWidth;
            compareTable.LockedWidth = true;
            compareTable.SpacingBefore = 10f;
            PdfPCell fieldName = new PdfPCell(new Phrase(Source.Tables.FirstOrDefault().Name, new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
            fieldName.Colspan = 1;
            fieldName.PaddingLeft = 0;
            fieldName.PaddingRight = 0;
            fieldName.HorizontalAlignment = Element.ALIGN_CENTER;
            compareTable.AddCell(fieldName);
            foreach (SupplierScoreItemView item in Source.Tables.FirstOrDefault().Rows.FirstOrDefault().Fields)
            {
                PdfPCell fieldTitle = new PdfPCell(new Phrase(item.Name, new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black)));
                fieldTitle.Colspan = 1;
                fieldTitle.PaddingLeft = 0;
                fieldTitle.PaddingRight = 0;
                fieldTitle.HorizontalAlignment = Element.ALIGN_CENTER;
                compareTable.AddCell(fieldTitle);
            }

            foreach (SupplierScoreRow row in Source.Tables.FirstOrDefault().Rows)
            {
                PdfPCell rowName = new PdfPCell(new Phrase(row.Name, new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
                rowName.Colspan = 1;
                rowName.HorizontalAlignment = Element.ALIGN_CENTER;
                rowName.PaddingLeft = 0;
                rowName.PaddingRight = 0;
                compareTable.AddCell(rowName);
                foreach (SupplierScoreItemView field in row.Fields)
                {
                    if (field.Ranking == null)
                    {
                        PdfPCell fieldRank = new PdfPCell(new Phrase(" - ", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.White)));
                        fieldRank.Colspan = 1;
                        fieldRank.HorizontalAlignment = Element.ALIGN_CENTER;
                        fieldRank.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml(field.Color));
                        compareTable.AddCell(fieldRank);

                    }
                    else
                    {
                        PdfPCell fieldRank = new PdfPCell(new Phrase(field.Ranking.ToString(), new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.White)));
                        fieldRank.Colspan = 1;
                        fieldRank.HorizontalAlignment = Element.ALIGN_CENTER;
                        fieldRank.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml(field.Color));
                        compareTable.AddCell(fieldRank);
                    }
                }
            }
            compareTable.CompleteRow();
            Doc.Add(compareTable);

            Doc.NewPage();

            generateDescriptionTable();
        }

        public void generateDescriptionTable()
        {
            PdfPTable descriptionTable = new PdfPTable(7);
            descriptionTable.TotalWidth = PageContentWidth;
            descriptionTable.LockedWidth = true;
            descriptionTable.SplitLate = false;

            PdfPCell desTitle = new PdfPCell(new Phrase("Definitions", new Font(Font.BOLD, 14f, Font.BOLD, BaseColor.Black)));
            desTitle.Colspan = 7;
            desTitle.HorizontalAlignment = Element.ALIGN_CENTER;
            descriptionTable.AddCell(desTitle);

            foreach (SupplierScoreGroupView item in Source.ScoreGroups)
            {
                PdfPCell title = new PdfPCell(new Phrase(item.Name, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
                title.Colspan = 1;

                PdfPCell description = new PdfPCell();
                description.Colspan = 6;
                Phrase desPhrase = new Phrase();

                desPhrase.AddRange(ConvertHtml(item.GroupDescription));

                description.AddElement(desPhrase);

                descriptionTable.AddCell(title);
                descriptionTable.AddCell(description);
            }
            Doc.Add(descriptionTable);
        }

        public PdfPTable generateBaseTable(SupplierScoreGroupView ScoreGroup, int columnNum)
        {
            PdfPTable baseTable = new PdfPTable(2);
            baseTable.TotalWidth = PageContentWidth / columnNum - 4f;
            baseTable.LockedWidth = true;
            string iconPath = Path.GetFullPath("../../../Icon/");
            var fontAwesomeIcon = BaseFont.CreateFont(iconPath + "MaterialIcons-Regular.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font fontAwe = new Font(fontAwesomeIcon, 10f, Font.UNDEFINED, BaseColor.Black);

            Chunk iconPhrase = new Chunk(DecodeEncodedNonAsciiCharacters(ScoreGroup.Icon), fontAwe);

            Phrase groupTitle = new Phrase(ScoreGroup.Name + " ", new Font(Font.BOLD, 11f, Font.BOLD, BaseColor.Black));
            groupTitle.Add(iconPhrase);
            PdfPCell baseTableTitle = new PdfPCell(groupTitle);
            baseTableTitle.VerticalAlignment = Element.ALIGN_MIDDLE;
            baseTableTitle.Colspan = 2;
            baseTableTitle.HorizontalAlignment = Element.ALIGN_CENTER;
            baseTable.AddCell(baseTableTitle);


            int spanCol = 0;
            if (ScoreGroup.Orientation == Orientation.Horizontal)
            {
                spanCol = 1;
            }
            else if (ScoreGroup.Orientation == Orientation.Vertical)
            {
                spanCol = 2;
            }

            foreach (var item in ScoreGroup.ItemValues)
            {
                string itemValue = item.Value?.ToString(item.Unit);

                if (spanCol == 1)
                {
                    PdfPCell itemTitle = new PdfPCell(new Phrase(item.Name, new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black)));
                    itemTitle.Colspan = spanCol;
                    baseTable.AddCell(itemTitle);

                    string itemValueEmpty = " - ";
                    if (itemValue == null)
                    {
                        PdfPCell itemInfo = new PdfPCell(new Phrase(itemValueEmpty, new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.White)));
                        itemInfo.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml(item.Color));
                        itemInfo.HorizontalAlignment = Element.ALIGN_CENTER;
                        itemInfo.Colspan = spanCol;
                        baseTable.AddCell(itemInfo);
                    }
                    else
                    {
                        PdfPCell itemInfo = new PdfPCell(new Phrase(itemValue, new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.White)));
                        itemInfo.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml(item.Color));
                        itemInfo.Colspan = spanCol;
                        baseTable.AddCell(itemInfo);
                    }
                }
                else if (spanCol == 2)
                {
                    PdfPCell basecell = new PdfPCell();
                    basecell.Colspan = 1;
                    basecell.Padding = 0;
                    basecell.Border = 0;
                    PdfPTable baseCellTable = new PdfPTable(1);
                    baseCellTable.TotalWidth = (PageContentWidth / columnNum - 4f) / 2;
                    baseCellTable.LockedWidth = true;
                    baseCellTable.DefaultCell.Padding = 0;
                    PdfPCell baseCellTableCell = new PdfPCell(new Phrase(item.Name, new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
                    baseCellTableCell.Colspan = 1;
                    baseCellTable.AddCell(baseCellTableCell);
                    string itemValueEmpty = " - ";
                    if (itemValue == null)
                    {
                        PdfPCell baseCellTableCell2 = new PdfPCell(new Phrase(itemValueEmpty, new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.White)));
                        baseCellTableCell2.Colspan = 1;
                        baseCellTableCell2.HorizontalAlignment = Element.ALIGN_CENTER;
                        baseCellTableCell2.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml(item.Color));
                        baseCellTable.AddCell(baseCellTableCell2);
                        basecell.AddElement(baseCellTable);
                        baseTable.AddCell(basecell);

                    }
                    else
                    {
                        PdfPCell baseCellTableCell2 = new PdfPCell(new Phrase(itemValue, new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.White)));
                        baseCellTableCell2.Colspan = 1;
                        baseCellTableCell2.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml(item.Color));
                        baseCellTable.AddCell(baseCellTableCell2);
                        basecell.AddElement(baseCellTable);
                        baseTable.AddCell(basecell);
                    }
                }
            }
            baseTable.CompleteRow();
            return baseTable;
        }

        static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        static string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }

        protected class ENDataClassicHeader : PdfPageEventBase
        {
            PdfSupplierScorecard ThisDocument => (PdfSupplierScorecard)BaseDocument;

            public ENDataClassicHeader(PdfBase pdfbase) : base(pdfbase)
            {

            }

            public override void OnStartPage(PdfWriter writer, Document document)
            {

            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);

                PdfPTable footerTable = new PdfPTable(2);
                footerTable.TotalWidth = 820f;
                footerTable.LockedWidth = true;
                footerTable.DefaultCell.Border = Rectangle.NO_BORDER;
                PdfPCell printDate = new PdfPCell(new Phrase("Printed on " + System.DateTime.Now, StyleFooterAndPageNumber));
                printDate.HorizontalAlignment = Element.ALIGN_LEFT;
                printDate.Border = Rectangle.NO_BORDER;
                printDate.Colspan = 1;
                footerTable.AddCell(printDate);
                PdfPCell footerPageNum = new PdfPCell(new Phrase("Page " + writer.PageNumber.ToString(),
                    StyleFooterAndPageNumber
                    ));
                footerPageNum.HorizontalAlignment = Element.ALIGN_RIGHT;
                footerPageNum.Border = Rectangle.NO_BORDER;
                footerPageNum.Colspan = 1;
                footerTable.AddCell(footerPageNum);

                footerTable.WriteSelectedRows(0, -1, 10, 20f, writer.DirectContent);
            }

            static readonly Font StyleFooterAndPageNumber = FontFactory.GetFont(BaseFont.HELVETICA, 10f);
        }
    }



}
