namespace WorldDirect.CoAP.Messages.Options
{
    public class IfNoneMatch : EmptyOptionFormat
    {
        public override ushort Number => 5;

        public override string ToString()
        {
            return $"If-None-Match ({this.Number})";
        }
    }
}
