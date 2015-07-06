using System;
using FluentMigrator;
namespace AppFrame.ServiceInterface
{
	// 参考
	// https://github.com/schambers/fluentmigrator/wiki
	// https://github.com/schambers/fluentmigrator/wiki/Fluent-Interface
	[Migration(1)]
	public class CreateUser : Migration
	{

		public override void Up()
		{
			// 删除之前用 ormlite 创建的 表
			if(Schema.Table("User").Exists())
				Delete.Table("User");
				
			Create.Table("User")
				.WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
				.WithColumn("Name").AsString(255).NotNullable().WithDefaultValue("")
				.WithColumn("Password").AsString(255).NotNullable().WithDefaultValue("")
				.WithColumn("Email").AsString(50).Nullable().Unique()
				.WithColumn("Mobile").AsString(20).Nullable().Unique()
				.WithColumn("Avatar").AsString(255).Nullable()
				.WithColumn("AuthToken").AsString(255).NotNullable()
				.WithColumn("CreatedAt").AsDateTime()
			;
		}

		public override void Down()
		{
			Delete.Table("User");
		}
	}
}

