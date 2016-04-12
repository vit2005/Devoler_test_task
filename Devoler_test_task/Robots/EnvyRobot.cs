using System.Collections.Generic;

namespace Devoler_test_task
{
    class EnvyRobot : Robot
    {
        public EnvyRobot(string title = null, int hp = 50, List<Item> items = null) : base("E-" + title, hp, items)
        {

        }

        protected override void DoWaitingStrategy()
        {
            if (hp < 60)
            {
                mode = Mode.charging;
                chargingTime = 0;
                GrabItems();
            }
            else
            {
                if (CanStartRecharge() && hp < 100)
                {
                    mode = Mode.charging;
                    chargingTime = 0;
                    GrabItems();
                }
                else
                {
                    mode = Mode.sleeping;
                    sleepingTime = 500;
                }
            }    
        }

        protected override void DoChargingStrategy()
        {
            if (CanContinueRecharge())
            {
                if (chargingTime == 500)
                {
                    chargingTime = 0;
                    hp += 10;
                }
                if (hp >= 100)
                {
                    ReleaseItems();
                    mode = Mode.sleeping;
                    sleepingTime = 500;
                }
            }
            else
            {
                chargingTime = 0;
                mode = Mode.charging;
                GrabItems();
            }
        }

        protected override void DoSleepingStartegy()
        {
            if (sleepingTime <= 0)
            {
                mode = Mode.waiting;
            }
        }
        
    }
}
