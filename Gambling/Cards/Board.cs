/*using System.Buffers.Text;
using System.Diagnostics;
using System.Text;
using Jay;
using Jay.Text;

namespace TinyCards
{
    /*  [♠] [♥] [♣] [♦]     [ ] [ ]   <- Discard, draw (24 cards)
     *  [U] [D] [D] [D] [D] [D] [D]   <- 28 cards used in these piles
     *      [U] [D] [D] [D] [D] [D]
     *          [U] [D] [D] [D] [D]
     *              [U] [D] [D] [D]
     *                  [U] [D] [D]
     *                      [U] [D]
     *                          [U]
     *  013 014 015 016 017 018 019   <- Worst case stack length, K here and someone wants to move Q->A on top (+12 length to each column)
     *
     * 4 + 24 + 28 + (13 + 14 + 15 + 16 + 17 + 18 + 19) = 168
     * #1#
    
    public class Board
    {
        private readonly byte[] _bytes;

        public ref byte Spades => ref _bytes[0];
        public ref byte Hearts => ref _bytes[1];
        public ref byte Clubs => ref _bytes[2];
        public ref byte Diamonds => ref _bytes[3];
        
        public Board()
        {
            _bytes = new byte[168];
        }

        public void Reset()
        {
            _bytes[0] = (byte) ((int) Suit.Spade << 4);
            _bytes[1] = (byte) ((int) Suit.Heart << 4);
            _bytes[2] = (byte) ((int) Suit.Club << 4);
            _bytes[3] = (byte) ((int) Suit.Diamond << 4);
            NotSafe.InitBlock(ref _bytes[4], 0, 168 - 4);
        }

        public void Deal(byte[] cards)
        {
            Debug.Assert(cards.Length == 52);
            Reset();
            int b = 4;
            int c = 0;
            for (; b <= 4 + 24; b++)
            {
                _bytes[b] = cards[c++];
            }
            for (var emptySlots = 0; emptySlots < 7; emptySlots++)
            {
                for (var i = 0; i < emptySlots; i++)
                {
                    // This is empty
                    b++;
                }
                _bytes[b++] = cards[c++].WithFace(Face.Up);
                for (var i = emptySlots + 1; i < 7; i++)
                {
                    _bytes[b++] = cards[c++];
                }
            }
            Debug.Assert(c == 52);
            Debug.Assert(b == 77);
        }

        public string Base64Encoded()
        {
            return TextEncoding.Base64Encode(_bytes);
        }
    }
}*/