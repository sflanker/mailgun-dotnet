using Mailgun.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Shouldly;

namespace Mailgun.Tests.Messages
{
    [TestClass]
    public class MessageBuilderSpec
    {
        [TestMethod]
        public void MessageBuilderRecipientVariables()
        {
            var builder = new MessageBuilder();

            var message =
                builder.AddToRecipient(new Recipient {DisplayName = "Mailgun C#", Email = "test@host.com"},
                    JObject.Parse("{\"id\":\"123\"}"))
                    .GetMessage();

            message.RecipientVariables.ShouldNotBeNull();
            message.RecipientVariables["test@host.com"].ShouldNotBeNull();
        }
    }
}