using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Telegram.Bot;
using Telegram.Bot.Types;
using System;
using System.IO;
using System.Threading.Tasks;

var scraper = new FlightScraper();
var messenger = new TelegramSender();

while (true)
{
    try
    {
        Console.WriteLine($"{DateTime.Now}: İşlem başlatılıyor...");

        // 1. Site
        string path1 = await scraper.GetScreenshotAsync(
            "https://www.enuygun.com/ucak-bileti/arama/istanbul-helsinki-vantaa-havalimani-ista-hel/?gidis=13.01.2026&yetiskin=1&sinif=ekonomi&currency=TRY&save=1&ref=homepage&geotrip=international&trip=international",
            "site1.png"
        );
        await messenger.SendImage(path1, "Site 1 Güncel Uçuşlar");

        // 2. Site
        string path2 = await scraper.GetScreenshotAsync(
            "https://www.google.com/travel/flights/search?tfs=CBwQAhojEgoyMDI2LTAxLTEyagcIARIDU0FXcgwIAxIIL20vMDNraG5AAUgBcAGCAQsI____________AZgBAg&hl=en&gl=TR",
            "site2.png"
        );
        await messenger.SendImage(path2, "Site 2 Güncel Uçuşlar");

        Console.WriteLine("Mesajlar gönderildi. 1 saat bekleniyor...");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Hata oluştu: " + ex.Message);
    }

    await Task.Delay(TimeSpan.FromHours(1)); // 1 saat bekle
}

public class FlightScraper
{
    public async Task<string> GetScreenshotAsync(string url, string fileName)
    {
        Console.WriteLine("Screenshot alınıyor: " + url);

        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--disable-blink-features=AutomationControlled");

        using (IWebDriver driver = new ChromeDriver(options))
        {
            driver.Navigate().GoToUrl(url);

            await Task.Delay(5000); // Sayfa yüklenmesi için bekle

            // 🔽 Sayfayı biraz aşağı kaydır
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 600);");

            await Task.Delay(1000); // scroll sonrası 1sn bekle (render için)

            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            screenshot.SaveAsFile(fullPath);

            Console.WriteLine("Screenshot kaydedildi: " + fullPath);
            return fullPath;
        }
    }

}

public class TelegramSender
{
    private readonly string _token = "//token";
    private readonly long _chatId = //telegram id;

    public async Task SendImage(string filePath, string caption)
    {
        Console.WriteLine("Telegram gönderimi başlıyor: " + filePath);

        ITelegramBotClient botClient = new TelegramBotClient(_token);

        using (var stream = System.IO.File.OpenRead(filePath))
        {
            var photoFile = InputFile.FromStream(stream, Path.GetFileName(filePath));

            var msg = await botClient.SendPhotoAsync(
                chatId: _chatId,
                photo: photoFile,
                caption: caption
            );

            Console.WriteLine("Telegram gönderimi başarılı. Mesaj ID: " + msg.MessageId);
        }
    }
}

