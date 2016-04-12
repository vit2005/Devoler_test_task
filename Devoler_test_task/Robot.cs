using System.Collections.Generic;

namespace Devoler_test_task
{

    /// <summary>
    /// Enum with possible modes
    /// </summary>
    enum Mode
    {
        waiting,    // waiting for recharge
        charging,   // increasing hp
        sleeping    // doing nothing
    }

    abstract class Robot
    {
        bool isDischarged;              // for mark of discharged robots

        protected string title;         // for debug log
        protected int hp;               // health pointh of robot
        protected int chargingTime;     // variable between 0 and 500
        protected int dischargingTime;  // variable between 0 and 1000
        protected int sleepingTime;     // variable of sleepi9ng time
        protected Mode mode;            // current mode of robot
        protected List<Item> ItemsToRecharge; // list of items that can be grabbed for recharge

        public int HP { get { return hp; } }            // public property of protected hp variable with getter 
        public string Title { get { return title; } }   // public property of protected title variable with getter 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">Robot title string</param>
        /// <param name="hp">Initial hp of robot</param>
        /// <param name="items">Items to recharge robot</param>
        public Robot (string title = null, int hp = 50, List<Item> items = null)
        {
            isDischarged = false;
            this.title = title;
            this.hp = hp;
            mode = Mode.waiting;
            ItemsToRecharge = new List<Item>();
            if (items != null)
                ItemsToRecharge.AddRange(items);
        }

        /// <summary>
        /// Methad for adding an item to ItemsToRecharge list
        /// </summary>
        /// <param name="item">Item that will be added</param>
        public void AddItem(Item item)
        {
            ItemsToRecharge.Add(item);
        }

        /// <summary>
        /// Factory method that returns class wich depends on type of robot
        /// </summary>
        /// <param name="type">Type of robot: 1 - Random, 2 - Envy, 3 - Gentleman</param>
        /// <param name="title">Title of robot</param>
        /// <param name="hp">Initial hp of robot</param>
        /// <param name="items">Items to recharge robot</param>
        /// <returns>Specific child of abstract Robot class</returns>
        public static Robot RobotFactory(byte type, string title = null, int hp = 50, List<Item> items = null)
        {
            switch (type)
            {
                case 1:
                    return new RandomRobot(title, hp, items);
                case 2:
                    return new EnvyRobot(title, hp, items);
                case 3:
                    return new GentlemanRobot(title, hp, items);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Public method of strategy of robot
        /// </summary>
        public void DoStrategy()
        {
            if (isDischarged)
                return;

            switch (mode)
            {
                case Mode.waiting:
                    DoWaitingStrategy();
                    break;
                case Mode.charging:
                    DoChargingStrategy();
                    break;
                case Mode.sleeping:
                    DoSleepingStartegy();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method of spending time intervsl (100 ms by default)
        /// </summary>
        public void SpendTime()
        {
            if (isDischarged)
                return;

            dischargingTime += 100;
            if (dischargingTime == 1000)
            {
                hp -= 10;
                dischargingTime = 0;
            }
            if (hp == 0)
            {
                ReleaseItems();
                isDischarged = true;
                return;
            }
                
            switch (mode)
            {
                case Mode.waiting:
                    break;
                case Mode.charging:
                    chargingTime += 100;
                    break;
                case Mode.sleeping:
                    sleepingTime -= 100;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Checking possibility of starting recharge (if (item.Owner != null && item.Owner != this))
        /// </summary>
        /// <returns>Boolean variable of possibility of start recharge</returns>
        protected bool CanStartRecharge()
        {
            bool result = true;
            foreach (Item item in ItemsToRecharge)
            {
                if (item.Owner != null && item.Owner != this)
                    result = false;
            }
            return result;
        }

        /// <summary>
        /// Checking possibility of continue recharge (if (item.Owner != this))
        /// </summary>
        /// <returns></returns>
        protected bool CanContinueRecharge()
        {
            bool result = true;
            foreach (Item item in ItemsToRecharge)
            {
                if (item.Owner != this)
                    result = false;
            }
            return result;
        }

        /// <summary>
        /// Release all items to recharge from ItemsToRecharge list
        /// </summary>
        protected void ReleaseItems()
        {
            foreach (Item item in ItemsToRecharge)
            {
                if (item.Owner == this)
                    item.Owner = null;
            }
        }

        /// <summary>
        /// Grab all items from ItemsToRecharge list
        /// </summary>
        protected void GrabItems()
        {
            foreach (Item item in ItemsToRecharge)
            {
                item.Owner = this;
            }
        }

        /// <summary>
        /// Abstract WaitingStrategy method which must be implemented in child classes
        /// </summary>
        protected abstract void DoWaitingStrategy();

        /// <summary>
        /// Abstract ChargingStrategy method which must be implemented in child classes
        /// </summary>
        protected abstract void DoChargingStrategy();

        /// <summary>
        /// Abstract SleepingStartegy method which must be implemented in child classes
        /// </summary>
        protected abstract void DoSleepingStartegy();
    }
}
