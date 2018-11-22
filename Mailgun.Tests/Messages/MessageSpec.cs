using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using Mailgun.Core.Messages;
using Mailgun.Exceptions;
using Mailgun.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Shouldly;

namespace Mailgun.Tests.Messages
{
    [TestClass]
    public class MessageSpec
    {
        [TestMethod]
        [ExpectedException(typeof (RequiredPropertyNullException))]
        public void TestEmptyMessageToKeyValuePairs()
        {
            var message = new Message();
            //At least 'To' and 'From' is required, so a RequiredPropertyException should be thrown
            message.AsKeyValueCollection();
        }

        [TestMethod]
        public void TestMessageToKeyValuePairs()
        {
            var message = new Message
            {
                To = new List<IRecipient> {new Recipient {Email = "test@host.com", DisplayName = "test"}},
                From = new Recipient { DisplayName = "Test", Email = "test@host.com"}
            };

            var kvps = message.AsKeyValueCollection();

            //boolean values should always exist
            kvps.FirstOrDefault(k => k.Key == "o:dkim").Key.ShouldNotBeNull();
            kvps.FirstOrDefault(k => k.Key == "o:testmode").Key.ShouldNotBeNull();
            kvps.FirstOrDefault(k => k.Key == "o:tracking").Key.ShouldNotBeNull();
            kvps.FirstOrDefault(k => k.Key == "o:tracking-clicks").Key.ShouldNotBeNull();
            kvps.FirstOrDefault(k => k.Key == "o:tracking-opens").Key.ShouldNotBeNull();

            //to should exist
            kvps.FirstOrDefault(k => k.Key == "to").Key.ShouldNotBeNull();
            kvps.FirstOrDefault(k => k.Key == "to").Value.ShouldBe("\"test\" <test@host.com>,");

            //add Tags
            message.Tags = new Collection<string> {"tag1", "tag2"};
            kvps = message.AsKeyValueCollection();
            kvps.Count(k => k.Key == "o:tag").ShouldBe(2);

            //add custom headers
            message.CustomHeaders = new Dictionary<string, string> {{"X-Custom-Header", "Test"}};
            kvps = message.AsKeyValueCollection();
            kvps.Count(k => k.Key == "h:X-Custom-Header").ShouldBe(1);
        }

        [TestMethod]
        public void TestRecipientVariables()
        {
            var message = new Message
            {
        To = new List<IRecipient> {new Recipient {Email = "test@host.com", DisplayName = "test"}},
                From = new Recipient { DisplayName = "Test", Email = "test@host.com"},
                RecipientVariables = JObject.Parse("{\"test\":{\"id\":123}}")
            };

            var kvps = message.AsKeyValueCollection();
            kvps.Count(k => k.Key == "recipient-variables").ShouldBe(1);
            kvps.First(k => k.Key == "recipient-variables").Value.ShouldBe("{\"test\":{\"id\":123}}");
        }

        [TestMethod]
        public void TestAttachmentDetection()
        {
            //A message with attachments should always be multi part
            var attachmentMessage = new Message
            {
                To = new List<IRecipient> { new Recipient { Email = "test@host.com", DisplayName = "test" } },
                From = new Recipient { DisplayName = "Test", Email = "test@host.com"},
                Attachments =
                    new Collection<FileInfo> {new FileInfo(Consts.PictureFileName)}
            };
            attachmentMessage.AsFormContent().ShouldBeOfType<MultipartFormDataContent>();
            //A message with inline images should always be multi part
            var inlineMessage = new Message
            {
                To = new List<IRecipient> { new Recipient { Email = "test@host.com", DisplayName = "test" } },
                From = new Recipient { DisplayName = "Test", Email = "test@host.com"},
                Inline =
                    new Collection<FileInfo> {new FileInfo(Consts.PictureFileName)}
            };
            inlineMessage.AsFormContent().ShouldBeOfType<MultipartFormDataContent>();

            //A message with both should always be multi part
            var both = new Message
            {
                To = new List<IRecipient> { new Recipient { Email = "test@host.com", DisplayName = "test" } },
                From = new Recipient { DisplayName = "Test", Email = "test@host.com"},
                Inline =
                    new Collection<FileInfo> {new FileInfo(Consts.PictureFileName)},
                Attachments =
                    new Collection<FileInfo> {new FileInfo(Consts.PictureFileName)}
            };
            both.AsFormContent().ShouldBeOfType<MultipartFormDataContent>();

            //a message without attachments should be form url encoded
            var noAttachments = new Message
            {
                To = new List<IRecipient> { new Recipient { Email = "test@host.com", DisplayName = "test" } },
                From = new Recipient { DisplayName = "Test", Email = "test@host.com" }
            };
            noAttachments.AsFormContent().ShouldBeOfType<MultipartFormDataContent>();
        }
    }
}