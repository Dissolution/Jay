using System;
using System.Diagnostics;

namespace TinyCards
{
    public ref struct SuitStack
    {
        private readonly Span<Card> _stack;
        
        public readonly Suit Suit;

        public SuitStack(Span<Card> stack, Suit suit)
        {
            Debug.Assert(stack.Length == 13);
            _stack = stack;
            Suit = suit;
        }
    }
}