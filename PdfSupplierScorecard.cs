using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PdfPlayGround
{
    using Model;
    using System.Linq;
    using System.Security.Claims;

    public class PdfSupplierScorecard: PdfBase
    {
        protected readonly ClaimScoreBoard Source;
        private string Id => Source.BoardId.ToString();

        private string InsurerHeader => Source.InsurerHeader;
        private string InsurerLog => Source.InsurerLogo;
        private string BorderTitle => Source.Title;
        protected List<ClaimScoreGroup> claimScoreGroups = new List<ClaimScoreGroup>();
        protected List<ClaimScoreTable> claimScoreTables = new List<ClaimScoreTable>();

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
                //var logoImg = Image.GetInstance(new Uri(insurerLogo.FirstOrDefault().Url));
                //logoImg.ScalePercent(30f);
                //logoImg.Alignment = Element.ALIGN_CENTER;

                //startTableLeftCell.AddElement(logoImg);

                PdfPCell startTableRightCell = new PdfPCell();
                startTableRightCell.Colspan = 6;
                startTableRightCell.BackgroundColor = new BaseColor(0, 0, 51);

                PdfPTable startTableRightCellTable = new PdfPTable(1);
                startTableRightCellTable.DefaultCell.Border = Rectangle.NO_BORDER;
                startTableRightCellTable.AddCell(new PdfPCell(new Phrase(BorderTitle, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White))));
                startTableRightCellTable.AddCell(new PdfPCell(new Phrase(InsurerHeader, new Font(Font.BOLD, 20f, Font.BOLD, BaseColor.White))));

                startTableRightCell.AddElement(startTableRightCellTable);

            //second table 

        }

    }


}
