using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace String2ServerService_ClientPart
{
    public static class MainWorker
    {
        private static List<string> sendedMessagesInFlow = new List<string>();




        public static async Task StartConsoleDebugCycle()
        {
            string curAnswer = "";

            


            while (curAnswer != "End")
            {
                TrySendNonSendedMessages();


                if(sendedMessagesInFlow.Count > 0)
                {
                    Console.WriteLine("Отправленные сообщения, которые ещё в потоке: \n" + string.Join("\n", sendedMessagesInFlow));
                }



                Console.WriteLine("");
                Console.Write("Сообщение (Введите 'End' чтобы выйти): ");
                curAnswer = Console.ReadLine() ?? "";

                if (curAnswer == "End") break;

                if (curAnswer == "") continue;


                sendedMessagesInFlow.Add(curAnswer);

                //var response = DataUploader.SendStringToServerAsync(curAnswer).ContinueWith(x =>
                //{
                //    Console.WriteLine("Какой то TASK отправки сообщения завершен");
                //    if (x.IsCompletedSuccessfully)
                //    {
                //        var response = x.Result;
                //        if (response.StatusCode != HttpStatusCode.OK)
                //        {
                //            // Неудача
                //            SaveToBuffer(curAnswer);
                //        }
                //    }
                //    else
                //    {
                //        // Неудача
                //        SaveToBuffer(curAnswer);
                //    }

                //    sendedMessagesInFlow.Remove(curAnswer);
                //});

                TrySendMessageWithProtect(curAnswer);
            }

        }



        public static void OnAppClosed()
        {
            /// Проходимся по отправленным

            foreach (var item in sendedMessagesInFlow)
            {
                SaveToBuffer(item);
            }
        }

        public static void SaveToBuffer(string message)
        {
            Console.WriteLine($"Отправка неудалась: Сохраняем неотправленный мессадж в локальный файл");

            using (FileStream fileStream = new FileStream(ProgramSettings.NonSendedFilesPath + $"NonSendedMessage_{DateTime.Now:yyyyMMddHHmmss}.txt", FileMode.OpenOrCreate))
            {
                byte[] buffer = Encoding.Default.GetBytes(message);
                fileStream.Write(buffer, 0, buffer.Length);
                Console.WriteLine("Не отправленный текст записан в файл");
            }
        }


        /// <summary>
        /// Отправляем сообщение на сервер.
        /// В случае неудачи - сохраняем в папку программы
        /// </summary>
        public static void TrySendMessageWithProtect(string message)
        {
            var response = DataUploader.SendStringToServerAsync(message).ContinueWith(x =>
            {
                Console.WriteLine("Какой то TASK отправки сообщения завершен");
                if (x.IsCompletedSuccessfully)
                {
                    var response = x.Result;
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        // Неудача
                        SaveToBuffer(message);
                    }
                }
                else
                {
                    // Неудача
                    SaveToBuffer(message);
                }

                sendedMessagesInFlow.Remove(message);
            });
        }

        public static async Task<NonSendedMessageData[]> ReadAllNonSendedMessages()
        {
            string folderPath = Path.Combine(AppContext.BaseDirectory, "NonSendedFiles");
            Directory.CreateDirectory(folderPath);


            List<NonSendedMessageData> results = new List<NonSendedMessageData>();

            string[] filePaths = Directory.GetFiles(folderPath);

            foreach (var path in filePaths)
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    byte[] buffer = new byte[fileStream.Length];

                    await fileStream.ReadAsync(buffer, 0, buffer.Length);

                    string textFromFile = Encoding.Default.GetString(buffer);

                    NonSendedMessageData newNonSendedMessageData = new NonSendedMessageData()
                    {
                        filePath = path,
                        message = textFromFile

                    };

                    results.Add(newNonSendedMessageData);
                }
            }
            
            return results.ToArray();
        }

        public static void TrySendNonSendedMessages()
        {
            // Получаем сохранённые (неотправленные) сообщения и пытаемся их отправить
            NonSendedMessageData[] nonSendedMessages = ReadAllNonSendedMessages().Result;


            if (nonSendedMessages.Length > 0)
            {
                // Есть сообщения, которые были записаны в файл. Пробуем отправить их.
                Console.WriteLine("Мессаджи, прочитанные из папки:\n" + string.Join("\n", nonSendedMessages.Select(x => x.message)) + "\n");

                foreach (var oldMessage in nonSendedMessages)
                {
                    Console.WriteLine("Пытаемся отправить неотправленное сообщение на сервер... ");
                    _ = DataUploader.SendStringToServerAsync(oldMessage.message).ContinueWith(x =>
                    {
                        if (x.IsCompletedSuccessfully)
                        {
                            if (x.Result.StatusCode == HttpStatusCode.OK)
                            {
                                // Отправлено. Удаляем файл.
                                File.Delete(oldMessage.filePath);
                                Console.Write("Успех!");
                            }
                            else
                            {
                                Console.Write("Неудача! Так как сервер не ответил положительно");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Неудача! Так как таск обосрался");
                        }
                    });
                }
            }
        }



    }
}


public struct NonSendedMessageData
{
    public string message;
    public string filePath;
}