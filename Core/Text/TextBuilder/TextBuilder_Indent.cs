using System;

namespace Jay.Text
{
    public partial class TextBuilder
    {
        public TextBuilder Indent(string indent,
                                  WriteText<TextBuilder> indentedWrite)
        {
            string oldNewLine = _newLine;
            _newLine = oldNewLine + indent;
            indentedWrite(this);
            _newLine = oldNewLine;
            return this;
        }
    }
}