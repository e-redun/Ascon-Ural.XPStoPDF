using Ascon.Pilot.SDK;
using System.Threading.Tasks;

namespace XPStoPDF.XpsConverters
{
    public interface IXpsToPdfConverter
    {
        Task GetFileData(IDataObject dataObject);
        Task ExportFileData(string pdfFileName);
    }
}