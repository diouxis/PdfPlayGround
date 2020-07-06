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
        protected readonly ClaimJobReportForm Source;
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
        protected List<InfoTableMetaData> SupplementaryCommInConfiReportTable = new List<InfoTableMetaData>();

        private Card CoverPageCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Cover Page");
        //private Card JobDetailCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Job Details");
        private Card BuildingConDetailCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Building Consultant's Details");
        private Card BuildingDescriptionCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Building Description");
        private Card ProjectContractDetailCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Project and Contact Details");
        private Card ClaimDetailCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Claim Details");
        private Card IntroductionCard => Source.ReportForm.Cards.FirstOrDefault(x => x.Title == "Introduction");
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

        protected List<InfoTableMetaData> InitialTableReferenceTable(Card card)
        {
            var table = new List<InfoTableMetaData> { };
            if (card.Fields[1].Value is List<List<Field>> fields && card.Fields[1].Value != null)
            {
                foreach (var field in fields.Select(x => x.FirstOrDefault()))
                {
                    var metaData = new InfoTableMetaData(field.Name, field.Value.ToString(), colLable: 0);
                    table.Add(metaData);
                }
            }
            return table;
        }

        public PdfTest(ClaimJobReportForm claimJob)
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
                new InfoTableMetaData("Address:", " "),
                new InfoTableMetaData("Client", Source.Insurer.CompanyName),
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
                ReferenceCurriVitaeTable = InitialTableReferenceTable(ReferenceCurriVitaeCard);
            }

            if (SupplementaryCommInConfiReportCard != null)
            {
                SupplementaryCommInConfiReportTable = InitialTables(SupplementaryCommInConfiReportTable, SupplementaryCommInConfiReportCard);

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

            Phrase infoList = new Phrase("Attention: " + Source.Insured.Name + "\n" + 
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
                    firstPageTableImgInfo.PaddingTop = 10f;
                    //foreach (var imgUrl in coverImages)
                    //{
                    //    var coverImg = Image.GetInstance(new Uri(imgUrl.Url));
                    //    coverImg.ScalePercent(30f);
                    //    coverImg.Alignment = Element.ALIGN_CENTER;
                    //    firstPageTableImgInfo.AddElement(coverImg);
                    //}
                    var coverImg = Image.GetInstance(new Uri(coverImages.FirstOrDefault().Url));
                    var imgName = coverImages.FirstOrDefault().Url.ToString();
                    coverImg.ScalePercent(30f);
                    coverImg.Alignment = Element.ALIGN_CENTER;
                    firstPageTableImgInfo.AddElement(coverImg);

                    firstPageTable.AddCell(firstPageTableImgInfo);
                }

                Doc.Add(firstPageTable);
                Doc.NewPage();

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
            if (AreaBCACard != null)
            {
                PdfPTable dutyTable = new PdfPTable(1);
                dutyTable.TotalWidth = 820f;
                dutyTable.LockedWidth = true;
                dutyTable.SpacingBefore = 10f;

                PdfPCell dutyTableHeader = new PdfPCell(new Phrase("DUTY TO THE TRIBUNAL", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
                dutyTableHeader.BackgroundColor = new BaseColor(0, 0, 51);
                dutyTableHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                dutyTableHeader.PaddingTop = 4f;
                dutyTableHeader.PaddingBottom = 4f;
                dutyTable.AddCell(dutyTableHeader);

                PdfPCell dutyTableInfo = new PdfPCell(new Phrase(AreaBCACard.Fields[1].Value.ToString(), new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black)));
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

            //if (SupplementaryCommInConfiReportCard != null)
            //{
            //    PdfPTable sccTable = new PdfPTable(1);
            //    sccTable.SpacingBefore = 10f;
            //    sccTable.DefaultCell.Border = Rectangle.NO_BORDER;

            //    PdfPCell SupplementaryCommInConfiReportDetail = new PdfPCell(GenerateTestTable(SupplementaryCommInConfiReportTable, "Supplementary Commercial In-Confidence Report", 4, 820f));
            //    SupplementaryCommInConfiReportDetail.HorizontalAlignment = Element.ALIGN_CENTER;
            //    SupplementaryCommInConfiReportDetail.VerticalAlignment = Element.ALIGN_MIDDLE;
            //    SupplementaryCommInConfiReportDetail.Border = 0;
            //    sccTable.AddCell(SupplementaryCommInConfiReportDetail);

            //    Doc.Add(sccTable);
            //}

            createPhotoAndScheduleTable();

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

            Doc.NewPage();

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!page Four!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
            //Doc.NewPage();
            if (ScheduleItemCard != null)
            {
                var fields = ScheduleItemCard.Fields.FirstOrDefault().Value as List<List<Field>>;

                Doc.Add(new Phrase("Schedule of Items – Recommended for ACCEPTANCE of DEFECTIVE WORK", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
                foreach (var field in fields)
                {
                    if (field.FirstOrDefault(x => x.Label == "Recommendation").Value.ToString() == "Accept")
                    {
                        CreateScheduleItem(field);
                        Doc.NewPage();
                    }
                }

                Doc.Add(new Phrase("Schedule of Items – Recommended for DENIAL oF DEFECTIVE WORK", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
                foreach (var field in fields)
                {
                    if (field.FirstOrDefault(x => x.Label == "Recommendation").Value.ToString() == "Decline")
                    {
                        CreateScheduleItem(field);
                        Doc.NewPage();
                    }
                }
            }

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!finish the pdf!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //report summary
            if (JobDetailContent != null)
            {
                Paragraph reportTitleCell = new Paragraph("REPORT SUMMARY", new Font(Font.BOLD, 14f, Font.BOLD, BaseColor.Black));
                Doc.Add(reportTitleCell);
                Paragraph reportTitleTime = new Paragraph("Completed on the date of " + 
                    DateTime.Now.Day.ToString() + " " + month + " " + DateTime.Now.Year.ToString(), 
                    new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black));
                reportTitleTime.Alignment = Element.ALIGN_CENTER;
                Doc.Add(reportTitleTime);
                PdfPTable ReportSummaryTable = new PdfPTable(2);
                ReportSummaryTable.SpacingBefore = 10f;
                ReportSummaryTable.SpacingAfter = 15f;
                ReportSummaryTable.TotalWidth = 700f;
                ReportSummaryTable.LockedWidth = true;
                var buildingOwner = JobDetailContent.FirstOrDefault(x => x.Name == "Building Owner:");
                var lossAddress = "";
                var client = Source.Insurer.CompanyName;
                var buildingConRef = BuildingConDetailCard.Fields.FirstOrDefault(x => x.Name == "ourReference");
                var clientRef = Source.RefNumber;
                string[] reportSumInfo = {
                    "Building Owner:", buildingOwner.Content,
                    "Loss Address:", "",
                    "Client:", client,
                    "Building Consultant’s Reference No:", buildingConRef.Value.ToString(),
                    "Client’s Reference No (Claim No):", clientRef
                };
                foreach (var cell in reportSumInfo)
                {
                    PdfPCell newcell = new PdfPCell(new Phrase(cell));
                    newcell.Colspan = 1;
                    ReportSummaryTable.AddCell(newcell);
                }
                Doc.Add(ReportSummaryTable);

                List reportSummaryList = new List(List.ORDERED, 20f);
                reportSummaryList.SetListSymbol("\u2022");
                reportSummaryList.IndentationLeft = 15f;
                string[] reportSummaryListInfo = {
                    //1
                    "This report contains a suggested scope of works, based on the site inspection. It is not a strict or exact detail of the work required. The tendering builders are to provide their tender on the basis of the scope of works that they receive from the owner.",
                    //2
                    "All works are to comply with SafeWork requirements (should this be , current Australian Standards, National Construction Code, Manufacturer's Installation Requirements and Standards, NSW Guide to Standards and Tolerances, and all relevant state and federal Government law and other requirements.",
                    //3
                    "The Tenderer is to allow for all required fall arrest and/or scaffolding as required in the tender price to complete the works in a safe manner.",
                    //4
                    "Prior to commencement of the works, it is recommended that a mutually agreeable and industry recognised building contract be entered between the Builder and the Owner. A clear progress payment schedule is to be included within the contract, progress payment amounts are not to exceed the actual stage of works completed.",
                    //5
                    "Unless noted otherwise, all finishes and materials are to match existing as close as practical. Where painting and rendering repairs are carried out, unless noted otherwise these must extend to the nearest possible architectural break.",
                    //6
                    "The Tenderer is to provide the estimated lead in time, and time of completion from the date of commencement.",
                    //7
                    "The Tenderer shall include within their offer all costs for Labour, Material, Plant and the associated Fees necessary to complete the Scope of Work to the required level of finish, trade quality and in compliance with statutory obligations.",
                    //8
                    "If chosen by the Home Owner, the successful Tenderer is to obtain and provide evidence of all required Insurances.",
                    //9
                    "During the works, the Principal Contractor is responsible for damage caused by any person under his care, custody and control, to the existing structure, services, paving, road, adjoining properties, etc. and will make good the damage at his/her own cost.",
                    //10
                    "The Tenderer shall allow for all required costs to protect all surfaces for the durations of the works. Where required this may include moving and storing furniture and plant items to allow for successful execution of the projects.",
                    //11
                    "All disturbed areas to be made good to match existing, and on completion, clean all areas and remove all building rubbish from site.",
                    //12
                    "At hand-over of the noted works, provide all warranties, insurance certificates and documentation relative to the project.",
                    //13
                    "Where necessary, a Final Occupation Certificate and/or other relevant certificates is/are to be provided to the Owner, Claims Administrator and Local Government Authority at completion sign off; final progress payment will be withheld without this documentation in place.",
                    //14
                    "The council regulations, such as the complying development consent, DA and CC, must be complied with."
                };

                foreach (string list in reportSummaryListInfo)
                {
                    iTextSharp.text.ListItem item = new iTextSharp.text.ListItem(list, new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black));

                    reportSummaryList.Add(item);
                }
                Doc.Add(reportSummaryList);

                Paragraph signPara = new Paragraph("With their signature below, the Claimant confirms that they accept the independent building consultant’s recommendations set out above and that the next step be quantification of the loss by obtaining quotations from independent licensed building contractor/s.",
                    new Font(Font.UNDEFINED, 12f, Font.UNDEFINED, BaseColor.Black));
                signPara.SpacingBefore = 25f;
                Doc.Add(signPara);

                PdfPTable signatureTable = new PdfPTable(2);
                signatureTable.TotalWidth = 820f;
                signatureTable.LockedWidth = true;
                signatureTable.DefaultCell.Border = Rectangle.NO_BORDER;
                signatureTable.SpacingBefore = 20f;

                string[] signInfo = {
                    "Name of Claimant: ……………………………………………………………………",
                    "Signature of the Claimant: ……………………………………………………………",
                    "Date signed: ……………………………………………………………………",
                    ""
                };

                foreach (var item in signInfo)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(item, new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black)));
                    cell.Border = 0;
                    cell.PaddingTop = 10f;
                    cell.PaddingBottom = 10f;
                    signatureTable.AddCell(cell);
                }

                Doc.Add(signatureTable);
            }

            if (ReferenceCurriVitaeCard != null)
            {
                Doc.NewPage();
                createAnnexureA();
                createAnnexureB();
            }

            //SUPPLEMENTARY COMMERCIAL IN-CONFIDENCE REPORT page 1
            PdfPTable supplementaryComInConfiReportTable = new PdfPTable(1);
            supplementaryComInConfiReportTable.DefaultCell.Border = Rectangle.NO_BORDER;
            supplementaryComInConfiReportTable.TotalWidth = 820f;
            supplementaryComInConfiReportTable.LockedWidth = true;
            PdfPCell supplementaryComInConfiReportCell = new PdfPCell(new Phrase("SUPPLEMENTARY COMMERCIAL IN-CONFIDENCE REPORT", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.Black)));
            supplementaryComInConfiReportCell.HorizontalAlignment = Element.ALIGN_CENTER;
            supplementaryComInConfiReportCell.Border = 0;
            supplementaryComInConfiReportTable.AddCell(supplementaryComInConfiReportCell);

            PdfPCell supplementaryComInConfiReportInfoCell = new PdfPCell(new Phrase("The report below:", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            supplementaryComInConfiReportInfoCell.HorizontalAlignment = Element.ALIGN_LEFT;
            supplementaryComInConfiReportInfoCell.Border = 0;
            supplementaryComInConfiReportTable.AddCell(supplementaryComInConfiReportInfoCell);

            PdfPCell supplementaryComInConfiReportListCell = new PdfPCell();
            List supplementaryComInConfiReportList = new List(List.UNORDERED, 10f);
            supplementaryComInConfiReportList.SetListSymbol("\u2022");
            supplementaryComInConfiReportList.IndentationLeft = 15f;
            supplementaryComInConfiReportList.Add("Is ancillary to the Technical Assessment and Inspection Report dated" + System.DateTime.Now.ToString("dd/MM/yyyy").Replace("-", "/") + "and should be read in accordance with it");
            supplementaryComInConfiReportList.Add("Should be used for underwriters’ internal purposes only and not relied upon for use in making decisions on the claim");
            supplementaryComInConfiReportListCell.AddElement(supplementaryComInConfiReportList);

            if (SupplementaryCommInConfiReportCard != null)
            {
                var policyContent = SupplementaryCommInConfiReportCard.Fields.FirstOrDefault(x => x.Name == "policy").Value.ToString();
                var nonCompletionContent = SupplementaryCommInConfiReportCard.Fields.FirstOrDefault(x => x.Name == "nonCompletion").Value.ToString();
                var confidenceDetailsContent = SupplementaryCommInConfiReportCard.Fields.FirstOrDefault(x => x.Name == "confidenceDetails").Value.ToString();
                var additionalCommentsContent = SupplementaryCommInConfiReportCard.Fields.FirstOrDefault(x => x.Name == "addComments").Value.ToString();
                var potentialRecovertContent = SupplementaryCommInConfiReportCard.Fields.FirstOrDefault(x => x.Name == "potentialRecovert").Value.ToString();

                PdfPTable policyTable = new PdfPTable(1);
                policyTable.TotalWidth = 820f;
                policyTable.LockedWidth = true;
                PdfPCell policyTableTitle = new PdfPCell(new Phrase("POLICY", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
                policyTableTitle.BackgroundColor = new BaseColor(0, 0, 51);
                policyTableTitle.HorizontalAlignment = Element.ALIGN_CENTER;
                policyTable.AddCell(policyTableTitle);

                PdfPCell policyTableInfo = new PdfPCell(new Phrase(policyContent, new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            }

            if (ScheduleItemCard != null)
            {
                Doc.NewPage();
                PdfPTable summaryAllAcceptWorkEstimate = new PdfPTable(6);
                summaryAllAcceptWorkEstimate.TotalWidth = 820f;
                summaryAllAcceptWorkEstimate.LockedWidth = true;
                PdfPCell summaryAllAcceptWorkEstimateTitle = new PdfPCell(new Phrase("SUMMARY OF ALL ACCEPTED WORKS & ESTIMATES", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
                summaryAllAcceptWorkEstimateTitle.BackgroundColor = new BaseColor(0, 0, 51);
                summaryAllAcceptWorkEstimateTitle.HorizontalAlignment = Element.ALIGN_CENTER;
                summaryAllAcceptWorkEstimateTitle.Colspan = 6;
                summaryAllAcceptWorkEstimate.AddCell(summaryAllAcceptWorkEstimateTitle);
                PdfPCell itemNo = new PdfPCell(new Phrase("ITEM NO", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
                itemNo.HorizontalAlignment = Element.ALIGN_CENTER;
                itemNo.Colspan = 1;
                summaryAllAcceptWorkEstimate.AddCell(itemNo);
                PdfPCell description = new PdfPCell(new Phrase("DESCRIPTION", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
                description.HorizontalAlignment = Element.ALIGN_CENTER;
                description.Colspan = 3;
                summaryAllAcceptWorkEstimate.AddCell(description);
                PdfPCell location = new PdfPCell(new Phrase("LOCATION", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
                location.HorizontalAlignment = Element.ALIGN_CENTER;
                location.Colspan = 1;
                summaryAllAcceptWorkEstimate.AddCell(location);
                PdfPCell estimate = new PdfPCell(new Phrase("ESTIMATE", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
                estimate.HorizontalAlignment = Element.ALIGN_CENTER;
                estimate.Colspan = 1;
                summaryAllAcceptWorkEstimate.AddCell(estimate);

                var fields = ScheduleItemCard.Fields.FirstOrDefault().Value as List<List<Field>>;
                double totalEstimate = 0;
                foreach (var field in fields)
                {
                    if (field.FirstOrDefault(x => x.Label == "Recommendation").Value.ToString() == "Accept")
                    {
                        var itemNumValue = field.FirstOrDefault(x => x.Name == "itemNumber").Value.ToString();
                        var descValue = field.FirstOrDefault(x => x.Name == "itemDescription").Value.ToString();
                        var locationValue = field.FirstOrDefault(x => x.Name == "itemLocation").Value.ToString();
                        var estimateCostValue = field.FirstOrDefault(x => x.Name == "costEstimate").Value.ToString();
                        var estNum = Convert.ToDouble(estimateCostValue);
                        totalEstimate += estNum;

                        PdfPCell itemNoInfo = new PdfPCell(new Phrase(itemNumValue));
                        itemNoInfo.HorizontalAlignment = Element.ALIGN_CENTER;
                        itemNoInfo.Colspan = 1;
                        summaryAllAcceptWorkEstimate.AddCell(itemNoInfo);
                        PdfPCell descInfo = new PdfPCell(new Phrase(descValue));
                        descInfo.HorizontalAlignment = Element.ALIGN_CENTER;
                        descInfo.Colspan = 3;
                        summaryAllAcceptWorkEstimate.AddCell(descInfo);
                        PdfPCell locationInfo = new PdfPCell(new Phrase(locationValue));
                        locationInfo.HorizontalAlignment = Element.ALIGN_CENTER;
                        locationInfo.Colspan = 1;
                        summaryAllAcceptWorkEstimate.AddCell(locationInfo);
                        PdfPCell estimateInfo = new PdfPCell(new Phrase(estimateCostValue));
                        estimateInfo.HorizontalAlignment = Element.ALIGN_CENTER;
                        estimateInfo.Colspan = 1;
                        summaryAllAcceptWorkEstimate.AddCell(estimateInfo);
                    }
                }
                PdfPCell total = new PdfPCell(new Phrase("TOTAL", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
                total.Colspan = 5;
                total.HorizontalAlignment = Element.ALIGN_CENTER;
                summaryAllAcceptWorkEstimate.AddCell(total);
                PdfPCell totalEstInfo = new PdfPCell(new Phrase(totalEstimate.ToString(), new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
                totalEstInfo.Colspan = 1;
                totalEstInfo.HorizontalAlignment = Element.ALIGN_CENTER;
                summaryAllAcceptWorkEstimate.AddCell(totalEstInfo);

                Doc.Add(summaryAllAcceptWorkEstimate);
            }

        }

        protected void createAnnexureA()
        {
            PdfPTable annexureATable = new PdfPTable(1);
            annexureATable.TotalWidth = 800f;
            annexureATable.LockedWidth = true;
            annexureATable.DefaultCell.Border = Rectangle.NO_BORDER;
            PdfPCell annexureATitle1 = new PdfPCell(new Paragraph("‘Annexure A’" , new Font(Font.BOLD, 11f, Font.BOLD, BaseColor.Black)));
            PdfPCell annexureATitle2 = new PdfPCell(new Paragraph("CURRICULUM VITAE-" + Source.Building.ScopingSupplier.CompanyName, new Font(Font.BOLD, 11f, Font.BOLD, BaseColor.Black)));
            PdfPCell annexureATitle3 = new PdfPCell(new Paragraph("LICENSED BUILDER / BUILDING CONSULTANT", new Font(Font.BOLD, 11f, Font.BOLD, BaseColor.Black)));
            annexureATitle1.Border = 0;
            annexureATitle1.HorizontalAlignment = Element.ALIGN_CENTER;
            annexureATitle2.Border = 0;
            annexureATitle2.HorizontalAlignment = Element.ALIGN_CENTER;
            annexureATitle3.Border = 0;
            annexureATitle3.HorizontalAlignment = Element.ALIGN_CENTER;

            annexureATable.AddCell(annexureATitle1);
            annexureATable.AddCell(annexureATitle2);
            annexureATable.AddCell(annexureATitle3);

            PdfPCell annexureAInfo = new PdfPCell(new Phrase(ReferenceCurriVitaeCard.Fields[0]?.Value.ToString(), new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            annexureAInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            annexureAInfo.Border = 0;

            annexureATable.AddCell(annexureAInfo);
            Doc.Add(annexureATable);
        }

        protected void createAnnexureB()
        {
            Doc.NewPage();
            PdfPTable annexureBTable = new PdfPTable(1);
            annexureBTable.TotalWidth = 800f;
            annexureBTable.LockedWidth = true;
            annexureBTable.DefaultCell.Border = Rectangle.NO_BORDER;
            PdfPCell annexureBTitle1 = new PdfPCell(new Paragraph("‘Annexure B’", new Font(Font.BOLD, 11f, Font.BOLD, BaseColor.Black)));
            PdfPCell annexureBTitle2 = new PdfPCell(new Paragraph("NCAT Procedural Direction 3", new Font(Font.BOLD, 11f, Font.BOLD, BaseColor.Black)));
            PdfPCell annexureBTitle3 = new PdfPCell(new Paragraph("EXPERT WITNESSES", new Font(Font.BOLD, 11f, Font.BOLD, BaseColor.Black)));
            annexureBTitle1.Border = 0;
            annexureBTitle1.HorizontalAlignment = Element.ALIGN_CENTER;
            annexureBTitle2.Border = 0;
            annexureBTitle2.HorizontalAlignment = Element.ALIGN_RIGHT;
            annexureBTitle3.Border = 0;
            annexureBTitle3.HorizontalAlignment = Element.ALIGN_CENTER;

            annexureBTable.AddCell(annexureBTitle1);
            annexureBTable.AddCell(annexureBTitle2);
            annexureBTable.AddCell(annexureBTitle3);

            PdfPTable expertWitnessTable = new PdfPTable(4);
            expertWitnessTable.SpacingBefore = 10f;
            expertWitnessTable.DefaultCell.Border = Rectangle.NO_BORDER;
            expertWitnessTable.TotalWidth = 800f;
            expertWitnessTable.LockedWidth = true;
            PdfPCell expertWitnessTableCell = new PdfPCell();
            expertWitnessTableCell.Border = 0;
            expertWitnessTableCell.PaddingTop = 10f;

            PdfPCell proceduralDirection = new PdfPCell(new Phrase("This Procedural Direction applies to:", new Font(Font.BOLD, 10f, Font.BOLD)));
            proceduralDirection.Colspan = 1;
            proceduralDirection.Border = 0;
            PdfPCell proceduralDirectionInfo = new PdfPCell(new Phrase("Proceedings in all Divisions", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED)));
            proceduralDirectionInfo.Colspan = 3;
            proceduralDirectionInfo.Border = 0;

            PdfPCell effectiveDate = new PdfPCell(new Phrase("Effective Date", new Font(Font.BOLD, 10f, Font.BOLD)));
            effectiveDate.Colspan = 1;
            effectiveDate.Border = 0;
            PdfPCell effectiveDateInfo = new PdfPCell(new Phrase("7 February 2014", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED)));
            effectiveDateInfo.Colspan = 3;
            effectiveDateInfo.Border = 0;

            PdfPCell replacesProceduralDirection = new PdfPCell(new Phrase("Replaces Procedural Direction:", new Font(Font.BOLD, 10f, Font.BOLD)));
            replacesProceduralDirection.Colspan = 1;
            replacesProceduralDirection.Border = 0;
            PdfPCell replacesProceduralDirectionInfo = new PdfPCell(new Phrase("NCAT Procedural Direction 3 (20 December 2013)", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED)));
            replacesProceduralDirectionInfo.Colspan = 3;
            replacesProceduralDirectionInfo.Border = 0;

            PdfPCell Notes = new PdfPCell(new Phrase("Notes:", new Font(Font.BOLD, 10f, Font.BOLD)));
            Notes.Colspan = 1;
            Notes.Border = 0;
            PdfPCell NotesInfo = new PdfPCell(new Phrase("You should ensure that you are using the current version of this Procedural Direction. A complete set of Procedural Directions and Guidelines is available on the Tribunal website at www.ncat.nsw.gov.au", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED)));
            NotesInfo.Colspan = 3;
            NotesInfo.Border = 0;

            expertWitnessTable.AddCell(proceduralDirection);
            expertWitnessTable.AddCell(proceduralDirectionInfo);
            expertWitnessTable.AddCell(effectiveDate);
            expertWitnessTable.AddCell(effectiveDateInfo);
            expertWitnessTable.AddCell(replacesProceduralDirection);
            expertWitnessTable.AddCell(replacesProceduralDirectionInfo);
            expertWitnessTable.AddCell(Notes);
            expertWitnessTable.AddCell(NotesInfo);

            expertWitnessTableCell.AddElement(expertWitnessTable);

            annexureBTable.AddCell(expertWitnessTableCell);

            //introduction
            PdfPCell introductionCell = new PdfPCell(new Paragraph("Introduction", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
            introductionCell.HorizontalAlignment = Element.ALIGN_LEFT;
            introductionCell.Border = 0;
            PdfPCell introductionInfo = new PdfPCell();
            introductionInfo.PaddingTop = 10f;

            RomanList romanlist1 = new RomanList(true, 20);
            romanlist1.Add(new ListItem("A code of conduct for expert witnesses (based upon Schedule 7 to the Uniform Civil Procedure Rules 2005); and", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanlist1.Add(new ListItem("Information on how experts may be required to give evidence.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));

            List list1 = new List(List.ORDERED, 20f);
            list1.SetListSymbol("\u2022");
            list1.IndentationLeft = 20f;
            list1.First = 1;
            list1.Add(new ListItem ("The Tribunal may rely on evidence from expert witnesses to reach a conclusion about a technical matter or area of specialised knowledge that is relevant to an issue to be determined in proceedings. It is important that experts’ opinions are soundly based, complete and reliable", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list1.Add(new ListItem ("This Procedural Direction sets out:", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list1.Add(romanlist1);
            
            introductionInfo.AddElement(list1);
            introductionInfo.Border = 0;
            annexureBTable.AddCell(introductionCell);
            annexureBTable.AddCell(introductionInfo);

            //Compliance and other matters
            PdfPCell complianceCell = new PdfPCell(new Paragraph("Compliance and other matters", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
            complianceCell.HorizontalAlignment = Element.ALIGN_LEFT;
            complianceCell.Border = 0;
            complianceCell.PaddingTop = 10f;
            PdfPCell complianceInfo = new PdfPCell();
            complianceInfo.PaddingTop = 10f;

            List list2 = new List(List.ORDERED, 20f);
            list2.SetListSymbol("\u2022");
            list2.IndentationLeft = 20f;
            list2.First = 3;
            list2.Add(new ListItem("The Tribunal may excuse an expert witness or any other person from complying with this Procedural Direction before or after the time for compliance", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list2.Add(new ListItem("Nothing in this Procedural Direction prevents the Tribunal from giving any directions concerning expert witnesses or expert evidence that the Tribunal considers appropriate in any particular proceedings before the Tribunal", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list2.Add(new ListItem("This Procedural Direction is made by the President under s 26 of the Civil and Administrative Tribunal Act 2013.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));

            complianceInfo.AddElement(list2);
            complianceInfo.Border = 0;
            annexureBTable.AddCell(complianceCell);
            annexureBTable.AddCell(complianceInfo);

            //definition
            PdfPCell definitionsCell = new PdfPCell(new Paragraph("Definitions", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
            definitionsCell.HorizontalAlignment = Element.ALIGN_LEFT;
            definitionsCell.Border = 0;
            definitionsCell.PaddingTop = 10f;
            definitionsCell.PaddingBottom = 10f;

            PdfPTable definitionsTable = new PdfPTable(8);
            definitionsTable.TotalWidth = 750f;
            definitionsTable.LockedWidth = true;

            PdfPCell wordCell = new PdfPCell(new Phrase("Word", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            wordCell.Colspan = 1;
            wordCell.PaddingTop = 4f;
            wordCell.PaddingBottom = 4f;
            PdfPCell wordInfo = new PdfPCell(new Phrase("Definition", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            wordInfo.Colspan = 7;
            wordInfo.PaddingTop = 4f;
            wordInfo.PaddingBottom = 4f;

            PdfPCell act = new PdfPCell(new Phrase("Act", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            act.Colspan = 1;
            act.PaddingTop = 4f;
            act.PaddingBottom = 4f;
            PdfPCell actInfo = new PdfPCell(new Phrase("Civil and Administrative Tribunal Act 2013", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            actInfo.Colspan = 7;
            actInfo.PaddingTop = 4f;
            actInfo.PaddingBottom = 4f;

            PdfPCell rules = new PdfPCell(new Phrase("Rules", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            rules.Colspan = 1;
            rules.PaddingTop = 4f;
            rules.PaddingBottom = 4f;
            PdfPCell rulesInfo = new PdfPCell(new Phrase("Civil and Administrative Tribunal Rules 2014", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            rulesInfo.Colspan = 7;
            rulesInfo.PaddingTop = 4f;
            rulesInfo.PaddingBottom = 4f;

            PdfPCell expressWitness = new PdfPCell(new Phrase("Expert Witness", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            expressWitness.Colspan = 1;
            expressWitness.PaddingTop = 4f;
            expressWitness.PaddingBottom = 4f;
            PdfPCell expressWitnessInfo = new PdfPCell(new Phrase("A person who has specialised knowledge based on the person’s training, study or experience and who give evidence of an opinion based wholly or substantially on that knowledge", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            expressWitnessInfo.Colspan = 7;
            expressWitnessInfo.PaddingTop = 4f;
            expressWitnessInfo.PaddingBottom = 4f;

            definitionsTable.AddCell(wordCell);
            definitionsTable.AddCell(wordInfo);

            definitionsTable.AddCell(act);
            definitionsTable.AddCell(actInfo);

            definitionsTable.AddCell(rules);
            definitionsTable.AddCell(rulesInfo);

            definitionsTable.AddCell(expressWitness);
            definitionsTable.AddCell(expressWitnessInfo);

            annexureBTable.AddCell(definitionsCell);
            annexureBTable.AddCell(definitionsTable);

            List list3 = new List(List.ORDERED, 20f);
            list3.SetListSymbol("\u2022");
            list3.IndentationLeft = 20f;
            list3.First = 6;
            list3.Add(new ListItem("Words used in this Procedural Direction have the same meaning as defined in the Act and the Rules.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));

            //Application
            PdfPCell applicationCell = new PdfPCell(new Paragraph("Application", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
            applicationCell.HorizontalAlignment = Element.ALIGN_LEFT;
            applicationCell.Border = 0;
            applicationCell.PaddingTop = 10f;
            applicationCell.PaddingBottom = 10f;
            annexureBTable.AddCell(applicationCell);

            PdfPCell applicationInfo = new PdfPCell();
            applicationInfo.PaddingTop = 10f;
            applicationInfo.Border = 0;

            RomanList romanList2 = new RomanList(true, 20);
            romanList2.Add(new ListItem("any evidence given by an expert witness in the Tribunal;", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList2.Add(new ListItem("any arrangement between an expert and a party for the expert to provide evidence or a report for the purposes of proceedings or proposed proceedings in the Tribunal; and", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList2.Add(new ListItem("any arrangements for Tribunal appointed experts,", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));

            List list4 = new List(List.ORDERED, 20f);
            list4.SetListSymbol("\u2022");
            list4.IndentationLeft = 20f;
            list4.First = 7;
            list4.Add(new ListItem("This Procedural Direction applies to:", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list4.Add(romanList2);
            applicationInfo.AddElement(list4);

            annexureBTable.AddCell(applicationInfo);
            PdfPCell applicationListInfo = new PdfPCell(new Paragraph("except that this Procedural Direction does not apply to evidence obtained from treating doctors, other health professionals or hospitals (who might otherwise fall within the definition of expert witness), unless the Tribunal otherwise directs", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            applicationListInfo.Border = 0;
            applicationListInfo.PaddingTop = 10f;
            annexureBTable.AddCell(applicationListInfo);

            //Parties’ and Experts’ Duties
            PdfPCell partAndExpCell = new PdfPCell(new Paragraph("Parties’ and Experts’ Duties", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
            partAndExpCell.HorizontalAlignment = Element.ALIGN_LEFT;
            partAndExpCell.Border = 0;
            partAndExpCell.PaddingTop = 10f;
            partAndExpCell.PaddingBottom = 10f;
            annexureBTable.AddCell(partAndExpCell);
            PdfPCell partAndExpInfo = new PdfPCell();
            partAndExpInfo.Border = 0;

            List list5 = new List(List.ORDERED, 20f);
            list5.SetListSymbol("\u2022");
            list5.IndentationLeft = 20f;
            list5.First = 8;
            list5.Add(new ListItem("Any party who retains an expert to provide evidence or a report for the purposes of proceedings or proposed proceedings in the Tribunal must bring to the expert’s attention the contents of this Procedural Direction, including the experts’ code of conduct.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list5.Add(new ListItem("Where an expert is unable to comply with the experts’ code of conduct, whether because of a conflict of interest or otherwise, the expert is not to give evidence or provide an expert’s report for use in proceedings in the Tribunal, unless the expert raises the inability with the Tribunal and the Tribunal expressly permits the expert to give evidence or provide a report.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            partAndExpInfo.AddElement(list5);

            annexureBTable.AddCell(partAndExpInfo);

            PdfPCell expertCodeConCell = new PdfPCell(new Paragraph("Experts’ Code of Conduct", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
            expertCodeConCell.HorizontalAlignment = Element.ALIGN_LEFT;
            expertCodeConCell.Border = 0;
            expertCodeConCell.PaddingTop = 10f;
            expertCodeConCell.PaddingBottom = 10f;
            annexureBTable.AddCell(expertCodeConCell);

            PdfPCell applicationCodeCell = new PdfPCell(new Paragraph("Application of code", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
            applicationCodeCell.HorizontalAlignment = Element.ALIGN_LEFT;
            applicationCodeCell.Border = 0;
            applicationCodeCell.PaddingTop = 10f;
            applicationCodeCell.PaddingBottom = 10f;
            annexureBTable.AddCell(applicationCodeCell);

            PdfPCell applicationCodeInfo = new PdfPCell();
            applicationCodeInfo.Border = 0;

            RomanList romanList3 = new RomanList(true, 20);
            romanList3.Add(new ListItem("provides an expert’s report for use as evidence in proceedings or proposed proceedings in the Tribunal, or", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList3.Add(new ListItem("gives opinion evidence in proceedings in the Tribunal.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            List list6 = new List(List.ORDERED, 20f);
            list6.SetListSymbol("\u2022");
            list6.IndentationLeft = 20f;
            list6.First = 10;
            list6.Add(new ListItem("Subject to paragraph 7, this experts’ code of conduct applies to any expert witness who:", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list6.Add(romanList3);
            applicationCodeInfo.AddElement(list6);

            annexureBTable.AddCell(applicationCodeInfo);

            PdfPCell generalDutyTriCell = new PdfPCell(new Paragraph("General duty to the Tribunal", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
            generalDutyTriCell.HorizontalAlignment = Element.ALIGN_LEFT;
            generalDutyTriCell.Border = 0;
            generalDutyTriCell.PaddingTop = 10f;
            generalDutyTriCell.PaddingBottom = 10f;
            annexureBTable.AddCell(generalDutyTriCell);

            PdfPCell generalDutyTriInfo = new PdfPCell();
            generalDutyTriInfo.Border = 0;

            RomanList romanList4 = new RomanList(true, 20);
            romanList4.Add(new ListItem("provides an expert’s report for use as evidence in proceedings or proposed proceedings in the Tribunal, or", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList4.Add(new ListItem("gives opinion evidence in proceedings in the Tribunal.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));

            List list7 = new List(List.ORDERED, 20f);
            list7.SetListSymbol("\u2022");
            list7.IndentationLeft = 20f;
            list7.First = 11;
            list7.Add(new ListItem("Subject to paragraph 7, this experts’ code of conduct applies to any expert witness who:", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list7.Add(romanList4);
            list7.Add(new ListItem("An expert witness’s paramount duty is to the Tribunal and not to any party to the proceedings (including the person retaining the expert witness).", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list7.Add(new ListItem("An expert witness is not an advocate for a party.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list7.Add(new ListItem("An expert witness must abide by any direction given by the Tribunal.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));

            generalDutyTriInfo.AddElement(list7);
            annexureBTable.AddCell(generalDutyTriInfo);

            PdfPCell dutyWorkCooperCell = new PdfPCell(new Paragraph("Duty to work co-operatively with other expert witnesses", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
            dutyWorkCooperCell.HorizontalAlignment = Element.ALIGN_LEFT;
            dutyWorkCooperCell.Border = 0;
            dutyWorkCooperCell.PaddingTop = 10f;
            dutyWorkCooperCell.PaddingBottom = 10f;
            annexureBTable.AddCell(dutyWorkCooperCell);

            PdfPCell dutyWorkCooperInfo = new PdfPCell();
            dutyWorkCooperInfo.Border = 0;

            RomanList romanList5 = new RomanList(true, 20);
            romanList5.Add(new ListItem("exercise his or her independent, professional judgment in relation to that issue;", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList5.Add(new ListItem("endeavour to reach agreement with any other expert witness on that issue; and", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList5.Add(new ListItem("not act on any instruction or request to withhold or avoid agreement with any other expert witness.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));

            List list8 = new List(List.ORDERED, 20f);
            list8.SetListSymbol("\u2022");
            list8.IndentationLeft = 20f;
            list8.First = 15;
            list8.Add(new ListItem("An expert witness, when complying with any direction of the Tribunal to confer with another expert witness or to prepare a joint report with another expert witness in relation to any issue must:", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list8.Add(romanList5);
            dutyWorkCooperInfo.AddElement(list8);
            annexureBTable.AddCell(dutyWorkCooperInfo);

            //Experts’ reports
            PdfPCell expertsReportsCell = new PdfPCell(new Paragraph("Experts’ reports", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
            expertsReportsCell.HorizontalAlignment = Element.ALIGN_LEFT;
            expertsReportsCell.Border = 0;
            expertsReportsCell.PaddingTop = 10f;
            expertsReportsCell.PaddingBottom = 10f;
            annexureBTable.AddCell(expertsReportsCell);

            PdfPCell expertsReportsInfo = new PdfPCell();
            expertsReportsInfo.Border = 0;

            RomanList romanList6 = new RomanList(true, 20);
            romanList6.Add(new ListItem("the expert’s qualifications as an expert on the issue the subject of the report;", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList6.Add(new ListItem("the facts, and assumptions of fact, on which the opinions in the report are based (a letter of instructions may be annexed);", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList6.Add(new ListItem("the expert’s reasons for each opinion expressed;", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList6.Add(new ListItem("if applicable, that a particular issue falls outside the expert’s field of expertise;", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList6.Add(new ListItem("any literature or other materials used in support of the opinions.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList6.Add(new ListItem("any examinations, tests or other investigations on which the expert has relied, including details of the qualifications of the person who carried them out;", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList6.Add(new ListItem("in the case of a report that is lengthy or complex, a brief summary of the report (to be located at the beginning of the report);", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList6.Add(new ListItem("an acknowledgement that the expert has read the experts’ code of conduct and agrees to be bound by it.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));

            List list9 = new List(List.ORDERED, 20f);
            list9.SetListSymbol("\u2022");
            list9.IndentationLeft = 20f;
            list9.First = 16;
            list9.Add(new ListItem("An expert witness, when complying with any direction of the Tribunal to confer with another expert witness or to prepare a joint report with another expert witness in relation to any issue must:", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list9.Add(romanList6);
            list9.Add(new ListItem("If an expert witness who prepares an expert’s report believes that it may be incomplete or inaccurate without some qualification, the qualification must be stated in the report.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list9.Add(new ListItem("If an expert witness considers that his or her opinion is not a concluded opinion because of insufficient research or insufficient data or for any other reason, this must be stated when the opinion is expressed.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list9.Add(new ListItem("If an expert witness changes his or her opinion on a material matter after providing a report, the expert witness must immediately provide a supplementary report to that effect containing such of the information referred to in paragraph 16 as is appropriate.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));

            expertsReportsInfo.AddElement(list9);
            annexureBTable.AddCell(expertsReportsInfo);

            PdfPCell expertsConferenceCell = new PdfPCell(new Paragraph("Experts’ conference", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
            expertsConferenceCell.HorizontalAlignment = Element.ALIGN_LEFT;
            expertsConferenceCell.Border = 0;
            expertsConferenceCell.PaddingTop = 10f;
            expertsConferenceCell.PaddingBottom = 10f;
            annexureBTable.AddCell(expertsConferenceCell);

            PdfPCell expertsConferenceInfo = new PdfPCell();
            expertsConferenceInfo.Border = 0;

            RomanList romanList7 = new RomanList(true, 20);
            romanList7.Add(new ListItem("to confer with any other expert witness;", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList7.Add(new ListItem("to endeavour to reach agreement on any matters in issue;", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList7.Add(new ListItem("to prepare a joint report, specifying matters agreed and matters not agreed and any reasons for any disagreement; and", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList7.Add(new ListItem("to base any joint report on specified facts or assumptions of fact.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));

            List list10 = new List(List.ORDERED, 20f);
            list10.SetListSymbol("\u2022");
            list10.IndentationLeft = 20f;
            list10.First = 20;
            list10.Add(new ListItem("An expert witness must abide by any direction of the Tribunal:", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list10.Add(romanList7);
            list10.Add(new ListItem("An expert witness must exercise his or her independent, professional judgment in relation to such a conference and joint report, and must not act on any instruction or request to withhold or avoid agreement.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            expertsConferenceInfo.AddElement(list10);
            annexureBTable.AddCell(expertsConferenceInfo);

            PdfPCell howManyExpertEvidenceCell = new PdfPCell(new Paragraph("How may expert evidence be given?", new Font(Font.BOLD, 10f, Font.BOLD, BaseColor.Black)));
            howManyExpertEvidenceCell.HorizontalAlignment = Element.ALIGN_LEFT;
            howManyExpertEvidenceCell.Border = 0;
            howManyExpertEvidenceCell.PaddingTop = 10f;
            howManyExpertEvidenceCell.PaddingBottom = 10f;
            annexureBTable.AddCell(howManyExpertEvidenceCell);

            PdfPCell howManyExpertEvidenceInfo = new PdfPCell();
            howManyExpertEvidenceInfo.Border = 0;

            RomanList romanList8 = new RomanList(true, 20);
            romanList8.Add(new ListItem("requiring expert evidence to be given by written report;", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList8.Add(new ListItem("requiring expert witnesses to confer and prepare a joint report, specifying matters agreed and matters not agreed and reasons for any disagreement;", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList8.Add(new ListItem("specifying when and in what order expert evidence at a hearing will be given;", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList8.Add(new ListItem("controlling the form and duration of cross examination of expert witnesses; and", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList8.Add(new ListItem("requiring expert witnesses to give evidence at a hearing concurrently.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            RomanList romanList9 = new RomanList(true, 20);
            romanList9.Add(new ListItem("sitting together in the witness box or some other convenient place in the hearing room;", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList9.Add(new ListItem("being asked questions by the Tribunal;", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList9.Add(new ListItem("being asked questions by the parties or their representatives (if any);", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList9.Add(new ListItem("being given the opportunity to respond to the other witness’s evidence, as that evidence is given; and", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            romanList9.Add(new ListItem("being given the opportunity to ask any questions of the other witness, as the evidence is being given, where those questions might assist the Tribunal in determining the matter.", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));

            List list11 = new List(List.ORDERED, 20f);
            list11.SetListSymbol("\u2022");
            list11.IndentationLeft = 20f;
            list11.First = 22;
            list11.Add(new ListItem("The Tribunal may regulate the conduct of proceedings involving expert witnesses including by:", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list11.Add(romanList8);
            list11.Add(new ListItem("If the Tribunal requires or permits expert witnesses to give evidence concurrently this will usually involve the expert witnesses in one particular field of expertise:", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            list11.Add(romanList9);
            howManyExpertEvidenceInfo.AddElement(list11);
            annexureBTable.AddCell(howManyExpertEvidenceInfo);

            PdfPCell endingCell = new PdfPCell();
            endingCell.Border = 0;
            endingCell.PaddingTop = 30f;
            PdfPTable endingTable = new PdfPTable(1);
            endingTable.HorizontalAlignment = Element.ALIGN_LEFT;
            endingTable.DefaultCell.Border = Rectangle.NO_BORDER;
            endingTable.AddCell(new Paragraph("Wright J" + "\n" + "President" + "\n" + "7 February 2014", new Font(Font.UNDEFINED, 10f, Font.UNDEFINED, BaseColor.Black)));
            endingCell.AddElement(endingTable);
            annexureBTable.AddCell(endingCell);

            //end of Annexure B 

            //SUPPLEMENTARY COMMERCIAL IN-CONFIDENCE REPORT

            Doc.Add(annexureBTable);
        }

        protected void createPhotoAndScheduleTable()
        {
            // photograph static table
            PdfPTable PhotoGraphTable = new PdfPTable(1);
            PhotoGraphTable.TotalWidth = 820f;
            PhotoGraphTable.LockedWidth = true;
            PhotoGraphTable.SpacingBefore = 10f;

            PdfPCell PhotoGraphHeader = new PdfPCell(new Phrase("Photographs", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            PhotoGraphHeader.BackgroundColor = new BaseColor(0, 0, 51);
            PhotoGraphHeader.HorizontalAlignment = Element.ALIGN_CENTER;
            PhotoGraphHeader.PaddingTop = 4f;
            PhotoGraphHeader.PaddingBottom = 4f;
            PhotoGraphTable.AddCell(PhotoGraphHeader);

            PdfPCell PhotoGraphInfo = new PdfPCell(new Phrase("Photographs taken during inspection of this property are set out in the item detail sections.", new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black)));
            PhotoGraphInfo.PaddingTop = 4f;
            PhotoGraphInfo.PaddingBottom = 4f;
            PhotoGraphTable.AddCell(PhotoGraphInfo);

            Doc.Add(PhotoGraphTable);

            //schedule of work static table
            PdfPTable ScheduleWorkTable = new PdfPTable(1);
            ScheduleWorkTable.TotalWidth = 820f;
            ScheduleWorkTable.LockedWidth = true;
            ScheduleWorkTable.SpacingBefore = 10f;

            PdfPCell ScheduleWorkHeader = new PdfPCell(new Phrase("Schedule of Works", new Font(Font.BOLD, 12f, Font.BOLD, BaseColor.White)));
            ScheduleWorkHeader.BackgroundColor = new BaseColor(0, 0, 51);
            ScheduleWorkHeader.HorizontalAlignment = Element.ALIGN_CENTER;
            ScheduleWorkHeader.PaddingTop = 4f;
            ScheduleWorkHeader.PaddingBottom = 4f;
            ScheduleWorkTable.AddCell(ScheduleWorkHeader);

            PdfPCell ScheduleWorkInfo = new PdfPCell(new Phrase("A recommended Schedule of Works is set out under each item of this report", new Font(Font.UNDEFINED, 11f, Font.UNDEFINED, BaseColor.Black)));
            ScheduleWorkInfo.PaddingTop = 4f;
            ScheduleWorkInfo.PaddingBottom = 4f;
            ScheduleWorkTable.AddCell(ScheduleWorkInfo);

            Doc.Add(ScheduleWorkTable);
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
                imgCell.AddElement(Image.GetInstance(new Uri(file.Url)));
                imgCell.AddElement(new Phrase(file.Name, StyleContentSmall));
                imgTable.AddCell(imgCell);
            }
        }

        protected void CreateScheduleItem(List<Field> field)
        {
            var itemNumValue = field.FirstOrDefault(x => x.Name == "itemNumber");
            var descValue = field.FirstOrDefault(x => x.Name == "itemDescription");
            var locationValue = field.FirstOrDefault(x => x.Name == "itemLocation");
            var crossRefValue = field.FirstOrDefault(x => x.Name == "crossRef");
            var estimateCostValue = field.FirstOrDefault(x => x.Name == "costEstimate");
            var lossTypeValue = field.FirstOrDefault(x => x.Name == "lossType");
            var compStatusValue = field.FirstOrDefault(x => x.Name == "completionStatus");
            var recomValue = field.FirstOrDefault(x => x.Name == "recommendation");
            var ReasonDenValue = field.FirstOrDefault(x => x.Name == "itemDenialReason");
            var observationValue = field.FirstOrDefault(x => x.Name == "observation");
            var causeValue = field.FirstOrDefault(x => x.Name == "cause");
            var breachValue = field.FirstOrDefault(x => x.Name == "breaches");
            var suggestScopeValue = field.FirstOrDefault(x => x.Name == "suggestedScope");
            var itemPhotos = field.FirstOrDefault(x => x.Name == "itemPhoto")?.Value as IEnumerable<ReportFile>;

            ItemReporData testData = new ItemReporData("ITEM " + itemNumValue.Value.ToString());
            testData.TitleFields.Add(new InfoTableMetaData(descValue.Label, descValue.Value.ToString(), colLable: 2, colData: 4));
            testData.TitleFields.Add(new InfoTableMetaData(locationValue.Label, locationValue.Value.ToString(), colData: 1));
            testData.Fields.Add(new InfoTableMetaData(crossRefValue.Label, crossRefValue.Value.ToString(), colData: 0));
            testData.Fields.Add(new InfoTableMetaData(lossTypeValue.Label, lossTypeValue.Value.ToString(), colData: 2));
            testData.Fields.Add(new InfoTableMetaData(compStatusValue.Label, compStatusValue.Value.ToString(), colData: 1));
            testData.Fields.Add(new InfoTableMetaData(recomValue.Label, recomValue.Value.ToString(), colLable: 2, colData: 1));
            testData.Rows.Add(new InfoTableMetaData(observationValue.Label, observationValue.Value.ToString()));
            testData.Rows.Add(new InfoTableMetaData(causeValue.Label, causeValue.Value.ToString()));
            testData.Rows.Add(new InfoTableMetaData(breachValue.Label, breachValue.Value.ToString()));
            testData.Rows.Add(new InfoTableMetaData(ReasonDenValue.Label, ReasonDenValue.Value.ToString()));
            testData.Rows.Add(new InfoTableMetaData(suggestScopeValue.Label, suggestScopeValue.Value.ToString()));

            if (itemPhotos != null)
            {
                testData.Images.AddRange(itemPhotos.Select(x => new ReportFile
                {
                    Name = x.Name,
                    Url= x.Url
                }));
            }

            Doc.Add(GenerateItemReportTable(testData, dataColNumber: 9));
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
                // create header table
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.TotalWidth = 820f;
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

                PdfPTable footerTable = new PdfPTable(1);
                footerTable.TotalWidth = 820f;
                footerTable.LockedWidth = true;
                footerTable.DefaultCell.Border = Rectangle.NO_BORDER;
                PdfPCell footerPageNum = new PdfPCell(new Phrase("Page " + writer.PageNumber.ToString(),
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
