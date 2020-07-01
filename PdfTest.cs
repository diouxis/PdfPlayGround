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
    using Org.BouncyCastle.Asn1.Cmp;
    using System.Linq;

    public class PdfTest : PdfBase
    {
        protected readonly ClaimJob Source;
        protected List<InfoTableMetaData> ClaimContent = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> JobDetailContent = new List<InfoTableMetaData>();

        //Page One 
        //protected List<InfoTableMetaData> JobDetailsTable = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> IntroductionTable = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> BuildingConsultantDetailTable = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> BuildingDescriptionTable = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> ProjectContractDetailTable = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> ClaimDetailTable = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> ProfessionalSerEngageTable = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> DocumentPreparationTable = new List<InfoTableMetaData>();
        protected List<InfoTableMetaData> ReferenceCurriVitaeTable = new List<InfoTableMetaData>();

        private Card CoverPageCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Cover Page");
        //private Card JobDetailCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Job Details");
        private Card BuildingConDetailCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Building Consultant's Details");
        private Card BuildingDescriptionCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Building Description");
        private Card ProjectContractDetailCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Project and Contact Details");
        private Card ClaimDetailCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Claim Details");
        private Card IntroductionCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Introduction");
        private Card DutyToTribunalCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Duty To The Tribunal");
        private Card InstructionCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Instructions from Gallagher Bassett");
        private Card AreaBCACard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Areas of Non-Compliance to BCA(Building Code of Australia)");
        private Card ProfessionalSerEngageCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Professional Services Engaged by Claimant");
        private Card DocumentPreparationCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Documents relied on in the preparation of this report");
        private Card OutstandingManCerRequirementCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Outstanding Mandatory Certification Requirements");
        private Card OpinionRelationSectionCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Opinion in relation to Section 95(2) Report");
        private Card ScheduleItemCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Schedule of Items");
        private Card ReferenceCurriVitaeCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "References and Curriculum Vitae");
        private Card SupplementaryCommInConfiReportCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Supplementary Commercial In-Confidence Report");


        protected List<InfoTableMetaData> InitialTables(List<InfoTableMetaData> table, Card card)
        {
            table = new List<InfoTableMetaData> { };
            for (int i = 0; i < card.Fields.Count(); i++)
            {
                table.Add(new InfoTableMetaData(card.Fields[i].Label, card.Fields[i].Value?.ToString()));
            }
            return table;
        }

        protected List<InfoTableMetaData> InitialTableForProfessionalServicesEngaged(Card card)
        {
            var table = new List<InfoTableMetaData> { };
            var fields = card.Fields.FirstOrDefault().Value as List<List<Field>>;
            if (fields != null)
            {
                foreach (var field in fields)
                {
                    for (int i = 0; i < field.Count; i += 2)
                    {
                        table.Add(new InfoTableMetaData(field[i].Value?.ToString(), field[i + 1].Value?.ToString()));
                    }
                }
            }
            return table;
        }

        //
        protected List<InfoTableMetaData> InitialTableDocumentReliedOnTable(Card card)
        {
            var table = new List<InfoTableMetaData> { };
            if (card.Fields.FirstOrDefault().Value is List<List<Field>> fields)
            {
                foreach (var field in fields.Select(x => x.FirstOrDefault()))
                {
                    if (field?.Value != null)
                    {
                        var metaData = new InfoTableMetaData(field.Name, field.Value.ToString(), colLable: 0);
                        table.Add(metaData);
                    }
                }
            }
            return table;
        }

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

            JobDetailContent = new List<InfoTableMetaData>
            {
                new InfoTableMetaData("Building Owner:", Source.Insured.Name),
                new InfoTableMetaData("Address:", Source.Insured.Name),
                new InfoTableMetaData("Our Reference:", BuildingConDetailCard.Fields[0].Value.ToString()),
                new InfoTableMetaData("Client Reference:", Source.RefNumber)
            };

            foreach (var card in Source.ReportForm.Cards)
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
            };

            if (IntroductionCard != null)
            {
                IntroductionTable = InitialTables(IntroductionTable, IntroductionCard);
            }

            if (BuildingConDetailCard != null)
            {
                BuildingConsultantDetailTable = InitialTables(BuildingConsultantDetailTable, BuildingConDetailCard);
            }

            if (BuildingDescriptionCard != null)
            {
                BuildingDescriptionTable = InitialTables(BuildingDescriptionTable, BuildingDescriptionCard);
            }

            if (ProjectContractDetailCard != null)
            {
                ProjectContractDetailTable = InitialTables(ProjectContractDetailTable, ProjectContractDetailCard);
            }

            if (ClaimDetailCard != null)
            {
                ClaimDetailTable = InitialTables(ClaimDetailTable, ClaimDetailCard);
            }

            if (ProfessionalSerEngageCard != null)
            {
                ProfessionalSerEngageTable = InitialTableForProfessionalServicesEngaged(ProfessionalSerEngageCard);
            }

            if (DocumentPreparationCard != null)
            {
                DocumentPreparationTable = InitialTableDocumentReliedOnTable(DocumentPreparationCard);
            }

            if (ReferenceCurriVitaeCard != null)
            {
                ReferenceCurriVitaeTable = InitialTables(ReferenceCurriVitaeTable, ReferenceCurriVitaeCard);
            }

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
                firstPageTableHeader.PaddingTop = 4f;
                firstPageTableHeader.PaddingBottom = 4f;
                firstPageTableHeader.BackgroundColor = new BaseColor(0, 0, 51);
                firstPageTableHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                firstPageTableHeader.Border = Rectangle.NO_BORDER;

                PdfPCell firstPageTableBody = new PdfPCell(
                    new Phrase(CoverPageCard.Fields[1].Value.ToString(),
                    new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black)));
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

            //set following to the new page
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!page two!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//

            //***********************first part************************//

            //put two table into one line
            if (JobDetailContent != null && IntroductionTable != null)
            {
                PdfPTable twoTable = new PdfPTable(2);
                twoTable.TotalWidth = 820f;
                twoTable.LockedWidth = true;

                PdfPCell twoTableLeft = new PdfPCell(GenerateTestTable(JobDetailContent, "JOB DETAILS", 2, 406f));
                twoTableLeft.Colspan = 1;
                twoTableLeft.Padding = 0;
                twoTableLeft.Border = Rectangle.NO_BORDER;

                //twoTableLeft.PaddingRight = 5f;

                //PdfPCell twoTableRight = new PdfPCell(introTable);
                //twoTableRight.Colspan = 1;
                //twoTableRight.Padding = 0;
                //twoTableRight.Border = Rectangle.NO_BORDER;

                PdfPCell twoTableRight = new PdfPCell(GenerateTestTable(IntroductionTable, "Introduction", 2, 406f));
                twoTableRight.Colspan = 1;
                twoTableRight.Padding = 0;
                twoTableRight.Border = Rectangle.NO_BORDER;



                //twoTableLeft.PaddingLeft = 5f;
                twoTable.AddCell(twoTableLeft);
                //twoTable.AddCell("");
                twoTable.AddCell(twoTableRight);

                Doc.Add(twoTable);
            }

            //***********************second part************************//
            //BuildingConsultantDetailTable
            if (BuildingConsultantDetailTable != null)
            {
                PdfPTable bcdTable = new PdfPTable(1);
                bcdTable.SpacingBefore = 10f;
                bcdTable.DefaultCell.Border = Rectangle.NO_BORDER;

                PdfPCell BuildingConsultantDetail = new PdfPCell(GenerateTestTable(BuildingConsultantDetailTable, "BUILDING CONSULTANT’S DETAILS", 4, 820f));
                BuildingConsultantDetail.HorizontalAlignment = Element.ALIGN_CENTER;
                BuildingConsultantDetail.VerticalAlignment = Element.ALIGN_MIDDLE;
                BuildingConsultantDetail.Border = 0;
                bcdTable.AddCell(BuildingConsultantDetail);

                Doc.Add(bcdTable);
            }

            //***********************Third part************************//
            //BuildingDescriptionTable
            if (BuildingDescriptionTable != null)
            {
                PdfPTable bdTable = new PdfPTable(1);
                bdTable.SpacingBefore = 10f;
                bdTable.DefaultCell.Border = Rectangle.NO_BORDER;

                PdfPCell BuildingDescription = new PdfPCell(GenerateTestTable(BuildingDescriptionTable, "BUILDING DESCRIPTION", 4, 820f));
                BuildingDescription.HorizontalAlignment = Element.ALIGN_CENTER;
                BuildingDescription.VerticalAlignment = Element.ALIGN_MIDDLE;
                BuildingDescription.Border = 0;
                bdTable.AddCell(BuildingDescription);

                Doc.Add(bdTable);
            }
            
            //***********************Forth part************************//
            //ProjectContractDetailTable
            if (ProjectContractDetailTable != null)
            {
                PdfPTable rcdTable = new PdfPTable(1);
                rcdTable.SpacingBefore = 10f;
                rcdTable.DefaultCell.Border = Rectangle.NO_BORDER;

                PdfPCell ProjectContractDetail = new PdfPCell(GenerateTestTable(ProjectContractDetailTable, "PROJECT & CONTRACT DETAILS", 4, 820f));
                ProjectContractDetail.HorizontalAlignment = Element.ALIGN_CENTER;
                ProjectContractDetail.VerticalAlignment = Element.ALIGN_MIDDLE;
                ProjectContractDetail.Border = 0;
                rcdTable.AddCell(ProjectContractDetail);


                Doc.Add(rcdTable);
            }

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!page Three!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
            //**********************first table in page three************************//
            //ClaimDetailTable
            if (ClaimDetailTable != null)
            {
                PdfPTable cdTable = new PdfPTable(1);
                cdTable.DefaultCell.Border = Rectangle.NO_BORDER;

                PdfPCell ClaimDetail = new PdfPCell(GenerateTestTable(ClaimDetailTable, "CLAIM DETAILS", 2, 820f));
                ClaimDetail.HorizontalAlignment = Element.ALIGN_CENTER;
                ClaimDetail.VerticalAlignment = Element.ALIGN_MIDDLE;
                ClaimDetail.Border = 0;
                cdTable.AddCell(ClaimDetail);

                Doc.Add(cdTable);
            }

            //**********************Second table********************//
            //Duty to the Tribunal Table 
            if (DutyToTribunalCard != null)
            {
                PdfPTable dutyTable = new PdfPTable(1);
                dutyTable.TotalWidth = 820f;
                dutyTable.LockedWidth = true;
                dutyTable.SpacingBefore = 10f;

                PdfPCell dutyTableHeader = new PdfPCell(new Phrase(DutyToTribunalCard.Title, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
                dutyTableHeader.BackgroundColor = new BaseColor(0, 0, 51);
                dutyTableHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                dutyTableHeader.PaddingTop = 4f;
                dutyTableHeader.PaddingBottom = 4f;
                dutyTable.AddCell(dutyTableHeader);

                PdfPCell dutyTableInfo = new PdfPCell(new Phrase(DutyToTribunalCard.Fields[0].Value.ToString(), new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black)));
                dutyTableInfo.PaddingTop = 4f;
                dutyTableInfo.PaddingBottom = 4f;
                dutyTable.AddCell(dutyTableInfo);

                Doc.Add(dutyTable);
            }

            //instruction table
            if (InstructionCard != null)
            {
                PdfPTable instructionTable = new PdfPTable(1);
                instructionTable.TotalWidth = 820f;
                instructionTable.LockedWidth = true;
                instructionTable.SpacingBefore = 10f;

                PdfPCell instructionHeader = new PdfPCell(new Phrase(InstructionCard.Title, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
                instructionHeader.BackgroundColor = new BaseColor(0, 0, 51);
                instructionHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                instructionHeader.PaddingTop = 4f;
                instructionHeader.PaddingBottom = 4f;
                instructionTable.AddCell(instructionHeader);

                PdfPCell instructionInfo = new PdfPCell(new Phrase(InstructionCard.Fields[0].Value.ToString(), new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black)));
                instructionInfo.PaddingTop = 4f;
                instructionInfo.PaddingBottom = 4f;
                instructionTable.AddCell(instructionInfo);

                Doc.Add(instructionTable);
            }

            //Professional Services Engaged by Claimant Table 
            if (ProfessionalSerEngageCard != null)
            {
                PdfPTable pscTable = new PdfPTable(1);
                pscTable.SpacingBefore = 10f;
                pscTable.DefaultCell.Border = Rectangle.NO_BORDER;

                PdfPCell ProfessionalSerEngageDetail = new PdfPCell(GenerateTestTable(ProfessionalSerEngageTable, "Professional Services Engaged by Claimant", 4, 820f));
                ProfessionalSerEngageDetail.HorizontalAlignment = Element.ALIGN_CENTER;
                ProfessionalSerEngageDetail.VerticalAlignment = Element.ALIGN_MIDDLE;
                ProfessionalSerEngageDetail.Border = 0;
                pscTable.AddCell(ProfessionalSerEngageDetail);

                Doc.Add(pscTable);
            }

            //Areas of Non-Compliance to BCA Table
            if (AreaBCACard != null)
            {
                PdfPTable AreaBCATable = new PdfPTable(1);
                AreaBCATable.TotalWidth = 820f;
                AreaBCATable.LockedWidth = true;
                AreaBCATable.SpacingBefore = 10f;

                PdfPCell AreaBCAHeader = new PdfPCell(new Phrase(AreaBCACard.Title, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
                AreaBCAHeader.BackgroundColor = new BaseColor(0, 0, 51);
                AreaBCAHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                AreaBCAHeader.PaddingTop = 4f;
                AreaBCAHeader.PaddingBottom = 4f;
                AreaBCATable.AddCell(AreaBCAHeader);

                PdfPCell AreaBCAInfo = new PdfPCell(new Phrase(AreaBCACard.Fields[0].Value.ToString(), new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black)));
                AreaBCAInfo.PaddingTop = 4f;
                AreaBCAInfo.PaddingBottom = 4f;
                AreaBCATable.AddCell(AreaBCAInfo);

                Doc.Add(AreaBCATable);
            }

            if (OutstandingManCerRequirementCard != null)
            {
                PdfPTable OutstandingManCerRequirementTable = new PdfPTable(1);
                OutstandingManCerRequirementTable.TotalWidth = 820f;
                OutstandingManCerRequirementTable.LockedWidth = true;
                OutstandingManCerRequirementTable.SpacingBefore = 10f;

                PdfPCell OutstandingManCerRequirementHeader = new PdfPCell(new Phrase(OutstandingManCerRequirementCard.Title, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
                OutstandingManCerRequirementHeader.BackgroundColor = new BaseColor(0, 0, 51);
                OutstandingManCerRequirementHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                OutstandingManCerRequirementHeader.PaddingTop = 4f;
                OutstandingManCerRequirementHeader.PaddingBottom = 4f;
                OutstandingManCerRequirementTable.AddCell(OutstandingManCerRequirementHeader);

                PdfPCell OutstandingManCerRequirementInfo = new PdfPCell(new Phrase(OutstandingManCerRequirementCard.Fields[0].Value.ToString(), new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black)));
                OutstandingManCerRequirementInfo.PaddingTop = 4f;
                OutstandingManCerRequirementInfo.PaddingBottom = 4f;
                OutstandingManCerRequirementTable.AddCell(OutstandingManCerRequirementInfo);

                Doc.Add(OutstandingManCerRequirementTable);
            }

            if (DocumentPreparationCard != null)
            {
                PdfPTable dpTable = new PdfPTable(1);
                dpTable.DefaultCell.Border = Rectangle.NO_BORDER;
                dpTable.SpacingBefore = 10f;

                PdfPCell DpDetail = new PdfPCell(GenerateTestTable(DocumentPreparationTable, "Documents relied on in the preparation of this report", 2, 820f));
                DpDetail.HorizontalAlignment = Element.ALIGN_CENTER;
                DpDetail.VerticalAlignment = Element.ALIGN_MIDDLE;
                DpDetail.Border = 0;
                dpTable.AddCell(DpDetail);

                Doc.Add(dpTable);
            }

            if (OpinionRelationSectionCard != null)
            {
                PdfPTable OpinionRelationSectionTable = new PdfPTable(1);
                OpinionRelationSectionTable.TotalWidth = 820f;
                OpinionRelationSectionTable.LockedWidth = true;
                OpinionRelationSectionTable.SpacingBefore = 10f;

                PdfPCell OpinionRelationSectionHeader = new PdfPCell(new Phrase(OpinionRelationSectionCard.Title, new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
                OpinionRelationSectionHeader.BackgroundColor = new BaseColor(0, 0, 51);
                OpinionRelationSectionHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                OpinionRelationSectionHeader.PaddingTop = 4f;
                OpinionRelationSectionHeader.PaddingBottom = 4f;
                OpinionRelationSectionTable.AddCell(OpinionRelationSectionHeader);

                PdfPCell OpinionRelationSectionInfo = new PdfPCell(new Phrase(OpinionRelationSectionCard.Fields[0].Value.ToString(), new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black)));
                OpinionRelationSectionInfo.PaddingTop = 4f;
                OpinionRelationSectionInfo.PaddingBottom = 4f;
                OpinionRelationSectionTable.AddCell(OpinionRelationSectionInfo);

                Doc.Add(OpinionRelationSectionTable);
            }

            if (ReferenceCurriVitaeCard != null)
            {
                PdfPTable rcvTable = new PdfPTable(1);
                rcvTable.DefaultCell.Border = Rectangle.NO_BORDER;
                rcvTable.SpacingBefore = 10f;

                PdfPCell ReferenceDetail = new PdfPCell(GenerateTestTable(ReferenceCurriVitaeTable, "Reference", 2, 820f));
                ReferenceDetail.HorizontalAlignment = Element.ALIGN_CENTER;
                ReferenceDetail.VerticalAlignment = Element.ALIGN_MIDDLE;
                ReferenceDetail.Border = 0;
                rcvTable.AddCell(ReferenceDetail);

                Doc.Add(rcvTable);
            }

            Doc.NewPage();

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!page Four!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
            //Doc.NewPage();
            if (ScheduleItemCard != null)
            {
                var fields = ScheduleItemCard.Fields.FirstOrDefault().Value as List<List<Field>>;
                foreach (var field in fields)
                {
                    var itemNumValue = field.FirstOrDefault(x => x.Label == "Item Number");
                    var descValue = field.FirstOrDefault(x => x.Label == "Description");
                    var locationValue = field.FirstOrDefault(x => x.Label == "Location");
                    var crossRefValue = field.FirstOrDefault(x => x.Label == "Cross Ref");
                    var estimateCostValue = field.FirstOrDefault(x => x.Label == "Estimate Cost");
                    var lossTypeValue = field.FirstOrDefault(x => x.Label == "Loss Type");
                    var compStatusValue = field.FirstOrDefault(x => x.Label == "Completion Status");
                    var recomValue = field.FirstOrDefault(x => x.Label == "Recommendation");
                    var ReasonDenValue = field.FirstOrDefault(x => x.Label == "Reason for Denial");
                    var itemPhotos = field.FirstOrDefault(x => x.Label == "Item Photo(s)");
                    var observationValue = field.FirstOrDefault(x => x.Label == "Observation");
                    var causeValue = field.FirstOrDefault(x => x.Label == "Cause");
                    var breachValue = field.FirstOrDefault(x => x.Label == "Breach(es)");
                    var suggestScopeValue = field.FirstOrDefault(x => x.Label == "Suggested Scope of Works");

                    ItemReporData testData = new ItemReporData("ITEM " + itemNumValue.Value.ToString());
                    testData.TitleFields.Add(new InfoTableMetaData("Description:", descValue.Value.ToString(), colLable: 2, colData: 4));
                    testData.TitleFields.Add(new InfoTableMetaData("Location:", locationValue.Value.ToString(), colData: 1));

                    testData.Fields.Add(new InfoTableMetaData("Cross Ref:", crossRefValue.Value.ToString(), colData: 0));
                    testData.Fields.Add(new InfoTableMetaData("Loss type:", lossTypeValue.Value.ToString(), colData: 2));
                    testData.Fields.Add(new InfoTableMetaData("Completion status:", compStatusValue.Value.ToString(), colData: 1));
                    testData.Fields.Add(new InfoTableMetaData("Recommendation:", recomValue.Value.ToString(), colLable: 2, colData: 1));

                    testData.Rows.Add(new InfoTableMetaData("Observation", observationValue.Value.ToString()));
                    testData.Rows.Add(new InfoTableMetaData("Cause", causeValue.Value.ToString()));
                    testData.Rows.Add(new InfoTableMetaData("Breach(es)", breachValue.Value.ToString()));
                    testData.Rows.Add(new InfoTableMetaData("Reason for Denial", ReasonDenValue.Value.ToString()));
                    testData.Rows.Add(new InfoTableMetaData("Suggested Scope of Works", suggestScopeValue.Value.ToString()));

                    testData.Images.Add(new ReportFile
                    {
                        Name = "this is an example",
                        Url = Path.Combine(FileUtil.ImagePath, "test.jpg")
                    });
                    testData.Images.Add(new ReportFile
                    {
                        Name = "this is an example",
                        Url = Path.Combine(FileUtil.ImagePath, "test.jpg")
                    });

                    Doc.Add(GenerateItemReportTable(testData, dataColNumber: 9));

                    Doc.NewPage();
                    //CreateTypeThreeTable();
                }
            }

            
        }

        protected PdfPTable GenerateItemReportTable(ItemReporData data, int dataColNumber)
        {
            PdfPTable itemReportTable = new PdfPTable(2)
            {
                TotalWidth = PageContentWidth,
                LockedWidth = true
            };
            PdfPTable dataTable = new PdfPTable(dataColNumber);
            PdfPTable imgTable = new PdfPTable(1);

            //dataTable.DefaultCell.FixedHeight = 700f;
            foreach (var titleFields in data.TitleFields) SetField(titleFields, isTitle: true);
            foreach (var field in data.Fields) SetField(field, isTitle: false);
            foreach (var row in data.Rows) SetRow(row);
            foreach (var file in data.Images) SetImage(file);

            itemReportTable.SetWidths(new float[] { 8f, 2f });
            itemReportTable.AddCell(dataTable.GenNestedCell());
            itemReportTable.AddCell(imgTable.GenNestedCell());

            return itemReportTable;

            void SetField(InfoTableMetaData metaData, bool isTitle)
            {
                BaseColor backGround = isTitle ? ThemeTableHeadShading : BaseColor.White;
                Font styleHeader = isTitle ? StyleTiltleHeader : StyleContentHeader;
                Font styleContent = isTitle ? StyleTiltleContent : StyleContent;

                PdfPCell titleHeaderCell = new PdfPCell(new Phrase(metaData.Name, styleHeader))
                {
                    BackgroundColor = backGround,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = metaData.ColspanLabel,
                    Padding = 5f
                };
                dataTable.AddCell(titleHeaderCell);
                if (metaData.ColspanContent > 0)
                {
                    PdfPCell titleContentCell = new PdfPCell(new Phrase(metaData.Content, styleContent))
                    {
                        BackgroundColor = backGround,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        Colspan = metaData.ColspanContent,
                        Padding = 5f
                    };
                    dataTable.AddCell(titleContentCell);
                }
                else if (!string.IsNullOrEmpty(metaData.Content))
                {
                    titleHeaderCell.Phrase.Add(new Phrase(Environment.NewLine + metaData.Content, styleContent));
                }
            }

            void SetRow(InfoTableMetaData metaData)
            {
                PdfPCell rowHeaderCell = new PdfPCell(new Phrase(metaData.Name, StyleContentHeader))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5f
                };
                dataTable.AddCell(rowHeaderCell);
                PdfPCell rowContentCell = new PdfPCell(new Phrase(metaData.Content, StyleContent))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Colspan = dataColNumber - 1,
                    Padding = 5f
                };
                dataTable.AddCell(rowContentCell);
            }

            void SetImage(ReportFile file)
            {
                PdfPCell imgCell = new PdfPCell
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    PaddingBottom = 8f
                };
                imgCell.AddElement(Image.GetInstance(file.Url));
                imgCell.AddElement(new Phrase(file.Name, StyleContentSmall));
                imgTable.AddCell(imgCell);
            }
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
        protected override BaseColor ThemeTableHeadShading => new BaseColor(0, 0, 51);

        protected virtual Font StyleTiltleContent => FontFactory.GetFont(BaseFont.HELVETICA, 11, Font.NORMAL, BaseColor.White);
        protected override Font StyleTiltleHeader => FontFactory.GetFont(BaseFont.HELVETICA, 11, Font.BOLD, BaseColor.White);
        protected override Font StyleHeader => FontFactory.GetFont(BaseFont.HELVETICA, 18, Font.BOLD);
        protected override Font StyleContent => FontFactory.GetFont(BaseFont.HELVETICA, 11);
        protected override Font StyleContentHeader => FontFactory.GetFont(BaseFont.HELVETICA, 11, Font.BOLD, ThemePrimary);
        protected override Font StyleContentBold => FontFactory.GetFont(BaseFont.HELVETICA, 11, Font.BOLD);
        protected override Font StyleContentSmall => FontFactory.GetFont(BaseFont.HELVETICA, 8);
        protected override Font StyleContentSmallBold => FontFactory.GetFont(BaseFont.HELVETICA, 10, Font.BOLD);
        protected override Font StyleAudit => FontFactory.GetFont(BaseFont.HELVETICA, 12, ThemeAudit);
        protected override Font StyleAuditBold => FontFactory.GetFont(BaseFont.HELVETICA, 12, Font.BOLD, ThemeAudit);
        protected override Font StyleAuditSmall => FontFactory.GetFont(BaseFont.HELVETICA, 10, ThemeAudit);
        protected override Font StyleAuditSmallBold => FontFactory.GetFont(BaseFont.HELVETICA, 10, Font.BOLD, ThemeAudit);


        static Dictionary<string, FormComponentType> cardFormDict = new Dictionary<string, FormComponentType>
        {
            { "Cover Page", FormComponentType.CoverPage },
            { "Job Details", FormComponentType.DoubleColTable},
            { "Introduction", FormComponentType.DoubleColTable},
            { "Building Consultant's Details", FormComponentType.FourColTable},
            { "Project and Contact Details", FormComponentType.FourColTable},
            { "Building Description", FormComponentType.FourColTable},
            { "Claim Details", FormComponentType.DoubleColTable},
            { "Instructions from Gallagher Bassett", FormComponentType.SingleColTable},
            { "Areas of Non-Compliance to BCA(Building Code of Australia)", FormComponentType.SingleColTable},
            { "Professional Services Engaged by Claimant", FormComponentType.FourColTable},
            { "Documents relied on in the preparation of this report", FormComponentType.FourColTable},
            { "Outstanding Mandatory Certification Requirements", FormComponentType.SingleColTable},
            { "Opinion in relation to Section 95(2) Report", FormComponentType.SingleColTable},
            { "Schedule of Items", FormComponentType.TableWithPicture},

        };

        enum FormComponentType
        {
            CoverPage,
            SingleColTable,
            DoubleColTable,
            FourColTable,
            TableWithPicture
        }

        protected struct ItemReporData
        {
            public List<InfoTableMetaData> TitleFields { get; set; }
            public List<InfoTableMetaData> Fields { get; set; }
            public List<InfoTableMetaData> Rows { get; set; }
            public List<ReportFile> Images { get; set; }

            public ItemReporData(string name)
            {
                TitleFields = new List<InfoTableMetaData>()
                {
                    new InfoTableMetaData(name, null, colData: 0)
                };
                Fields = new List<InfoTableMetaData>();
                Rows = new List<InfoTableMetaData>();
                Images = new List<ReportFile>();
            }
        }
    }
}
