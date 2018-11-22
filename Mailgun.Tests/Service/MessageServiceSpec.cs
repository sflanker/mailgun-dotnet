using System;
using System.IO;
using System.Threading.Tasks;
using Mailgun.Exceptions;
using Mailgun.Messages;
using Mailgun.Service;
using Mailgun.Tests.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Shouldly;

namespace Mailgun.Tests.Service
{
    [TestClass]
    public class MessageServiceSpec
    {
        private string Domain { get; }
        private string ApiKey { get; }
        private string RecipientDomain { get; }
        private string TestSender { get; }

        public MessageServiceSpec() {
            var config = new MailgunConfiguration();
            ConfigurationHelper.Root.GetSection("Mailgun").Bind(config);

            this.Domain = config.Domain;
            this.ApiKey = config.ApiKey;
            this.RecipientDomain = config.RecipientDomain;
            this.TestSender = config.TestSender;
        }

        [TestMethod]
        public void TestDefaults()
        {
            var mg = new MessageService(ApiKey, true,"api.mailgun.net/v3");
            mg.ApiKey.ShouldBe(ApiKey);
            mg.UseSSl.ShouldBeTrue();
            mg.BaseAddress.ShouldBe("api.mailgun.net/v3");
        }

        [TestMethod]
        public async Task TestSendBatchMessage()
        {
             var mg = new MessageService(ApiKey);

            //build a message
            var builder = new MessageBuilder()
                .SetTestMode(true)
                .SetSubject("Plain text test")
                .SetFromAddress(new Recipient {Email = this.TestSender, DisplayName = "Mailgun C#"})
                .SetTextBody("This is a test");

            //add 1000 users
            for (var i = 0; i < 1000; i++)
            {
                builder.AddToRecipient(new Recipient() {Email = string.Format("test{0}@test.com", i)},JObject.Parse("{\"id\":"+i+"}"));
            }

            var content = await mg.SendMessageAsync(Domain, builder.GetMessage());
            content.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task TestSendMessage()
        {
            var mg = new MessageService(ApiKey);

            //build a message
            var message = new MessageBuilder()
                .AddToRecipient(new Recipient
                {
                    Email = this.TestSender,
                    DisplayName = "Mailgun C#"
                })
                .SetTestMode(true)
                .SetSubject("Plain text test")
                .SetFromAddress(new Recipient {Email = this.TestSender, DisplayName = "Mailgun C#"})
                .SetTextBody("This is a test")
                .GetMessage();

            var content = await mg.SendMessageAsync(Domain, message);
            content.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task TestParseSimple() => await TestEmailParsing("", $"test@{this.RecipientDomain}");

        [TestMethod]
        public async Task TestWithDisplayName() => await TestEmailParsing("Test User", $"test@{this.RecipientDomain}");

        [TestMethod]
        public async Task TestWithBrackets() => await TestEmailParsing("Test (and thats all)", $"test@{this.RecipientDomain}");

        [TestMethod, ExpectedException(typeof(InvalidEmailException))]
        public async Task TestWithDot() => await TestEmailParsing("Test", $"test.@{this.RecipientDomain}");

        [TestMethod, ExpectedException(typeof(InvalidEmailException))]
        public async Task TestWithoutAt() => await TestEmailParsing("Test", "test");

        [TestMethod]
        public async Task TestWithQuotes() => await TestEmailParsing("\"Test User \"TUser\"", $"test@{this.RecipientDomain}");

        private async Task TestEmailParsing(string displayName, string email)
        {
            var mg = new MessageService(this.ApiKey);
            //build a message
            var message = new MessageBuilder()
                .AddToRecipient(new Recipient
                {
                    Email = email,
                    DisplayName = displayName
                })
                .SetTestMode(true)
                .SetSubject("Plain text test")
                .SetFromAddress(new Recipient {Email = email, DisplayName = displayName})
                .SetTextBody("This is a test")
                .GetMessage();

            var content = await mg.SendMessageAsync(this.Domain, message);
            content.ShouldNotBeNull();
            content.IsSuccessStatusCode.ShouldBeTrue(
                $"Email: {email}, DisplayName: {displayName}, Status code: {content.StatusCode}, Content: {await content.Content.ReadAsStringAsync()}");
        }

        [TestMethod]
        public async Task TestSendHtmlMessage()
        {
            var mg = new MessageService(ApiKey);

            //build a message
            var message = new MessageBuilder()
                .AddToRecipient(new Recipient
                {
                    Email = this.TestSender,
                    DisplayName = "Mailgun C#"
                })
                .SetTestMode(true)
                .SetSubject("Html test")
                .SetFromAddress(new Recipient {Email = this.TestSender, DisplayName = "Mailgun C#"})
                .SetHtmlBody("<html><h1>Hello from the Mailgun C# library</h1></html>")
                .GetMessage();

            var content = await mg.SendMessageAsync(Domain, message);
            content.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task TestSendAttachments()
        {
            var mg = new MessageService(ApiKey);

            //build a message
            var message = new MessageBuilder()
                .AddToRecipient(new Recipient
                {
                    Email = this.TestSender,
                    DisplayName = "Mailgun C#"
                })
                .SetTestMode(true)
                .SetSubject("Attachment test")
                .SetFromAddress(new Recipient {Email = this.TestSender, DisplayName = "Mailgun C#"})
                .SetHtmlBody("<html><h1>I have an attachment</h1></html>")
                .AddAttachment(new FileInfo(Consts.PictureFileName))
                .GetMessage();

            var content = await mg.SendMessageAsync(Domain, message);
            content.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task TestInlineImage()
        {
            var mg = new MessageService(ApiKey);

            //build a message
            var message = new MessageBuilder()
                .AddToRecipient(new Recipient
                {
                    Email = this.TestSender,
                    DisplayName = "Mailgun C#"
                })
                .SetTestMode(true)
                .SetSubject("Inline image test")
                .SetFromAddress(new Recipient {Email = this.TestSender, DisplayName = "Mailgun C#"})
                .SetHtmlBody("<html>Inline image here: <img src=\"cid:Desert.jpg\"></html>")
                .AddInlineImage(new FileInfo(Consts.PictureFileName))
                .GetMessage();

            var content = await mg.SendMessageAsync(Domain, message);
            content.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task TestKitchenSink()
        {
            var mg = new MessageService(ApiKey);

            //build a message
            var picturesDesert = Consts.PictureFileName;
            var message = new MessageBuilder()
                .SetTestMode(true)
                .AddToRecipient(new Recipient
                {
                    Email = this.TestSender,
                    DisplayName = "Mailgun C#"
                })
                .AddCcRecipient(new Recipient
                {
                    Email = "test@test.com",
                    DisplayName = "Tester"
                })
                .AddBccRecipient(new Recipient
                {
                    Email = "test1@test.com",
                    DisplayName = "Tester"
                })
                .SetReplyToAddress(new Recipient
                {
                    Email = "test_reply@test.com",
                    DisplayName = "Tester Reply"
                })
                .AddCustomData("Some Data", JObject.Parse("{\"test\":\"A test json object\"}"))
                .AddCustomHeader("X-My-Custom-Header", "Custom Header")
                .AddCustomParameter("CustomParam", "A custom parameter")
                .AddTag("Kitchen sink Tag")
                .AddCampaignId("fake_campaign_id")
                .SetClickTracking(true)
                .SetDkim(true)
                .SetOpenTracking(true)
                .SetDeliveryTime(DateTime.Now + TimeSpan.FromDays(1))
                .SetSubject("Kitchen Sink")
                .SetFromAddress(new Recipient {Email = this.TestSender, DisplayName = "Mailgun C#"})
                .SetTextBody("This is the text body")
                .SetHtmlBody("<html>Inline image here: <img src=\"cid:Desert.jpg\"></html>")
                .AddInlineImage(new FileInfo(picturesDesert))
                .AddAttachment(new FileInfo(picturesDesert))
                .GetMessage();

            var content = await mg.SendMessageAsync(Domain, message);
            content.ShouldNotBeNull();
        }
    }
}