using Ascon.Pilot.SDK;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Xps.Packaging;
using XPStoPDF.Services;
using IDataObject = Ascon.Pilot.SDK.IDataObject;

namespace XPStoPDF.XpsConverters
{
    public class XpsStreamConverter : IXpsToPdfConverter
    {
        private double _scale = 1;

        private IXpsRender _xpsRender = PilotServices.XpsRender;
        private Stream _stream;
        public const string LABEL = "XpsStreamConverter";


        public async Task GetFileData(IDataObject dataObject)
        {
            _stream = PilotServices.FileProvider.OpenRead(dataObject.Files.First());
        }


        public async Task ExportFileData(string pdfFileName)
        {
            await Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                Uri uri = new Uri(string.Format("memorystream://temp{0}", Guid.NewGuid().ToString("N")));
                
                FixedDocumentSequence pageSource;

                XpsDocument xpsDocument;
                
                List<DocumentPage> xpsPages = new List<DocumentPage>();

                IEnumerable<Stream> previews;

                using (var package = Package.Open(_stream, FileMode.Open, FileAccess.Read))
                {
                    PackageStore.AddPackage(uri, package);

                    xpsDocument = new XpsDocument(package, CompressionOption.SuperFast, uri.ToString());

                    pageSource = xpsDocument.GetFixedDocumentSequence();

                    DocumentPaginator paginator = pageSource.DocumentPaginator;

                    previews = _xpsRender.RenderXpsToBitmap(_stream, _scale);
                    
                    for (var i = 0; i < previews.Count(); i++)
                    {
                        xpsPages.Add(paginator.GetPage(i));
                    }
                }


                //MessageBox.Show("xpsPages.Size=" + xpsPages.Count());
                Stream preview;

                using (var document = new PdfDocument())
                {
                    for (var i = 0; i < xpsPages.Count(); i++)
                    {
                        DocumentPage xpsPage = xpsPages[i];

                        preview = previews.ToList()[i];

                        preview.Position = 0;

                        PdfPage pdfPage = document.AddPage();

                        var coef = pdfPage.Height.Point / pdfPage.Height.Presentation;

                        XGraphics gfx;
                        using (XImage img = XImage.FromStream(preview))
                        {
                            pdfPage.Width = coef * xpsPage.Size.Width;
                            pdfPage.Height = coef * xpsPage.Size.Height;

                            gfx = XGraphics.FromPdfPage(pdfPage);

                            gfx.DrawImage(img, 0, 0, pdfPage.Width, pdfPage.Height);
                        }
                    }

                    document.Save(pdfFileName);
                }

                PackageStore.RemovePackage(uri);

            }));
        }

        public void ExportFileDataTest(string pdfFileName, IFile xpsFile)
        {
            using (Stream stream = PilotServices.FileProvider.OpenRead(xpsFile))
            {   using (var package = Package.Open(stream, FileMode.Open, FileAccess.Read))
                {   var uri = new Uri(string.Format("memorystream://temp{0}", Guid.NewGuid().ToString("N")));
                    PackageStore.AddPackage(uri, package);
                    var xpsDocument = new XpsDocument(package, CompressionOption.SuperFast, uri.ToString());
                    IDocumentPaginatorSource pageSource = xpsDocument.GetFixedDocumentSequence();
                    DocumentPaginator paginator = pageSource.DocumentPaginator;
                    IEnumerable<Stream> previews = _xpsRender.RenderXpsToBitmap(stream, _scale);
                    using (var document = new PdfSharp.Pdf.PdfDocument())
                    {
                        for (var i = 0; i < previews.Count(); i++)
                        {
                            var xpsPage = paginator.GetPage(i);
                            // обработка
                        }
                        document.Save(pdfFileName);
                    }
                    PackageStore.RemovePackage(uri);
                }
            }
        }


        public void ProcessFile(string xpsFilePath, string pdfFilePath)
        {
            if (!File.Exists(xpsFilePath))
            {
                return;
            }

            DocumentPaginator paginator = null;

            Application.Current.Dispatcher.Invoke(() =>
            {

                XpsDocument xpsDocument = new XpsDocument(xpsFilePath, FileAccess.Read);

                IDocumentPaginatorSource pageSource = xpsDocument.GetFixedDocumentSequence();

                paginator = pageSource.DocumentPaginator;

                var xpsStream = File.OpenRead(xpsFilePath);

                IEnumerable<Stream> previews = _xpsRender.RenderXpsToBitmap(xpsStream, _scale);

                DocumentPage xpsPage;
                Stream preview;

                using (var document = new PdfDocument())
                {
                    for (var i = 0; i < previews.Count(); i++)
                    {
                        xpsPage = paginator.GetPage(i);

                        preview = previews.ToList()[i];

                        preview.Position = 0;

                        PdfPage pdfPage = document.AddPage();

                        var coef = pdfPage.Height.Point / pdfPage.Height.Presentation;

                        XGraphics gfx;
                        using (XImage img = XImage.FromStream(preview))
                        {
                            pdfPage.Width = coef * xpsPage.Size.Width;
                            pdfPage.Height = coef * xpsPage.Size.Height;

                            gfx = XGraphics.FromPdfPage(pdfPage);

                            gfx.DrawImage(img, 0, 0, pdfPage.Width, pdfPage.Height);
                        }
                    }

                    document.Save(pdfFilePath);
                }
            });
        }


        public void GetAndSavePdfDocument()
        {
            throw new NotImplementedException();
        }

        public Task GetFileData(string xpsFilePath)
        {
            throw new NotImplementedException();
        }
    }
}