using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Security.Claims;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PdfPlayGround
{
    using Model;
    using System.Linq;

    public class PdfTest : PdfBase
    {
        protected readonly ClaimJob Source;
        protected List<InfoTableMetaData> ClaimContent = new List<InfoTableMetaData>();

        //Page One 
        protected List<InfoTableMetaData> JobDetailsTable = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> IntroductionTable = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> BuildingConsultantDetailTable = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> BuildingDescriptionTable = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> ProjectContractDetailTable = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> ClaimDetailTable = new List<InfoTableMetaData>();

        private Card CoverPageCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Cover Page");

        public PdfTest(ClaimJob claimJob)
        {
            Source = claimJob;
            PageMargin = new Margin(10, 10, 90, 20);
            PageInfo = new Rectangle(PageSize.A4.Rotate());
        }

        protected override void InitialPdf()
        {


            ClaimContent = new List<InfoTableMetaData>
            {
                new InfoTableMetaData("Insurance Ref #:", Source.RefNumber ),
                new InfoTableMetaData("Event Type:", "Storm" )
            };

            foreach(var card in Source.ReportForm.Cards)
            {
                if (cardFormDict.TryGetValue(card.Title, out var componentType))
                {
                    foreach (var field in card.Fields)
                    {
                        switch (componentType)
                        {

                        }
                        switch (field.FieldType)
                        {
                            case "FileField":
                                var valueFiles = field.Value as IEnumerable<string>;
                                break;
                            default:
                                var value = field.Value as string;
                                break;
                        }
                    }
                }
            }

            JobDetailsTable = new List<InfoTableMetaData>
            {
                new InfoTableMetaData("Building Owner:","Brewster & Lily Hoo"),
                new InfoTableMetaData("Address:", "12 Sagittarius Drive, Colebee NSW 2761"),
                new InfoTableMetaData("Client:", "Gallagher Bassett Services Pty Ltd"),
                new InfoTableMetaData("Our Reference:", "ABC 00145"),
                new InfoTableMetaData("Client Reference:", "HBCF-CL-00XXXX"),
            };

            BuildingConsultantDetailTable = new List<InfoTableMetaData>
            {
                new InfoTableMetaData("Consultant name, phone No", "Building Consultant: 040404040"),
                new InfoTableMetaData("Consultant’s reference No:", "ABC 00145"),
                new InfoTableMetaData("Consultant’s email address:", "Building consulatant@abconsultants.com"),
                new InfoTableMetaData("Consultant’s license number:", "131490C"),
                new InfoTableMetaData("Date Consultant appointed:", "20 May 2019"),
                new InfoTableMetaData("Date Owner contacted:", "22 May 2019"),
            };

            BuildingDescriptionTable = new List<InfoTableMetaData>
            {
                new InfoTableMetaData("Ground floor:", "Concrete Slab"),
                new InfoTableMetaData("Occupancy:", "August 2017"),
                new InfoTableMetaData("First floor:", "Residential"),
                new InfoTableMetaData("Building Usage:", "131490C"),

                new InfoTableMetaData("External cladding:", "Brick Veneer & Weatherboard"),
                new InfoTableMetaData("Building Classification:", "Class 1a (House) and Class 10a(Garage)"),
                new InfoTableMetaData("Roof cladding:", "Concrete Tiles"),
                new InfoTableMetaData("Orientation:", "East - Sagittarius Drive"),
                new InfoTableMetaData("No of storeys:", "Two"),
                new InfoTableMetaData("Site topography:", "Minor downward slope from West to East"),
                new InfoTableMetaData("Condition:", "Good"),
                new InfoTableMetaData("Project stage:", "Complete")
            };

            ProjectContractDetailTable = new List<InfoTableMetaData>
            {
                new InfoTableMetaData("Contract Date:", "6 April 2017"),
                new InfoTableMetaData("Is Project Completed:", "Complete"),
                new InfoTableMetaData("Contract Type:", "NSW Fair Trading – Fixed Price"),
                new InfoTableMetaData("Occupation/handover date:", "August 2017"),
                new InfoTableMetaData("Contract Amount:", "$347,668.00"),
                new InfoTableMetaData("Builder’s name:", "XXX Building Services Pty Ltd"),
                new InfoTableMetaData("Variations Amount:", "Nil"),
                new InfoTableMetaData("Builder’s License No:", "XXXXXXXC")
            };

            ClaimDetailTable = new List<InfoTableMetaData>
            {
                new InfoTableMetaData("Date claim was reported (in writing) to Gallagher Bassett", "Claim Form signed 12/04/2019"),
                new InfoTableMetaData("Age of property at time claim was reported to Gallagher Bassett", "1 year 8 months"),
                new InfoTableMetaData("Age of property at date claim was first reported to Gallagher Bassett", "1 year 8 months"),
                new InfoTableMetaData("Is Property in time for Major Defects", "Yes"),
                new InfoTableMetaData("Is Property in time for other defects", "Yes"),
                new InfoTableMetaData("Was the Contract Price in line with industry standard", "Yes"),
                new InfoTableMetaData("Method used to measure contract price", "Rawlinsons Construction Cost Guide"),
                new InfoTableMetaData("Have any pre-payments been made", "N/A – Contract paid in full"),
                new InfoTableMetaData("Have any overpayments been identified and how much, if so?", "N/A – Contract paid in full")
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
            //header 
            CultureInfo ci = new CultureInfo("en-US");
            var month = DateTime.Now.ToString("MMMM", ci);

            Doc.Add(new Phrase((DateTime.Now.Day.ToString() + " " + month + " " + DateTime.Now.Year.ToString() + "\n"), new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));

            Phrase email = new Phrase("builderswarrantyclaims@gbtpa.com.au", new Font(Font.UNDEFINED, 12f, Font.UNDERLINE, BaseColor.Blue));

            Phrase infoList = new Phrase("Attention: Andrew Robinson" + "\n" + 
                "Gallagher Bassett Services Pty Ltd" + "\n" + 
                "Locked Bag 912, North Sydney NSW 2060" + "\n" + "Email: ",
                new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black
            ));

            Phrase emailInfo = new Phrase();
            emailInfo.Add(email);

            Doc.Add(infoList);
            Doc.Add(emailInfo);

            if (CoverPageCard != null)
            {
                PdfPTable firstPageTable = new PdfPTable(1);
                firstPageTable.TotalWidth = 820f;
                firstPageTable.LockedWidth = true;
                firstPageTable.SpacingBefore = 10f;
                firstPageTable.DefaultCell.Border = Rectangle.NO_BORDER;

                PdfPCell firstPageTableHeader = new PdfPCell(new Phrase(CoverPageCard.Fields[0].Value.ToString(), new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
                firstPageTableHeader.Colspan = 1;
                firstPageTableHeader.BackgroundColor = new BaseColor(0, 0, 51);
                firstPageTableHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                firstPageTableHeader.Border = Rectangle.NO_BORDER;

                PdfPCell firstPageTableBody = new PdfPCell(
                    new Phrase(CoverPageCard.Fields[1].Value.ToString(),
                    new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));
                firstPageTableBody.Border = Rectangle.NO_BORDER;
                firstPageTableBody.PaddingTop = 10f;
                firstPageTableBody.PaddingBottom = 10f;

                firstPageTable.AddCell(firstPageTableHeader);
                firstPageTable.AddCell(firstPageTableBody);

                var coverImages = CoverPageCard.Fields[2].Value as IEnumerable<ReportFile>;
                if (coverImages?.Count() > 0)
                {
                    PdfPCell firstPageTableImgInfo = new PdfPCell();
                    firstPageTableImgInfo.Colspan = 1;
                    firstPageTableImgInfo.Border = Rectangle.NO_BORDER;
                    firstPageTableImgInfo.HorizontalAlignment = Element.ALIGN_CENTER;
                    firstPageTableImgInfo.VerticalAlignment = Element.ALIGN_MIDDLE;
                    foreach (var imgUrl in coverImages)
                    {
                        var coverImg = Image.GetInstance(new Uri(imgUrl.Url));
                        coverImg.ScalePercent(30f);
                        coverImg.Alignment = Element.ALIGN_CENTER;
                        firstPageTableImgInfo.AddElement(coverImg);

                        //imgUrl.Name;
                        //Paragraph firstPageTableImgText = new Paragraph("Shows the front Eastern elevation", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black));
                        //firstPageTableImgText.Alignment = Element.ALIGN_CENTER;

                        //firstPageTableImgInfo.AddElement(firstPageTableImgText);
                    }

                    firstPageTable.AddCell(firstPageTableImgInfo);
                }

                Doc.Add(firstPageTable);

            }
            Doc.NewPage();


            //set following to the new page
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!page two!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//

            //***********************first part************************//
            //Table type 2  
            PdfPTable introTable = new PdfPTable(2);
            introTable.TotalWidth = 410f;
            introTable.LockedWidth = true;
            introTable.SpacingBefore = 20f;
            //introTable.SpacingBefore = 20;
            PdfPCell header = new PdfPCell(new Phrase("INTRODUCTION", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            header.BackgroundColor = new BaseColor(0, 0, 51);
            header.Colspan = 2;
            header.HorizontalAlignment = Element.ALIGN_CENTER;
            header.PaddingTop = 4f;
            header.PaddingBottom = 4f;

            //heading spacing
            introTable.AddCell(header);

            PdfPCell claimType = new PdfPCell(new Phrase("Claim Type: ", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));
            claimType.PaddingTop = 4f;
            claimType.PaddingBottom = 4f;
            claimType.VerticalAlignment = Element.ALIGN_MIDDLE;
            claimType.Colspan = 1;

            PdfPCell claimTypeInfo = new PdfPCell(new Phrase("Incomeplete & Defects", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black))); 
            claimTypeInfo.PaddingTop = 4f;
            claimTypeInfo.PaddingBottom = 4f;
            claimTypeInfo.VerticalAlignment = Element.ALIGN_MIDDLE;
            claimTypeInfo.Colspan = 1;

            introTable.AddCell(claimType);
            introTable.AddCell(claimTypeInfo);

            PdfPCell tableCombineLeft = new PdfPCell(new Phrase("Attendees: "));
            tableCombineLeft.Colspan = 1;
            tableCombineLeft.Rowspan = 1;
            tableCombineLeft.PaddingTop = 4f;
            tableCombineLeft.PaddingBottom = 4f;
            tableCombineLeft.VerticalAlignment = Element.ALIGN_MIDDLE;
            introTable.AddCell(tableCombineLeft);

            PdfPTable tableCombineInfo = new PdfPTable(1);
            PdfPCell attendeeInfoOne = new PdfPCell(new Phrase("Building Consultant – Ace Building Consultants"));
            attendeeInfoOne.PaddingTop = 4f;
            attendeeInfoOne.PaddingBottom = 4f;
            attendeeInfoOne.VerticalAlignment = Element.ALIGN_MIDDLE;

            PdfPCell attendeeInfoTwo = new PdfPCell(new Phrase("Joe Brandt - Tenant"));
            attendeeInfoTwo.PaddingTop = 4f;
            attendeeInfoTwo.PaddingBottom = 4f;
            attendeeInfoTwo.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableCombineInfo.AddCell(attendeeInfoOne);
            tableCombineInfo.AddCell(attendeeInfoTwo);

            PdfPCell tableCombineLineing = new PdfPCell(tableCombineInfo);
            tableCombineLineing.Padding = 0f;
            introTable.AddCell(tableCombineLineing);


            string[] introTableInfo = {
                "Inspection date:", "25 June 2019",
                "Inspection time:", "11:00am"
            };

            foreach (string i in introTableInfo)
            {
                PdfPCell introRestInfo = new PdfPCell(new Phrase(i));
                introRestInfo.PaddingTop = 4f;
                introRestInfo.PaddingBottom = 4f;
                introTable.AddCell(introRestInfo);
            }

            ////put two table into one line

            PdfPTable twoTable = new PdfPTable(2);
            twoTable.TotalWidth = 820f;
            twoTable.LockedWidth = true;

            PdfPCell twoTableLeft = new PdfPCell(GenerateTestTable(JobDetailsTable, "JOB DETAILS", 2, 406f));
            twoTableLeft.Colspan = 1;
            twoTableLeft.Padding = 0;
            twoTableLeft.Border = Rectangle.NO_BORDER;

            //twoTableLeft.PaddingRight = 5f;

            PdfPCell twoTableRight = new PdfPCell(introTable);
            twoTableRight.Colspan = 1;
            twoTableRight.Padding = 0;
            twoTableRight.Border = Rectangle.NO_BORDER;

            //twoTableLeft.PaddingLeft = 5f;
            twoTable.AddCell(twoTableLeft);
            //twoTable.AddCell("");
            twoTable.AddCell(twoTableRight);

            Doc.Add(twoTable);

            //***********************second part************************//
            //BuildingConsultantDetailTable
            PdfPTable bcdTable = new PdfPTable(1);
            bcdTable.SpacingBefore = 10f;
            bcdTable.DefaultCell.Border = Rectangle.NO_BORDER;


            PdfPCell BuildingConsultantDetail = new PdfPCell(GenerateTestTable(BuildingConsultantDetailTable, "BUILDING CONSULTANT’S DETAILS", 4, 820f));
            BuildingConsultantDetail.HorizontalAlignment = Element.ALIGN_CENTER;
            BuildingConsultantDetail.VerticalAlignment = Element.ALIGN_MIDDLE;
            BuildingConsultantDetail.Border = 0;
            bcdTable.AddCell(BuildingConsultantDetail);

            Doc.Add(bcdTable);


            //***********************Third part************************//
            //BuildingDescriptionTable
            PdfPTable bdTable = new PdfPTable(1);
            bdTable.SpacingBefore = 10f;
            bdTable.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell BuildingDescription = new PdfPCell(GenerateTestTable(BuildingDescriptionTable, "BUILDING DESCRIPTION", 4, 820f));
            BuildingDescription.HorizontalAlignment = Element.ALIGN_CENTER;
            BuildingDescription.VerticalAlignment = Element.ALIGN_MIDDLE;
            BuildingDescription.Border = 0;
            bdTable.AddCell(BuildingDescription);

            Doc.Add(bdTable);

            //***********************Forth part************************//
            //ProjectContractDetailTable
            PdfPTable rcdTable = new PdfPTable(1);
            rcdTable.SpacingBefore = 10f;
            rcdTable.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell ProjectContractDetail = new PdfPCell(GenerateTestTable(ProjectContractDetailTable, "PROJECT & CONTRACT DETAILS", 4, 820f));
            ProjectContractDetail.HorizontalAlignment = Element.ALIGN_CENTER;
            ProjectContractDetail.VerticalAlignment = Element.ALIGN_MIDDLE;
            ProjectContractDetail.Border = 0;
            rcdTable.AddCell(ProjectContractDetail);


            Doc.Add(rcdTable);


            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!page Three!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
            //**********************first table in page three************************//
            //ClaimDetailTable
            PdfPTable cdTable = new PdfPTable(1);
            cdTable.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell ClaimDetail = new PdfPCell(GenerateTestTable(ClaimDetailTable, "CLAIM DETAILS", 2, 820f));
            ClaimDetail.HorizontalAlignment = Element.ALIGN_CENTER;
            ClaimDetail.VerticalAlignment = Element.ALIGN_MIDDLE;
            ClaimDetail.Border = 0;
            cdTable.AddCell(ClaimDetail);

            Doc.Add(cdTable);

            //**********************Second table********************//

            PdfPTable dutyTable = new PdfPTable(1);
            dutyTable.TotalWidth = 820f;
            dutyTable.LockedWidth = true;
            dutyTable.SpacingBefore = 10f;

            PdfPCell dutyTableHeader = new PdfPCell(new Phrase("DUTY TO THE TRIBUNAL", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            dutyTableHeader.BackgroundColor = new BaseColor(0, 0, 51);
            dutyTableHeader.HorizontalAlignment = Element.ALIGN_CENTER;
            dutyTable.AddCell(dutyTableHeader);

            Paragraph dutyPara = new Paragraph();
            Phrase dutyPhrase1 = new Phrase("I affirm that I have read the NSW Civil and Administrative Tribunal (NCAT), Expert Witness Code of Conduct NCAT’s Procedural Direction 3 to Expert Witnesses. (See ‘Annexure B’)", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
            Phrase dutyPhrase2 = new Phrase("This report has been prepared in accordance with that Code and those directions.", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
            Phrase dutyPhrase3 = new Phrase("In accordance with those directions, I confirm it is my duty to assist the Tribunal and not act as an advocate for any party in this matter.", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
            dutyPara.Add(dutyPhrase1);
            dutyPara.Add("\n");
            dutyPara.Add(dutyPhrase2);
            dutyPara.Add("\n");
            dutyPara.Add(dutyPhrase3);
            dutyPara.Leading = 60;

            PdfPCell dutyTableInfo = new PdfPCell(
                //new Phrase(
                //"I affirm that I have read the NSW Civil and Administrative Tribunal (NCAT), " +
                //"Expert Witness Code of Conduct NCAT’s Procedural Direction 3 to Expert Witnesses. (See ‘Annexure B’)" +
                //"\n" + "This report has been prepared in accordance with that Code and those directions." +
                //"\n" + "In accordance with those directions, I confirm it is my duty to assist the Tribunal and not act as an advocate for any party in this matter.",
                //new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)
                //)
                dutyPara
                );
            dutyTableInfo.PaddingTop = 10f;
            dutyTableInfo.PaddingBottom = 10f;

            dutyTable.AddCell(dutyTableInfo);

            Doc.Add(dutyTable);

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!page Four!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
            Doc.NewPage();

            CreateTypeThreeTable();
            //table type 3

            //PdfPTable table3 = new PdfPTable(9);
            //table3.TotalWidth = 820f;
            //table3.LockedWidth = true;
            //table3.DefaultCell.FixedHeight = 700f;

            ////left part of table 3 

            ////left First part of Table 3 
            //PdfPTable table3Left = new PdfPTable(7);
            ////item
            //PdfPCell table3LeftItemNum = new PdfPCell(new Phrase("ITEM 1", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            //table3LeftItemNum.BackgroundColor = new BaseColor(0, 0, 51);
            //table3LeftItemNum.HorizontalAlignment = Element.ALIGN_CENTER;
            ////description
            //PdfPCell table3LeftDescription = new PdfPCell(new Phrase("Description: ", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            //table3LeftDescription.BackgroundColor = new BaseColor(0, 0, 51);
            //table3LeftDescription.HorizontalAlignment = Element.ALIGN_CENTER;
            ////descriptionInfo
            //PdfPCell table3LeftInfo = new PdfPCell(new Phrase("Architrave cut around light switch", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            //table3LeftInfo.BackgroundColor = new BaseColor(0, 0, 51);
            //table3LeftInfo.Colspan = 3;
            //table3LeftInfo.HorizontalAlignment = Element.ALIGN_CENTER;
            ////location
            //PdfPCell table3LeftLocation = new PdfPCell(new Phrase("Location: ", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            //table3LeftLocation.BackgroundColor = new BaseColor(0, 0, 51);
            //table3LeftLocation.HorizontalAlignment = Element.ALIGN_CENTER;
            ////locationInfo
            //PdfPCell table3LeftLocationInfom = new PdfPCell(new Phrase("Entry", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            //table3LeftLocationInfom.BackgroundColor = new BaseColor(0, 0, 51);
            //table3LeftLocationInfom.HorizontalAlignment = Element.ALIGN_CENTER;

            //table3Left.AddCell(table3LeftItemNum);
            //table3Left.AddCell(table3LeftDescription);
            //table3Left.AddCell(table3LeftInfo);
            //table3Left.AddCell(table3LeftLocation);
            //table3Left.AddCell(table3LeftLocationInfom);

            ////left Second part of table 3 
            //PdfPCell crossRef = new PdfPCell(new Phrase("Cross Ref:" + "\n" + "11111", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            //crossRef.HorizontalAlignment = Element.ALIGN_LEFT;

            //PdfPCell lossType = new PdfPCell(new Phrase("Loss type: ", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            //lossType.HorizontalAlignment = Element.ALIGN_LEFT;

            //PdfPCell lossTypeInfo = new PdfPCell(new Phrase("Minor Defect ", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            //lossTypeInfo.HorizontalAlignment = Element.ALIGN_LEFT;

            //PdfPCell completionStatus = new PdfPCell(new Phrase("Completion status:", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            //completionStatus.HorizontalAlignment = Element.ALIGN_LEFT;

            //PdfPCell completionStatusInfo = new PdfPCell(new Phrase("N/A", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            //completionStatusInfo.HorizontalAlignment = Element.ALIGN_LEFT;

            //PdfPCell recommendation = new PdfPCell(new Phrase("Recommendation:", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            //recommendation.HorizontalAlignment = Element.ALIGN_LEFT;

            //PdfPCell recommendationInfo = new PdfPCell(new Phrase("Decline:", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            //recommendationInfo.HorizontalAlignment = Element.ALIGN_LEFT;

            //table3Left.AddCell(crossRef);
            //table3Left.AddCell(lossType);
            //table3Left.AddCell(lossTypeInfo);
            //table3Left.AddCell(completionStatus);
            //table3Left.AddCell(completionStatusInfo);
            //table3Left.AddCell(recommendation);
            //table3Left.AddCell(recommendationInfo);

            ////info from third line to last line 
            //PdfPCell observation = new PdfPCell(new Phrase("Observation", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            //observation.HorizontalAlignment = Element.ALIGN_LEFT;
            //observation.VerticalAlignment = Element.ALIGN_MIDDLE;

            //PdfPCell observationInfo = new PdfPCell(new Phrase(
            //    "The item of claim is unspecific to the precise locations of the scuff marks." + "\n" +
            //    "The inspection revealed several minor scuff marks to the internal walls of the residence, consistent with accidental damage." + "\n" +
            //    "As the residence was completed in August 2017, and occupied since, and the inspection carried out in July 2019, it cannot be confirmed who caused the damage to the internal walls. " +
            //    "There is insufficient evidence to confirm whether the damage to the internal walls was caused by the Builder during the construction works. " +
            //    "The damage may have been caused by others during occupation of the residence."
            //    , new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));
            //observationInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            //observationInfo.Colspan = 6;
            //observationInfo.PaddingLeft = 4f;

            //PdfPCell cause = new PdfPCell(new Phrase("Cause", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            //cause.HorizontalAlignment = Element.ALIGN_LEFT;
            //cause.VerticalAlignment = Element.ALIGN_MIDDLE;

            //PdfPCell causeInfo = new PdfPCell(new Phrase("Minor building movement and poor fixing", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));
            //causeInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            //causeInfo.Colspan = 6;
            //causeInfo.Padding = 4f;

            //PdfPCell breach = new PdfPCell(new Phrase("Breach(es)", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            //breach.HorizontalAlignment = Element.ALIGN_LEFT;
            //breach.VerticalAlignment = Element.ALIGN_MIDDLE;

            //PdfPCell breachInfo = new PdfPCell(new Phrase(
            //    "The NSW Office of Fair Trading - Guide to Standards and Tolerances 2017" +
            //    ", Clause 11.1 Gaps associated with internal fixing, " +
            //    "which states: “Unless documented otherwise, gaps between mouldings or between mouldings and other fixtures, " +
            //    "at mitre or butt joints, or at junctions with a wall or other surfaces, are defective if they exist at handover, " +
            //    "or exceed 1 mm in width within the first 12 months of completion and are visible from a normal viewing position. " +
            //    "After the first 12 months, gaps are defective if they exceed 2 mm in width and are visible from a normal viewing position. " +
            //    "Gaps between skirting and flooring are defective if they exceed 2 mm within the first 24 months after handover and are visible from a normal viewing position.”"
            //    , new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));

            //breachInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            //breachInfo.Colspan = 6;
            ////breachInfo.PaddingTop = 4f;
            ////breachInfo.PaddingBottom = 4f;
            //breachInfo.PaddingLeft = 4f;

            //PdfPCell reasonForDenial = new PdfPCell(new Phrase("Reason for Denial", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            //reasonForDenial.HorizontalAlignment = Element.ALIGN_LEFT;
            //reasonForDenial.VerticalAlignment = Element.ALIGN_MIDDLE;

            //PdfPCell reasonForDenialInfo = new PdfPCell(new Phrase("N/A", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));
            //reasonForDenialInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            //reasonForDenialInfo.Colspan = 6;
            //reasonForDenialInfo.PaddingLeft = 4f;

            ////list in Suggested Scope of Works
            //List table3List = new List(List.ORDERED, 15f);
            //table3List.SetListSymbol("\u2022");
            //table3List.IndentationLeft = 12f;
            //string[] listInfo = {
            //    "Re-orientate the light switch adjacent to the front door vertically to clear the architrave",
            //    "Replace the cut-out section or architrave with a matching profiled section.",
            //    "Repair the plasterboard wall lining as part of the switch re-orientation. Work to AS 2589 – Gypsum lining – Application and finishing.",
            //    "Prepare and paint the affected wall and architrave, to the nearest break or joint using a similar colour and finish to the existing and in accordance with AS 2311 - Guide To The Painting Of Buildings.",
            //    "Make good all affected surfaces as part of the works, to their prior condition.",
            //};

            //foreach (string i in listInfo)
            //{
            //    iTextSharp.text.ListItem item = new iTextSharp.text.ListItem(i, new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
            //    table3List.Add(item);
            //}

            ////list in Suggested Scope of Works ends
            //PdfPCell suggestedScopeOfWorks = new PdfPCell(new Phrase("Suggested Scope of Works", 
            //    new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            //suggestedScopeOfWorks.HorizontalAlignment = Element.ALIGN_LEFT;
            //suggestedScopeOfWorks.VerticalAlignment = Element.ALIGN_MIDDLE;

            //PdfPCell suggestedScopeOfWorksList = new PdfPCell();
            //suggestedScopeOfWorksList.HorizontalAlignment = Element.ALIGN_LEFT;
            //suggestedScopeOfWorksList.Colspan = 6;
            //suggestedScopeOfWorksList.PaddingLeft = 4f;


            ////add list title
            //Phrase listTitle = new Phrase("Allow: ", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
            //Paragraph listTitleNamePah = new Paragraph();
            //string listTitleNameText = "Entity";
            //listTitleNamePah.Add(new Phrase(listTitleNameText, new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));
            ////listTitleNamePah.Add(listTitleNameText);
            //listTitleNamePah.IndentationLeft = 12f;
            //listTitleNamePah.Font.Size = 12f;

            ////Phrase listTitleName = new Phrase(listTitleNamePah, new Font(Font.UNDEFINED, 12f, Font.UNDERLINE, BaseColor.Black));

            //suggestedScopeOfWorksList.AddElement(listTitle);
            //suggestedScopeOfWorksList.AddElement(listTitleNamePah);
            //suggestedScopeOfWorksList.AddElement(table3List);

            //table3Left.AddCell(observation);
            //table3Left.AddCell(observationInfo);

            //table3Left.AddCell(cause);
            //table3Left.AddCell(causeInfo);

            //table3Left.AddCell(breach);
            //table3Left.AddCell(breachInfo);

            //table3Left.AddCell(reasonForDenial);
            //table3Left.AddCell(reasonForDenialInfo);

            //table3Left.AddCell(suggestedScopeOfWorks);
            //table3Left.AddCell(suggestedScopeOfWorksList);

            //PdfPCell table3Header = new PdfPCell(table3Left);
            //table3Header.Colspan = 7;

            ////right part of table 3 

            ////image phrase 
            //Image textImg = Image.GetInstance(Path.Combine(FileUtil.ImagePath, "test.jpg"));
            //textImg.SpacingAfter = 5f;
            //textImg.ScalePercent(40f);

            //PdfPCell table3Right = new PdfPCell();
            //table3Right.AddElement(textImg);
            //table3Right.Colspan = 2;
            //table3Right.HorizontalAlignment = Element.ALIGN_LEFT;
            ////table3Right.Rowspan = 6;

            //table3.AddCell(table3Header);
            //table3.AddCell(table3Right);

            //Doc.Add(table3);
        } 

        protected void CreateTypeThreeTable()
        {
            PdfPTable table3 = new PdfPTable(9);
            table3.TotalWidth = 820f;
            table3.LockedWidth = true;
            table3.DefaultCell.FixedHeight = 700f;

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
            crossRef.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell lossType = new PdfPCell(new Phrase("Loss type: ", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            lossType.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell lossTypeInfo = new PdfPCell(new Phrase("Minor Defect ", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            lossTypeInfo.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell completionStatus = new PdfPCell(new Phrase("Completion status:", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            completionStatus.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell completionStatusInfo = new PdfPCell(new Phrase("N/A", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            completionStatusInfo.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell recommendation = new PdfPCell(new Phrase("Recommendation:", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            recommendation.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell recommendationInfo = new PdfPCell(new Phrase("Decline:", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            recommendationInfo.HorizontalAlignment = Element.ALIGN_LEFT;

            table3Left.AddCell(crossRef);
            table3Left.AddCell(lossType);
            table3Left.AddCell(lossTypeInfo);
            table3Left.AddCell(completionStatus);
            table3Left.AddCell(completionStatusInfo);
            table3Left.AddCell(recommendation);
            table3Left.AddCell(recommendationInfo);

            //info from third line to last line 
            PdfPCell observation = new PdfPCell(new Phrase("Observation", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            observation.HorizontalAlignment = Element.ALIGN_LEFT;
            observation.VerticalAlignment = Element.ALIGN_MIDDLE;

            PdfPCell observationInfo = new PdfPCell(new Phrase(
                "The item of claim is unspecific to the precise locations of the scuff marks." + "\n" +
                "The inspection revealed several minor scuff marks to the internal walls of the residence, consistent with accidental damage." + "\n" +
                "As the residence was completed in August 2017, and occupied since, and the inspection carried out in July 2019, it cannot be confirmed who caused the damage to the internal walls. " +
                "There is insufficient evidence to confirm whether the damage to the internal walls was caused by the Builder during the construction works. " +
                "The damage may have been caused by others during occupation of the residence."
                , new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));
            observationInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            observationInfo.Colspan = 6;
            observationInfo.PaddingLeft = 4f;

            PdfPCell cause = new PdfPCell(new Phrase("Cause", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            cause.HorizontalAlignment = Element.ALIGN_LEFT;
            cause.VerticalAlignment = Element.ALIGN_MIDDLE;

            PdfPCell causeInfo = new PdfPCell(new Phrase("Minor building movement and poor fixing", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));
            causeInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            causeInfo.Colspan = 6;
            causeInfo.Padding = 4f;

            PdfPCell breach = new PdfPCell(new Phrase("Breach(es)", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            breach.HorizontalAlignment = Element.ALIGN_LEFT;
            breach.VerticalAlignment = Element.ALIGN_MIDDLE;

            Image textImg11 = Image.GetInstance(Path.Combine(FileUtil.ImagePath, "test.jpg"));
            textImg11.SpacingBefore = 3f;
            textImg11.SpacingAfter = 5f;
            textImg11.ScalePercent(40f);

            PdfPCell breachInfo = new PdfPCell();
            Phrase breachPhrase = new Phrase(
                "The NSW Office of Fair Trading - Guide to Standards and Tolerances 2017" +
                ", Clause 11.1 Gaps associated with internal fixing, " +
                "which states: “Unless documented otherwise, gaps between mouldings or between mouldings and other fixtures, " +
                "at mitre or butt joints, or at junctions with a wall or other surfaces, are defective if they exist at handover, " +
                "or exceed 1 mm in width within the first 12 months of completion and are visible from a normal viewing position. " +
                "After the first 12 months, gaps are defective if they exceed 2 mm in width and are visible from a normal viewing position. " +
                "Gaps between skirting and flooring are defective if they exceed 2 mm within the first 24 months after handover and are visible from a normal viewing position.”"
                , new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
            breachInfo.AddElement(breachPhrase);
            breachInfo.AddElement(textImg11);

            breachInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            breachInfo.Colspan = 6;
            //breachInfo.PaddingTop = 4f;
            //breachInfo.PaddingBottom = 4f;
            breachInfo.PaddingLeft = 4f;

            PdfPCell reasonForDenial = new PdfPCell(new Phrase("Reason for Denial", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            reasonForDenial.HorizontalAlignment = Element.ALIGN_LEFT;
            reasonForDenial.VerticalAlignment = Element.ALIGN_MIDDLE;

            PdfPCell reasonForDenialInfo = new PdfPCell(new Phrase("N/A", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));
            reasonForDenialInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            reasonForDenialInfo.Colspan = 6;
            reasonForDenialInfo.PaddingLeft = 4f;

            //list in Suggested Scope of Works
            List table3List = new List(List.ORDERED, 15f);
            table3List.SetListSymbol("\u2022");
            table3List.IndentationLeft = 12f;
            string[] listInfo = {
                "Re-orientate the light switch adjacent to the front door vertically to clear the architrave",
                "Replace the cut-out section or architrave with a matching profiled section.",
                "Repair the plasterboard wall lining as part of the switch re-orientation. Work to AS 2589 – Gypsum lining – Application and finishing.",
                "Prepare and paint the affected wall and architrave, to the nearest break or joint using a similar colour and finish to the existing and in accordance with AS 2311 - Guide To The Painting Of Buildings.",
                "Make good all affected surfaces as part of the works, to their prior condition.",
            };

            foreach (string i in listInfo)
            {
                iTextSharp.text.ListItem item = new iTextSharp.text.ListItem(i, new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
                table3List.Add(item);
            }

            //list in Suggested Scope of Works ends
            PdfPCell suggestedScopeOfWorks = new PdfPCell(new Phrase("Suggested Scope of Works",
                new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            suggestedScopeOfWorks.HorizontalAlignment = Element.ALIGN_LEFT;
            suggestedScopeOfWorks.VerticalAlignment = Element.ALIGN_MIDDLE;

            PdfPCell suggestedScopeOfWorksList = new PdfPCell();
            suggestedScopeOfWorksList.HorizontalAlignment = Element.ALIGN_LEFT;
            suggestedScopeOfWorksList.Colspan = 6;
            suggestedScopeOfWorksList.PaddingLeft = 4f;


            //add list title
            Phrase listTitle = new Phrase("Allow: ", new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
            Paragraph listTitleNamePah = new Paragraph();
            string listTitleNameText = "Entity";
            listTitleNamePah.Add(new Phrase(listTitleNameText, new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black)));
            //listTitleNamePah.Add(listTitleNameText);
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

            table3Left.AddCell(reasonForDenial);
            table3Left.AddCell(reasonForDenialInfo);

            table3Left.AddCell(suggestedScopeOfWorks);
            table3Left.AddCell(suggestedScopeOfWorksList);

            PdfPCell table3Header = new PdfPCell(table3Left);
            table3Header.Colspan = 7;

            //right part of table 3 

            //image phrase 
            Image textImg = Image.GetInstance(Path.Combine(FileUtil.ImagePath, "test.jpg"));
            textImg.SpacingAfter = 5f;
            textImg.ScalePercent(40f);

            PdfPCell table3Right = new PdfPCell();
            table3Right.AddElement(textImg);
            table3Right.Colspan = 2;
            table3Right.HorizontalAlignment = Element.ALIGN_LEFT;
            //table3Right.Rowspan = 6;

            table3Right.AddElement(new Phrase("this is an example", new Font(Font.UNDEFINED, 8f, Font.UNDEFINED, BaseColor.Black)));

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

        protected PdfPTable GenerateTestTable(List<InfoTableMetaData> model, string title, byte columNum = 4, float totalWidths = 0,  float[] widths = null)
        {
            // Set the title cell stylle
            PdfPCell titleCell = new PdfPCell(new Phrase(title, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)))
            {
                //BackgroundColor = BaseColor.LightGray,
                BackgroundColor = new BaseColor(0, 0, 51),
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 4f,
                PaddingBottom = 4f,
            };

            // Set the config title style
            // You can change this to null to see the difference (using the default style in base class)
            //PdfPCell cell = null;
            PdfPCell cell = new PdfPCell()
            {
                Border = Rectangle.BOX,
                PaddingTop = 4f,
                PaddingBottom = 4f,
            };

            var table = GenerateInfoTable(model, columNum, widths, headerCell: cell, contentCell: cell, titleCell: titleCell);
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            if (totalWidths <= 0)
            {
                table.TotalWidth = 820f;
            }
            else
            {
                table.TotalWidth = totalWidths;
            }
            table.SpacingAfter = 10f;
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
            //protected Phrase PDFFooter_Date;
            //protected Phrase PDFHeader_Title;


            public ENDataClassicHeader(PdfBase pdfbase) : base(pdfbase)
            {
                //PDFFooter_Date = new Phrase($"Printed On: {BaseDocument.Date}", StyleFooterAndPageNumber);
                //PDFHeader_Title = new Phrase(BaseDocument.Title, ThisDocument.StyleTiltleHeader);
            }

            public override void OnStartPage(PdfWriter writer, Document document)
            {
                //base.OnStartPage(writer, document);

                //ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT,
                //    PDFHeader_Title,
                //    ThisDocument.PageMargin.Left,
                //    ThisDocument.PageInfo.Height - 36f, 0);

                //ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_RIGHT,
                //    new Phrase("Page " + writer.PageNumber.ToString(), StyleFooterAndPageNumber),
                //    ThisDocument.PageInfo.Width - ThisDocument.PageMargin.Right,
                //    ThisDocument.PageInfo.Height - 20f, 0);

                // create header table
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.TotalWidth = 820f;
                //headerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin; //this centers [table]
                headerTable.LockedWidth = true;
                headerTable.DefaultCell.Border = Rectangle.NO_BORDER;

                //create cells 1
                PdfPCell headerFirstLineLeft = new PdfPCell(new Phrase("ACE BUILDING CONSULTANTS", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
                headerFirstLineLeft.HorizontalAlignment = Element.ALIGN_LEFT;
                headerFirstLineLeft.Border = Rectangle.NO_BORDER;
                PdfPCell headerFirstLineRight = new PdfPCell(new Phrase("Prepared by:", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
                headerFirstLineRight.HorizontalAlignment = Element.ALIGN_RIGHT;
                headerFirstLineRight.Border = Rectangle.NO_BORDER;
                //create cells 2
                PdfPCell headerSecondLineLeft = new PdfPCell(new Phrase("147 Burly St" + "\n" + "Wyong, NSW 2772", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
                headerSecondLineLeft.HorizontalAlignment = Element.ALIGN_LEFT;
                headerSecondLineLeft.Border = Rectangle.NO_BORDER;
                PdfPCell headerSecondLineRight = new PdfPCell(new Phrase("The Building Consultant" + "\n" + "040404040", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
                headerSecondLineRight.HorizontalAlignment = Element.ALIGN_RIGHT;
                headerSecondLineRight.Border = Rectangle.NO_BORDER;

                headerTable.AddCell(headerFirstLineLeft);
                headerTable.AddCell(headerFirstLineRight);
                headerTable.AddCell(headerSecondLineLeft);
                headerTable.AddCell(headerSecondLineRight);

                headerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height -25, writer.DirectContent);

            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);

                //ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_CENTER,
                //    PDFFooter_Date,
                //    ThisDocument.PageMargin.Left,
                //    10f, 0);
                //ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_RIGHT,
                //    new Phrase("Page " + writer.PageNumber.ToString(), StyleFooterAndPageNumber),
                //    ThisDocument.PageInfo.Width - ThisDocument.PageMargin.Right,
                //    ThisDocument.PageInfo.Height - 20f, 0);

                PdfPTable footerTable = new PdfPTable(1);
                footerTable.TotalWidth = 820f;
                footerTable.LockedWidth = true;
                footerTable.DefaultCell.Border = Rectangle.NO_BORDER;
                PdfPCell footerPageNum = new PdfPCell(new Phrase("Page " + writer.PageNumber.ToString(),
                    //new Font(Font.UNDEFINED, 8f, Font.UNDEFINED, BaseColor.Black)
                    StyleFooterAndPageNumber
                    ));
                footerPageNum.HorizontalAlignment = Element.ALIGN_CENTER;
                footerPageNum.Border = Rectangle.NO_BORDER;
                footerTable.AddCell(footerPageNum);

                footerTable.WriteSelectedRows(0, -1, 10, 20f, writer.DirectContent);
            }

            static readonly Font StyleFooterAndPageNumber = FontFactory.GetFont(BaseFont.HELVETICA, 10f);
        }

        //Define the style
        protected override BaseColor ThemeTableHeadShading => BaseColor.White;

        protected override Font StyleTiltleHeader => FontFactory.GetFont(BaseFont.HELVETICA, 23, Font.BOLD);
        protected override Font StyleHeader => FontFactory.GetFont(BaseFont.HELVETICA, 18, Font.BOLD);
        protected override Font StyleContent => FontFactory.GetFont(BaseFont.HELVETICA, 12);
        protected override Font StyleContentHeader => FontFactory.GetFont(BaseFont.HELVETICA, 12, Font.BOLD, ThemePrimary);
        protected override Font StyleContentBold => FontFactory.GetFont(BaseFont.HELVETICA, 12, Font.BOLD);
        protected override Font StyleContentSmall => FontFactory.GetFont(BaseFont.HELVETICA, 9);
        protected override Font StyleContentSmallBold => FontFactory.GetFont(BaseFont.HELVETICA, 10, Font.BOLD);
        protected override Font StyleAudit => FontFactory.GetFont(BaseFont.HELVETICA, 12, ThemeAudit);
        protected override Font StyleAuditBold => FontFactory.GetFont(BaseFont.HELVETICA, 12, Font.BOLD, ThemeAudit);
        protected override Font StyleAuditSmall => FontFactory.GetFont(BaseFont.HELVETICA, 10, ThemeAudit);
        protected override Font StyleAuditSmallBold => FontFactory.GetFont(BaseFont.HELVETICA, 10, Font.BOLD, ThemeAudit);


        static Dictionary<string, FormComponentType> cardFormDict = new Dictionary<string, FormComponentType>
        {
            { "Cover Page", FormComponentType.CoverPage }
        };

        enum FormComponentType
        {
            CoverPage,
            NormalTable,
            ComplexTable,
            TableAndPicture
        }
    }
}
