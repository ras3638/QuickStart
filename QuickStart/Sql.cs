using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickStart
{
	public static class Sql
	{
//        public static string sLoadSchema =
//        @"SELECT  obj.name AS fk_name, 
//		sch.name AS [schema_name], 
//		tab1.name AS [table],
//		col1.name AS [column],
//		tab2.name AS [referenced_table],
//		col2.name AS [referenced_column]
//		FROM sys.foreign_key_columns fkc
//		INNER JOIN sys.objects obj
//			ON obj.object_id = fkc.constraint_object_id
//		INNER JOIN sys.tables tab1
//			ON tab1.object_id = fkc.parent_object_id
//		INNER JOIN sys.schemas sch
//			ON tab1.schema_id = sch.schema_id
//		INNER JOIN sys.columns col1
//			ON col1.column_id = parent_column_id AND col1.object_id = tab1.object_id
//		INNER JOIN sys.tables tab2
//			ON tab2.object_id = fkc.referenced_object_id
//		INNER JOIN sys.columns col2
//			ON col2.column_id = referenced_column_id AND col2.object_id = tab2.object_id
//		order by tab1.name ";

        public static string sLoadSchema =
            @"SELECT [fk_name],[schema_name],B.[table],[column],B.[referenced_table],B.[referenced_column] from (
SELECT  obj.name AS [fk_name], 
		sch.name AS [schema_name], 
		tab1.name AS [table],
		col1.name AS [column],
		tab2.name AS [referenced_table],
		col2.name AS [referenced_column]
		FROM sys.foreign_key_columns fkc
		INNER JOIN sys.objects obj
			ON obj.object_id = fkc.constraint_object_id
		INNER JOIN sys.tables tab1
			ON tab1.object_id = fkc.parent_object_id
		INNER JOIN sys.schemas sch
			ON tab1.schema_id = sch.schema_id
		INNER JOIN sys.columns col1
			ON col1.column_id = parent_column_id AND col1.object_id = tab1.object_id
		INNER JOIN sys.tables tab2
			ON tab2.object_id = fkc.referenced_object_id
		INNER JOIN sys.columns col2
			ON col2.column_id = referenced_column_id AND col2.object_id = tab2.object_id
)  B

LEFT JOIN (
				select [referenced_table],count(*) [referenenced_table_count] from 
				(
				SELECT  obj.name AS [fk_name], 
		sch.name AS [schema_name], 
		tab1.name AS [table],
		col1.name AS [column],
		tab2.name AS [referenced_table],
		col2.name AS [referenced_column]
		FROM sys.foreign_key_columns fkc
		INNER JOIN sys.objects obj
			ON obj.object_id = fkc.constraint_object_id
		INNER JOIN sys.tables tab1
			ON tab1.object_id = fkc.parent_object_id
		INNER JOIN sys.schemas sch
			ON tab1.schema_id = sch.schema_id
		INNER JOIN sys.columns col1
			ON col1.column_id = parent_column_id AND col1.object_id = tab1.object_id
		INNER JOIN sys.tables tab2
			ON tab2.object_id = fkc.referenced_object_id
		INNER JOIN sys.columns col2
			ON col2.column_id = referenced_column_id AND col2.object_id = tab2.object_id
				) A group by [referenced_table] 
)
C on B.[referenced_table] = C.[referenced_table]

LEFT JOIN  (
				select [table],count(*) [table_count] from 
				(
				SELECT  obj.name AS [fk_name], 
		sch.name AS [schema_name], 
		tab1.name AS [table],
		col1.name AS [column],
		tab2.name AS [referenced_table],
		col2.name AS [referenced_column]
		FROM sys.foreign_key_columns fkc
		INNER JOIN sys.objects obj
			ON obj.object_id = fkc.constraint_object_id
		INNER JOIN sys.tables tab1
			ON tab1.object_id = fkc.parent_object_id
		INNER JOIN sys.schemas sch
			ON tab1.schema_id = sch.schema_id
		INNER JOIN sys.columns col1
			ON col1.column_id = parent_column_id AND col1.object_id = tab1.object_id
		INNER JOIN sys.tables tab2
			ON tab2.object_id = fkc.referenced_object_id
		INNER JOIN sys.columns col2
			ON col2.column_id = referenced_column_id AND col2.object_id = tab2.object_id
				) A group by [table] 
)
D on B.[table] = D.[table]

order by [referenenced_table_count],[table_count]";
	}
}
