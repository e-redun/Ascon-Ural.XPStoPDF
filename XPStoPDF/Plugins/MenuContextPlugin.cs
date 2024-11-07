using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Menu;
using XPStoPDF.Services;
using XPStoPDF.XpsConverters;
using IDataObject = Ascon.Pilot.SDK.IDataObject;

namespace XPStoPDF
{
    [Export(typeof(IMenu<ObjectsViewContext>))]
    public class MenuContextPlugin : IMenu<ObjectsViewContext>
    {
        [ImportingConstructor]
        public MenuContextPlugin(PilotServices pilotServices)
        {}


        public void Build(IMenuBuilder builder, ObjectsViewContext context)
        {
            var sendButton = builder.ReplaceItem("miSend");

            var sendMenu = sendButton.WithSubmenu();

            sendMenu.AddSeparator(sendMenu.Count);

            sendMenu.AddItem(XpsSaveConverter.LABEL, sendMenu.Count)
                    .WithHeader(XpsSaveConverter.LABEL);

            sendMenu.AddItem(XpsStreamConverter.LABEL, sendMenu.Count)
                    .WithHeader(XpsStreamConverter.LABEL);

            sendMenu.AddItem(XpsToJpgSaver.LABEL, sendMenu.Count)
                    .WithHeader(XpsToJpgSaver.LABEL);

        }


        public async void OnMenuItemClick(string name, ObjectsViewContext context)
        {

            if (context.SelectedObjects == null ||
                context.SelectedObjects.Count() != 1
                )
            {
                return;
            }

            var xpsedObject = context.SelectedObjects.First(p => FileHelper.CheckOnXps(p));

            if (xpsedObject == null)
            {
                return;
            }

            var converter = InitCoverter(name);

            if (converter == null)
            {
                return;
            }


            var pdfFilePath = GetPdfFilePath(xpsedObject.DisplayName);

            if (pdfFilePath == "")
            {
                return;
            }

            //var stopwatch = new Stopwatch();
        
            //stopwatch.Start();

            await converter.GetFileData(xpsedObject);

            //var tmp1 = stopwatch.Elapsed.TotalSeconds;

            await converter.ExportFileData(pdfFilePath);

            //stopwatch.Stop();

            //var tmp2 = stopwatch.Elapsed.TotalSeconds;

            //var tmp = "tmp1.TotalSeconds :" + tmp1 + "\n";
            //tmp += "tmp2.TotalSeconds :" + (tmp2 - tmp1);

            //MessageBox.Show(tmp);
        }

        private string GetPdfFilePath(string displayName)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = displayName;
            var result = sfd.ShowDialog();

            if (result == DialogResult.OK)
            {
                return sfd.FileName + ".pdf";
            }

            return "";
        }

        private IXpsToPdfConverter InitCoverter(string name)
        {
            switch (name)
            {
                case XpsSaveConverter.LABEL:
                    return new XpsSaveConverter();

                case XpsStreamConverter.LABEL:
                    return new XpsStreamConverter();

                case XpsToJpgSaver.LABEL:
                    return new XpsToJpgSaver();

                default:
                    return null;
            }
        }
    }
}