using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Jay.Text;

namespace TinyCards
{
    [StructLayout(LayoutKind.Explicit, Size = 1)]
    public struct Card
    {
        private int _value;
        
        public Rank Rank
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Rank) (_value & 0b00001111);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _value = ((_value & 0b11110000) | (int)value);
        }

        public Suit Suit
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Suit) ((_value& 0b00110000) >> 4);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _value = ((_value & 0b11001111) | ((int) value) << 4);
        }
        
        public Face Face
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Face) ((_value & 0b01000000) >> 6);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _value = ((_value & 0b10111111) | ((int) value) << 6);
        }

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value == 0;
        }

        internal Card(int value)
        {
            _value = value;
        }

        public bool Equals(Card card)
        {
            return _value == card._value;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => false;

        /// <inheritdoc />
        public override int GetHashCode() => _value;

        private static TextBuilder AppendUnicode(TextBuilder text, Rank rank, Suit suit, CardDisplay display)
        {
            if (display == CardDisplay.UnicodeSymbol)
            {
                if (rank == Rank.Joker)
                {
                    if (suit.IsRed())
                        return text.Append("🃏");
                    return text.Append("🂿");
                    // White Joker
                    // 🃟
                }

                switch (suit, rank)
                {
                    case (Suit.Spade, Rank.Ace):
                        return text.Append("🂡");
                    case (Suit.Spade, Rank.Two):
                        return text.Append("🂢");
                    case (Suit.Spade, Rank.Three):
                        return text.Append("🂣");
                    case (Suit.Spade, Rank.Four):
                        return text.Append("🂤");
                    case (Suit.Spade, Rank.Five):
                        return text.Append("🂥");
                    case (Suit.Spade, Rank.Six):
                        return text.Append("🂦");
                    case (Suit.Spade, Rank.Seven):
                        return text.Append("🂧");
                    case (Suit.Spade, Rank.Eight):
                        return text.Append("🂨");
                    case (Suit.Spade, Rank.Nine):
                        return text.Append("🂩");
                    case (Suit.Spade, Rank.Ten):
                        return text.Append("🂪");
                    case (Suit.Spade, Rank.Jack):
                        return text.Append("🂫");
                    case (Suit.Spade, Rank.Knight):
                        return text.Append("🂬");
                    case (Suit.Spade, Rank.Queen):
                        return text.Append("🂭");
                    case (Suit.Spade, Rank.King):
                        return text.Append("🂮");

                    case (Suit.Heart, Rank.Ace):
                        return text.Append("🂱");
                    case (Suit.Heart, Rank.Two):
                        return text.Append("🂲");
                    case (Suit.Heart, Rank.Three):
                        return text.Append("🂳");
                    case (Suit.Heart, Rank.Four):
                        return text.Append("🂴");
                    case (Suit.Heart, Rank.Five):
                        return text.Append("🂵");
                    case (Suit.Heart, Rank.Six):
                        return text.Append("🂶");
                    case (Suit.Heart, Rank.Seven):
                        return text.Append("🂷");
                    case (Suit.Heart, Rank.Eight):
                        return text.Append("🂸");
                    case (Suit.Heart, Rank.Nine):
                        return text.Append("🂹");
                    case (Suit.Heart, Rank.Ten):
                        return text.Append("🂺");
                    case (Suit.Heart, Rank.Jack):
                        return text.Append("🂻");
                    case (Suit.Heart, Rank.Knight):
                        return text.Append("🂼");
                    case (Suit.Heart, Rank.Queen):
                        return text.Append("🂽");
                    case (Suit.Heart, Rank.King):
                        return text.Append("🂾");

                    case (Suit.Diamond, Rank.Ace):
                        return text.Append("🃁");
                    case (Suit.Diamond, Rank.Two):
                        return text.Append("🃂");
                    case (Suit.Diamond, Rank.Three):
                        return text.Append("🃃");
                    case (Suit.Diamond, Rank.Four):
                        return text.Append("🃄");
                    case (Suit.Diamond, Rank.Five):
                        return text.Append("🃅");
                    case (Suit.Diamond, Rank.Six):
                        return text.Append("🃆");
                    case (Suit.Diamond, Rank.Seven):
                        return text.Append("🃇");
                    case (Suit.Diamond, Rank.Eight):
                        return text.Append("🃈");
                    case (Suit.Diamond, Rank.Nine):
                        return text.Append("🃉");
                    case (Suit.Diamond, Rank.Ten):
                        return text.Append("🃊");
                    case (Suit.Diamond, Rank.Jack):
                        return text.Append("🃋");
                    case (Suit.Diamond, Rank.Knight):
                        return text.Append("🃌");
                    case (Suit.Diamond, Rank.Queen):
                        return text.Append("🃍");
                    case (Suit.Diamond, Rank.King):
                        return text.Append("🃎");

                    case (Suit.Club, Rank.Ace):
                        return text.Append("🃑");
                    case (Suit.Club, Rank.Two):
                        return text.Append("🃒");
                    case (Suit.Club, Rank.Three):
                        return text.Append("🃓");
                    case (Suit.Club, Rank.Four):
                        return text.Append("🃔");
                    case (Suit.Club, Rank.Five):
                        return text.Append("🃕");
                    case (Suit.Club, Rank.Six):
                        return text.Append("🃖");
                    case (Suit.Club, Rank.Seven):
                        return text.Append("🃗");
                    case (Suit.Club, Rank.Eight):
                        return text.Append("🃘");
                    case (Suit.Club, Rank.Nine):
                        return text.Append("🃙");
                    case (Suit.Club, Rank.Ten):
                        return text.Append("🃚");
                    case (Suit.Club, Rank.Jack):
                        return text.Append("🃛");
                    case (Suit.Club, Rank.Knight):
                        return text.Append("🃜");
                    case (Suit.Club, Rank.Queen):
                        return text.Append("🃝");
                    case (Suit.Club, Rank.King):
                        return text.Append("🃞");

                    default:
                        return text.Append("🂠");
                }
            }
            else if (display == CardDisplay.UnicodeSymbols)
            {
                switch (rank)
                {
                    case Rank.Ace:
                        text.Append("Ａ");
                        break;
                    case Rank.Two:
                        text.Append("２");
                        break;
                    case Rank.Three:
                        text.Append("３");
                        break;
                    case Rank.Four:
                        text.Append("４");
                        break;
                    case Rank.Five:
                        text.Append("５");
                        break;
                    case Rank.Six:
                        text.Append("６");
                        break;
                    case Rank.Seven:
                        text.Append("７");
                        break;
                    case Rank.Eight:
                        text.Append("８");
                        break;
                    case Rank.Nine:
                        text.Append("９");
                        break;
                    case Rank.Ten:
                        text.Append("10");
                        break;
                    case Rank.Jack:
                        text.Append("Ｊ");
                        break;
                    case Rank.Queen:
                        text.Append("Ｑ");
                        break;
                    case Rank.King:
                        text.Append("Ｋ");
                        break;
                    case Rank.Knight:
                        text.Append("Ｃ");
                        break;
                    case Rank.Joker:
                        text.Append("Ｋ");
                        break;
                }
                switch (suit)
                {
                    case Suit.Spade:
                        text.Append("♠");
                        break;
                    case Suit.Heart:
                        text.Append("♥");
                        break;
                    case Suit.Club:
                        text.Append("♣");
                        break;
                    case Suit.Diamond:
                        text.Append("♦");
                        break;
                }
                return text;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return TextBuilder.Build<Card>(this, (text, card) =>
            {
                // Face Down?
                if (card.Face == Face.Down)
                {
                    text.Append("🂠 (");
                }

                AppendUnicode(text, card.Rank, card.Suit, CardDisplay.UnicodeSymbol);
                
                if (card.Face == Face.Down)
                {
                    text.Append(')');
                }
            });
        }
    }
}