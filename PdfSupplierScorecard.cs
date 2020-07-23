using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PdfPlayGround
{
    using Model;
    using PdfPlayGround.Contract;

    public class PdfSupplierScorecard: PdfBase
    {
        protected readonly ClaimScoreBoard Source;

        private int BorderId => Source.BoardId;
        private string InsurerHeader => Source.InsurerHeader;
        private string InsurerLogo => Source.InsurerLogo;
        private string BorderTitle => Source.Title;

        public PdfSupplierScorecard(ClaimScoreBoard claimJob)
        {
            Source = claimJob;
            PageMargin = new Margin(10, 10, 90, 20);
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
            startTable.TotalWidth = 820f;
            startTable.LockedWidth = true;

            PdfPCell startTableLeftCell = new PdfPCell();
            startTableLeftCell.Colspan = 1;
            var logoImg = Image.GetInstance(new Uri(InsurerLogo));
            logoImg.ScalePercent(30f);
            logoImg.Alignment = Element.ALIGN_CENTER;

            startTableLeftCell.AddElement(logoImg);

            PdfPCell startTableRightCell = new PdfPCell();
            startTableRightCell.Colspan = 6;
            startTableRightCell.BackgroundColor = new BaseColor(0, 0, 51);

            PdfPTable startTableRightCellTable = new PdfPTable(1);
            startTableRightCellTable.DefaultCell.Border = Rectangle.NO_BORDER;
            startTableRightCellTable.AddCell(new PdfPCell(new Phrase(BorderTitle, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White))));
            startTableRightCellTable.AddCell(new PdfPCell(new Phrase(InsurerHeader, new Font(Font.BOLD, 20f, Font.BOLD, BaseColor.White))));

            startTableRightCell.AddElement(startTableRightCellTable);

            //second table 
            PdfPTable scoreGroupTable = new PdfPTable(4);
            scoreGroupTable.TotalWidth = 820f;
            scoreGroupTable.LockedWidth = true;
            scoreGroupTable.DefaultCell.Border = Rectangle.NO_BORDER;
            foreach (ClaimScoreGroup item in Source.ScoreGroups) 
            {
                PdfPCell scoreGroupCell = new PdfPCell();
                scoreGroupCell.Colspan = 1;
                scoreGroupCell.Border = 0;
                PdfPTable scoreGroupTb = generateBaseTable(item);
                scoreGroupCell.AddElement(scoreGroupTb);
                scoreGroupTable.AddCell(scoreGroupCell);
            }

            //third table
            PdfPTable compareTable = new PdfPTable(10);
            compareTable.TotalWidth = 820f;
            compareTable.LockedWidth = true;
            PdfPCell fieldName = new PdfPCell(new Phrase(Source.Tables.FirstOrDefault().Name, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            fieldName.Colspan = 2;
            compareTable.AddCell(fieldName);
            foreach (ClaimScoreItem item in Source.Tables.FirstOrDefault().Rows.FirstOrDefault().Fields)
            {
                PdfPCell fieldTitle = new PdfPCell(new Phrase(item.Name, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
                fieldTitle.Colspan = 1;
                compareTable.AddCell(fieldTitle);
            }

            foreach (ClaimScoreRow row in Source.Tables.FirstOrDefault().Rows)
            {
                PdfPCell rowName = new PdfPCell(new Phrase(row.Name, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
                rowName.Colspan = 2;
                compareTable.AddCell(rowName);
                foreach (ClaimScoreItem field in row.Fields)
                {
                    PdfPCell fieldRank = new PdfPCell(new Phrase(field.Ranking.ToString(), new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
                    fieldRank.Colspan = 1;
                    fieldRank.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml(field.Color));
                    compareTable.AddCell(fieldRank);
                }
            }
        }

        public PdfPTable generateBaseTable(ClaimScoreGroup ScoreGroup)
        {

            PdfPTable baseTable = new PdfPTable(2);
            baseTable.TotalWidth = 820f;
            baseTable.LockedWidth = true; 
            PdfPCell baseTableTitle = new PdfPCell(new Phrase(ScoreGroup.Name + ScoreGroup.Icon, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            baseTableTitle.Colspan = 2;
            baseTable.AddCell(baseTableTitle);

            int spanCol = 0;
            if (ScoreGroup.Orientation == UI.Orientation.Horizontal)
            {
                spanCol = 1;
            }
            else if(ScoreGroup.Orientation == UI.Orientation.Vertical)
            {
                spanCol = 2;
            }

            foreach (ClaimScoreItem item in ScoreGroup.Items)
            {
                PdfPCell itemTitle = new PdfPCell(new Phrase(item.Name, new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black)));
                itemTitle.Colspan = spanCol;
                baseTable.AddCell(itemTitle);
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
                PdfPCell itemInfo = new PdfPCell(new Phrase(itemValue, new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.White)));
                itemInfo.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml(item.Color));
                itemInfo.Colspan = spanCol;
                baseTable.AddCell(itemInfo);

            }
            return baseTable;
        }

    }


}
