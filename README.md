# apprise
Templated e-mail notification engine for .NET

## How it works
FlareStudio's Apprise stores each message in MS SQL database as a model and a reference to the template. During internal queue processing each message is being rendered by Razor engine and then sent to SMTP. In this way, sending operation is rapid and all long lasting operations are moved to separated thread. Moreover, retries and auditing are more convinient. All you have to do is just create an instance of your strongly-typed e-mail model and pass it to the Apprise's facade.

## How to use it

1. Get [FlareStudio.Apprise](https://www.nuget.org/packages/FlareStudio.Apprise/) from NuGet.

2. Create a single instance of `NotificationsFacade`:
```C#
var smtpConfiguration = new SmtpConfiguration();

_notificationsFacade = new NotificationsFacade(sqlConnectionString, smtpConfiguration);
```
, where smtpConfiguration is your own type that satisfies `ISmtpConfiguration` interface. As an option you can provide an implementation of `ILoggerPort` to write logs in other way that console.

3. To start processing loop, invoke
```C#
_notificationsFacade.StartMessageQueue();
```
Dispose of `_notificationsFacade` to stop processing.

4. Create a separate project with Templates and Models. This project should also have a reference to `FlareStudio.Apprise`. Make sure this project is referenced in any way in your build chain for Templates to be copied into the output directory. Add your first e-mail model as a class that satisfies `ITemplateModel`:
```C#
public class OneTimePassEmailTemplateModel : ITemplateModel
{
    public string OneTimePass { get; set; }
    public string CshtmlTemplatePath => "EmailTemplates/OneTimePass.cshtml";
}
```

5. Create `EmailTemplates/OneTimePass.cshtml` file as defined in Model class and set its build action to `Content` and `Copy always`:
```razor
@model OneTimePassEmailTemplateModel
Set a new password
<h2>Howdy!</h2>
<p>Your password recovery code is: @Model.OneTimePass</p>
<h4>Have a nice day :)</h4>
```
**The first line of rendered template will be used as email subject**. The rest is the HTML body of the message.

6. Send your first message simply by:
```C#
var model = new OneTimePassEmailTemplateModel
{
    OneTimePass = "a-one-time-pass-code"
};

_notificationsFacade.SendMessage(model, "recipient@test.com");
```
