using FluentMigrator;
using Migrations.Tables;

namespace Migrations;

// TABLES

[Migration(100)]
public class M100 : Users { }

[Migration(200)]
public class M200 : UserRoles { }

[Migration(300)]
public class M300 : Avatars { }

[Migration(400)]
public class M400 : Applications { }

[Migration(1000)]
public class M1000 : ApplicationIcons { }

[Migration(500)]
public class M500 : Redirects { }

[Migration(600)]
public class M600 : Scopes { }

[Migration(700)]
public class M700 : Authorizations { }

[Migration(800)]
public class M800 : Tokens { }

[Migration(900)]
public class M900 : Transactions { }

// ENTITIES

// USERS

[Migration(1001001)]
public class M100100 : Entities.Users.System { }

// APPLICATIONS

[Migration(4001001)]
public class M4001001 : Entities.Applications.System { }

// REDIRECTS

[Migration(5001010)]
public class M5001010 : Entities.Redirects.Localhost { }

[Migration(5001011)]
public class M5001011 : Entities.Redirects.TobyMeehan { }

// SCOPES

[Migration(6001001)]
public class M6001001 : Entities.Scopes.Account { }

[Migration(6001002)]
public class M6001002 : Entities.Scopes.Account_Delete { }

[Migration(6001003)]
public class M6001003 : Entities.Scopes.Account_Identify { }

[Migration(6001004)]
public class M6001004 : Entities.Scopes.Account_Update { }

[Migration(6001011)]
public class M6001011 : Entities.Scopes.Applications { }

[Migration(6001012)]
public class M6001012 : Entities.Scopes.Applications_Create { }

[Migration(6001013)]
public class M6001013 : Entities.Scopes.Applications_Delete { }

[Migration(6001014)]
public class M6001014 : Entities.Scopes.Applications_Read { }

[Migration(6001015)]
public class M6001015 : Entities.Scopes.Applications_Update { }

[Migration(6001021)]
public class M6001021 : Entities.Scopes.Transactions { }

[Migration(6001022)]
public class M6001022 : Entities.Scopes.Transactions_Read { }

[Migration(6001023)]
public class M6001023 : Entities.Scopes.Transactions_Send { }

[Migration(6001024)]
public class M6001024 : Entities.Scopes.Transactions_Transfer { }

[Migration(202311261736)]
public class M1100 : Downloads { }

[Migration(202311261737)]
public class M1200 : DownloadAuthors { }

[Migration(202311261738)]
public class M6001031 : Entities.Scopes.Downloads { }

[Migration(202311261739)]
public class M6001032 : Entities.Scopes.Downloads_Authors { }
[Migration(202311261740)]
public class M6001033 : Entities.Scopes.Downloads_Create { }
[Migration(202311261741)]
public class M6001034 : Entities.Scopes.Downloads_Delete { }
[Migration(202311261742)]
public class M6001035 : Entities.Scopes.Downloads_Files { }
[Migration(202311261743)]
public class M6001036 : Entities.Scopes.Downloads_Read { }
[Migration(202311261744)]
public class M6001037 : Entities.Scopes.Downloads_Update { }