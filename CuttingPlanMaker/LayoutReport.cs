﻿using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttingPlanMaker
{
    class LayoutReport:ReportBase
    {
        public PdfSharp.Pdf.PdfDocument Generate(Settings Settings, BindingList<Material> Materials, BindingList<StockItem> Stock, BindingList<Part> Parts)
        {
            #region // populate header text ...
            document.Info.Title = "Layout Report";
            headerTable[2, 0].AddParagraph("Client:");
            headerTable[2, 1].AddParagraph(Settings.ClientName);
            //headerTable[2, 2].AddParagraph("Material:");
            //headerTable[2, 3].AddParagraph("Kiaat (AB)-25mm");

            headerTable[3, 0].AddParagraph("Tel nr.:");
            headerTable[3, 1].AddParagraph(Settings.ClientTelNr);
            headerTable[3, 2].AddParagraph("Kerf:");
            headerTable[3, 3].AddParagraph(Settings.BladeKerf);

            headerTable[4, 0].AddParagraph("Address:");
            headerTable[4, 1].AddParagraph(Settings.ClientAddr);
            headerTable[4, 2].AddParagraph("Part-padding:");
            headerTable[4, 3].AddParagraph($"{Settings.PartPaddingLength} x {Settings.PartPaddingWidth}");
            headerTable.Columns[2].Width = Unit.FromCentimeter(2.6);
            #endregion

            #region // write content into document ...
            var heading = mainSection.AddParagraph("Solution Diagram");
            heading.Format.Font.Bold = true;
            heading.Format.Font.Size = 10;
            heading.Format.Font.Underline = Underline.Single;

            mainSection.AddPageBreak();
            heading = mainSection.AddParagraph("Solution Detail");
            heading.Format.Font.Bold = true;
            heading.Format.Font.Size = 10;
            heading.Format.Font.Underline = Underline.Single;

            // Create table 
            Table table = mainSection.AddTable();
            table.Format.Font.Size = 9;
            table.AddColumn("4cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("4cm").Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn("1.5cm").Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn("1.5cm").Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn("1.5cm").Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn("2cm").Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn("1.5cm").Format.Alignment = ParagraphAlignment.Right;
            
            var iRow = table.AddRow();
            //table.Borders.Width = 1;
            table.Rows.LeftIndent = 10;
            iRow.HeadingFormat = true;
            iRow.Format.Font.Bold = true;
            iRow.Shading.Color = Colors.Gray;
            iRow[0].AddParagraph("Stock"); iRow[1].AddParagraph("Part"); iRow[2].AddParagraph("Length"); iRow[3].AddParagraph("Width"); iRow[4].AddParagraph("Thick"); iRow[5].AddParagraph("%/dLen"); iRow[6].AddParagraph("dWid");
            for (int i = 0; i < Stock.Count; i++)
            {
                var iStock = Stock[i];
                var iMaterial = Materials.First(t => t.Name == Stock[i].Material);

                iRow = table.AddRow();
                iRow.Shading.Color = Colors.LightGray;
                iRow.Format.Font.Bold = true;
                iRow[0].MergeRight = 1;
                iRow[0].Format.Alignment = ParagraphAlignment.Left;
                iRow[0].AddParagraph($"{iStock.Name} ({iStock.Material})");
                iRow[2].AddParagraph(iStock.Length.ToString("0.0"));
                iRow[3].AddParagraph(iStock.Width.ToString("0.0"));
                iRow[4].AddParagraph(iMaterial.Thickness.ToString("0.0"));
                iRow[5].AddParagraph($"({(iStock.PackedPartsTotalArea/iStock.Area).ToString("0.0%")})");

                for (int j = 0; j < iStock.PackedPartsCount; j++)
                {
                    var iPart = iStock.PackedParts[j];

                    iRow = table.AddRow();
                    if (j % 2 == 1) iRow.Shading.Color = Colors.WhiteSmoke;
                    iRow[0].MergeRight = 1;
                    iRow[0].Format.Alignment = ParagraphAlignment.Right;
                    iRow[0].AddParagraph(iPart.Name);
                    iRow[2].AddParagraph(iPart.Length.ToString("0.0"));
                    iRow[3].AddParagraph(iPart.Width.ToString("0.0"));
                    iRow[4].AddParagraph("@");
                    iRow[5].AddParagraph(iStock.PackedPartdLengths[j].ToString("0.0"));
                    iRow[6].AddParagraph(iStock.PackedPartdWidths[j].ToString("0.0"));
                }
                
            }

            mainSection.AddPageBreak();
            heading = mainSection.AddParagraph("Solution Summary");
            heading.Format.Font.Bold = true;
            heading.Format.Font.Size = 10;
            heading.Format.Font.Underline = Underline.Single;

            table = mainSection.AddTable();
            //table.Borders.Color = Colors.WhiteSmoke;
            table.Rows.LeftIndent = 10;
            table.AddColumn("3cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("0.3cm").Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn("1.5cm").Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn("1cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("0.3cm").Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn("1.5cm").Format.Alignment = ParagraphAlignment.Right;
            table.AddColumn("1.5cm").Format.Alignment = ParagraphAlignment.Left;
            table.AddRow();
            table.AddRow();
            table.AddRow();
            table.AddRow();
            table.AddRow();
            table.AddRow();
            table[0, 0].AddParagraph("Stock boards");
            table[1, 0].AddParagraph("Used boards");
            table[2, 0].AddParagraph("Parts");
            table[3, 0].AddParagraph("Placed parts");
            table[4, 0].AddParagraph("Waste");
            table[5, 0].AddParagraph("Coverage");

            double UsedStockArea = Stock.Where(q => q.isComplete).Sum(t => t.Area) / 1e6;
            double PlacedPartsArea = Parts.Where(q => q.isPacked).Sum(t => t.Area) / 1e6;

            table[0, 2].AddParagraph(Stock.Count.ToString());
            table[0, 5].AddParagraph((Stock.Sum(t => t.Area)/1e6).ToString("0.000"));

            table[1, 2].AddParagraph(Stock.Count(t=>t.isComplete).ToString());
            table[1, 5].AddParagraph(UsedStockArea.ToString("0.000"));

            table[2, 2].AddParagraph(Parts.Count.ToString());
            table[2, 5].AddParagraph((Parts.Sum(t => t.Area)/1e6).ToString("0.000"));

            table[3, 2].AddParagraph(Parts.Count(t => t.isPacked).ToString());
            table[3, 5].AddParagraph(PlacedPartsArea.ToString("0.000"));


            table[4, 2].AddParagraph(((UsedStockArea-PlacedPartsArea)/UsedStockArea*100).ToString("0.0"));
            table[4, 5].AddParagraph((UsedStockArea - PlacedPartsArea).ToString("0.000"));
            table[5, 2].AddParagraph((PlacedPartsArea/UsedStockArea*100).ToString("0.0"));
            table[5, 5].AddParagraph(PlacedPartsArea.ToString("0.000"));

            for (int i = 0; i < 6; i++)
            {
                table[i, 1].AddParagraph(":");
                table[i, 4].AddParagraph("(");
                table[i, 6].AddParagraph("m\u00b2)");
            }
            table[4, 3].AddParagraph("%");
            table[5, 3].AddParagraph("%");


            #endregion
            return RenderPdf();
        }
    }
}
