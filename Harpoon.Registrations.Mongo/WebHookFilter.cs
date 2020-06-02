namespace Harpoon.Registrations.Mongo
{
    /// <summary>
    /// Default implementation of <see cref="IWebHookFilter"/>
    /// </summary>
    public class WebHookFilter : GuidBaseDocument, IWebHookFilter
    {
        /// <inheritdoc />
        public string Trigger { get; set; }
    }
}
