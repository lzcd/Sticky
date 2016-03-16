namespace Sticky.Cypher
{
    internal class ReturnProjectionDescription
    {
        public ReturnProjectionDescription()
        {
        }

        public string Alias { get; internal set; }
        public string NodeName { get; internal set; }
        public string PropertyName { get; internal set; }
    }
}