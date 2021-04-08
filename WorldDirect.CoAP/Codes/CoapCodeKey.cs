namespace WorldDirect.CoAP.Codes
{
    using System;

    public class CoapCodeKey : IEquatable<CoapCodeKey>
    {
        public CoapCodeKey(CodeClass @class, CodeDetail detail)
        {
            this.Class = @class;
            this.Detail = detail;
        }

        public CodeClass Class { get; }

        public CodeDetail Detail { get; }

        public static bool operator ==(CoapCodeKey left, CoapCodeKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CoapCodeKey left, CoapCodeKey right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{this.Class}.{this.Detail}";
        }

        public bool Equals(CoapCodeKey other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(this.Class, other.Class) && Equals(this.Detail, other.Detail);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((CoapCodeKey)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Class, this.Detail);
        }
    }
}