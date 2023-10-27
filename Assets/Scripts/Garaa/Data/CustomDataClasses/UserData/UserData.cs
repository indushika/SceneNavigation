using System;
using MonsterFactory.Services.DataManagement;

[Serializable]
public class UserData : MFData
{
    private string userName;

    public string UserName
    {
        get => userName;
        set => UpdateDataIfNeeded(ref userName, value);
    }

    public override Type GetDataType()
    {
        return GetType();
    }

    public override bool IsLocallyStored()
    {
        return true;
    }
}

