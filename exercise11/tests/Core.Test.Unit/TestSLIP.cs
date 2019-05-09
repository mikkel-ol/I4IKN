using System;
using System.Linq;

using NSubstitute;
using NUnit.Framework;

using Core;

namespace Tests
{
    public class Tests
    {
        private ISerial _serial;
        private SLIP _uut;

        private byte[] _buffer;

        [SetUp]
        public void Setup()
        {
            _serial = Substitute.For<ISerial>();
            _uut = new SLIP(_serial);
        }

        [TestCase("MNO", "AMNOA", TestName = "SLIP Sending does encode data, basic encoding")]
        [TestCase("A", "ABCA", TestName = "SLIP Sending does encode data, delimiter encoding")]
        [TestCase("B", "ABDA", TestName = "SLIP Sending does encode data, escaping encoding")]
        [TestCase("ABC", "ABCBDCA", TestName = "SLIP Sending does encode data, full encoding")]
        public void TestSendIsEncoded(string toEncode, string expectedEncoding)
        {
            _buffer = new byte[toEncode.Length];

            for (int i = 0; i < toEncode.Length; i++)
            {
                _buffer[i] = (byte) toEncode[i];
            }

            _uut.Send(_buffer, toEncode.Length);

            int l = expectedEncoding.Length;
            byte[] expectedBuffer = new byte[l];

            for (int i = 0; i < l; i++)
            {
                expectedBuffer[i] = (byte) expectedEncoding[i];
            }

            _serial.Received(1).Send(Arg.Is<byte[]>(actualBuffer => expectedBuffer.SequenceEqual(actualBuffer)), l);
        }

        //[TestCase("AMNOA", "MNO", TestName = "SLIP Receiving does decode data, basic encoding")]
        //[TestCase("ABCA", "A", TestName = "SLIP Receiving does decode data, delimiter encoding")]
        //[TestCase("ABDA", "B", TestName = "SLIP Receiving does decode data, escaping encoding")]
        //[TestCase("ABCBDCA", "ABC", TestName = "SLIP Receiving does decode data, full encoding")]
        public void TestReceiveIsDecoded(string toDecode, string expectedDecoded)
        {
            _buffer = new byte[toDecode.Length];

            for (int i = 0; i < toDecode.Length; i++)
            {
                _buffer[i] = (byte) toDecode[i];
            }

            _serial.Receive(ref Arg.Any<byte[]>()).Returns(x => {
                x[1] = _buffer;
                return toDecode.Length;
            });

            int l = expectedDecoded.Length;

            var actual = new byte[l];
            int recv = _uut.Receive(ref actual, toDecode.Length);

            byte[] expected = new byte[l];

            for (int i = 0; i < l; i++)
            {
                expected[i] = (byte) expectedDecoded[i];
            }

            Assert.That(recv, Is.EqualTo(toDecode.Length));
            Assert.That(actual.SequenceEqual(expected));
        }
    }
}