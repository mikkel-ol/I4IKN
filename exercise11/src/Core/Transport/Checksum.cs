using System;

namespace Core
{
    public class Checksum : IChecksum, IEquatable<Checksum>
    {
        public long Value { get; private set; }

        public Checksum(byte[] data) 
        {
            this.Value = Calculate(data);
        }

        public override bool Equals(object obj)
        {
            // Try casting
            var another = obj as Checksum;

            // If object is not of type Checksum, they cannot be the same
            if (another == null) return false;

            // We can safely type cast after first trying GetType()
            return obj.GetType() == GetType() && Equals( (Checksum) obj);
        }

        public bool Equals(Checksum other)
        {
            // If other Checksum is null, it can't be the same if this object
            if (ReferenceEquals(null, other)) return false;
            // If other Checksum is the same reference as this one, it is the same object
            if (ReferenceEquals(this, other)) return true;

            // If Value property on both objects are equal, both objects are considered equal
            return this.Value.Equals(other.Value);
        }

        public static bool operator ==(Checksum chksum1, Checksum chksum2)
        {
            // If same reference, they are the same object
            if (ReferenceEquals(chksum1, chksum2)) return true;

            // If one of the references if null, they can't be equal
            if (ReferenceEquals(chksum1, null)) return false;
            if (ReferenceEquals(chksum2, null)) return false;

            return chksum1.Equals(chksum2);
        }

        public static bool operator !=(Checksum chksum1, Checksum chksum2)
        {
            return !(chksum1 == chksum2);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        private long Calculate(byte[] data)
        {
            int i = 0;
            int length = data.Length;

            long sum = 0;

            while (length > 0)
            {
                // Add even bytes to MSB
                sum += data[i++] << 8;
                if ((--length) == 0) break;

                // Add odd bytes to LSB
                sum += data[i++];
                --length;
            }

            // Return total 16 sum bytes, negated
            return (~((sum & 0xFFFF) + (sum >> 16))) & 0xFFFF;
        }
    }
}
