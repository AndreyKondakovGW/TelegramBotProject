using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace NeuralNetwork1
{
    class TLGBotik
    {
        public Telegram.Bot.TelegramBotClient botik = null;
        public AIMLBotik aimlBot;

        private UpdateTLGMessages formUpdater;

        public BaseNetwork accordNet;
        public BaseNetwork studentNet;
        public Dictionary<int, string> ind2class = new Dictionary<int, string>();

        public Processor imageProcessor;
        // CancellationToken - инструмент для отмены задач, запущенных в отдельном потоке
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        public TLGBotik(BaseNetwork net,  UpdateTLGMessages updater)
        {
            ModelLoader loader = new ModelLoader();
            accordNet = loader.loadAccordNetwork();
            studentNet = loader.loadStudentNetwork();
            ind2class = loader.traindata.ind2class;
            imageProcessor = new Processor();
            var botKey = System.IO.File.ReadAllText("botkey.txt");
            botik = new Telegram.Bot.TelegramBotClient(botKey);
            formUpdater = updater;
            aimlBot = new AIMLBotik();
        }

        private async Task HandleUpdateMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //  Тут очень простое дело - банально отправляем назад сообщения
            var message = update.Message;
            formUpdater("Тип сообщения : " + message.Type.ToString());

            //  Получение файла (картинки)
            if (message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
            {
                formUpdater("Picture loadining started");
                var photoId = message.Photo.Last().FileId;
                Telegram.Bot.Types.File fl = botik.GetFileAsync(photoId).Result;
                var imageStream = new MemoryStream();
                await botik.DownloadFileAsync(fl.FilePath, imageStream, cancellationToken: cancellationToken);
                var img = System.Drawing.Image.FromStream(imageStream);
                
                System.Drawing.Bitmap bm = new System.Drawing.Bitmap(img);

                var processed = imageProcessor.ProcessImage(bm);
                if (processed == null)
                {
                    await botik.SendTextMessageAsync(message.Chat.Id, "К сожалению Картинка не подлежит обработке нужна буква на белом фоне");
                }
                else{
                    Sample sampleFromCamera = new Sample(processed, 10);
                    Console.WriteLine(sampleFromCamera.input.Length);
                    int ind = accordNet.Predict(sampleFromCamera);
                    Console.WriteLine($"Aforge На картинке буква: {ind2class[ind]}");
                    string Accord_class = ind2class[ind];
                    ind = studentNet.Predict(sampleFromCamera);
                    Console.WriteLine($"Student На картинке буква: {ind2class[ind]}");
                    string Student_class = ind2class[ind];

                    aimlBot.Talk(message.Chat.Id, message.Chat.FirstName, "буква " + Student_class);

                    await botik.SendTextMessageAsync(message.Chat.Id, "Предсказание accord сети: Буква: " + Accord_class);
                    await botik.SendTextMessageAsync(message.Chat.Id, "Предсказание student сети: Буква: " + Student_class);
                }


                formUpdater("Picture recognized!");
                return;
            }
            //aiml
            if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                var answer = aimlBot.Talk(message.Chat.Id,message.Chat.FirstName ,message.Text);
                if (answer == "")
                {
                    formUpdater("update variable");
                    return;
                }
                await botik.SendTextMessageAsync(message.Chat.Id, answer);
                formUpdater(answer);
                return;
            }
            if (message == null || message.Type != MessageType.Text) return;
            if(message.Text == "Authors")
            {
                string authors = "Гаянэ Аршакян, Луспарон Тызыхян, Дамир Казеев, Роман Хыдыров, Владимир Садовский, Анастасия Аскерова, Константин Бервинов, и Борис Трикоз (но он уже спать ушел) и молчаливый Даниил Ярошенко, а год спустя ещё Иванченко Вячеслав";
                await botik.SendTextMessageAsync(message.Chat.Id, "Авторы проекта : " + authors);
            }
           await botik.SendTextMessageAsync(message.Chat.Id, "Bot reply : " + message.Text);
            formUpdater(message.Text);
            return;
        }
        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var apiRequestException = exception as ApiRequestException;
            if (apiRequestException != null)
                Console.WriteLine($"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}");
            else
                Console.WriteLine(exception.ToString());
            return Task.CompletedTask;
        }

        public bool Act()
        {
            try
            {
                botik.StartReceiving(HandleUpdateMessageAsync, HandleErrorAsync, new ReceiverOptions
                {   // Подписываемся только на сообщения
                    AllowedUpdates = new[] { UpdateType.Message }
                },
                cancellationToken: cts.Token);
                // Пробуем получить логин бота - тестируем соединение и токен
                Console.WriteLine($"Connected as {botik.GetMeAsync().Result}");
            }
            catch(Exception e) { 
                return false;
            }
            return true;
        }

        public void Stop()
        {
            cts.Cancel();
        }

    }
}
