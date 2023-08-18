# Flow Attributes:

`[DoesNotReturn]`

- Specifies that a method that will never return under any circumstance.
- Place _before_ the Method declaration

`[DoesNotReturnIf(bool)]`

- Specifies that the method will not return if the associated Boolean parameter is passed the specified value.
- `void Method([DoesNotReturnIf(true|false)] bool b)`