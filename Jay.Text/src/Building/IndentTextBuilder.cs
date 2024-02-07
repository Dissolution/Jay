// using System.Diagnostics;
// using Jay.Text.Splitting;
//
// namespace Jay.Text.Building;
//
// public class IndentTextBuilder<B> : FluentTextBuilder<B>, IIndentTextBuilder<B>
//     where B : IndentTextBuilder<B>
// {
//     protected static string DefaultNewLine { get; set; } = Environment.NewLine;
//     protected readonly NewLineAndIndentManager _newLineAndIndentManager = new();
//
//
//     public IndentTextBuilder()
//         : this(TextPool.MINIMUM_CAPACITY)
//     { }
//
//     public IndentTextBuilder(int minCapacity) 
//         : base(minCapacity)
//     {
//     }
//
//     /// <summary>
//     /// Gets the NewLine + Indent that exists at the current position
//     /// </summary>
//     /// <returns></returns>
//     protected ReadOnlySpan<char> GetCurrentIndent()
//     {
//         var nli = _newLineAndIndentManager.CurrentNewLineAndIndent;
//         var written = this.Written;
//         var i = written.LastIndexOf<char>(nli);
//         if (i == -1)
//         {
//             // No new indent
//             return default;
//         }
//
//         var indent = written.Slice(i + nli.Length);
//         //string ind = indent.ToString();
//         return indent;
//     }
//
//     internal void IndentAwareAction(Action<B> build)
//     {
//         var newIndent = GetCurrentIndent();
//         if (newIndent.Length == 0)
//         {
//             build(_self);
//         }
//         else
//         {
//             _newLineAndIndentManager.AddIndent(newIndent);
//             build(_self);
// #if DEBUG
//             _newLineAndIndentManager.RemoveIndent(out var removedIndent);
//             Debug.Assert(newIndent.SequenceEqual(removedIndent));
//
// #else
//             _newLineAndIndentManager.RemoveIndent();
// #endif
//         }
//     }
//
//     internal void WriteIndentAwareText(ReadOnlySpan<char> text)
//     {
//         // Replace embedded NewLines with NewLine + Indent
//         var split = text.TextSplit(DefaultNewLine);
//         while (split.MoveNext())
//         {
//             this.Append(split.Text);
//             while (split.MoveNext())
//             {
//                 this.NewLine().Append(split.Text);
//             }
//         }
//     }
//     
//     
//     public override B NewLine()
//     {
//         base.Write(_newLineAndIndentManager.CurrentNewLineAndIndent);
//         return _self;
//     }
//
//     public override void Write(params char[]? characters) 
//         => WriteIndentAwareText(characters.AsSpan());
//
//     public override void Write(scoped ReadOnlySpan<char> text) 
//         => WriteIndentAwareText(text);
//
//     public override void Write(string? str) 
//         => WriteIndentAwareText(str.AsSpan());
//
//     public override void Write<T>([AllowNull] T value)
//     {
//         switch (value)
//         {
//             case null:
//             {
//                 return;
//             }
//             case Action<B> build:
//             {
//                 IndentAwareAction(build);
//                 return;
//             }
//             case string str:
//             {
//                 this.Write(str);
//                 return;
//             }
//             default:
//             {
//                 this.Write(value.ToString());
//                 return;
//             }
//         }
//     }
//
//     public B AddIndent(char indent)
//     {
//         _newLineAndIndentManager.AddIndent(indent);
//         return _self;
//     }
//
//     public B AddIndent(string indent)
//     {
//         _newLineAndIndentManager.AddIndent(indent);
//         return _self;
//     }
//
//     public B AddIndent(scoped ReadOnlySpan<char> indent)
//     {
//         _newLineAndIndentManager.AddIndent(indent);
//         return _self;
//     }
//
//     public B RemoveIndent()
//     {
//         _newLineAndIndentManager.RemoveIndent();
//         return _self;
//     }
//
//     public B RemoveIndent(out ReadOnlySpan<char> lastIndent)
//     {
//         _newLineAndIndentManager.RemoveIndent(out lastIndent);
//         return _self;
//     }
//
//     public B Indented(char indent, Action<B> buildIndentedText) 
//         => AddIndent(indent)
//             .Invoke(buildIndentedText)
//             .RemoveIndent();
//
//     public B Indented(string indent, Action<B> buildIndentedText)
//         => AddIndent(indent)
//             .Invoke(buildIndentedText)
//             .RemoveIndent();
//
//     public B Indented(scoped ReadOnlySpan<char> indent, Action<B> buildIndentedText)
//         => AddIndent(indent)
//             .Invoke(buildIndentedText)
//             .RemoveIndent();
//
//     public override void Dispose()
//     {
//         _newLineAndIndentManager.Dispose();
//         base.Dispose();
//     }
// }