using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PdfPlayGround
{
    public class PdfTest : PdfBase
    {
        protected List<InfoTableMetaData> ClaimContent = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> ClaimTable = new List<InfoTableMetaData>();

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

        protected override void FillTemplate()
        {
            ClaimTable = new List<InfoTableMetaData>
            {
                new InfoTableMetaData("Building Owner:","Brewster & Lily Hoo"),
                new InfoTableMetaData("Address:", "12 Sagittarius Drive, Colebee NSW 2761"),
                new InfoTableMetaData("Client:", "Gallagher Bassett Services Pty Ltd"),
                new InfoTableMetaData("Our Reference:", "ABC 00145"),
                new InfoTableMetaData("Client Reference:", "HBCF-CL-00XXXX"),
            };
        }

        protected override void WriteDocument()
        {
            Doc.AddAuthor("ENData");
            Doc.AddCreator("ENData for example");
            Doc.AddKeywords("This is an report");
            Doc.AddSubject("Test");
            Doc.AddTitle("XXX Report");

            base.WriteDocument();
            Doc.Add(new Paragraph("Site Inspection - Claim Information", StyleHeader) { SpacingAfter = 15f });
            Doc.Add(new Paragraph("06 November 2019"));
            Doc.Add(DividingLine);
            Doc.Add(GenerateInfoTable(ClaimContent));

            Doc.Add(new Paragraph("JOB DETAILS"));
            Doc.Add(GenerateInfoTable(ClaimTable, 2));


            //PdfPTable table = new PdfPTable(4);
            //table.TotalWidth = 400f;
            //table.LockedWidth = true;
            //PdfPCell header = new PdfPCell(new Phrase("this is a different table"));
            //header.HorizontalAlignment = 1;
            //header.Colspan = 4;
            //table.AddCell(header);
            //table.AddCell("Cell 1");
            //table.AddCell("Cell 2");
            //table.AddCell("Cell 3");
            //table.AddCell("Cell 4");
            //PdfPTable nested = new PdfPTable(1);
            //nested.AddCell("Nested Row 1");
            //nested.AddCell("Nested Row 2");
            //nested.AddCell("Nested Row 3");
            //PdfPCell nesthousing = new PdfPCell(nested);
            //nesthousing.Padding = 0f;
            //table.AddCell(nesthousing);
            //PdfPCell bottom = new PdfPCell(new Phrase("bottom"));
            //bottom.Colspan = 3;
            //table.AddCell(bottom);
            //Doc.Add(table);

            //test 
            PdfPTable Table1 = new PdfPTable(2);
            PdfPCell header1 = new PdfPCell(new Phrase("JOB DETAILS normal table"));
            header1.Colspan = 2;
            header1.BackgroundColor = new CmykColor(0, 0, 0, 50);
            header1.HorizontalAlignment = 1;
            Table1.AddCell(header1);
            Table1.AddCell("Building Owner:");
            Table1.AddCell("Brewster & Lily Hoo");

            Table1.AddCell("Address:");
            Table1.AddCell("12 Sagittarius Drive, Colebee NSW 2761");

            Table1.AddCell("Client");
            Table1.AddCell("Gallagher Bassett Services Pty Ltd");

            Table1.AddCell("Our Reference:");
            Table1.AddCell("ABC 00145");

            Table1.AddCell("Client Reference:");
            Table1.AddCell("HBCF-CL-00XXXX");
            Doc.Add(Table1);

            //table example 
            PdfPTable table = new PdfPTable(2);

        }


        protected PdfPTable GenerateInfoTable(List<InfoTableMetaData> model, byte columNum = 4, float[] widths = null)
        {
            PdfPCell cell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                Rowspan = 1,
                Colspan = 1
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
