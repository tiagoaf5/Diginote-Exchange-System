using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using Common;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread] 
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            System.Diagnostics.Debug.WriteLine("-----------------benfica----------------");
            Application.Run(new Form1());
            

        }
        

        // Creates an empty database file
    }

    public class Users : MarshalByRefObject, IUsers
    {
        private const string DatabaseName = "database.db";
        private SQLiteConnection _mDbConnection;

        public Users()
        {
            OpenDatabase();
        }
        
        void OpenDatabase()
        {
            if (!File.Exists(DatabaseName))
            {
                SQLiteConnection.CreateFile(DatabaseName);
                ConnectToDatabase();
                CreateTables();
                FillTable();
            }
            else
            {
                ConnectToDatabase();
            }
            
        }

        // Creates a connection with our database file.
        void ConnectToDatabase()
        {
            _mDbConnection = new SQLiteConnection("Data Source="+ DatabaseName +";Version=3;");
            _mDbConnection.Open();
        }

        // Creates a tables
        void CreateTables()
        {
            string sql = "CREATE TABLE USER (idUser INTEGER PRIMARY KEY, " +
                         "name TEXT NOT NULL," +
                         "nickname TEXT NOT NULL UNIQUE," +
                         "password TEXT NOT NULL);";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            command.ExecuteNonQuery();
        }

        // Testing purpose
        void FillTable()
        {
            string sql = "INSERT INTO USER (name, nickname, password) values ('jose','ze', 'nabo')";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            command.ExecuteNonQuery();
        }



        public IUser LogUser(string nickname, string password)
        {
            string sql = "SELECT * FROM USER WHERE nickname = '" + nickname + "' and password = '" + password + "'";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            
            if (reader.Read())
                System.Diagnostics.Debug.WriteLine("Name: " + reader["name"] + "    nickname: " + reader["nickname"] + "    password: " + reader["password"]);
            else
                return null;
            

            return new User(Convert.ToString(reader["name"]), Convert.ToString(reader["nickname"]));
        }

        public IUser RegisterUser(string nickname, string password, string name)
        {
            string sql = String.Format("INSERT INTO USER (name, nickname, password) values ('{0}','{1}', '{2}')", name, nickname, password);
            
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (SQLiteException exception)
            {
                System.Diagnostics.Debug.WriteLine("exception in " +exception.Source +": '" + exception.Message + "'");
                return null;
            }
            
            return new User(name, nickname);
            
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }

    public class User : MarshalByRefObject, IUser
    {


        public User(string name, string nickname)
        {
            Name = name;
            Nickname = nickname;
        }
        public string Name
        {
            get;
            private set;
        }

        public string Nickname { get; private set; }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
