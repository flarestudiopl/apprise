using RazorLight;
using System;
using System.IO;
using System.Net;

namespace FlareStudio.Apprise.Application
{
    internal interface IMessageRenderer
    {
        RenderedMessage Render(string templateKey, object model);
    }

    internal class RenderedMessage
    {
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
    }

    internal class MessageRenderer : IMessageRenderer
    {
        private readonly RazorLightEngine _engine;

        public MessageRenderer()
        {
            _engine = new RazorLightEngineBuilder()
                        .UseFileSystemProject(Path.GetDirectoryName(typeof(MessageRenderer).Assembly.Location))
                        .UseMemoryCachingProvider()
                        .Build();
        }

        public RenderedMessage Render(string templateKey, object model)
        {
            var renderTask = _engine.CompileRenderAsync(templateKey, model);
            renderTask.Wait();

            var renderedTemplate = renderTask.Result;
            var firstLineEndIndex = renderedTemplate.IndexOf(Environment.NewLine);

            return new RenderedMessage
            {
                Subject = WebUtility.HtmlDecode(renderedTemplate.Substring(0, firstLineEndIndex)),
                HtmlBody = renderedTemplate.Substring(firstLineEndIndex + Environment.NewLine.Length)
            };
        }
    }
}
