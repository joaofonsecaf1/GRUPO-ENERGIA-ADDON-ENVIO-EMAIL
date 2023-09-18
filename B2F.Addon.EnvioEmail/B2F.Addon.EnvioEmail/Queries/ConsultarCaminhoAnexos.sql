SELECT T0."DocEntry"
     , T0."DocNum"
	 , TO_VARCHAR(T2."trgtPath") || '\' || T2."FileName" || '.' || T2."FileExt" AS "Caminho"
FROM "{0}".OPOR AS T0
INNER JOIN "{0}".OATC AS T1 ON T1."AbsEntry" = T0."AtcEntry"
INNER JOIN "{0}".ATC1 AS T2 ON T2."AbsEntry" = T1."AbsEntry"
WHERE T0."DocEntry" = '{1}'