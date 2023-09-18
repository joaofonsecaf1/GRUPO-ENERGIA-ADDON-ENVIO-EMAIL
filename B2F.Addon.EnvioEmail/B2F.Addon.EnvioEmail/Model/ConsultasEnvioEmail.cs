namespace B2F.Addon.EnvioEmail.Model
{
    public class ConsultasEnvioEmail
    {
        public static string ConsultaFormatada
        {
            get
            {
                return @"SELECT T0.""U_B2F_DtInteg""
	                          , T1.""DocEntry""
	                          , T1.""DocNum""
	                          , T1.""CardCode""
	                          , T2.""CardName""
	                          , SUM(T1.""DocTotal"") - IFNULL(SUM(T4.""LineTotal""), 0) AS ""Valor Pedido""
                         FROM ""@B2F_LOG"" AS T0
                         INNER JOIN OPOR AS T1 ON TO_VARCHAR(T1.""DocEntry"") = T0.""U_B2F_IdDoc""
                         INNER JOIN OCRD AS T2 ON T2.""CardCode"" = T1.""CardCode""
                         INNER JOIN POR1 AS T3 ON T3.""DocEntry"" = T1.""DocEntry""
                         LEFT JOIN POR2 AS T4 ON T4.""DocEntry"" = T3.""DocEntry""
                                             AND T4.""LineNum"" = T3.""LineNum""
                         WHERE ""Name"" = 'ADDON_ENVIO_EMAIL'
                         GROUP BY T0.""U_B2F_DtInteg""
	                            , T1.""DocEntry""
	                            , T1.""DocNum""
	                            , T1.""CardCode""
	                            , T2.""CardName""";
            }
        }
    }
}
