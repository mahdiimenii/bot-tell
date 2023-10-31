using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Telegram.Bot;
using Newtonsoft.Json;
using bot_tell;

class program
{
    static TelegramBotClient bot;
    static int updatesnow;

    static int lastStartId = 0;
    static bool active = false;
    static async Task Main()
    {
        bot = new TelegramBotClient("token");
        Console.WriteLine("started");

        while (true)
        {
            var updates = await bot.GetUpdatesAsync(updatesnow);

            foreach (var updated in updates)
            {
                updatesnow = updated.Id + 1;
                var txt = updated.Message.Text != null ? updated.Message.Text : null;
                var msg = updated.Message != null ? updated.Message : null;
                var chatid = updated.Message.Chat.Id;
                var msgid = updated.Message.MessageId;

                if (txt.Trim() != null && msg != null)
                {
                    if (txt.ToLower() == "/start")
                    {
                        lastStartId = msg.MessageId;
                        active = true;
                        await bot.SendTextMessageAsync(chatid, "hello"+ "\n" + "if u wanna see a joke enter 1" + "\n" + "if u wanna see star wars movies enter 2" + "\n" + "if u wanna a news enter 3"+ "\n" +"and for Exit write exit", replyToMessageId: msgid);
                    }
                    else if ((msg.MessageId >= lastStartId + 2) && active)
                    {
                        switch (txt)
                        {
                            case "1":
// random joke
                                //await bot.SendTextMessageAsync(chatid, "1", replyToMessageId: msgid);
                                var api = "https://api.chucknorris.io/jokes/random";
                                using (HttpClient cli = new HttpClient())
                                {
                                    HttpResponseMessage response = await cli.GetAsync(api);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        string random_joke = await response.Content.ReadAsStringAsync();
                                        joke_value joke = JsonConvert.DeserializeObject<joke_value>(random_joke);
                                        await bot.SendTextMessageAsync(chatid, joke.value, replyToMessageId: msgid);
                                    }
                                }
                                break;
// done + bog
                            case "2":
// war stars movies
                                //await bot.SendTextMessageAsync(chatid, "2", replyToMessageId: msgid);
                                await bot.SendTextMessageAsync(chatid, "?", replyToMessageId: msgid);
                                var api1 = new Uri($"https://swapi.dev/api/films/{txt}/");
                                using (HttpClient cli = new HttpClient())
                                {
                                    HttpResponseMessage response = await cli.GetAsync(api1);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        string movie = await response.Content.ReadAsStringAsync();
                                        war_stars warstarsmovies = JsonConvert.DeserializeObject<war_stars>(movie);
                                        await bot.SendTextMessageAsync(chatid, warstarsmovies.title+ "\n" + warstarsmovies.release_date + "\n" + warstarsmovies.producer + "\n" + warstarsmovies.release_date, replyToMessageId: msgid);
                                    }
                                    else
                                        await bot.SendTextMessageAsync(chatid, "not found. enter a corect number", replyToMessageId: msgid);
                                }
                                break;
// done + bog
                            case "3":
// news                                
                                //await bot.SendTextMessageAsync(chatid, "3", replyToMessageId: msgid);
                                var api2 = "https://newsapi.org/v2/everything?q=apple&from=2023-10-30&to=2023-10-30&sortBy=popularity&apiKey=bb2d672cb99c479a9a370f5c4f80cca2";
                                using (HttpClient cli = new HttpClient())
                                {
                                    HttpResponseMessage response = await cli.GetAsync(api2);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        string news_news = await response.Content.ReadAsStringAsync();
                                        news new_news = JsonConvert.DeserializeObject<news>(news_news);
                                        List<articles> listofnews = new_news.Articles;
                                    }
                                }
                                 break;
// bog
                            case "exit":
                                await bot.SendTextMessageAsync(chatid, "Bye", replyToMessageId: msgid);
                                active = false;
                                break;
                            default:
                                await bot.SendTextMessageAsync(chatid, "enter 1, 2 ,3 or exit. adiot!!!", replyToMessageId: msgid);
                                break;
                        }
                    }
                }
            }
        }
    }
}
