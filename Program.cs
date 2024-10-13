using System;
using Telegram.Bot;
using Flurl.Http;
using System.Dynamic;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

UserImgflip user = new UserImgflip();

if (string.IsNullOrEmpty(user.Token))
{
    Console.WriteLine("Ошибка: Токен бота не найден. Установите переменную окружения BOT_TOKEN.");
    return;
}

using var cts = new CancellationTokenSource();

var bot = new TelegramBotClient(user.Token, cancellationToken: cts.Token);
var me = await bot.GetMeAsync();
Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

var memeTemplates = new[] {
    "112126428",
    "61579", 
    "101470",
    "438680",
    "89370399",
    "217743513",
    "102156234",
    "181913649",
    "178591752",
    "135678846",
    "188390779",
    "101440"
};

bot.StartReceiving(UpdateHandler, PollingErrorHandler);

Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
Console.ReadLine();
cts.Cancel();

async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
{
    if (update.Message?.Text != null)
    {
        var chatId = update.Message.Chat.Id;
        var userText = update.Message.Text;

        var texts = userText.Split(new[] { ',', '.', '?', '-' }, 2, StringSplitOptions.RemoveEmptyEntries);
        var topText = texts.Length > 0 ? texts[0] : "Top text";
        var bottomText = texts.Length > 1 ? texts[1] : "Bottom text";

        var random = new Random();
        var randomTemplateId = memeTemplates[random.Next(memeTemplates.Length)];

        var imgflipUsername = user.Login;
        var imgflipPassword = user.Password;

        try
        {
            var response = await "https://api.imgflip.com/caption_image"
                .PostUrlEncodedAsync(new
                {
                    template_id = randomTemplateId,
                    username = imgflipUsername,
                    password = imgflipPassword,
                    text0 = topText,
                    text1 = bottomText
                })
                .ReceiveJson<ImgflipResponse>();

            if (response.Success && response.Data?.Url != null)
            {
                await bot.SendTextMessageAsync(chatId, "Держи свой мем");
                await bot.SendPhotoAsync(chatId, response.Data.Url);
            }
            else await bot.SendTextMessageAsync(chatId, "Не удалось создать мем. Попробуйте еще раз.");
        }
        catch (Exception ex)
        {
            await bot.SendTextMessageAsync(chatId, "Ошибка при создании мема. Попробуйте позже.");
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}

Task PollingErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine($"Ошибка: {exception.Message}");
    return Task.CompletedTask;
}