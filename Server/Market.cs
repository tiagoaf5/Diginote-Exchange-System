using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Remoting;
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

        private SQLiteConnection _mDbConnection;
        private MainWindowServer _myWindow;

        public event ChangeDelegate ChangeEvent;
        public event UpdateTimerLockingDelegate UpdateLockingEvent;

        public float SharePrice { get; private set; }

        public int CountDown { get; private set; } // Seconds
        private Timer _timer;

        private readonly Hashtable _table = new Hashtable();

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
            RegisterUser("ze", "nabo", "jose", null);
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




        public IUser LogUser(string nickname, string password, string address)
        {

            string sql = "SELECT * FROM USER WHERE nickname = '" + nickname + "' and password = '" + GetHashSha1(password) + "'";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            if (reader.Read())
                Debug.WriteLine("Name: " + reader["name"] + "    nickname: " + reader["nickname"] + "    password: " + reader["password"]);
            else
                return null;

            var u = new User(Convert.ToInt32(reader["idUser"]), Convert.ToString(reader["name"]), Convert.ToString(reader["nickname"]));

            //add User to panel
            _myWindow.AddUser(u, true);

            if (address != null)
                _table.Add(u.IdUser, address);



            return u;
        }

        public List<IDiginote> GetUserDiginotes(IUser user)
        {
            string sql = "SELECT * FROM DIGINOTE WHERE user = '" + user.IdUser + "'";
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

        public List<IDiginote> GetUserDiginotes(string nickname)
        {

            string sql = "SELECT idUser, name FROM USER WHERE nickname = '" + nickname + "'";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            var u = new User(Convert.ToInt32(reader["idUser"]), Convert.ToString(reader["name"]), nickname);


            return GetUserDiginotes(u);
        }

        public IUser RegisterUser(string nickname, string password, string name, string address)
        {
            string sql = String.Format("INSERT INTO USER (name, nickname, password) values ('{0}','{1}', '{2}')", name, nickname, GetHashSha1(password));

            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            int id;
            try
            {
                command.ExecuteNonQuery();
                id = (int)_mDbConnection.LastInsertRowId;
            }
            catch (SQLiteException exception)
            {
                Debug.WriteLine("exception in " + exception.Source + ": '" + exception.Message + "'");
                return null;
            }

            //add User to panel

            User u = new User(id, name, nickname);

            if (_myWindow != null)
                _myWindow.AddUser(u, true);

            if (address != null)
                _table.Add(u.IdUser, address);


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

        public void Logout(IUser user)
        {
            _myWindow.AddUser(user, false);
            _table.Remove(user.IdUser);
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

        public void InsertBuyOrder(int quantity, IUser user, int satisfied = 0, bool closed = false)
        {
            string sql = String.Format("INSERT INTO BUYORDER (date, user, sharePrice, wanted, closed, satisfied) SELECT '{0}', idUser, '{1}','{3}',{4}, {5} FROM USER WHERE nickname = '{2}'",
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
                User u = new User(Convert.ToInt32(reader["idUser"]), Convert.ToString(reader["name"]), Convert.ToString(reader["nickname"]));
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

        public int BuyDiginotes(int quantity, IUser user)
        {

            SQLiteTransaction t = _mDbConnection.BeginTransaction();
            string sql = "SELECT * FROM SELLORDER WHERE not closed ORDER BY date asc";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<IOrder> orders = new List<IOrder>();

            int howManyLeftToBuy = quantity;

            while (reader.Read())
            {
                Order o1 = new Order(OrderOptionEnum.Sell, Convert.ToInt32(reader["idSellOrder"]), Convert.ToInt32(reader["user"]), Convert.ToInt32(reader["wanted"]), Convert.ToInt32(reader["satisfied"]));

                int howManyToSell = o1.Wanted - o1.Satisfied;
                if (howManyLeftToBuy > howManyToSell)
                {
                    o1.Satisfied = o1.Wanted;
                    orders.Add(o1);
                    howManyLeftToBuy -= howManyToSell;
                    TransferDiginotes(o1.IdUser, user.IdUser, howManyToSell);
                }
                else
                {
                    o1.Satisfied += howManyLeftToBuy;
                    orders.Add(o1);
                    TransferDiginotes(o1.IdUser, user.IdUser, howManyLeftToBuy);
                    howManyLeftToBuy = 0;
                    break;
                }
            }

            InsertBuyOrder(quantity, user, quantity - howManyLeftToBuy, true);

            foreach (IOrder o in orders)
            {
                if (o.Satisfied < o.Wanted)
                    sql = String.Format("UPDATE SELLORDER SET satisfied = {0} WHERE idSellOrder = {1}", o.Satisfied, o.IdOrder);
                else sql = String.Format("UPDATE SELLORDER SET satisfied = {0}, closed = 1 WHERE idSellOrder = {1}", o.Satisfied, o.IdOrder);

                try
                {
                    command = new SQLiteCommand(sql, _mDbConnection);
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException exception)
                {
                    Debug.WriteLine("exception in " + exception.Source + ": '" + exception.Message + "'");
                    //TODO fazer try catch global para rollback
                }

            }

            t.Commit();

            foreach (IOrder o in orders)
            {
                IClientNotify c = GetUserChannel(o.IdUser);
                if(c!=null)
                    c.UpdateClientView();
            }

            return quantity - howManyLeftToBuy;
        }

        public int SellDiginotes(int quantity, IUser user) // apenas para as que estao disponiveis
        {

            SQLiteTransaction t = _mDbConnection.BeginTransaction();
            string sql = "SELECT * FROM BUYORDER WHERE not closed ORDER BY date asc";
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<IOrder> orders = new List<IOrder>();

            int howManyLeft = quantity;

            while (reader.Read())
            {
                Order o1 = new Order(OrderOptionEnum.Sell, Convert.ToInt32(reader["idBuyOrder"]), Convert.ToInt32(reader["user"]), Convert.ToInt32(reader["wanted"]), Convert.ToInt32(reader["satisfied"]));

                int howManyToOffer = o1.Wanted - o1.Satisfied;
                if (howManyLeft > howManyToOffer)
                {
                    o1.Satisfied = o1.Wanted;
                    orders.Add(o1);
                    howManyLeft -= howManyToOffer;
                    TransferDiginotes(user.IdUser, o1.IdUser, howManyToOffer);
                }
                else
                {
                    o1.Satisfied += howManyLeft;
                    orders.Add(o1);
                    TransferDiginotes(user.IdUser, o1.IdUser, howManyLeft);
                    howManyLeft = 0;
                    break;
                }
            }


            InsertSellOrder(quantity, user, quantity - howManyLeft, true);

            foreach (IOrder o in orders)
            {
                if (o.Satisfied < o.Wanted)
                    sql = String.Format("UPDATE BUYORDER SET satisfied = {0} WHERE idBuyOrder = {1}", o.Satisfied, o.IdOrder);
                else sql = String.Format("UPDATE BUYORDER SET satisfied = {0}, closed = 1 WHERE idBuyOrder = {1}", o.Satisfied, o.IdOrder);

                try
                {
                    command = new SQLiteCommand(sql, _mDbConnection);
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException exception)
                {
                    Debug.WriteLine("exception in " + exception.Source + ": '" + exception.Message + "'");
                    //TODO fazer try catch global para rollback
                }


            }

            t.Commit();

            foreach (IOrder o in orders)
            {
                IClientNotify c = GetUserChannel(o.IdUser);
                if (c != null)
                    c.UpdateClientView();
            }

            return quantity - howManyLeft;
        }

        public IOrder GetUserPendingOrder(IUser user)
        {
            string sql = String.Format("SELECT * FROM BUYORDER WHERE closed = 0 and user = {0} LIMIT 1", user.IdUser);
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            if (reader.Read())
                return new Order(OrderOptionEnum.Buy, Convert.ToInt32(reader["idBuyOrder"]), user.IdUser, Convert.ToInt32(reader["wanted"]), Convert.ToInt32(reader["satisfied"]), Convert.ToBoolean(reader["closed"]));

            sql = String.Format("SELECT * FROM SELLORDER WHERE closed = 0 and user = {0} LIMIT 1", user.IdUser);
            command = new SQLiteCommand(sql, _mDbConnection);
            reader = command.ExecuteReader();

            if (reader.Read())
                return new Order(OrderOptionEnum.Sell, Convert.ToInt32(reader["idSellOrder"]), user.IdUser, Convert.ToInt32(reader["wanted"]), Convert.ToInt32(reader["satisfied"]), Convert.ToBoolean(reader["closed"]));

            return null;
        }

        /*
        private void UpdateDiginoteOwner(int destination, IDiginote d)
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
        }
        */
        private void TransferDiginotes(int origin, int destination, int quantity)
        {
            String sql = String.Format("UPDATE DIGINOTE SET user = {0} WHERE idDiginote in (SELECT idDiginote FROM Diginote where user = {1} LIMIT {2})", destination, origin, quantity);
            try
            {
                SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
                command.ExecuteNonQuery();
            }
            catch (SQLiteException exception)
            {
                Debug.WriteLine("exception in " + exception.Source + ": '" + exception.Message + "'");
            }
        }

        public void SuggestNewSharePrice(float newPrice, IUser user, IOrder order)
        {
            //alterar cotacao
            UpdateShare(newPrice, user);
            KeepOrderOn(order);
            NotifySharePriceChange(user);

        }

        public void SuggestNewSharePrice(float newPrice, IUser user, bool sell, int quantity)
        {
            UpdateShare(newPrice, user);

            if (sell)
                InsertSellOrder(quantity, user);
            else
                InsertBuyOrder(quantity, user);

            NotifySharePriceChange(user);

        }

        private void NotifySharePriceChange(IUser user)
        {
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
                            handler1(ChangeOperation.ShareChange, user.IdUser);
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
            _myWindow.UpdateChart();
        }

        private void UpdateShare(float newPrice, IUser user)
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
        }

        public void KeepOrderOn(IOrder order)
        {
            string sql;
            if (order.OrderType == OrderOptionEnum.Buy)
                 sql = String.Format("UPDATE BUYORDER SET shareprice = {0} where idBuyOrder = {1}", SharePrice, order.IdOrder);
            else sql = String.Format("UPDATE SELLORDER SET shareprice = {0} where idSellOrder = {1}", SharePrice, order.IdOrder);

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


        public void RevokeOrder(IOrder order)
        {
            string sql;
            if (order.OrderType == OrderOptionEnum.Buy)
                sql = String.Format("UPDATE BUYORDER SET closed = 1 where idBuyOrder = {0}", order.IdOrder);
            else sql = String.Format("UPDATE SELLORDER SET closed = 1 where idSellOrder = {0}", order.IdOrder);

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

        private void SetTimer()
        {
            Debug.WriteLine("setting Timer");

            if (UpdateLockingEvent != null)
            {
                Delegate[] invkList = UpdateLockingEvent.GetInvocationList();

                //foreach (ChangeDelegate handler in invkList)
                foreach (var @delegate in invkList)
                {
                    var handler = (UpdateTimerLockingDelegate)@delegate;
                    var handler1 = handler;
                    new Thread(() =>
                    {
                        try
                        {
                            handler1(true);
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

            CountDown = Constants.TimerSeconds + 2;
            _timer = new Timer(timer1_Tick, null, 0, 1000);

        }

        private void timer1_Tick(object sender)
        {
            CountDown--;

            if (CountDown == 0)
            {
                if (UpdateLockingEvent != null)
                {
                    Delegate[] invkList = UpdateLockingEvent.GetInvocationList();

                    //foreach (ChangeDelegate handler in invkList)
                    foreach (var @delegate in invkList)
                    {
                        var handler = (UpdateTimerLockingDelegate)@delegate;
                        var handler1 = handler;
                        new Thread(() =>
                        {
                            try
                            {
                                handler1(false);
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
                _timer.Dispose();
            }

        }

        public IClientNotify GetUserChannel(int userId)
        {
            string addr = _table[userId] as string;

            if (addr != null)
                return (IClientNotify)RemotingServices.Connect(typeof(IClientNotify), addr); // Obtain a reference to the client remote object

            return null;
        }

        public int GetNumberOfAvailableDiginotes()
        {
            int available = 0;
            string sql = String.Format("SELECT * FROM SELLORDER WHERE NOT closed");
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                available += (Convert.ToInt32(reader["wanted"]) - Convert.ToInt32(reader["satisfied"]));
            }

            return available;
        }

        public int  GetNumberOfDemmandingDiginotes()
        {
            int demand = 0;
            string sql = String.Format("SELECT * FROM BUYORDER WHERE NOT closed");
            SQLiteCommand command = new SQLiteCommand(sql, _mDbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                demand += (Convert.ToInt32(reader["wanted"]) - Convert.ToInt32(reader["satisfied"]));
            }

            return demand;
        }

        private void UpdateClients() //TODO: NOT USED YET 
        {
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
                            handler1(ChangeOperation.UpdateInterface,0);
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
}