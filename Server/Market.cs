using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
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
        private const string DatePatt = @"yyyy-MM-dd HH:mm:ss";
        private const int TimerSeconds = 10;

        private SQLiteConnection _mDbConnection;
        private MainWindowServer _myWindow;

        public event ChangeDelegate ChangeEvent;
        public event UpdateLockingTimeDelegate UpdateLockingEvent;

        public float SharePrice { get; private set; }

        private int _countDown; // Seconds
        private System.Threading.Timer _timer;


        public Market()
        {
            OpenDatabase();
            LoadSharePrice();
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

        private void LoadSharePrice()
        {
            string sql = "SELECT * FROM SHAREHISTORY ORDER BY idShare DESC LIMIT 1";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                SharePrice = (float)Convert.ToDouble(reader["newSharePrice"]);
                Debug.WriteLine("-----------> " + Convert.ToDouble(reader["newSharePrice"].ToString()));
            }
            else
            {
                Debug.WriteLine("-----------> No records");
                SharePrice = (float)1.0;
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
                  "sharePrice DOUBLE NOT NULL DEFAULT 1.0)";
            command = new SQLiteCommand(sql, _mDbConnection);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE SHAREHISTORY ( " +
                  "idShare INTEGER PRIMARY KEY, " +
                  "date TEXT NOT NULL," +
                  "newSharePrice DOUBLE NOT NULL," +
                  "user INTEGER NOT NULL," +
                  "FOREIGN KEY(user) REFERENCES 'USER'(idUser))";
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

            sql = "CREATE TABLE BUYORDER (" +
                  "idBuyOrder INTEGER PRIMARY KEY," +
                  "date TEXT NOT NULL," +
                  "wanted INTEGER NOT NULL," +
                  "satisfied INTEGER DEFAULT 0," +
                  "user INTEGER," +
                  "sharePrice DOUBLE," +
                  "closed Boolean DEFAULT FALSE," +
                  "FOREIGN KEY(user) REFERENCES 'USER'(idUser))";
            command = new SQLiteCommand(sql, _mDbConnection);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE SELLORDER (" +
                  "idSellOrder INTEGER PRIMARY KEY," +
                  "date TEXT NOT NULL," +
                  "wanted INTEGER NOT NULL," +
                  "satisfied INTEGER DEFAULT 0," +
                  "user INTEGER," +
                  "sharePrice DOUBLE," +
                  "closed Boolean DEFAULT FALSE," +
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
            
            u = new User(Convert.ToString(reader["name"]), Convert.ToString(reader["nickname"]), diginotes);

            u.IdUser = Convert.ToInt32(reader["idUser"]);


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
                diginotes.Add(new Diginote((string)reader["serialNumber"]));
            /*
            if (diginotes.Count == 0)
                return null;
            */
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

        public ArrayList GetSharePricesList()
        {
            string sql = "SELECT * FROM SHAREHISTORY";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            ArrayList history = new ArrayList();

            while (reader.Read())
                history.Add((float)Convert.ToDouble(reader["newSharePrice"]));
            /*
            if (diginotes.Count == 0)
                return null;
            */
            return history;
        }


        private void RegisterDiginotes(string serialNumber)
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
            }

        }

        private string RealToString(float num)
        {
            return Convert.ToString(num, CultureInfo.InvariantCulture).Replace(",", ".");
        }

        public void InsertBuyOrder(int quantity, IUser user)
        {
            string sql = String.Format("INSERT INTO BUYORDER (date, user, sharePrice, wanted) SELECT '{0}', idUser, '{1}','{3}' FROM USER WHERE nickname = '{2}'",
                DateTime.Now.ToString(DatePatt), RealToString(SharePrice), user.Nickname, quantity);

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

        private void InsertSellOrder(int quantity, IUser user, int satisfied = 0, bool closed = false)
        {
            string sql = String.Format("INSERT INTO SELLORDER (date, user, sharePrice, wanted, closed, satisfied) SELECT '{0}', idUser, '{1}','{3}',{4}, {5} FROM USER WHERE nickname = '{2}'",
                DateTime.Now.ToString(DatePatt), RealToString(SharePrice), user.Nickname, quantity, Convert.ToInt32(closed), satisfied);

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

        //            List<IDiginote> diginotes_to_transfer = new List<IDiginote>();

        // List<IDiginote> digis = GetUserDiginotes(o1.IdUser).GetRange(0, how_many_left);
        // diginotes_to_transfer.AddRange(digis);
       
        public List<IDiginote> BuyDiginotes(int quantity)
        {
            throw new NotImplementedException();
        }

        public int SellDiginotes(int quantity, IUser user) // apenas para as que estao disponiveis
        {

            SQLiteTransaction t = _mDbConnection.BeginTransaction();
           string sql = "SELECT * FROM BUYORDER WHERE not closed ORDER BY date asc";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<IOrder> orders = new List<IOrder>();

            int how_many_left = quantity;

            while (reader.Read())
            {
                Order o1 = new Order(Convert.ToInt32(reader["idBuyOrder"]), Convert.ToInt32(reader["user"]), Convert.ToInt32(reader["wanted"]), Convert.ToInt32(reader["satisfied"]));
                
                int how_many_to_offer = o1.Wanted - o1.Satisfied;
                    if(how_many_left > how_many_to_offer)
                    {
                        o1.Satisfied = o1.Wanted;
                        orders.Add(o1);
                        how_many_left -= how_many_to_offer;
                        for (int i = 0; i < how_many_to_offer; i++)
                        {
                            IDiginote d = user.Diginotes[0];
                            user.Diginotes.RemoveAt(0);
                            UpdateDiginoteOwner(o1.IdUser, d);
                        }
                        InsertSellOrder(quantity, user, how_many_to_offer, true);
                    } else {
                        o1.Satisfied += how_many_left;
                        orders.Add(o1);
                        for (int i = 0; i < how_many_left; i++)
                        {
                            IDiginote d = user.Diginotes[0];
                            user.Diginotes.RemoveAt(0);
                            UpdateDiginoteOwner(o1.IdUser, d);
                        }
                        InsertSellOrder(quantity, user,how_many_left, true);
                        how_many_left = 0;
                        break;
                    }
            }

            //TODO transacoes

            foreach(IOrder o in orders)
            {
                if (o.Satisfied < o.Wanted)
                 sql = String.Format("UPDATE BUYORDER SET satisfied = {0} WHERE idBuyOrder = {1}",o.Satisfied, o.IdOrder);
                else sql =  String.Format("UPDATE BUYORDER SET satisfied = {0}, closed = 1 WHERE idBuyOrder = {1}",o.Satisfied,o.IdOrder);

                try
                {
                    command = new SQLiteCommand(sql, _mDbConnection);
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException exception)
                {
                    Debug.WriteLine("exception in " + exception.Source + ": '" + exception.Message + "'");
                }

            }

            t.Commit();

            return quantity - how_many_left;
        }

        private string UpdateDiginoteOwner(int destination, IDiginote d)
        {
            String sql = String.Format("UPDATE DIGINOTE SET user = {0} WHERE serialNumber = '{1}'", destination, d.SerialNumber);
            try
            {
                SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
                command.ExecuteNonQuery();
            }
            catch (SQLiteException exception)
            {
                Debug.WriteLine("exception in " + exception.Source + ": '" + exception.Message + "'");
            }
            return sql;
        }

        public void SuggestNewSharePrice(float newPrice, IUser user) // aqui acrescentar se é por venda/compra e quantas quer vender/comprar
        {
            SharePrice = newPrice;

            string sql = String.Format("INSERT INTO SHAREHISTORY (date, user, newSharePrice) SELECT '{0}', idUser, '{1}' FROM USER WHERE nickname = '{2}'",
                DateTime.Now.ToString(DatePatt), RealToString(SharePrice), user.Nickname);

            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (SQLiteException exception)
            {
                Debug.WriteLine("exception in " + exception.Source + ": '" + exception.Message + "'");

            }

            if (ChangeEvent != null)
            {
                Delegate[] invkList = ChangeEvent.GetInvocationList();

                //foreach (ChangeDelegate handler in invkList)
                foreach (var @delegate in invkList)
                {
                    var handler = (ChangeDelegate)@delegate;
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

            SetTimer();

        }

        private void SetTimer()
        {
            Debug.WriteLine("setting Timer");
            _countDown = TimerSeconds;
            _timer = new System.Threading.Timer(new TimerCallback(timer1_Tick), null, 0, 1000);
        }

        private void timer1_Tick(object sender)
        {
            if (UpdateLockingEvent != null)
            {
                Delegate[] invkList = UpdateLockingEvent.GetInvocationList();

                //foreach (ChangeDelegate handler in invkList)
                foreach (var @delegate in invkList)
                {
                    var handler = (UpdateLockingTimeDelegate)@delegate;
                    var handler1 = handler;
                    new Thread(() =>
                    {
                        try
                        {
                            handler1(_countDown);
                            Debug.WriteLine("Invoking event handler");
                        }
                        catch (Exception)
                        {
                            UpdateLockingEvent -= handler1;
                            Debug.WriteLine("Exception: Removed an event handler");
                        }
                    }).Start();
                }
            }

            _countDown--;

            if (_countDown == 0)
                _timer.Dispose();

        }
    }
}