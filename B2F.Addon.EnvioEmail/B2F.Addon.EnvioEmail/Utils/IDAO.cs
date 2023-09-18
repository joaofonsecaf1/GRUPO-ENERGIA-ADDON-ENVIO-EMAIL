using System.Collections.Generic;

namespace B2F.Addon.EnvioEmail.Utils
{
	public interface IDAO
	{
		object Model
		{
			get;
			set;
		}

		void BeginTransaction();

		void Close();

		void CommitTransaction();

		void Connect();

		void ExecuteNonQuery(string command);

		object ExecuteScalar(string command);

		List<T> FillListFromCommand<T>(string command);

		string GetConnectedServer();

		void RollbackTransaction();
	}
}
