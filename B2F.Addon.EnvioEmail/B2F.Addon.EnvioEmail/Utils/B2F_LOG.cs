using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace B2F.Addon.EnvioEmail
{
    public class B2F_LOG
    {
        public string Code { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string ObjectType { get; set; }
        public string DocEntry { get; set; }
        [JsonProperty("U_B2F_TipoDoc")]
        public string B2F_TipoDoc { get; set; }
        [JsonProperty("U_B2F_IdDoc")]
        public string B2F_IdDoc { get; set; }
        [JsonProperty("U_B2F_DtInteg")]
        public string B2F_DtInteg { get; set; }
        [JsonProperty("U_B2F_Status")]
        public string B2F_Status { get; set; }
        [JsonProperty("U_B2F_IdRet")]
        public string B2F_IdRet { get; set; }
        [JsonProperty("U_B2F_MsgRet")]
        public string B2F_MsgRet { get; set; }
        [JsonProperty("U_B2F_JsonEnv")]
        public object B2F_JsonEnv { get; set; }
        [JsonProperty("U_B2F_JsonRet")]
        public object B2F_JsonRet { get; set; }
        [JsonProperty("U_B2F_IdDocLeg")]
        public string B2F_IdDocLeg { get; set; }
        
        public void InsertOrUpdateLog()
        {
            HanaDAO dao = new HanaDAO();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;

            var AppName = "ADDON_ENVIO_EMAIL";
            if (!string.IsNullOrEmpty(this.ObjectType))
            {
                this.B2F_TipoDoc = (string)dao.ExecuteScalar($@"SELECT ""U_Table_Desc"" FROM ""{HanaDAO.Database}"".""@B2F_OBJETOS"" WHERE ""U_ObjectType"" ='{this.ObjectType}'");
            }

            this.B2F_JsonEnv = this.B2F_JsonEnv != null ? JsonConvert.SerializeObject(this.B2F_JsonEnv, settings).Replace("'", "") : null;
            this.B2F_JsonRet = this.B2F_JsonRet != null ? JsonConvert.SerializeObject(this.B2F_JsonRet, settings).Replace("'", "") : null;
            var total = dao.ExecuteScalar($@"SELECT COUNT(*) AS ""Total"" FROM ""{HanaDAO.Database}"".""OUDO"" WHERE ""TableName"" = 'B2F_LOG' ");
            if (total.ToString() != "1")
            {

                var exists = dao.ExecuteScalar(string.Format(File.ReadAllText(@"Queries\SelectB2F_LOG.sql"), HanaDAO.Database, AppName, B2F_TipoDoc, B2F_IdDocLeg));
                if (exists != null)
                {
                    dao.ExecuteNonQuery(string.Format(File.ReadAllText(@"Queries\UpdateB2F_LOG_OLD.sql"), HanaDAO.Database, AppName, B2F_TipoDoc, B2F_IdDoc, B2F_DtInteg, B2F_Status, B2F_IdRet, B2F_MsgRet, B2F_JsonEnv, B2F_JsonRet, B2F_IdDocLeg));
                }
                else
                {
                    dao.ExecuteNonQuery(string.Format(File.ReadAllText(@"Queries\InsertB2F_LOG_OLD.sql"), HanaDAO.Database, AppName, B2F_TipoDoc, B2F_IdDoc, B2F_DtInteg, B2F_Status, B2F_IdRet, B2F_MsgRet, B2F_JsonEnv, B2F_JsonRet, B2F_IdDocLeg));
                }
            }
            else
            {
                var exists = dao.ExecuteScalar(string.Format(File.ReadAllText(@"Queries\SelectB2F_LOG.sql"), HanaDAO.Database, AppName, B2F_TipoDoc, B2F_IdDocLeg));
                if (exists != null)
                {
                    dao.ExecuteNonQuery(string.Format(File.ReadAllText(@"Queries\UpdateB2F_LOG.sql"), HanaDAO.Database, AppName, B2F_TipoDoc, B2F_IdDoc, B2F_DtInteg, B2F_Status, B2F_IdRet, B2F_MsgRet, B2F_JsonEnv, B2F_JsonRet, B2F_IdDocLeg));
                }
                else
                {
                    dao.ExecuteNonQuery(string.Format(File.ReadAllText(@"Queries\InsertB2F_LOG.sql"), HanaDAO.Database, AppName, B2F_TipoDoc, B2F_IdDoc, B2F_DtInteg, B2F_Status, B2F_IdRet, B2F_MsgRet, B2F_JsonEnv, B2F_JsonRet, B2F_IdDocLeg));
                }
            }
        }
    }
}
