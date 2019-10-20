using Protocol;

using System.Collections;

using ProtocolCore;

public class RpcResponese : ProtoData
{
    [ProtocolSerializeField]
    public int Code;

    [ProtocolSerializeField]
    public string Message;

    [ProtocolSerializeField]
    public byte[] Data;

    public RpcResponese()
    {
    }
    public RpcResponese(int code)
    {
        Code = code;
       // Message = MultiLanguage.Instance.GetMessage(code);
    }
}
