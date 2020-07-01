using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

namespace PdfPlayGround
{
    public class PdfBase
    {
        private static string FilePath => Path.Combine(Directory.GetCurrentDirectory(), "Pdf");
        public string FileName { get; protected set; } = $"{DateTime.Today.ToString("dd-MM-yyyy")}-{Guid.NewGuid().ToString()}.pdf";
        public string FileLocation => Path.Combine(FilePath, FileName);


        // Content Data
        public string Date { get; protected set; } = DateTime.Today.ToShortDateString();
        public string Title { get; protected set; } = string.Empty;

        // Pdf Setting
        protected Rectangle PageInfo = new Rectangle(PageSize.A4);
        protected Margin PageMargin = new Margin(20, 20, 40, 60);

        // Page Property
        protected float PageContentWidth => PageInfo.Width - (PageMargin.Left + PageMargin.Right);
        protected float PageContentHeight => PageInfo.Height - (PageMargin.Top + PageMargin.Bottom);
        protected float PageAvailableSpace => Writer.GetVerticalPosition(true) - Doc.BottomMargin;


        protected PdfPageEventHelper PdfPageEvent;

        // Pdf Generator
        protected Document Doc;
        protected PdfWriter Writer;

        // Font and style
        protected virtual BaseColor ThemePrimary => BaseColor.Black;
        protected virtual BaseColor ThemeDark => BaseColor.Black;
        protected virtual BaseColor ThemeLight => BaseColor.Gray;
        protected virtual BaseColor ThemeTableBorder => BaseColor.Black;
        protected virtual BaseColor ThemeTableHeadShading => BaseColor.LightGray;
        protected virtual BaseColor ThemeAudit => BaseColor.Red;

        protected virtual Font StyleTiltleHeader => FontFactory.GetFont(BaseFont.HELVETICA_BOLD, 22, Font.BOLD, ThemeDark);
        protected virtual Font StyleHeader => FontFactory.GetFont(BaseFont.HELVETICA, 14, Font.BOLD, ThemePrimary);
        protected virtual Font StyleContent => FontFactory.GetFont(BaseFont.HELVETICA, 9);
        protected virtual Font StyleContentBold => FontFactory.GetFont(BaseFont.HELVETICA, 9, Font.BOLD);
        protected virtual Font StyleContentSmall => FontFactory.GetFont(BaseFont.HELVETICA, 8);
        protected virtual Font StyleContentSmallBold => FontFactory.GetFont(BaseFont.HELVETICA, 8, Font.BOLD);
        protected virtual Font StyleContentHeader => FontFactory.GetFont(BaseFont.HELVETICA, 9, Font.BOLD, ThemePrimary);
        protected virtual Font StyleAudit => FontFactory.GetFont(BaseFont.HELVETICA, 9, ThemeAudit);
        protected virtual Font StyleAuditBold => FontFactory.GetFont(BaseFont.HELVETICA, 9, Font.BOLD, ThemeAudit);
        protected virtual Font StyleAuditSmall => FontFactory.GetFont(BaseFont.HELVETICA, 8, ThemeAudit);
        protected virtual Font StyleAuditSmallBold => FontFactory.GetFont(BaseFont.HELVETICA, 8, Font.BOLD, ThemeAudit);
        protected virtual Font StyleAuditHeader => FontFactory.GetFont(BaseFont.HELVETICA, 9, Font.BOLD, ThemeAudit);

        public PdfBase()
        {
            //if (!FontFactory.IsRegistered("Arial"))
            //{
            //    FontFactory.RegisterDirectory(Path.Combine(CONST.FOLDER_RESOURCE, "Font"));
            //}
        }

        protected virtual void FillTemplate() { }
        protected virtual void InitialPdf() { }

        protected virtual void WriteDocument()
        {
            InitialPdf();
            Writer.PageEvent = null;
            Writer.PageEvent = PdfPageEvent;
            if (!Doc.IsOpen())
            {
                Doc.Open();
            }
        }

        protected PdfPTable DividingLine
        {
            get
            {
                var linetable = new PdfPTable(1)
                {
                    TotalWidth = PageContentWidth,
                    LockedWidth = true
                };
                linetable.DefaultCell.MinimumHeight = 5;
                linetable.DefaultCell.Border = Rectangle.BOTTOM_BORDER;
                linetable.DefaultCell.BorderColor = ThemeTableBorder;
                linetable.DefaultCell.BorderWidth = 1f;
                linetable.AddCell("");
                return linetable;
            }
        }

