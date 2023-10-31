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
    static int lastMovieId = 0;
    static bool active = false;
    static async Task Main()
    {
        bot = new TelegramBotClient("6586082789:AAG62d2XzWLzkHzse78H_CPjAqGA9foYtas");
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
                        await bot.SendTextMessageAsync(chatid, "hello"+ "\n" + "if u wanna see a joke enter 1" + "\n" + "if u wanna see star wars movies enter 2" + "\n" +"and for Exit write exit", replyToMessageId: msgid);
                    }
                    else if ((msg.MessageId >= lastStartId + 2) && active)
                    {
                        if (msg.MessageId == lastMovieId + 2)
                        {
                            var api1 = new Uri($"https://swapi.dev/api/films/{txt}/");
                            using (HttpClient cli = new HttpClient())
                            {
                                HttpResponseMessage response = await cli.GetAsync(api1);
                                if (response.IsSuccessStatusCode)
                                {
                                    string movie = await response.Content.ReadAsStringAsync();
                                    war_stars warstarsmovies = JsonConvert.DeserializeObject<war_stars>(movie);
                                    await bot.SendTextMessageAsync(chatid, "plz wait...", replyToMessageId: msgid);
                                    await bot.SendTextMessageAsync(chatid, "title: " + warstarsmovies.title + "\n" + "release: " + warstarsmovies.release_date + "\n" + "producer: " + warstarsmovies.producer + "\n" + "director: " + warstarsmovies.director, replyToMessageId: msgid);
                                }
                                else
                                    await bot.SendTextMessageAsync(chatid, "not found. enter a corقect version", replyToMessageId: msgid);
                            }
                        }
                        else
                        {
                            switch (txt)
                            {
                                case "1":
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
                                case "2":
                                    lastMovieId = msg.MessageId;
                                    await bot.SendTextMessageAsync(chatid, "Which version of the movie do you want?", replyToMessageId: msgid);
                                    break;
                                case "exit":
                                    await bot.SendTextMessageAsync(chatid, "Bye", replyToMessageId: msgid);
                                    active = false;
                                    break;
                                default:
                                    await bot.SendTextMessageAsync(chatid, "enter 1, 2 or exit. adiot!!!", replyToMessageId: msgid);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
