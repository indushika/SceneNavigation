namespace MonsterFactory.Services.DataManagement
{
    public partial class DataConnector
    {
        public string GetUserName()
        {
            return dataInstanceProvider.GetDataObjectOfType<UserData>().UserName;
        }

        public void SetUserName(string userName)
        {
            dataInstanceProvider.GetDataObjectOfType<UserData>().UserName = userName;
        }
    }


    public interface IGenericConnector
    {
        public string GetUserName();
        public void SetUserName(string userName);
    }
}