        protected PdfPTable GenerateInfoTable(List<InfoTableMetaData> model, byte columNum = 4, float[] widths = null, PdfPCell headerCell = null, PdfPCell contentCell = null, PdfPCell titleCell = null)
        {
            PdfPTable table = new PdfPTable(columNum)
            {
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
                TotalWidth = PageContentWidth,
                SpacingAfter = 20f,
                LockedWidth = true
            };
            if (widths == null)
            {
                widths = new float[columNum];
                for (int i = 0; i < columNum; i++)
                {
                    widths[i] = i % 2 == 0 ? 4f : 5f;
                }
            }
            table.SetWidths(widths);
            if (titleCell != null)
            {
                titleCell.Colspan = columNum;
                table.AddCell(titleCell);
            }
            PdfPCell cellHeader = headerCell ?? new PdfPCell()
            {
                Border = Cell.BOX,
                BorderColor = ThemeTableBorder,
                BackgroundColor = ThemeTableHeadShading,
                VerticalAlignment = Cell.ALIGN_MIDDLE,
                Padding = 8f
            };
            PdfPCell cellContent = contentCell ?? new PdfPCell()
            {
                Border = Cell.BOX,
                BorderColor = ThemeTableBorder,
                VerticalAlignment = Cell.ALIGN_MIDDLE,
                Padding = 8f
            };
            if (model != null)
            {
                foreach (var item in model)
                {
                    if (item.ColspanLabel > 0) 
                    {
                        table.AddCell(cellHeader.SetContent(new Phrase(item.Name, StyleContentHeader), item.ColspanLabel));
                    }
                    if (item.ColspanContent > 0)
                    {
                        table.AddCell(cellContent.SetContent(new Phrase(item.Content, StyleContent), item.ColspanContent));
                    }
                }
            }
            table.CompleteRow();
            return table;
        }

        protected PdfPTable GenerateListTable(List<TableHeaderMetaData> model, PdfPCell defaultcell = null, PdfPCell headercell = null, Font styleheader = null)
        {
            PdfPTable table = new PdfPTable(model.Count)
            {
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
                TotalWidth = PageContentWidth,
                SpacingAfter = 15f,
                LockedWidth = true
            };
            table.SetWidths(model.Select(x => x.Width).ToArray());
            table.DefaultCell.BorderColor = defaultcell?.BorderColor ?? ThemeTableBorder;
            table.DefaultCell.Border = defaultcell?.Border ?? Rectangle.BOX;
            table.DefaultCell.VerticalAlignment = defaultcell?.VerticalAlignment ?? Cell.ALIGN_MIDDLE;
            table.DefaultCell.PaddingBottom = defaultcell?.PaddingBottom ?? 8f;
            table.DefaultCell.PaddingTop = defaultcell?.PaddingTop ?? 8f;
            table.DefaultCell.PaddingLeft = defaultcell?.PaddingLeft ?? 8f;
            table.DefaultCell.PaddingRight = defaultcell?.PaddingRight ?? 8f;

            PdfPCell cell = new PdfPCell(headercell ?? table.DefaultCell);
            if (headercell == null) { cell.BackgroundColor = ThemeTableHeadShading; }

            foreach (var col in model)
            {
                cell.HorizontalAlignment = col.HorizontalAlign;
                table.AddCell(cell.SetContent(new Phrase(col.Name, styleheader ?? StyleContentHeader)));
            }
            return table;
        }

        private void ProducePdf(Stream stream)
        {
            FillTemplate();
            FileName = FileName.ToValidFilename();

            Doc = new Document(PageInfo, PageMargin.Left, PageMargin.Right, PageMargin.Top, PageMargin.Bottom);
            Writer = PdfWriter.GetInstance(Doc, stream);

            WriteDocument();
            if (Doc.IsOpen())
            {
                Doc.Close();
            }
        }

        public string GeneratePdf(string path = null)
        {
            path = path ?? FileLocation;
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                ProducePdf(fs);
            }
            string filePath = $"{dir}\\{FileName}";
            return filePath;
        }

        protected class PdfPageEventBase : PdfPageEventHelper
        {
            protected PdfBase BaseDocument;
            public PdfPageEventBase(PdfBase pdfbase)
            {
                BaseDocument = pdfbase;
            }
        }

        protected struct Margin
        {
            public float Top;
            public float Bottom;
            public float Left;
            public float Right;
            public Margin(float left, float right, float top, float bottom)
            {
                Top = top;
                Bottom = bottom;
                Left = left;
                Right = right;
            }
        }

        protected struct TableHeaderMetaData
        {
            public string Name;
            public float Width;
            public int HorizontalAlign;
            public TableHeaderMetaData(string name, float width, int align = Rectangle.ALIGN_LEFT)
            {
                Name = name;
                Width = width;
                HorizontalAlign = align;
            }
        }
        protected struct InfoTableMetaData
        {
            public string Name;
            public string Content;
            public int ColspanLabel;
            public int ColspanContent;
            public InfoTableMetaData(string name, string data, int colLable = 1, int colData = 1)
            {
                Name = name;
                Content = data;
                ColspanLabel = colLable;
                ColspanContent = colData;
            }
        }
    }


    public static class PdfExtention
    {
        public static PdfPCell GenNestedCell(this PdfPTable table)
        {
            table.LockedWidth = false;
            table.SpacingAfter = 0;
            PdfPCell nestedCell = new PdfPCell(table)
            {
                Border = Rectangle.NO_BORDER
            };
            return nestedCell;
        }

        public static PdfPCell SetContent(this PdfPCell cell, Phrase phrase, int colspan = 0)
        {
            if (colspan > 0) { cell.Colspan = colspan; }
            cell.Phrase = phrase;
            return cell;
        }
    }
}
