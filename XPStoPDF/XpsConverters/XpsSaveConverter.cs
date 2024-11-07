using Ascon.Pilot.SDK;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Xps.Packaging;
using XPStoPDF.Services;
using IDataObject = Ascon.Pilot.SDK.IDataObject;

namespace XPStoPDF.XpsConverters
{
    public class XpsSaveConverter : IXpsToPdfConverter
    {
        private double _scale = 1;
        private string _xpsFilePath;
        public const string LABEL = "XpsSaveConverter";
        private List<string> _jpgFiles;
        private PdfDocument _pdfDocument;
        private string _pdfFileName;
        private FileSaverOptions _fso;


        public async Task GetFileData(IDataObject dataObject)
        {
            _xpsFilePath = Path.GetTempFileName();

            _fso = new FileSaverOptions
            {
                GraphicLayerOption = GraphicLayerOption.ForceInject
            };

            await PilotServices.FileSaver.SaveFileAsync(dataObject, _xpsFilePath, _fso);
        }


        public async Task ExportFileData(string pdfFileName)
        {
            await Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                _pdfFileName = pdfFileName;
                _jpgFiles = new List<string>();

                FixedDocumentSequence pageSource;

                using (var xpsDocument = new XpsDocument(_xpsFilePath, FileAccess.Read))
                {
                    pageSource = xpsDocument.GetFixedDocumentSequence();
                }

                DocumentPaginator paginator = pageSource.DocumentPaginator;

                using (var document = new PdfDocument())
                {
                    for (var i = 0; i < paginator.PageCount; i++)
                    {
                        FrameworkElement fe;

                        using (DocumentPage xpsPage = paginator.GetPage(i))
                        {
                            fe = xpsPage.Visual as FrameworkElement;
                        }

                        if (fe == null)
                        {
                            return;
                        }

                        var pageFilePath = FileHelper.GetJpgFilePath(fe, _scale);

                        if (pageFilePath == "")
                        {
                            return;
                        }

                        _jpgFiles.Add(pageFilePath);
                    }
                }

                _pdfDocument = new PdfDocument();

                foreach (var jpgFile in _jpgFiles)
                {
                    PdfPage pdfPage = _pdfDocument.AddPage();

                    using (XImage img = XImage.FromFile(jpgFile))
                    {
                        pdfPage.Width = img.PointWidth;
                        pdfPage.Height = img.PointHeight;

                        using (var gfx = XGraphics.FromPdfPage(pdfPage))
                        {
                            gfx.DrawImage(img, 0, 0, pdfPage.Width, pdfPage.Height);
                        }
                    }

                    if (File.Exists(jpgFile))
                    {
                        File.Delete(jpgFile);
                    }
                }

                _pdfDocument.Save(_pdfFileName);
            }
            ));
        }
    }
}