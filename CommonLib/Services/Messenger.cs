using System.Threading.Tasks;
using System.Windows;

namespace CommonLib.Services
{
    public static class Messenger
    {
        public static async Task MesAsync(object obj)
        {
            Task.Run(() => MessageBox.Show(obj.ToString()));
        }
    }
}