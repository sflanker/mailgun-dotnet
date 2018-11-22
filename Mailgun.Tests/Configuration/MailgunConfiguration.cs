using System;

namespace Mailgun.Tests.Configuration {
    public class MailgunConfiguration {
        public String Domain { get; set; }

        public String ApiKey { get; set; }

        public String RecipientDomain { get; set; }

        public String TestSender { get; set; }
    }
}