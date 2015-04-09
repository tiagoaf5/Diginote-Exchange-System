using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Common;

namespace Server
{
    public class Market : MarshalByRefObject, IMarket
    {
        private const string DatabaseName = "database.db";
        private const int NumberOfDiginotes = 100;
        private const string datePatt = @"yyyy-MM-dd HH:mm:ss";
        private SQLiteConnection _mDbConnection;
        private MainWindowServer _myWindow;

        public event ChangeDelegate ChangeEvent;
        public float SharePrice { get; private set; }

        public Market()
        {
            OpenDatabase();
            SharePrice = (float)1.0;
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
                  "sharePrice REAL NOT NULL DEFAULT 1.0)";
            command = new SQLiteCommand(sql, _mDbConnection);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE SHAREHISTORY ( " +
                  "idShare INTEGER PRIMARY KEY, " +
                  "date TEXT NOT NULL," +
                  "newSharePrice REAL NOT NULL)";
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
            for (int i = 0; i < NumberOfDiginotes; i++)
                RegisterDiginotes(GetHashSha1(i + "diginote"));

            //Create System
            string sql = "INSERT INTO SYSTEM (sharePrice) values ('1.0')";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (SQLiteException exception)
            {
                Debug.WriteLine("exception in " + exception.Source + ": '" + exception.Message + "'");
            }
        }



        public IUser LogUser(string nickname, string password)
        {

            string sql = "SELECT * FROM USER WHERE nickname = '" + nickname + "' and password = '" + GetHashSha1(password) + "'";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            if (reader.Read())
                Debug.WriteLine("Name: " + reader["name"] + "    nickname: " + reader["nickname"] + "    password: " + reader["password"]);
            else
                return null;

            List<IDiginote> diginotes = GetUserDiginotes(Convert.ToInt16(reader["idUser"]));

            User u;
            
            if (diginotes != null)
                u = new User(Convert.ToString(reader["name"]), Convert.ToString(reader["nickname"]), diginotes);
            else
                u = new User(Convert.ToString(reader["name"]), Convert.ToString(reader["nickname"]));
            

            //add User to panel
            _myWindow.AddUser(u, true);

            return u;
        }

        private List<IDiginote> GetUserDiginotes(int idUser)
        {
            string sql = "SELECT * FROM DIGINOTE WHERE user = '" + idUser + "'";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<IDiginote> diginotes = new List<IDiginote>();

            while (reader.Read())
                diginotes.Add(new Diginote((string) reader["serialNumber"]));

            if (diginotes.Count == 0)
                return null;

            return diginotes;
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
                Debug.WriteLine("exception in " + exception.Source + ": '" + exception.Message + "'");
                return null;
            }

            //add User to panel

            User u = new User(name, nickname);

            if (_myWindow != null)
                _myWindow.AddUser(u, true);

            return u;

        }


        private bool RegisterDiginotes(string serialNumber)
        {
            string sql = String.Format("INSERT INTO DIGINOTE (serialNumber, user) values ('{0}','1')", serialNumber);

            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (SQLiteException exception)
            {
                Debug.WriteLine("exception in " + exception.Source + ": '" + exception.Message + "'");
                return false;
            }

            return true;
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
        
        //MARKET
        
       
        public List<IDiginote> BuyDiginotes(int quantity)
        {
            throw new NotImplementedException();
        }

        public int SellDiginotes(int quantity)
        {
            throw new NotImplementedException();
        }

        public void SuggestNewSharePrice(float newPrice)
        {
            SharePrice = newPrice;
            /*DateTime saveNow = DateTime.Now;
            saveNow.
            string sql = String.Format("INSERT INTO SHAREHISTORY (date, user) values ('{0}','1')", DateTime.Now.ToString(datePatt));

            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (SQLiteException exception)
            {
                Debug.WriteLine("exception in " + exception.Source + ": '" + exception.Message + "'");
                return false;
            }*/

            if (ChangeEvent != null)
            {
                Delegate[] invkList = ChangeEvent.GetInvocationList();

                foreach (ChangeDelegate handler in invkList)
                {
                    var handler1 = handler;
                    new Thread(() =>
                    {
                        try
                        {
                            handler1(newPrice, ChangeOperation.ShareUp);
                            Debug.WriteLine("Invoking event handler");
                        }
                        catch (Exception)
                        {
                            ChangeEvent -= handler1;
                            Debug.WriteLine("Exception: Removed an event handler");
                        }
                    }).Start();
                }
            }
        }
    }


    public class Diginote : MarshalByRefObject, IDiginote
    {
        public string SerialNumber { get; set; }
        public int Value { get; private set; }

        public IUser User { get; set; }

        public Diginote(string serialNumber)
        {
            SerialNumber = serialNumber;
            Value = 1;
        }

        public Diginote(string serialNumber, IUser u)
        {
            SerialNumber = serialNumber;
            User = u;
            Value = 1;
        }
    }
}