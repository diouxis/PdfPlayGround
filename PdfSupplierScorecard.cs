﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PdfPlayGround
{
    using UI;
    using Model;
    using PdfPlayGround.Contract;
    using System.IO;


    public class PdfSupplierScorecard : PdfBase
    {
        protected readonly SupplierScoreBoardView Source;

        //private int BorderId => Source.BoardId;
        private string InsurerHeader => Source.InsurerHeader;
        private string InsurerLogo => Source.InsurerLogo;
        private string BorderTitle => Source.Title;
        private DateTime DateFrom => Source.DateFrom;
        private DateTime DateTo => Source.DateTo;

        Dictionary<string, string> iconUnicode = new Dictionary<string, string>();

        public PdfSupplierScorecard(SupplierScoreBoardView claimJob)
        {
            Source = claimJob;
            PageMargin = new Margin(20, 20, 90, 20);
            PageInfo = new Rectangle(PageSize.A4.Rotate());
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
            startTableRightCellTable.AddCell(new Phrase(InsurerHeader, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
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

            Chunk dateFromChunk = new Chunk("Date From ", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black));
            Chunk dateFromChu = new Chunk(DateFrom.ToString("dd/MM/yyyy"));
            Chunk dateToChunk = new Chunk("Date To ", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black));
            Chunk dateToChu = new Chunk(DateTo.ToString("dd/MM/yyyy"));

            Phrase dateFromPhrase = new Phrase();
            dateFromPhrase.Add(dateFromChunk);
            dateFromPhrase.Add(dateFromChu);

            Phrase dateToPhrase = new Phrase();
            dateToPhrase.Add(dateToChunk);
            dateToPhrase.Add(dateToChu);

            PdfPCell dateFromCell = new PdfPCell();
            dateFromCell.Border = 0;
            dateFromCell.AddElement(dateFromPhrase);

            PdfPCell dateToCell = new PdfPCell();
            dateToCell.Border = 0;
            dateToCell.AddElement(dateToPhrase);

            dateTimeTable.AddCell(dateFromCell);
            dateTimeTable.AddCell(dateToCell);
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
            int compareTableColumnNum = Source.Tables.FirstOrDefault().Rows.FirstOrDefault().Fields.Count() + 2;
            PdfPTable compareTable = new PdfPTable(compareTableColumnNum);
            compareTable.TotalWidth = PageContentWidth;
            compareTable.LockedWidth = true;
            compareTable.SpacingBefore = 10f;
            PdfPCell fieldName = new PdfPCell(new Phrase(Source.Tables.FirstOrDefault().Name, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            fieldName.Colspan = 2;
            fieldName.HorizontalAlignment = Element.ALIGN_CENTER;
            compareTable.AddCell(fieldName);
            foreach (SupplierScoreItemView item in Source.Tables.FirstOrDefault().Rows.FirstOrDefault().Fields)
            {
                PdfPCell fieldTitle = new PdfPCell(new Phrase(item.Name, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
                fieldTitle.Colspan = 1;
                fieldTitle.HorizontalAlignment = Element.ALIGN_CENTER;
                compareTable.AddCell(fieldTitle);
            }

            foreach (SupplierScoreRow row in Source.Tables.FirstOrDefault().Rows)
            {
                PdfPCell rowName = new PdfPCell(new Phrase(row.Name, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
                rowName.Colspan = 2;
                rowName.HorizontalAlignment = Element.ALIGN_CENTER;
                compareTable.AddCell(rowName);
                foreach (SupplierScoreItemView field in row.Fields)
                {
                    PdfPCell fieldRank = new PdfPCell(new Phrase(field.Ranking.ToString(), new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
                    fieldRank.Colspan = 1;
                    fieldRank.HorizontalAlignment = Element.ALIGN_CENTER;
                    fieldRank.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml(field.Color));
                    compareTable.AddCell(fieldRank);
                }
            }
            compareTable.CompleteRow();
            Doc.Add(compareTable);

        }


        public PdfPTable generateBaseTable(SupplierScoreGroupView ScoreGroup, int columnNum)
        {
            PdfPTable baseTable = new PdfPTable(2);
            baseTable.TotalWidth = PageContentWidth / columnNum - 4f;
            baseTable.LockedWidth = true;
            string iconPath = Path.GetFullPath("../../../Icon/");
            var fontAwesomeIcon = BaseFont.CreateFont(iconPath + "MaterialIcons-Regular.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font fontAwe = new Font(fontAwesomeIcon, 12, Font.NORMAL, BaseColor.Black);

            Chunk iconPhrase = new Chunk(ScoreGroup.Icon, fontAwe);

            Phrase groupTitle = new Phrase(ScoreGroup.Name, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black));
            groupTitle.Add(iconPhrase);
            PdfPCell baseTableTitle = new PdfPCell(groupTitle);
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
                string itemValue = "";
                if (item.Unit == DataUnit.Number)
                {
                    itemValue = item.Value?.ToString();
                }
                else if (item.Unit == DataUnit.Percentage)
                {
                    itemValue = (item.Value * 100)?.ToString() + "%";
                }
                else if (item.Unit == DataUnit.Currency)
                {
                    itemValue = "$" + item.Value?.ToString();
                }

                if (spanCol == 1)
                {
                    PdfPCell itemTitle = new PdfPCell(new Phrase(item.Name, new Font(Font.BOLD, 11f, Font.BOLD, BaseColor.Black)));
                    itemTitle.Colspan = spanCol;
                    baseTable.AddCell(itemTitle);

                    PdfPCell itemInfo = new PdfPCell(new Phrase(itemValue, new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.White)));
                    itemInfo.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml(item.Color));
                    itemInfo.Colspan = spanCol;
                    baseTable.AddCell(itemInfo);
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
                    PdfPCell baseCellTableCell = new PdfPCell(new Phrase(item.Name, new Font(Font.BOLD, 11f, Font.BOLD, BaseColor.Black)));
                    baseCellTableCell.Colspan = 1;
                    baseCellTable.AddCell(baseCellTableCell);
                    PdfPCell baseCellTableCell2 = new PdfPCell(new Phrase(itemValue, new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.White)));
                    baseCellTableCell2.Colspan = 1;
                    baseCellTableCell2.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml(item.Color));
                    baseCellTable.AddCell(baseCellTableCell2);
                    basecell.AddElement(baseCellTable);
                    baseTable.AddCell(basecell);
                }
            }
            baseTable.CompleteRow();
            return baseTable;
        }
    }



}
