
namespace String2ServerService_ClientPart
{
    public static class ProgramSettings
    {
        public static readonly string ServerURL = "https://mygamefeedback.loca.lt";

        public static readonly string CheckLifeGetURL = "/";

        public static readonly string UploadStringPostURL = "/SendString";


        public static readonly string NonSendedFilesPath = "NonSendedFiles/";



        public static string GetFullURL(string[] urls) => string.Join("", urls);
    }
}