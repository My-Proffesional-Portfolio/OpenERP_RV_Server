using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using OpenERP_RV_Server.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend.PDF
{
    public class PdfService : BaseService
    {
        static iTextSharp.text.Font headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 15, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        static iTextSharp.text.Font subHeaderFont = new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        static iTextSharp.text.Font tableHeaderFont = new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        static iTextSharp.text.Font normalTextFont = new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 9, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        static iTextSharp.text.Font smallFont = new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.Black);
        static iTextSharp.text.Font boldHeaderFont = new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 9, iTextSharp.text.Font.BOLD, BaseColor.Black);
        static iTextSharp.text.Font xsmallFont = new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.Black);



        public byte[] PrintExpenseItemsPDF()
        {
            var expenseItems = new ExpenseService().GetExpenseItems();

            MemoryStream workStream = new MemoryStream();
            Document doc = new Document();

            PdfWriter.GetInstance(doc, workStream).CloseStream = false;


            doc.AddTitle("Listado de activos y servicios adquiridos al " + DateTime.Now);
            doc.AddCreator("Open ERP");

            doc.Open();
            //iTextSharp.text.Font smallFont = new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.Black);

            var companyData = new CompanyOrganizationService().GetCompanyByID(Guid.Parse(accessor.HttpContext.Session.GetString("companyID")));

            InsertHeaderExpenseItemsDocument(companyData, doc);
            InsertProductAndServiceItems(expenseItems, doc);


            doc.Close();
            var docBytes = workStream.ToArray();
            File.WriteAllBytes("Listado al " + DateTime.Now.ToString("yyy-MM-dd") + ".pdf", docBytes);
            return docBytes;


        }


        public void InsertHeaderExpenseItemsDocument(Company companyData, Document doc)
        {
            AddParagraph(doc,  companyData.CommercialName, normalTextFont);
            AddParagraph(doc, companyData.Address, smallFont);
            AddParagraph(doc, companyData.CorporateOffice.ContactName, smallFont);
            //AddParagraph(doc, "Folio fiscal " + cfdi., subHeaderFont);
            doc.Add(Chunk.Newline);

        }

        private void AddParagraph(Document doc, string text, Font font, int alligment = Element.ALIGN_CENTER)
        {
            var invoiceNameParagraph = new Paragraph(text, font);
            invoiceNameParagraph.Alignment = alligment;
            doc.Add(invoiceNameParagraph);
            //doc.Add(Chunk.Newline);
        }

        internal void InsertProductAndServiceItems(IOrderedQueryable<ExpenseItem> expenseItems, Document doc)
        {

            float[] anchoDeColumnas = new float[] { 34f, 18f, 6f, 6f, 6f };
            PdfPTable table = new PdfPTable(5);
            table.SetWidths(anchoDeColumnas);
            PdfPCell cell = new PdfPCell(new Phrase("Productos y servicios"));
            cell.Colspan = 5;
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.BackgroundColor = new iTextSharp.text.BaseColor(194, 235, 236);
            table.AddCell(cell);
            //table.AddCell("Producto");
            InsertConfiguredCellItem(table, "Producto", isHeader: true);
            InsertConfiguredCellItem(table, "Proveedor", isHeader: true);
            InsertConfiguredCellItem(table, "Cantidad", isHeader: true);
            InsertConfiguredCellItem(table, "Entregado", isHeader: true);
            InsertConfiguredCellItem(table, "CFDI", isHeader: true);


            foreach (var item in expenseItems)
            {
                InsertConfiguredCellItem(table, item.Description);
                InsertConfiguredCellItem(table, item.Expense.Supplier.CompanyName);
                InsertConfiguredCellItem(table, item.Quantity.ToString());
                InsertConfiguredCellItem(table, item.FullFilled.Value ? "OK" : "NO");
                InsertConfiguredCellItem(table, item.Expense.Uuid.HasValue ? "OK" : "NO");
            }

            //InsertResumeRecord("Subtotal", "$" + cfdi.SubTotal.ToString() + " " + cfdi.Moneda.ToString(), table);
            //InsertResumeRecord("Impuestos", "$" + cfdi.Impuestos.TotalImpuestosTrasladados.ToString() + " " + cfdi.Moneda.ToString(), table);
            //InsertResumeRecord("Total", "$" + cfdi.Total.ToString() + " " + cfdi.Moneda.ToString(), table);

            doc.Add(table);
            doc.Add(Chunk.Newline);
        }

        private static void InsertResumeRecord(string label, string value, PdfPTable table)
        {
            InsertConfiguredCellItem(table, "", isBlankCell: true);
            InsertConfiguredCellItem(table, "", isBlankCell: true);
            InsertConfiguredCellItem(table, "", isBlankCell: true);
            InsertConfiguredCellItem(table, label);
            InsertConfiguredCellItem(table, value);
        }

        private static void InsertConfiguredCellItem(PdfPTable table, string content, string subContent = "", bool isHeader = false, bool isBlankCell = false)
        {
           
            var text = new Paragraph(content, isHeader ? boldHeaderFont : smallFont);
            var productCelll = new PdfPCell();
            text.Alignment = Element.ALIGN_CENTER;
            productCelll.AddElement(text);
            if (!string.IsNullOrWhiteSpace(subContent))
            {
                var subtext = new Paragraph(subContent, xsmallFont);
                subtext.Alignment = Element.ALIGN_CENTER;
                productCelll.AddElement(subtext);
            }

            if (isBlankCell)
                productCelll.Border = Rectangle.NO_BORDER;

            table.AddCell(productCelll);
        }

        public void AddRowInTable(PdfPTable table, string rowTitle, string value)
        {

            table.AddCell(new Phrase(rowTitle, normalTextFont));
            table.AddCell(new Phrase(value, normalTextFont));


        }

    }
}
