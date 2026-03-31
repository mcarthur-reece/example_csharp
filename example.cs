using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Gmail.v1.Data;

using System;
using System.IO;
using System.Text;
using System.Threading;

class Program
{
    static void Main()
    {
        // AUTH (popup browser once)
        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromFile("credentials.json").Secrets,
            new[] { GmailService.Scope.GmailModify },
            "user",
            CancellationToken.None,
            new FileDataStore("token.json", true)
        ).Result;

        var service = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Quick Gmail Test"
        });

        // ===== SEND EMAIL =====
        string raw =
            "From: me\r\n" +
            "To: your_email@gmail.com\r\n" +
            "Subject: Quick Test\r\n\r\n" +
            "Hello from C# Gmail API!";

        string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw))
            .Replace("+", "-").Replace("/", "_").Replace("=", "");

        var msg = new Message { Raw = encoded };
        service.Users.Messages.Send(msg, "me").Execute();

        Console.WriteLine("✅ Email sent!");

        // ===== READ 3 EMAILS =====
        var list = service.Users.Messages.List("me");
        list.MaxResults = 3;

        var emails = list.Execute();

        foreach (var m in emails.Messages)
        {
            var email = service.Users.Messages.Get("me", m.Id).Execute();
            Console.WriteLine($"📩 {email.Snippet}");
        }
    }
}
