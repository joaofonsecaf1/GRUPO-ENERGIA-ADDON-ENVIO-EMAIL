namespace B2F.Addon.EnvioEmail.Utils
{
    public class Base
    {
        public static string Database;

        public static string DatabaseInvent;

        public IDAO DAO
        {
            get;
            set;
        }

        public IDAO DAOInvent
        {
            get;
            set;
        }

        static Base()
        {
            Base.Database = System.Configuration.ConfigurationManager.AppSettings["Database"];
            Base.DatabaseInvent = System.Configuration.ConfigurationManager.AppSettings["DatabaseInvent"];
        }

        public Base()
        {
            this.DAO = new HanaDAO();
        }
    }
}
