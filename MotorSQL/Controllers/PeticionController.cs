using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MotorSQL.Data;
using MotorSQL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MotorSQL.DB;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using ClosedXML.Excel;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Grid;

namespace MotorSQL.Controllers
{
    public class PeticionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public PeticionController(IConfiguration configuration, ApplicationDbContext context)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("ConSQL");
        }

        public IActionResult Index()
        {   
            return View();
        }

        [HttpPost]
        public IActionResult ConsultaPeticion(PeticionSQL consulta)
        {
            DataSet ds = new DataSet();
            var result = new List<CatalogoModelo>();
            consulta.FechaEjecucion = DateTime.Now;
            if (ModelState.IsValid)
            {
                MensajeRespuesta respuesta = new MensajeRespuesta();
                SQL con = new SQL(_connectionString);
                ds  = con.ConsultaGeneral(consulta, ref respuesta);
                
                if (respuesta.Codigo != 0)
                {
                    TempData["mensaje"] = respuesta.Mensaje;
                    return RedirectToAction("Index", result);
                }

                if (respuesta.CantidadTablas > 0)
                {
                    foreach (DataTable item in ds.Tables)
                    {
                        if (consulta.TipoReporte == "XLSX")
                        {
                            return Excel(item);
                        }
                        else if (consulta.TipoReporte == "CSV")
                        {
                            return Csv(item);
                        }
                        else if (consulta.TipoReporte == "PDF")
                        {
                            return PDF(item);
                        }
                        else if (consulta.TipoReporte == "TXT")
                        {
                            string ruta = "";
                            return TXT(item, ruta );
                        }
                    }
                }
            }
            return View();
        }

        public FileContentResult Csv(DataTable tabla)
        {
            string nombre = tabla.TableName + DateTime.Now.ToString("yyyyMMddHHmmss");
            var builder = new StringBuilder();
            string line = "";
            int columnas = tabla.Columns.Count;

            for (int i = 0; i < columnas; i++)
            {
                if (i == (columnas - 1))
                {
                    line = line + tabla.Columns[i].ColumnName;
                }
                else
                {
                    line = line + tabla.Columns[i].ColumnName + ",";
                }
            }
            builder.AppendLine(line);
            foreach (DataRow fila in tabla.Rows)
            {
                string linea = "";
                for (int i = 0; i < columnas; i++)
                {
                    if (i == (columnas - 1))
                    {
                        linea = linea + fila[i].ToString();
                    }
                    else
                    {
                        linea = linea + fila[i].ToString() + ",";
                    }
                }
                builder.AppendLine(linea);
            }
            return File(
                Encoding.UTF8.GetBytes(builder.ToString()), 
                "text/csv", 
                nombre+".csv");
        }

        public FileContentResult Excel(DataTable tabla)
        {
            using (var workbook = new XLWorkbook())
            {
                string nombre = tabla.TableName + DateTime.Now.ToString("yyyyMMddHHmmss");
                var worksheet = workbook.Worksheets.Add(nombre);
                var currentRow = 1;
                int columnas = tabla.Columns.Count;
                
                for (int i = 0; i < columnas; i++)
                {
                    worksheet.Cell(currentRow, i+1).Value = tabla.Columns[i].ColumnName;
                }
                
                foreach (DataRow fila in tabla.Rows)
                {
                    currentRow++;
                    for (int i = 0; i < columnas; i++)
                    {
                        worksheet.Cell(currentRow, i+1).Value = fila[i].ToString();
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        nombre+".xlsx");
                }
            }
        }

        public FileContentResult PDF(DataTable tabla)
        {
            string nombre = tabla.TableName + DateTime.Now.ToString("yyyyMMddHHmmss");
            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();
            //Add a page.
            PdfPage page = doc.Pages.Add();
            //Create a PdfGrid.
            PdfGrid pdfGrid = new PdfGrid();
            //Add values to list
            //List<object> data = new List<object>();
            //Assign data source.
            pdfGrid.DataSource = tabla;
            //Draw grid to the page of PDF document.
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(10, 10));
            //Save the PDF document to stream
            //MemoryStream stream = new MemoryStream();
            using (var stream = new MemoryStream())
            {
                doc.Save(stream);
                //If the position is not set to '0' then the PDF will be empty.
                stream.Position = 0;
                //Close the document.
                doc.Close(true);
                var content = stream.ToArray();
                //Defining the ContentType for pdf file.
                string contentType = "application/pdf";
                //Define the file name.
                string fileName = nombre+".pdf";
                //Creates a FileContentResult object by using the file contents, content type, and file name.
                return File(content, contentType, fileName);
            }
        }

        public FileContentResult TXT(DataTable tabla, string ruta)
        {
            string nombre = tabla.TableName + DateTime.Now.ToString("yyyyMMddHHmmss");
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Reportes\");
            ruta = path + nombre;
            //string path = @"D:\";
            var currentRow = 1;
            int columnas = tabla.Columns.Count;
            string linea = "";

            string[] txtList = Directory.GetFiles(path, "");

            foreach (string f in txtList)
            {
                System.IO.File.Delete(f);
            }

            /*if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }*/

            using (StreamWriter SW = new StreamWriter(path + nombre))
            {
                for (int i = 0; i < columnas; i++)
                {
                    linea = linea + tabla.Columns[i].ColumnName + "\t";
                }
                SW.WriteLine(linea);

                linea = "";
                foreach (DataRow fila in tabla.Rows)
                {
                    currentRow++;
                    for (int i = 0; i < columnas; i++)
                    {
                        linea = linea + fila[i].ToString() + "\t";
                    }
                    SW.WriteLine(linea);
                    linea = "";
                }
                SW.Close();
            }

            using (StreamReader sr = new StreamReader(path + nombre))
            {
                using (var stream = new MemoryStream())
                {
                    sr.BaseStream.CopyTo(stream);
                    var content = stream.ToArray();
                    string contentType = "text/plain";                    

                    return File(
                        content,
                        contentType,
                        nombre + ".txt");
                }
            }
        }
    }
}
