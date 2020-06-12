using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PdfPlayGround
{
    public class PdfTest : PdfBase
    {
        protected List<InfoTableMetaData> ClaimContent = new List<InfoTableMetaData>();

        public PdfTest()
        {
            PageMargin = new Margin(10, 10, 50, 35);
        }

        protected override void InitialPdf()
        {
            ClaimContent = new List<InfoTableMetaData>
            {
                new InfoTableMetaData("Insurance Ref #:", "Test Claim" ),
                new InfoTableMetaData("Event Type:", "Storm" )
            };

            PdfPageEvent = new ENDataClassicHeader(this);
        }

        protected override void WriteDocument()
        {
            base.WriteDocument();
            Doc.Add(new Paragraph("Site Inspection - Claim Information", StyleHeader) { SpacingAfter = 15f });
            Doc.Add(GenerateInfoTable(ClaimContent));
        }

        protected PdfPTable GenerateInfoTable(List<InfoTableMetaData> model, byte columNum = 4, float[] widths = null)
        {
            PdfPCell cell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER
            };
            return GenerateInfoTable(model, columNum, widths, cell, cell);
        }

        protected PdfPTable GenerateListTable(List<TableHeaderMetaData> model)
        {
            PdfPCell headerCell = new PdfPCell()
            {
                MinimumHeight = 20,
                Border = Rectangle.BOTTOM_BORDER,
                BorderWidth = 1f
            };
            PdfPCell defaultCell = new PdfPCell()
            {
                MinimumHeight = 25,
                Border = Rectangle.NO_BORDER
            };
            PdfPTable listTable = GenerateListTable(model, defaultCell, headerCell, StyleContentSmallBold);
            listTable.SpacingAfter = 0;
            return listTable;
        }

        protected class ENDataClassicHeader : PdfPageEventBase
        {
            PdfTest ThisDocument => (PdfTest)BaseDocument;
            protected Phrase PDFFooter_Date;
            protected Phrase PDFHeader_Title;

            public ENDataClassicHeader(PdfBase pdfbase) : base(pdfbase)
            {
                PDFFooter_Date = new Phrase($"Printed On: {BaseDocument.Date}", StyleFooterAndPageNumber);
                PDFHeader_Title = new Phrase(BaseDocument.Title, ThisDocument.StyleTiltleHeader);
            }

            public override void OnStartPage(PdfWriter writer, Document document)
            {
                base.OnStartPage(writer, document);

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT,
                    PDFHeader_Title,
                    ThisDocument.PageMargin.Left,
                    ThisDocument.PageInfo.Height - 36f, 0);

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_RIGHT,
                    new Phrase("Page " + writer.PageNumber.ToString(), StyleFooterAndPageNumber),
                    ThisDocument.PageInfo.Width - ThisDocument.PageMargin.Right,
                    ThisDocument.PageInfo.Height - 20f, 0);
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT,
                    PDFFooter_Date,
                    ThisDocument.PageMargin.Left,
                    10f, 0);
            }

            static readonly Font StyleFooterAndPageNumber = FontFactory.GetFont(BaseFont.HELVETICA, 10);
        }

        //Define the style
        protected override BaseColor ThemeTableHeadShading => BaseColor.White;

        protected override Font StyleTiltleHeader => FontFactory.GetFont(BaseFont.HELVETICA, 23, Font.BOLD);
        protected override Font StyleHeader => FontFactory.GetFont(BaseFont.HELVETICA, 18, Font.BOLD);
        protected override Font StyleContent => FontFactory.GetFont(BaseFont.HELVETICA, 12);
        protected override Font StyleContentBold => FontFactory.GetFont(BaseFont.HELVETICA, 12, Font.BOLD);
        protected override Font StyleContentSmall => FontFactory.GetFont(BaseFont.HELVETICA, 9);
        protected override Font StyleContentSmallBold => FontFactory.GetFont(BaseFont.HELVETICA, 10, Font.BOLD);
        protected override Font StyleAudit => FontFactory.GetFont(BaseFont.HELVETICA, 12, ThemeAudit);
        protected override Font StyleAuditBold => FontFactory.GetFont(BaseFont.HELVETICA, 12, Font.BOLD, ThemeAudit);
        protected override Font StyleAuditSmall => FontFactory.GetFont(BaseFont.HELVETICA, 10, ThemeAudit);
        protected override Font StyleAuditSmallBold => FontFactory.GetFont(BaseFont.HELVETICA, 10, Font.BOLD, ThemeAudit);
    }
}
