using B2F.Addon.EnvioEmail.Model;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Newtonsoft.Json;
using SAPbobsCOM;
using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Xml.Linq;

namespace B2F.Addon.EnvioEmail
{

    [FormAttribute("142", "Pedido de compra - dividido.b1f")]
    class Pedido_de_compra___dividido : SystemFormBase
    {
        #region CRIAÇÃO DOS COMPONENTES
        private EditText eDocNum;
        private EditText eCodFornecedor;
        private EditText eNomeFornecedor;
        public Pedido_de_compra___dividido()
        {
        }
        #endregion

        #region INICIALIZAÇÃO DO FORMULÁRIO
        public override void OnInitializeComponent()
        {
            this.OnCustomInitialize();
        }

        private void OnCustomInitialize()
        {
            eDocNum = (EditText)GetItem("8").Specific;
            eCodFornecedor = (EditText)GetItem("4").Specific;
            eNomeFornecedor = (EditText)GetItem("54").Specific;
        }

        public override void OnInitializeFormEvents()
        {
            this.DataAddAfter += new SAPbouiCOM.Framework.FormBase.DataAddAfterHandler(this.Form_DataAddAfter);
        }
        #endregion

        #region EVENTOS DO FORMULÁRIO
        private void Form_DataAddAfter(ref BusinessObjectInfo pVal)
        {
            if (pVal.ActionSuccess)
            {
                EnviarEmail(XElement.Parse(pVal.ObjectKey).Value.ToString());
            }
        }
        #endregion

        #region MÉTODOS DO USUÁRIO
        private void EnviarEmail(string docEntry)
        {
            try
            {
                string email = "";
                HanaDAO dao = new HanaDAO();
                var sql = string.Format(File.ReadAllText(@"Queries\ConsultarCaminhoAnexos.sql"), HanaDAO.Database, docEntry);
                List<Anexo> listaAnexos = dao.FillListFromCommand<Anexo>(sql);

                if (listaAnexos.Count > 0)
                {
                    #region RECUPERA OS DADOS SMTP
                    SmtpClient client = new SmtpClient
                    {
                        Host = ConfigurationManager.AppSettings["HostEmail"],
                        Port = int.Parse(ConfigurationManager.AppSettings["PortEmail"]),
                        EnableSsl = true,
                        UseDefaultCredentials = false,
                        Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Email"], ConfigurationManager.AppSettings["SenhaEmail"])
                    };
                    #endregion

                    MailMessage mail = new MailMessage
                    {
                        Sender = new MailAddress(ConfigurationManager.AppSettings["Email"]),
                        From = new MailAddress(ConfigurationManager.AppSettings["Email"]),
                        Subject = $"Pedido de Compra: {eDocNum.Value} - Fornecedor: {eCodFornecedor.Value} - {eNomeFornecedor.Value}",
                        Body = $"Olá! <br/> Segue PEDIDO DE COMPRA NÚMERO {eDocNum.Value}, com base na proposta aprovada anexa, bem como demais documentos que subsidiam a presente contratação. <br/> Grato(a), <br/> Time de Suprimentos!",
                        IsBodyHtml = true
                    };

                    var emailFornecedor = dao.ExecuteScalar(string.Format(File.ReadAllText(@"Queries\ConsultarEmailFornecedor.sql"), HanaDAO.Database, eCodFornecedor.Value));
                    mail.To.Add(new MailAddress(emailFornecedor.ToString()));

                    //mail.Attachments.Add(new System.Net.Mail.Attachment(GerarPDF()));
                    mail.Attachments.Add(new System.Net.Mail.Attachment(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Report", "Condições Gerais de Contratação.pdf")));

                    foreach (var anexo in listaAnexos)
                    {
                        mail.Attachments.Add(new System.Net.Mail.Attachment(anexo.Caminho));
                    }

                    mail.Priority = MailPriority.High;
                    client.Send(mail);

                    SalvarLogEnvioDeEmail(docEntry, $"Pedido de compra enviado com sucesso para o E-mail: {email}", "2");
                }
            }
            catch (Exception err)
            {
                SalvarLogEnvioDeEmail(docEntry, $"Não foi possível enviar E-mail : Erro = {err.Message}", "3");
            }
        }

        private void SalvarLogEnvioDeEmail(string docEntry, string mensagem, string status)
        {
            B2F_LOG b2F_LOG = new B2F_LOG
            {
                ObjectType = "22",
                B2F_IdDoc = docEntry,
                B2F_Status = status,
                B2F_MsgRet = mensagem,
                B2F_DtInteg = DateTime.Now.ToString()
            };

            b2F_LOG.InsertOrUpdateLog();
        }

        private string GerarPDF()
        {
            #region CONFIGURAÇÕES JSON
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            #endregion

            string folderComprov = ConfigurationManager.AppSettings["Anexo"];
            if (!Directory.Exists(folderComprov))
            {
                Directory.CreateDirectory(folderComprov);
            }

            #region CONEXÃO CRYSTAL
            string caminho = $@"Report\rptPedidoCompra.rpt";
            ReportDocument rd = new ReportDocument();
            rd.Load(caminho);

            ConnectionInfo oConnectInfo = new ConnectionInfo
            {
                ServerName = ConfigurationManager.AppSettings["Server"],
                DatabaseName = ConfigurationManager.AppSettings["Database"],
                UserID = ConfigurationManager.AppSettings["DBUser"],
                Password = ConfigurationManager.AppSettings["DBPassword"]
            };

            rd.DataSourceConnections.Clear();
            SetCredentials(rd, oConnectInfo);
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, folderComprov + @"\" + "Pedido.pdf");
            caminho = folderComprov + @"\" + "Pedido.pdf";

            rd.SetDatabaseLogon(oConnectInfo.UserID, oConnectInfo.Password, oConnectInfo.ServerName, oConnectInfo.DatabaseName);
            rd.Dispose();
            #endregion

            return caminho;
        }

        public void SetCredentials(ReportDocument rd, ConnectionInfo oConnectInfo)
        {
            string strConnection = $@"DRIVER={{HDBODBC32}};UID={oConnectInfo.UserID};PWD={oConnectInfo.Password};SERVERNODE={oConnectInfo.ServerName};CS={oConnectInfo.DatabaseName}";
            for (int ii = 0; ii < rd.DataSourceConnections.Count; ii++)
            {
                NameValuePairs2 logonProps2 = rd.DataSourceConnections[ii].LogonProperties;
                logonProps2.Set("Connection String", strConnection);
                logonProps2.Set("SERVERNODE", oConnectInfo.ServerName);
                rd.DataSourceConnections[ii].SetLogonProperties(logonProps2);

                rd.DataSourceConnections[ii].SetConnection(oConnectInfo.ServerName, oConnectInfo.DatabaseName, oConnectInfo.UserID, oConnectInfo.Password);
                rd.DataSourceConnections[ii].SetLogon(oConnectInfo.UserID, oConnectInfo.Password);
            }
        }

        private Recordset MontarRecordSet()
        {
            return (Recordset)CommonController.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
        }
        #endregion
    }
}
