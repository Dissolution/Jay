namespace Jay.Reflection.Emitting.Scratch.Simple;

public interface IConvertEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    Self IntPtr(bool overflowCheck = false, bool unsigned = false);
    Self SByte(bool overflowCheck = false, bool unsigned = false);
    Self Short(bool overflowCheck = false, bool unsigned = false);
    Self Int(bool overflowCheck = false, bool unsigned = false);
    Self Long(bool overflowCheck = false, bool unsigned = false);
    Self UIntPtr(bool overflowCheck = false, bool unsigned = false);
    Self Byte(bool overflowCheck = false, bool unsigned = false);
    Self UShort(bool overflowCheck = false, bool unsigned = false);
    Self UInt(bool overflowCheck = false, bool unsigned = false);
    Self ULong(bool overflowCheck = false, bool unsigned = false);
    Self Float(bool unsigned = false);
    Self Double();
}