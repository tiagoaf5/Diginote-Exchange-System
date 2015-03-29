using System;
using System.Data.SQLite;
using System.IO;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Common;

namespace Server
{
    static class ProgramServer
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            RemotingConfiguration.Configure("Server.exe.config", false);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //creating users object to give a reference to MainWindow
            Users users = (Users)RemotingServices.Connect(typeof(Users), "tcp://localhost:9000/Server/UsersManager");
            MainWindowServer myWindow = new MainWindowServer(users);
            users.AddWindow(myWindow);

            System.Diagnostics.Debug.WriteLine("---------------------------------");

            Application.Run(myWindow);

            
        }


        // Creates an empty database file
    }

    public class Users : MarshalByRefObject, IUsers
    {
        private const string DatabaseName = "database.db";
        private SQLiteConnection _mDbConnection;
        private MainWindowServer _myWindow;

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
            _mDbConnection = new SQLiteConnection("Data Source=" + DatabaseName + ";Version=3;");
            _mDbConnection.Open();
        }

        // Creates a tables
        void CreateTables()
        {
            string sql = "CREATE TABLE USER (" +
                         "idUser INTEGER PRIMARY KEY, " +
                         "name TEXT NOT NULL," +
                         "nickname TEXT NOT NULL UNIQUE," +
                         "password TEXT NOT NULL);";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE SYSTEM ( " +
                  "idSystem INTEGER PRIMARY KEY, " +
                  "cotation REAL NOT NULL DEFAULT 1.0)";
            command = new SQLiteCommand(sql, _mDbConnection);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE DIGINOTE (" +
                  "idDiginote INTEGER PRIMARY KEY," +
                  "serialNumber TEXT NOT NULL," +
                  "value INTEGER DEFAULT 1," +
                  "user INTEGER," +
                  "system INTEGER," +
                  "FOREIGN KEY(user) REFERENCES 'USER'(idUser))";
            command = new SQLiteCommand(sql, _mDbConnection);
            command.ExecuteNonQuery();

        }

        // Testing purpose
        void FillTable()
        {
            RegisterUser("ze", "nabo", "jose");
        }



        public IUser LogUser(string nickname, string password)
        {
            
            string sql = "SELECT * FROM USER WHERE nickname = '" + nickname + "' and password = '" + GetHashSha1(password) + "'";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            if (reader.Read())
                System.Diagnostics.Debug.WriteLine("Name: " + reader["name"] + "    nickname: " + reader["nickname"] + "    password: " + reader["password"]);
            else
                return null;

            User u = new User(Convert.ToString(reader["name"]), Convert.ToString(reader["nickname"]));

            //add User to panel
            _myWindow.AddUser(u, true);

            return u;
        }

        public IUser RegisterUser(string nickname, string password, string name)
        {
            string sql = String.Format("INSERT INTO USER (name, nickname, password) values ('{0}','{1}', '{2}')", name, nickname, GetHashSha1(password));

            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (SQLiteException exception)
            {
                System.Diagnostics.Debug.WriteLine("exception in " + exception.Source + ": '" + exception.Message + "'");
                return null;
            }

            //add User to panel

            User u = new User(name, nickname);
            
            if (_myWindow != null)
                _myWindow.AddUser(u, true);

            return u;

        }

        public void AddWindow(MainWindowServer x)
        {
            _myWindow = x;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void LoadUsers()
        {
            string sql = "SELECT * FROM USER";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                User u = new User(Convert.ToString(reader["name"]), Convert.ToString(reader["nickname"]));
                //add User to panel
                _myWindow.AddUser(u, false);
            }
        }

        private static string GetHashSha1(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA1Managed hashstring = new SHA1Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:X2}", x);
            }
            return hashString;
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
