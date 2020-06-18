using System;
using System.Collections.Generic;
using System.Globalization;
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

            ClaimTable = new List<InfoTableMetaData>
            {
                new InfoTableMetaData("Building Owner:","Brewster & Lily Hoo"),
                new InfoTableMetaData("Address:", "12 Sagittarius Drive, Colebee NSW 2761"),
                new InfoTableMetaData("Client:", "Gallagher Bassett Services Pty Ltd"),
                new InfoTableMetaData("Our Reference:", "ABC 00145"),
                new InfoTableMetaData("Client Reference:", "HBCF-CL-00XXXX"),
            };

            PdfPageEvent = new ENDataClassicHeader(this);
        }

        protected override void WriteDocument()
        {
            base.WriteDocument();
            Doc.AddAuthor("ENData");
            Doc.AddCreator("ENData for example");
            Doc.AddKeywords("This is an report");
            Doc.AddSubject("Test");
            Doc.AddTitle("XXX Report");


            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!page one!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
            //Doc.Add(new Paragraph("Site Inspection - Claim Information", StyleHeader) { SpacingAfter = 15f });
            //Doc.Add(new Paragraph("06 November 2019"));
            //Doc.Add(DividingLine);
            //Doc.Add(GenerateInfoTable(ClaimContent));

            //Doc.Add(new Paragraph("JOB DETAILS"));
            //Doc.Add(GenerateInfoTable(ClaimTable, 2));

            CultureInfo ci = new CultureInfo("en-US");
            var month = DateTime.Now.ToString("MMMM", ci);

            Doc.Add(new Phrase(DateTime.Now.Day.ToString() + " " + month + " " + DateTime.Now.Year.ToString()));
            Phrase email = new Phrase("builderswarrantyclaims@gbtpa.com.au", new Font(Font.BOLD, 12f, Font.NORMAL, BaseColor.Blue));

            Phrase infoList = new Phrase("Attention: Andrew Robinson" + "\n" + "Gallagher Bassett Services Pty Ltd" + "\n" + "Locked Bag 912, North Sydney NSW 2060" + "\n"  + "Email: ", new Font(Font.NORMAL, 10f, Font.NORMAL, BaseColor.Black));

            Doc.Add(infoList);
            Doc.NewPage();

            //set following to the new page
            //Doc.NewPage();
            //test 
            //Doc.Add(GenerateTestTable(ClaimTable, "JOB DETAILS normal table", 2));

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!page two!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//

            //***********************first part************************//
            //Table type 2  
            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 410f;
            table.LockedWidth = true;
            //table.SpacingBefore = 20;
            PdfPCell header = new PdfPCell(new Phrase("INTRODUCTION", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            header.BackgroundColor = new BaseColor(0, 0, 51);
            header.Colspan = 2;
            header.HorizontalAlignment = Element.ALIGN_CENTER;

            //heading spacing
            table.AddCell(header);
            table.AddCell("Claim Type:");
            table.AddCell("Incomeplete $ Defects");

            PdfPCell tableCombineLeft = new PdfPCell(new Phrase("Attendees: "));
            tableCombineLeft.Colspan = 1;
            tableCombineLeft.Rowspan = 1;
            tableCombineLeft.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(tableCombineLeft);

            PdfPTable tableCombineInfo = new PdfPTable(1);
            tableCombineInfo.AddCell("Building Consultant – Ace Building Consultants");
            tableCombineInfo.AddCell("Joe Brandt - Tenant");
            PdfPCell tableCombineLineing = new PdfPCell(tableCombineInfo);
            tableCombineLineing.Padding = 0f;
            table.AddCell(tableCombineLineing);

            table.AddCell("Inspection date:");
            table.AddCell("25 June 2019");
            table.AddCell("Inspection time:");
            table.AddCell("11:00am");

            //Doc.Add(table);


            ////put two table into one line
            ///
            //PdfPTable twoTable = new PdfPTable(new float[] { 250f, 20f, 250f});
            PdfPTable twoTable = new PdfPTable(2);
            twoTable.TotalWidth = 820f;
            twoTable.LockedWidth = true;

            PdfPCell twoTableLeft = new PdfPCell(GenerateTestTable(ClaimTable, "JOB DETAILS normal table", 2));
            twoTableLeft.Colspan = 1;
            twoTableLeft.Padding = 0;
            twoTableLeft.Border = Rectangle.NO_BORDER;

            //twoTableLeft.PaddingRight = 5f;

            PdfPCell twoTableRight = new PdfPCell(table);
            twoTableRight.Colspan = 1;
            twoTableRight.Padding = 0;
            twoTableRight.Border = Rectangle.NO_BORDER;

            //twoTableLeft.PaddingLeft = 5f;
            twoTable.AddCell(twoTableLeft);
            //twoTable.AddCell("");
            twoTable.AddCell(twoTableRight);

            Doc.Add(twoTable);
            //***********************second part************************//
            PdfPTable bcdTable = new PdfPTable(4);
            bcdTable.SpacingBefore = 10f;
            bcdTable.TotalWidth = 820f;
            bcdTable.LockedWidth = true;
            PdfPCell bcdTableHeader = new PdfPCell(new Phrase("BUILDING CONSULTANT’S DETAILS", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            bcdTableHeader.BackgroundColor = new BaseColor(0, 0, 51);
            bcdTableHeader.Colspan = 4;
            bcdTableHeader.HorizontalAlignment = Element.ALIGN_CENTER;

            bcdTable.AddCell(bcdTableHeader);
            string[] bcdInfo = {
                "Consultant name, phone No", "Building Consultant: 040404040",//line one 
                "Consultant’s reference No:", "ABC 00145",  
                "Consultant’s email address:", "Building consulatant@abconsultants.com",//line two
                "Consultant’s license number:", "131490C",  
                "Date Consultant appointed:", "20 May 2019",//line three
                "Date Owner contacted:","22 May 2019"   
            };

            foreach (string i in bcdInfo)
            {
                bcdTable.AddCell(i);
            }
            Doc.Add(bcdTable);

            //***********************Third part************************//
            PdfPTable bdTable = new PdfPTable(4);
            bdTable.SpacingBefore = 10f;
            bdTable.TotalWidth = 820f;
            bdTable.LockedWidth = true;
            PdfPCell bdTableHeader = new PdfPCell(new Phrase("BUILDING DESCRIPTION", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            bdTableHeader.BackgroundColor = new BaseColor(0, 0, 51);
            bdTableHeader.Colspan = 4;
            bdTableHeader.HorizontalAlignment = Element.ALIGN_CENTER;

            bdTable.AddCell(bdTableHeader);
            string[] bdInfo = {
                "Ground floor:", "Concrete Slab",//line one 
                "Occupancy:", "August 2017",  
                "First floor:", "Residential", //line two
                "Building Usage:", "131490C",
                "External cladding:", "Brick Veneer & Weatherboard", //line three
                "Building Classification:","Class 1a (House) and Class 10a (Garage)",
                "Roof cladding:", "Concrete Tiles", //line four
                "Orientation:", "East - Sagittarius Drive",
                "No of storeys:", "Two", //line five
                "Site topography:", "Minor downward slope from West to East",
                "Condition:", "Good", //line six
                "Project stage:", "Complete",
            };

            foreach (string i in bdInfo)
            {
                bdTable.AddCell(i);
            }
            Doc.Add(bdTable);

            //***********************Forth part************************//
            PdfPTable rcdTable = new PdfPTable(4);
            rcdTable.SpacingBefore = 12f;
            rcdTable.TotalWidth = 820f;
            rcdTable.LockedWidth = true;
            PdfPCell rcdTableHeader = new PdfPCell(new Phrase("PROJECT & CONTRACT DETAILS", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            rcdTableHeader.BackgroundColor = new BaseColor(0, 0, 51);
            rcdTableHeader.Colspan = 4;
            rcdTableHeader.HorizontalAlignment = Element.ALIGN_CENTER;

            rcdTable.AddCell(rcdTableHeader);
            string[] rcdInfo = {
                "Contract Date:", "6 April 2017",//line one 
                "Is Project Completed:", "Complete",
                "Contract Type:", "NSW Fair Trading – Fixed Price", //line two
                "Occupation/handover date:", "August 2017",
                "Contract Amount:", "$347,668.00", //line three
                "Builder’s name:","XXX Building Services Pty Ltd",
                "Variations Amount:", "Nil", //line four
                "Builder’s License No:", "XXXXXXXC"
            };

            foreach (string i in rcdInfo)
            {
                rcdTable.AddCell(i);
            }
            Doc.Add(rcdTable);


            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!page three!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
            Doc.NewPage();
            //table type 3
            PdfPTable table3 = new PdfPTable(9);
            table3.TotalWidth =820f;
            table3.LockedWidth = true;

            //left part of table 3 

            //left First part of Table 3 
            PdfPTable table3Left = new PdfPTable(7);
            //item
            PdfPCell table3LeftItemNum = new PdfPCell(new Phrase("ITEM 1", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            table3LeftItemNum.BackgroundColor = new BaseColor(0, 0, 51);
            table3LeftItemNum.HorizontalAlignment = Element.ALIGN_CENTER;
            //description
            PdfPCell table3LeftDescription = new PdfPCell(new Phrase("Description: ", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            table3LeftDescription.BackgroundColor = new BaseColor(0, 0, 51);
            table3LeftDescription.HorizontalAlignment = Element.ALIGN_CENTER;
            //descriptionInfo
            PdfPCell table3LeftInfo = new PdfPCell(new Phrase("Architrave cut around light switch", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            table3LeftInfo.BackgroundColor = new BaseColor(0, 0, 51);
            table3LeftInfo.Colspan = 3;
            table3LeftInfo.HorizontalAlignment = Element.ALIGN_CENTER;
            //location
            PdfPCell table3LeftLocation = new PdfPCell(new Phrase("Location: ", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            table3LeftLocation.BackgroundColor = new BaseColor(0, 0, 51);
            table3LeftLocation.HorizontalAlignment = Element.ALIGN_CENTER;
            //locationInfo
            PdfPCell table3LeftLocationInfom = new PdfPCell(new Phrase("Entry", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            table3LeftLocationInfom.BackgroundColor = new BaseColor(0, 0, 51);
            table3LeftLocationInfom.HorizontalAlignment = Element.ALIGN_CENTER;

            table3Left.AddCell(table3LeftItemNum);
            table3Left.AddCell(table3LeftDescription);
            table3Left.AddCell(table3LeftInfo);
            table3Left.AddCell(table3LeftLocation);
            table3Left.AddCell(table3LeftLocationInfom);

            //left Second part of table 3 
            PdfPCell crossRef = new PdfPCell(new Phrase("Cross Ref:" + "\n" + "11111", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            crossRef.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell lossType = new PdfPCell(new Phrase("Loss type: ", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            lossType.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell lossTypeInfo = new PdfPCell(new Phrase("Minor Defect ", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            lossTypeInfo.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell completionStatus = new PdfPCell(new Phrase("Completion status:", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            completionStatus.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell completionStatusInfo = new PdfPCell(new Phrase("N/A", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            completionStatusInfo.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell recommendation = new PdfPCell(new Phrase("Recommendation:", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            recommendation.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell recommendationInfo = new PdfPCell(new Phrase("Decline:", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            recommendationInfo.HorizontalAlignment = Element.ALIGN_CENTER;

            table3Left.AddCell(crossRef);
            table3Left.AddCell(lossType);
            table3Left.AddCell(lossTypeInfo);
            table3Left.AddCell(completionStatus);
            table3Left.AddCell(completionStatusInfo);
            table3Left.AddCell(recommendation);
            table3Left.AddCell(recommendationInfo);

            //info from third line to last line 
            PdfPCell observation = new PdfPCell(new Phrase("Observation", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            observation.HorizontalAlignment = Element.ALIGN_CENTER;
            observation.VerticalAlignment = Element.ALIGN_MIDDLE;

            PdfPCell observationInfo = new PdfPCell(new Phrase(
                "The item of claim is unspecific to the precise locations of the scuff marks." + "\n" +
                "The inspection revealed several minor scuff marks to the internal walls of the residence, consistent with accidental damage." + "\n" +
                "As the residence was completed in August 2017, and occupied since, and the inspection carried out in July 2019, it cannot be confirmed who caused the damage to the internal walls. There is insufficient evidence to confirm whether the damage to the internal walls was caused by the Builder during the construction works. The damage may have been caused by others during occupation of the residence."
                , new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));
            observationInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            observationInfo.Colspan = 6;

            PdfPCell cause = new PdfPCell(new Phrase("Cause", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            cause.HorizontalAlignment = Element.ALIGN_CENTER;
            cause.VerticalAlignment = Element.ALIGN_MIDDLE;

            PdfPCell causeInfo = new PdfPCell(new Phrase("Minor building movement and poor fixing", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));
            causeInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            causeInfo.Colspan = 6;

            PdfPCell breach = new PdfPCell(new Phrase("Breach(es)", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            breach.HorizontalAlignment = Element.ALIGN_CENTER;
            breach.VerticalAlignment = Element.ALIGN_MIDDLE;

            PdfPCell breachInfo = new PdfPCell(new Phrase("The NSW Office of Fair Trading - Guide to Standards and Tolerances 2017" +
                ", Clause 11.1 Gaps associated with internal fixing, " +
                "which states: “Unless documented otherwise, gaps between mouldings or between mouldings and other fixtures, " +
                "at mitre or butt joints, or at junctions with a wall or other surfaces, are defective if they exist at handover, " +
                "or exceed 1 mm in width within the first 12 months of completion and are visible from a normal viewing position. " +
                "After the first 12 months, gaps are defective if they exceed 2 mm in width and are visible from a normal viewing position. " +
                "Gaps between skirting and flooring are defective if they exceed 2 mm within the first 24 months after handover and are visible from a normal viewing position.”"
                , new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));

            breachInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            breachInfo.Colspan = 6;

            PdfPCell reasoForDenial = new PdfPCell(new Phrase("Reason for Denial", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            reasoForDenial.HorizontalAlignment = Element.ALIGN_CENTER;
            reasoForDenial.VerticalAlignment = Element.ALIGN_MIDDLE;

            PdfPCell reasoForDenialInfo = new PdfPCell(new Phrase("N/A", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));
            reasoForDenialInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            reasoForDenialInfo.Colspan = 6;

            //list in Suggested Scope of Works
            List table3List = new List(List.ORDERED, 20f);
            table3List.SetListSymbol("\u2022");
            table3List.IndentationLeft = 12f;
            iTextSharp.text.ListItem item1 = new iTextSharp.text.ListItem("Re-orientate the light switch adjacent to the front door vertically to clear the architrave", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
            iTextSharp.text.ListItem item2 = new iTextSharp.text.ListItem("Replace the cut-out section or architrave with a matching profiled section.", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
            iTextSharp.text.ListItem item3 = new iTextSharp.text.ListItem("Repair the plasterboard wall lining as part of the switch re-orientation. Work to AS 2589 – Gypsum lining – Application and finishing.", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
            iTextSharp.text.ListItem item4 = new iTextSharp.text.ListItem("Prepare and paint the affected wall and architrave, to the nearest break or joint using a similar colour and finish to the existing and in accordance with AS 2311 - Guide To The Painting Of Buildings.", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
            iTextSharp.text.ListItem item5 = new iTextSharp.text.ListItem("Make good all affected surfaces as part of the works, to their prior condition.", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));

            table3List.Add(item1);
            table3List.Add(item2);
            table3List.Add(item3);
            table3List.Add(item4);
            table3List.Add(item5);
            //list in Suggested Scope of Works ends

            PdfPCell suggestedScopeOfWorks = new PdfPCell(new Phrase("Suggested Scope of Works", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            suggestedScopeOfWorks.HorizontalAlignment = Element.ALIGN_CENTER;
            suggestedScopeOfWorks.VerticalAlignment = Element.ALIGN_MIDDLE;

            PdfPCell suggestedScopeOfWorksList = new PdfPCell();
            suggestedScopeOfWorksList.HorizontalAlignment = Element.ALIGN_LEFT;
            suggestedScopeOfWorksList.Colspan = 6;


            //add list title
            Phrase listTitle = new Phrase("Allow: ", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
            Paragraph listTitleNamePah = new Paragraph();
            string listTitleNameText = "Entity";
            listTitleNamePah.Add(listTitleNameText);
            listTitleNamePah.IndentationLeft = 12f;
            listTitleNamePah.Font.Size = 12f;

            //Phrase listTitleName = new Phrase(listTitleNamePah, new Font(Font.UNDEFINED, 12f, Font.UNDERLINE, BaseColor.Black));

            suggestedScopeOfWorksList.AddElement(listTitle);
            suggestedScopeOfWorksList.AddElement(listTitleNamePah);
            suggestedScopeOfWorksList.AddElement(table3List);

            table3Left.AddCell(observation);
            table3Left.AddCell(observationInfo);

            table3Left.AddCell(cause);
            table3Left.AddCell(causeInfo);

            table3Left.AddCell(breach);
            table3Left.AddCell(breachInfo);

            table3Left.AddCell(reasoForDenial);
            table3Left.AddCell(reasoForDenialInfo);

            table3Left.AddCell(suggestedScopeOfWorks);
            table3Left.AddCell(suggestedScopeOfWorksList);

            PdfPCell table3Header = new PdfPCell(table3Left);
            table3Header.Colspan = 7;

            //right part of table 3 

            //image phrase 
            string imagePath = "C:/Users/ChrisLi/Desktop/";
            Image textImg = Image.GetInstance(imagePath + "ENDataTransparent.png");
            textImg.SpacingAfter = 5f;

            PdfPCell table3Right = new PdfPCell();
            table3Right.AddElement(textImg);
            table3Right.AddElement(textImg);
            table3Right.AddElement(textImg);
            table3Right.Colspan = 2;
            table3Right.HorizontalAlignment = Element.ALIGN_LEFT;
            //table3Right.Rowspan = 6;

            table3.AddCell(table3Header);
            table3.AddCell(table3Right);

            Doc.Add(table3);


        }


        protected PdfPTable GenerateInfoTable(List<InfoTableMetaData> model, byte columNum = 4, float[] widths = null)
        {
            PdfPCell cell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER
            };
            return GenerateInfoTable(model, columNum, widths, cell, cell);
        }

        protected PdfPTable GenerateTestTable(List<InfoTableMetaData> model, string title, byte columNum = 4, float[] widths = null)
        {
            // Set the title cell stylle
            PdfPCell titleCell = new PdfPCell(new Phrase(title, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)))
            {
                //BackgroundColor = BaseColor.LightGray,
                BackgroundColor = new BaseColor(0, 0, 51),
                HorizontalAlignment = Element.ALIGN_CENTER
            };

            // Set the config title style
            // You can change this to null to see the difference (using the default style in base class)
            //PdfPCell cell = null;
            PdfPCell cell = new PdfPCell()
            {
                Border = Rectangle.BOX
            };

            var table = GenerateInfoTable(model, columNum, widths, headerCell: cell, contentCell: cell, titleCell: titleCell);
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.TotalWidth = 410f;
            return table;
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
