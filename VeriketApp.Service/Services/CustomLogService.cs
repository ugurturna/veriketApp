using Microsoft.Extensions.Options;
using System.Security.Principal;
using VeriketApp.Core;

namespace VeriketApp.Service.Services
{
    internal class CustomLogService : ICustomLogService
    {
        private readonly AppSettings _settings;
        private readonly string BaseFilePath;

        public CustomLogService(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
            BaseFilePath = Path.Combine(_settings.BaseFilePath, _settings.ServiceName, _settings.LogFileName);
        }
        private static async Task<string> PrepareLogFile() => $"{DateTime.UtcNow},{Environment.MachineName},{System.DirectoryServices.AccountManagement.UserPrincipal.Current.Name ?? Environment.UserName}";
        public async Task WriteLog()
        {
            try
            {
                if (File.Exists(BaseFilePath))
                {
                    using (StreamWriter sw = File.AppendText(BaseFilePath))
                    {
                        sw.WriteLine(await PrepareLogFile());
                    }

                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(BaseFilePath));
                    using (StreamWriter sw = File.AppendText(BaseFilePath))
                    {
                        sw.WriteLine(await PrepareLogFile());
                    }

                }

                return;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
