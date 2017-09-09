using SlimDX;
using SlimDX.D3DCompiler;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMF.MME
{
    public class EffectLoader : System.IDisposable
    {
        private static EffectLoader instance;

        private static readonly string cacheFileName = "effect.cache";

        private static readonly string connectionString = "Data Source=effect.cache";

        private static readonly string tableCreationSQL = "CREATE TABLE 'DBHeader' (\r\n             'Id' INTEGER PRIMARY KEY ON CONFLICT FAIL AUTOINCREMENT UNIQUE ON CONFLICT FAIL DEFAULT '',\r\n            'Property' CHAR NOT NULL ON CONFLICT FAIL,\r\n            'Value' CHAR);\r\n            INSERT INTO DBHeader VALUES(NULL,'DBType','EffectCacheDatabase');\r\n            INSERT INTO DBHeader VALUES(NULL,'FileVersion','1.0');\r\n            CREATE TABLE 'EffectCache'(\r\n            'Id' INTEGER PRIMARY KEY ON CONFLICT FAIL AUTOINCREMENT UNIQUE ON CONFLICT FAIL DEFAULT '',\r\n            'FileName' CHAR NOT NULL ON CONFLICT FAIL,\r\n            'HashCode' CHAR NOT NULL ON CONFLICT FAIL,\r\n            'ShaderByteCode' BLOB);";

        private static readonly string getBlobQuery = "SELECT ShaderByteCode FROM EffectCache WHERE FileName=='{0}' AND HashCode=='{1}';";

        private static readonly string getByFileNameQuery = "SELECT Id FROM EffectCache WHERE FileName=='{0}';";

        private static readonly string deleteByIdQuery = "DELETE FROM EffectCache WHERE Id=={0};";

        private static readonly string insertSQL = "INSERT INTO EffectCache VALUES(NULL,'{0}','{1}',@resource);";

        public event System.EventHandler<EffectLoaderCompilingEventArgs> OnCompiling = delegate (object param0, EffectLoaderCompilingEventArgs param1)
        {
        };

        public event System.EventHandler<EffectLoaderCompiledEventArgs> OnCompiled = delegate (object param0, EffectLoaderCompiledEventArgs param1)
        {
        };

        public static EffectLoader Instance
        {
            get
            {
                if (EffectLoader.instance == null)
                {
                    EffectLoader.instance = new EffectLoader();
                }
                return EffectLoader.instance;
            }
            private set
            {
                EffectLoader.instance = value;
            }
        }

        public SQLiteConnection Connection
        {
            get;
            set;
        }

        public EffectLoader()
        {
            bool flag = System.IO.File.Exists(EffectLoader.cacheFileName);
            Connection = new SQLiteConnection(EffectLoader.connectionString);
            Connection.Open();
            if (!flag)
            {
                using (SQLiteCommand sQLiteCommand = new SQLiteCommand(EffectLoader.tableCreationSQL, Connection))
                {
                    sQLiteCommand.ExecuteNonQuery();
                }
            }
        }

        public void CleanCache()
        {
            bool flag = System.IO.File.Exists(EffectLoader.cacheFileName);
            if (flag)
            {
                System.IO.File.Delete(EffectLoader.cacheFileName);
            }
        }

        public ShaderBytecode GetShaderBytecode(string fileName, System.IO.Stream fileStream)
        {
            string hashStr = getFileHash(fileStream);
            fileStream.Seek(0L, System.IO.SeekOrigin.Begin);
            ShaderBytecode result;
            using (SQLiteCommand sQLiteCommand = new SQLiteCommand(string.Format(EffectLoader.getBlobQuery, fileName, hashStr), Connection))
            {
                using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
                {
                    if (sQLiteDataReader.Read())
                    {
                        System.IO.MemoryStream bytesToMemoryStream = getBytesToMemoryStream(sQLiteDataReader, 0);
                        DataStream dataStream = new DataStream(bytesToMemoryStream.Length, true, true);
                        bytesToMemoryStream.CopyTo(dataStream);
                        result = new ShaderBytecode(dataStream);
                    }
                    else
                    {
                        System.IO.StreamReader streamReader = new System.IO.StreamReader(fileStream);
                        string shaderCode = streamReader.ReadToEnd();
                        ShaderFlags shaderFlags = ShaderFlags.None;
                        shaderFlags |= ShaderFlags.Debug;
                        Debug.WriteLine("Start compiling shader:{0},please wait...", fileName);
                        OnCompiling(this, new EffectLoaderCompilingEventArgs(fileName));
                        ShaderBytecode sbc = null;
                        Task task = new Task(delegate
                        {
                            sbc = ShaderBytecode.Compile(shaderCode, "fx_5_0", ShaderFlags.Debug, EffectFlags.None, MMEEffectManager.EffectMacros.ToArray(), MMEEffectManager.EffectInclude);
                            using (SQLiteCommand sQLiteCommand2 = new SQLiteCommand(string.Format(EffectLoader.getByFileNameQuery, fileStream), Connection))
                            {
                                using (SQLiteDataReader sQLiteDataReader2 = sQLiteCommand2.ExecuteReader())
                                {
                                    while (sQLiteDataReader2.Read())
                                    {
                                        using (SQLiteCommand sQLiteCommand3 = new SQLiteCommand(string.Format(EffectLoader.deleteByIdQuery, sQLiteDataReader2.GetInt32(0)), Connection))
                                        {
                                            sQLiteCommand3.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                            sbc.Data.CopyTo(memoryStream);
                            byte[] array = memoryStream.ToArray();
                            using (SQLiteCommand sQLiteCommand4 = new SQLiteCommand(string.Format(EffectLoader.insertSQL, fileName, hashStr), Connection))
                            {
                                sQLiteCommand4.Parameters.Add("@resource", DbType.Binary, array.Length).Value = array;
                                sQLiteCommand4.ExecuteNonQuery();
                            }
                        });
                        task.Start();
                        while (!task.IsCompleted)
                        {
                            Application.DoEvents();
                        }
                        OnCompiled(this, new EffectLoaderCompiledEventArgs());
                        result = sbc;
                    }
                }
            }
            return result;
        }

        protected System.IO.MemoryStream getBytesToMemoryStream(SQLiteDataReader reader, int index)
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            byte[] array = new byte[2048];
            long num = 0L;
            long bytes;
            while ((bytes = reader.GetBytes(index, num, array, 0, array.Length)) > 0L)
            {
                memoryStream.Write(array, 0, (int)bytes);
                num += bytes;
            }
            memoryStream.Seek(0L, System.IO.SeekOrigin.Begin);
            return memoryStream;
        }

        private string getFileHash(string filePath)
        {
            string fileHash;
            using (System.IO.FileStream fileStream = System.IO.File.OpenRead(filePath))
            {
                fileHash = getFileHash(fileStream);
            }
            return fileHash;
        }

        private string getFileHash(System.IO.Stream stream)
        {
            string result;
            using (System.Security.Cryptography.MD5 mD = System.Security.Cryptography.MD5.Create())
            {
                byte[] array = mD.ComputeHash(stream);
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                byte[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    byte b = array2[i];
                    stringBuilder.Append(b.ToString("x2"));
                }
                result = stringBuilder.ToString();
            }
            return result;
        }

        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.Close();
            }
        }
    }
}
