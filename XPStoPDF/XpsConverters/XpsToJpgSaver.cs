using Ascon.Pilot.SDK;
using PdfSharp.Pdf;
using System;
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
    public class XpsToJpgSaver : IXpsToPdfConverter
    {
        private readonly double _scale = 1;
        private string _xpsFilePath;
        public const string LABEL = "XpsToJpgSaver";


        public async Task GetFileData(IDataObject dataObject)
        {
            var _fso = new FileSaverOptions
            {
                GraphicLayerOption = GraphicLayerOption.ForceInject
            };

            _xpsFilePath = Path.GetTempFileName();

            await PilotServices.FileSaver.SaveFileAsync(dataObject, _xpsFilePath, _fso);
        }



        public async Task ExportFileData(string pdfFilePath)
        {
            var pdfFileDir = Path.GetDirectoryName(pdfFilePath);
            var pdfFileName = Path.GetFileNameWithoutExtension(pdfFilePath);

            await Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
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

                        var suffix = " (" + (i + 1).ToString() + ")";
                        var newFilePath = Path.Combine(pdfFileDir, pdfFileName + suffix + ".jpg");

                        FileHelper.MoveFile(pageFilePath, newFilePath);
                    }
                }
            }
            ));
        }
    }
}