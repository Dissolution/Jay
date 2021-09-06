using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Jay.Text;

using static InlineIL.IL;
// ReSharper disable EntityNameCapturedOnly.Global

namespace Jay.Gambling.Cards
{
    internal sealed class RankComparer : IComparer<Card>
    {
        public int Compare(Card x, Card y) => x.Rank.CompareTo(y.Rank);
    }
    
    [StructLayout(LayoutKind.Explicit, Size = 1)]
    public readonly struct Card : IEquatable<Card>
    {
        private static readonly Display[] _displays;

        static Card()
        {
            _displays = new Display[byte.MaxValue];
            for (Suit suit = Cards.Suit.Spade; suit <= Cards.Suit.Diamond; suit++)
            {
                char suitChar = suit.ToString()[0];
                
                for (Rank rank = Cards.Rank.Ace; rank <= Cards.Rank.King; rank++)
                {
                    byte card = (byte) (((int) rank) & ((int) suit << 4));
                    //_displays[card] = new Display()
                }
            }
        }
        
        //private (char Char, string Unicode)
        
        public static Card[] GetDeck(Face face)
        {
            var cards = new Card[52];
            int i = 0;
            for (Suit suit = Suit.Spade; suit <= Suit.Diamond; suit++)
            {
                for (Rank rank = Rank.Ace; rank <= Rank.King; rank++)
                {
                    cards[i++] = (Card) ((int) rank & ((int) suit) << 4 & ((int) face << 6));
                }
            }
            return cards;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Card x, Card y)
        {
            Emit.Ldarg(nameof(x));
            Emit.Ldarg(nameof(y));
            Emit.Ceq();
            return Return<bool>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Card x, Card y)
        {
            Emit.Ldarg(nameof(x));
            Emit.Ldarg(nameof(y));
            Emit.Ceq();
            Emit.Not();
            return Return<bool>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator &(Card card, int number)
        {
            Emit.Ldarg(nameof(card));
            Emit.Conv_I4();
            Emit.Ldarg(nameof(number));
            Emit.And();
            return Return<int>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator |(Card card, int number)
        {
            Emit.Ldarg(nameof(card));
            Emit.Conv_I4();
            Emit.Ldarg(nameof(number));
            Emit.Or();
            return Return<int>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Card(byte b)
        {
            Emit.Ldarg(nameof(b));
            return Return<Card>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator byte(Card card)
        {
            Emit.Ldarg(nameof(card));
            return Return<byte>();
        }

        public static IComparer<Card> RankComparer { get; } = new RankComparer();
        
      
            
      

        public Rank Rank => (Rank) (this & 0b00001111);
        public Suit Suit => (Suit) ((this & 0b00110000) >> 4);
        public Face Face => (Face) ((this & 0b01000000) >> 6);

        public Card WithRank(Rank rank)
        {
            return (Card) ((this & 0b11110000) | (int) rank);
        }
        public Card WithSuit(Suit suit)
        {
            return (Card) ((this & 0b11001111) | ((int) suit << 4));
        }
        public Card WithFace(Face face)
        {
            return (Card) ((this & 0b10111111) | ((int) face << 6));
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Card card)
        {
            Emit.Ldarg_0();
            Emit.Ldarg(nameof(card));
            Emit.Ceq();
            return Return<bool>();
        }
        
        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Card card && Equals(card);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            Emit.Ldarg_0();
            Emit.Conv_I4();
            return Return<int>();
        }

        /*
        public void Write<TWriter>(TWriter writer, CardDisplay display = CardDisplay.Default)
            where TWriter : ITextWriter<TWriter>
        {
            if (display == CardDisplay.UnicodeSymbols)
            {
                switch (this.Rank)
                {
                    case Rank.Ace:
                        writer.Append("Ａ");
                        break;
                    case Rank.Two:
                        writer.Append("２");
                        break;
                    case Rank.Three:
                        writer.Append("３");
                        break;
                    case Rank.Four:
                        writer.Append("４");
                        break;
                    case Rank.Five:
                        writer.Append("５");
                        break;
                    case Rank.Six:
                        writer.Append("６");
                        break;
                    case Rank.Seven:
                        writer.Append("７");
                        break;
                    case Rank.Eight:
                        writer.Append("８");
                        break;
                    case Rank.Nine:
                        writer.Append("９");
                        break;
                    case Rank.Ten:
                        writer.Append("10");
                        break;
                    case Rank.Jack:
                        writer.Append("Ｊ");
                        break;
                    case Rank.Queen:
                        writer.Append("Ｑ");
                        break;
                    case Rank.King:
                        writer.Append("Ｋ");
                        break;
                    case Rank.Knight:
                        writer.Append("Ｃ");
                        break;
                    case Rank.Joker:
                        writer.Append("Ｋ");
                        break;
                }

                switch (this.Suit)
                {
                    case Suit.Spade:
                        writer.Append("♠");
                        break;
                    case Suit.Heart:
                        writer.Append("♥");
                        break;
                    case Suit.Club:
                        writer.Append("♣");
                        break;
                    case Suit.Diamond:
                        writer.Append("♦");
                        break;
                }

                return;
            }
            
            switch (Suit, Rank)
            {
                case (Suit.Spade, Rank.Ace):
                {
                    if (display == CardDisplay.Default)
                    {
                        writer.Write("AS");
                    }
                    else if (display == CardDisplay.UnicodeSymbol)
                    {
                        writer.Write("🂡");
                    }
                    return;
                }
                case (Suit.Spade, Rank.Two):
                {
                    if (display == CardDisplay.Default)
                    {
                        writer.Write("2S");
                    }
                    else if (display == CardDisplay.UnicodeSymbol)
                    {
                        writer.Write("🂢");
                    }
                    return;
                }
                case (Suit.Spade, Rank.Three):
                {
                    if (display == CardDisplay.Default)
                    {
                        writer.Write("3S");
                    }
                    else if (display == CardDisplay.UnicodeSymbol)
                    {
                        writer.Write("🂣");
                    }
                    return;
                }
                case (Suit.Spade, Rank.Four):
                {
                    if (display == CardDisplay.Default)
                    {
                        writer.Write("4S");
                    }
                    else if (display == CardDisplay.UnicodeSymbol)
                    {
                        writer.Write("🂤");
                    }
                    return;
                }
                case (Suit.Spade, Rank.Five):
                {
                    if (display == CardDisplay.Default)
                    {
                        writer.Write("5S");
                    }
                    else if (display == CardDisplay.UnicodeSymbol)
                    {
                        writer.Write("🂤");
                    }
                    return;
                }
                    return writer.Append("🂥");
                case (Suit.Spade, Rank.Six):
                    return writer.Append("🂦");
                case (Suit.Spade, Rank.Seven):
                    return writer.Append("🂧");
                case (Suit.Spade, Rank.Eight):
                    return writer.Append("🂨");
                case (Suit.Spade, Rank.Nine):
                    return writer.Append("🂩");
                case (Suit.Spade, Rank.Ten):
                    return writer.Append("🂪");
                case (Suit.Spade, Rank.Jack):
                    return writer.Append("🂫");
                case (Suit.Spade, Rank.Knight):
                    return writer.Append("🂬");
                case (Suit.Spade, Rank.Queen):
                    return writer.Append("🂭");
                case (Suit.Spade, Rank.King):
                    return writer.Append("🂮");

                case (Suit.Heart, Rank.Ace):
                    return writer.Append("🂱");
                case (Suit.Heart, Rank.Two):
                    return writer.Append("🂲");
                case (Suit.Heart, Rank.Three):
                    return writer.Append("🂳");
                case (Suit.Heart, Rank.Four):
                    return writer.Append("🂴");
                case (Suit.Heart, Rank.Five):
                    return writer.Append("🂵");
                case (Suit.Heart, Rank.Six):
                    return writer.Append("🂶");
                case (Suit.Heart, Rank.Seven):
                    return writer.Append("🂷");
                case (Suit.Heart, Rank.Eight):
                    return writer.Append("🂸");
                case (Suit.Heart, Rank.Nine):
                    return writer.Append("🂹");
                case (Suit.Heart, Rank.Ten):
                    return writer.Append("🂺");
                case (Suit.Heart, Rank.Jack):
                    return writer.Append("🂻");
                case (Suit.Heart, Rank.Knight):
                    return writer.Append("🂼");
                case (Suit.Heart, Rank.Queen):
                    return writer.Append("🂽");
                case (Suit.Heart, Rank.King):
                    return writer.Append("🂾");

                case (Suit.Diamond, Rank.Ace):
                    return writer.Append("🃁");
                case (Suit.Diamond, Rank.Two):
                    return writer.Append("🃂");
                case (Suit.Diamond, Rank.Three):
                    return writer.Append("🃃");
                case (Suit.Diamond, Rank.Four):
                    return writer.Append("🃄");
                case (Suit.Diamond, Rank.Five):
                    return writer.Append("🃅");
                case (Suit.Diamond, Rank.Six):
                    return writer.Append("🃆");
                case (Suit.Diamond, Rank.Seven):
                    return writer.Append("🃇");
                case (Suit.Diamond, Rank.Eight):
                    return writer.Append("🃈");
                case (Suit.Diamond, Rank.Nine):
                    return writer.Append("🃉");
                case (Suit.Diamond, Rank.Ten):
                    return writer.Append("🃊");
                case (Suit.Diamond, Rank.Jack):
                    return writer.Append("🃋");
                case (Suit.Diamond, Rank.Knight):
                    return writer.Append("🃌");
                case (Suit.Diamond, Rank.Queen):
                    return writer.Append("🃍");
                case (Suit.Diamond, Rank.King):
                    return writer.Append("🃎");

                case (Suit.Club, Rank.Ace):
                    return writer.Append("🃑");
                case (Suit.Club, Rank.Two):
                    return writer.Append("🃒");
                case (Suit.Club, Rank.Three):
                    return writer.Append("🃓");
                case (Suit.Club, Rank.Four):
                    return writer.Append("🃔");
                case (Suit.Club, Rank.Five):
                    return writer.Append("🃕");
                case (Suit.Club, Rank.Six):
                    return writer.Append("🃖");
                case (Suit.Club, Rank.Seven):
                    return writer.Append("🃗");
                case (Suit.Club, Rank.Eight):
                    return writer.Append("🃘");
                case (Suit.Club, Rank.Nine):
                    return writer.Append("🃙");
                case (Suit.Club, Rank.Ten):
                    return writer.Append("🃚");
                case (Suit.Club, Rank.Jack):
                    return writer.Append("🃛");
                case (Suit.Club, Rank.Knight):
                    return writer.Append("🃜");
                case (Suit.Club, Rank.Queen):
                    return writer.Append("🃝");
                case (Suit.Club, Rank.King):
                    return writer.Append("🃞");

                default:
                    return writer.Append("🂠");
            }
        }
        */
        


        /// <inheritdoc />
        public override string ToString()
        {
            return TextBuilder.Build(this, (text, card) =>
            {
                var rank = card.Rank;
                if (rank == Rank.Ace)
                    text.Append('A');
                else if (rank >= Cards.Rank.Two && rank <= Cards.Rank.Ten)
                    text.Append((int) rank);
                else if (rank == Rank.Jack)
                    text.Append('J');
                else if (rank == Rank.Queen)
                    text.Append('Q');
                else if (rank == Rank.King)
                    text.Append('K');
                switch (card.Suit)
                {
                    case Suit.Spade:
                        text.Append('S');
                        return;
                    case Suit.Heart:
                        text.Append('H');
                        return;
                    case Suit.Club:
                        text.Append('C');
                        return;
                    case Suit.Diamond:
                        text.Append('D');
                        return;
                }
            });
        }

     
    }
    /*
   
        public static TextBuilder AppendCard(this TextBuilder textBuilder,
                                             byte card,
                                             CardDisplay display = CardDisplay.UnicodeSymbol)
        {
            var rank = card.GetRank();
            var suit = card.GetSuit();

            if (display == CardDisplay.UnicodeSymbol)
            {
                if (rank == Rank.Joker)
                {
                    if (suit.IsRed())
                        return writer.Append("🃏");
                    return writer.Append("🂿");
                    // White Joker
                    // 🃟
                }

                switch (suit, rank)
                {
                    case (Suit.Spade, Rank.Ace):
                        return writer.Append("🂡");
                    case (Suit.Spade, Rank.Two):
                        return writer.Append("🂢");
                    case (Suit.Spade, Rank.Three):
                        return writer.Append("🂣");
                    case (Suit.Spade, Rank.Four):
                        return writer.Append("🂤");
                    case (Suit.Spade, Rank.Five):
                        return writer.Append("🂥");
                    case (Suit.Spade, Rank.Six):
                        return writer.Append("🂦");
                    case (Suit.Spade, Rank.Seven):
                        return writer.Append("🂧");
                    case (Suit.Spade, Rank.Eight):
                        return writer.Append("🂨");
                    case (Suit.Spade, Rank.Nine):
                        return writer.Append("🂩");
                    case (Suit.Spade, Rank.Ten):
                        return writer.Append("🂪");
                    case (Suit.Spade, Rank.Jack):
                        return writer.Append("🂫");
                    case (Suit.Spade, Rank.Knight):
                        return writer.Append("🂬");
                    case (Suit.Spade, Rank.Queen):
                        return writer.Append("🂭");
                    case (Suit.Spade, Rank.King):
                        return writer.Append("🂮");

                    case (Suit.Heart, Rank.Ace):
                        return writer.Append("🂱");
                    case (Suit.Heart, Rank.Two):
                        return writer.Append("🂲");
                    case (Suit.Heart, Rank.Three):
                        return writer.Append("🂳");
                    case (Suit.Heart, Rank.Four):
                        return writer.Append("🂴");
                    case (Suit.Heart, Rank.Five):
                        return writer.Append("🂵");
                    case (Suit.Heart, Rank.Six):
                        return writer.Append("🂶");
                    case (Suit.Heart, Rank.Seven):
                        return writer.Append("🂷");
                    case (Suit.Heart, Rank.Eight):
                        return writer.Append("🂸");
                    case (Suit.Heart, Rank.Nine):
                        return writer.Append("🂹");
                    case (Suit.Heart, Rank.Ten):
                        return writer.Append("🂺");
                    case (Suit.Heart, Rank.Jack):
                        return writer.Append("🂻");
                    case (Suit.Heart, Rank.Knight):
                        return writer.Append("🂼");
                    case (Suit.Heart, Rank.Queen):
                        return writer.Append("🂽");
                    case (Suit.Heart, Rank.King):
                        return writer.Append("🂾");

                    case (Suit.Diamond, Rank.Ace):
                        return writer.Append("🃁");
                    case (Suit.Diamond, Rank.Two):
                        return writer.Append("🃂");
                    case (Suit.Diamond, Rank.Three):
                        return writer.Append("🃃");
                    case (Suit.Diamond, Rank.Four):
                        return writer.Append("🃄");
                    case (Suit.Diamond, Rank.Five):
                        return writer.Append("🃅");
                    case (Suit.Diamond, Rank.Six):
                        return writer.Append("🃆");
                    case (Suit.Diamond, Rank.Seven):
                        return writer.Append("🃇");
                    case (Suit.Diamond, Rank.Eight):
                        return writer.Append("🃈");
                    case (Suit.Diamond, Rank.Nine):
                        return writer.Append("🃉");
                    case (Suit.Diamond, Rank.Ten):
                        return writer.Append("🃊");
                    case (Suit.Diamond, Rank.Jack):
                        return writer.Append("🃋");
                    case (Suit.Diamond, Rank.Knight):
                        return writer.Append("🃌");
                    case (Suit.Diamond, Rank.Queen):
                        return writer.Append("🃍");
                    case (Suit.Diamond, Rank.King):
                        return writer.Append("🃎");

                    case (Suit.Club, Rank.Ace):
                        return writer.Append("🃑");
                    case (Suit.Club, Rank.Two):
                        return writer.Append("🃒");
                    case (Suit.Club, Rank.Three):
                        return writer.Append("🃓");
                    case (Suit.Club, Rank.Four):
                        return writer.Append("🃔");
                    case (Suit.Club, Rank.Five):
                        return writer.Append("🃕");
                    case (Suit.Club, Rank.Six):
                        return writer.Append("🃖");
                    case (Suit.Club, Rank.Seven):
                        return writer.Append("🃗");
                    case (Suit.Club, Rank.Eight):
                        return writer.Append("🃘");
                    case (Suit.Club, Rank.Nine):
                        return writer.Append("🃙");
                    case (Suit.Club, Rank.Ten):
                        return writer.Append("🃚");
                    case (Suit.Club, Rank.Jack):
                        return writer.Append("🃛");
                    case (Suit.Club, Rank.Knight):
                        return writer.Append("🃜");
                    case (Suit.Club, Rank.Queen):
                        return writer.Append("🃝");
                    case (Suit.Club, Rank.King):
                        return writer.Append("🃞");

                    default:
                        return writer.Append("🂠");
                }
            }
            else if (display == CardDisplay.UnicodeSymbols)
            {
                switch (rank)
                {
                    case Rank.Ace:
                        writer.Append("Ａ");
                        break;
                    case Rank.Two:
                        writer.Append("２");
                        break;
                    case Rank.Three:
                        writer.Append("３");
                        break;
                    case Rank.Four:
                        writer.Append("４");
                        break;
                    case Rank.Five:
                        writer.Append("５");
                        break;
                    case Rank.Six:
                        writer.Append("６");
                        break;
                    case Rank.Seven:
                        writer.Append("７");
                        break;
                    case Rank.Eight:
                        writer.Append("８");
                        break;
                    case Rank.Nine:
                        writer.Append("９");
                        break;
                    case Rank.Ten:
                        writer.Append("10");
                        break;
                    case Rank.Jack:
                        writer.Append("Ｊ");
                        break;
                    case Rank.Queen:
                        writer.Append("Ｑ");
                        break;
                    case Rank.King:
                        writer.Append("Ｋ");
                        break;
                    case Rank.Knight:
                        writer.Append("Ｃ");
                        break;
                    case Rank.Joker:
                        writer.Append("Ｋ");
                        break;
                }
                switch (suit)
                {
                    case Suit.Spade:
                        writer.Append("♠");
                        break;
                    case Suit.Heart:
                        writer.Append("♥");
                        break;
                    case Suit.Club:
                        writer.Append("♣");
                        break;
                    case Suit.Diamond:
                        writer.Append("♦");
                        break;
                }
                return textBuilder;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
    */
}