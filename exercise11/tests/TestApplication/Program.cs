using System;

using Core;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] test = new byte[10];

            test[0] = 0b11111111;
            test[1] = 0b11111111;
            test[2] = 0b00000001;
            test[3] = 0b00000001;
            test[4] = 0b00000001;
            test[5] = 0b00000001;
            test[6] = 0b00000001;
            test[7] = 0b00000001;
            test[8] = 0b00000001;
            test[9] = 0b00000001;

            var chksum1 = new Checksum(test);
            var chksum2 = new Checksum(test);

            // Should be the same
            Console.WriteLine(chksum1.Equals(chksum2));
            Console.WriteLine(chksum1 == chksum2);

            //Console.WriteLine(Convert.ToString(chksum1.Value, 2));
        }
    }
}
