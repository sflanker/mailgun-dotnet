mailgun-dotnet
==============

A Mailgun API library for .Net Core

Made with inspiration from the [Mailgun-php](https://github.com/mailgun/mailgun-php) implementation, this library wraps the Mailgun HTTP API for easy use in .Net applications.

## Installation

TODO: This project has not yet been published on nuget

## Basic usage

The current implementation supports creating a MessageService and sending Messages. A Message can be created manually or you can use the recommended MessageBuilder.

```csharp
     var mg = new MessageService(ApiKey);
     // var mg = new MessageService(ApiKey, false); // you can specify to use SSL or not, which determines the url API scheme to use
     // var mg = new MessageService(ApiKey,false, "api.mailgun.net/v3"); // you can also override the base URL, which defaults to v2

     // build a message
     var message = new MessageBuilder()
          .AddToRecipient(new Recipient
                {
                    Email = "your-email@host.com",
                    DisplayName = "Your Name"
                })
          .SetSubject("Plain text test")
          .SetFromAddress(new Recipient { Email = "recipient@host.com", DisplayName = "Recipient Name" })
          .SetTextBody("This is a test")
          .GetMessage();

     await mg.SendMessageAsync(Domain, message);
```

The current Message object supports all the options listed in the Mailgun documentation [here](http://documentation.mailgun.com/api-sending.html#sending)

## TODO

There is much more to do, but on the plate next are:

* Stored Messages
* Events

Contributors Welcome
====================

This library is currently minimally maintained by Paul Wheeler (@sflanker), if you're interested in contributing features or fixes don't hesitate to submit PRs, or request to be made a project owner.

Thanks
======

Thank you to Charles King who originally created this library.
