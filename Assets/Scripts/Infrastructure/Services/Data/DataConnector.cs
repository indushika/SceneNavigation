using VContainer;

namespace MonsterFactory.Services.DataManagement
{
    public partial class DataConnector : IGenericConnector
    {
        private readonly DataInstanceProvider dataInstanceProvider;

        [Inject]
        public DataConnector(DataInstanceProvider dataInstanceProvider)
        {
            this.dataInstanceProvider = dataInstanceProvider;
        }
    }
}