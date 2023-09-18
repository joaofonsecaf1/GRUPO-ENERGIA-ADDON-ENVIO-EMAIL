UPDATE "{0}"."@B2F_LOG" 
SET "U_B2F_TipoDoc" = '{2}',
"U_B2F_IdDoc"  = '{3}',
"U_B2F_DtInteg" = '{4}',
"U_B2F_Status" = '{5}',
"U_B2F_IdRet" = '{6}',
"U_B2F_MsgRet" = '{7}',
"U_B2F_JsonEnv" = '{8}',
"U_B2F_JsonRet" = '{9}',
"U_B2F_IdDocLeg" = '{10}',
"UpdateDate" = current_date,
"UpdateTime" = TO_INT(LPAD(REPLACE(TO_VARCHAR(current_Time),':',''),4))
WHERE  "Name" = '{1}' AND "U_B2F_TipoDoc" = '{2}' AND "U_B2F_IdDoc" = '{3}'