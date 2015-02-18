namespace Moth.Extensions
{
    public static class QueryExtension
    {
        public static Executor Execute(this Query source)
        {
           return new Executor {ExtensionQuery = source};
        }
    }
}