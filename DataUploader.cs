using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace String2ServerService_ClientPart
{
    public static class DataUploader
    {
        private static readonly HttpClientHandler clientHandler = new HttpClientHandler() { UseProxy = false };
        private static readonly HttpClient client = new HttpClient(clientHandler);


        /// <summary>
        /// Отправляет файл на сервер из настроек.
        /// Возвращает Response
        /// </summary>
        public async static Task<HttpResponseMessage> SendStringToServerAsync(string data = "")
        {
            string targetURL = ProgramSettings.ServerURL + ProgramSettings.UploadStringPostURL;


            using var form = new MultipartFormDataContent();

            StringContent mainStringContent = new StringContent(data);
            form.Add(mainStringContent, "MainString");

            var response = await client.PostAsync(targetURL, form);

            return response;
        }
    }
}
