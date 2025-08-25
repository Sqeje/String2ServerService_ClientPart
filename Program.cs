using String2ServerService_ClientPart;
using System.Security.Principal;



//string serverURL = "http://localhost:5000/";
//string serverURL_post = "http://localhost:5000/SendString";
//string serverURL_post = "https://mygamefeedback.loca.lt/SendString";


AppDomain.CurrentDomain.ProcessExit += (s, e) =>
{
    Console.WriteLine("Сохраняем неотправленые данные...");
    
    MainWorker.OnAppClosed();
};

Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true; // чтобы дать время завершить сохранение
    Console.WriteLine("Ctrl+C. Сохраняем неотправленые данные...");

    MainWorker.OnAppClosed();

    Environment.Exit(0);
};



Thread.Sleep(1000);


MainWorker.StartConsoleDebugCycle();

//string curAnswer = "";

//List<Task<HttpRequestMessage>> loadedResponses = new List<Task<HttpRequestMessage>>();


//while (curAnswer != "End")
//{
//    Console.Write("Сообщение (Введите 'End' чтобы выйти): ");
//    curAnswer = Console.ReadLine();

//    if (curAnswer == "End") break;

//    var response = DataUploader.SendStringToServerAsync(curAnswer);



//    //Console.WriteLine(response.Result.StatusCode);
//}





//// Сетевые приколы
//HttpMessageHandler handler = new HttpClientHandler()
//{
//    UseProxy = false
//};

//// Задаём настройки
//using HttpClient client = new HttpClient(handler);
//using var form = new MultipartFormDataContent();


//// Создаём форму-токен
//StringContent mainStringContent = new StringContent(consoleWriten);

//// Заполняем форму
//form.Add(mainStringContent, "MainString");


//var response = await client.PostAsync(serverURL_post, form);
////var response = FileUploader.SendStringToServerAsync(consoleWriten);

//Console.WriteLine(response.StatusCode);


// Завершение
Console.Write("Нажимите Enter чтобы завершить работу ПО: ");
Console.ReadLine();