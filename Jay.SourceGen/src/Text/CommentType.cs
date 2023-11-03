namespace Jay.SourceGen.Text;

[Flags]
public enum CommentType
{
    Any = 0,
    
    /// <summary>
    /// <c>// comment</c><br/>
    /// <i>or</i><br/>
    /// <c>
    /// // comment 1<br/>
    /// // comment 2<br/>
    /// </c>
    /// </summary>a
    SingleLine = 1 << 0,

    /// <summary>
    /// <c>/* comment */</c><br/>
    /// <i>or</i><br/>
    /// <c>
    /// /* comment 1<br/>
    ///  * comment 2<br/>
    ///  */<br/>
    /// </c>
    /// </summary>
    MultiLine = 1 << 1,

    /// <summary>
    /// <c>/// comment</c><br />
    /// <i>or</i><br />
    /// <c>
    /// /// comment 1<br />
    /// /// comment 2<br />
    /// </c>
    /// </summary>
    Xml = 1 << 2,
}