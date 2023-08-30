namespace Jay.Reflection.Text;

public enum Naming
{
    /// <summary>
    /// Default / No Changes
    /// </summary>
    /// <remarks>
    /// "membername" -> "membername"<br/>
    /// "memberName" -> "memberName"<br/>
    /// "MemberName" -> "MemberName"<br/>
    /// "MEMBERNAME" -> "MEMBERNAME"<br/>
    /// </remarks>
    Default = 0,

    /// <summary>
    /// Lowercase
    /// </summary>
    /// <remarks>
    /// "membername" -> "membername"<br/>
    /// "memberName" -> "membername"<br/>
    /// "MemberName" -> "membername"<br/>
    /// "MEMBERNAME" -> "membername"<br/>
    /// </remarks>
    Lower,

    /// <summary>
    /// Uppercase
    /// </summary>
    /// <remarks>
    /// "membername" -> "MEMBERNAME"<br/>
    /// "memberName" -> "MEMBERNAME"<br/>
    /// "MemberName" -> "MEMBERNAME"<br/>
    /// "MEMBERNAME" -> "MEMBERNAME"<br/>
    /// </remarks>
    Upper,

    /// <summary>
    /// Camelcase
    /// </summary>
    /// <remarks>
    /// "membername" -> "membername"<br/>
    /// "memberName" -> "memberName"<br/>
    /// "MemberName" -> "memberName"<br/>
    /// "MEMBERNAME" -> "mEMBERNAME"<br/>
    /// </remarks>
    Camel,

    /// <summary>
    /// Pascalcase
    /// </summary>
    /// <remarks>
    /// "membername" -> "Membername"<br/>
    /// "memberName" -> "MemberName"<br/>
    /// "MemberName" -> "MemberName"<br/>
    /// "MEMBERNAME" -> "MEMBERNAME"<br/>
    /// </remarks>
    Pascal,

    ///// <summary>
    ///// Title Case
    ///// </summary>
    ///// <remarks>
    ///// <see cref="TextInfo.ToTitleCase(string)"/>
    ///// </remarks>
    //Title,

    /// <summary>
    /// Snake Case
    /// </summary>
    /// <remarks>
    /// "MemberName" => "member_name"
    /// "NameOfFBI" => "name_of_fbi"
    /// </remarks>
    //Snake,

    /// <summary>
    /// C# private field naming convention
    /// </summary>
    /// <remarks>
    /// "membername" -> "_membername"<br/>
    /// "memberName" -> "_memberName"<br/>
    /// "MemberName" -> "_memberName"<br/>
    /// "MEMBERNAME" -> "_mEMBERNAME"<br/>
    /// </remarks>
    Field,
}