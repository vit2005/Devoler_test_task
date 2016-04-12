using System;
using System.Collections.Generic;

namespace Devoler_test_task
{
    class RandomRobot : Robot
    {
        Random r;
        const int minSleepingTime = 1;
        const int maxSleepingTime = 3;

        public RandomRobot(string title = null, int hp = 50, List<Item> items = null) : base("R-" + title, hp, items)
        {
            r = new Random();
        }

        protected override void DoWaitingStrategy()
        {
            if (CanStartRecharge() && hp < 100)
            {
                mode = Mode.charging;
                chargingTime = 0;
                GrabItems();
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
                    ReleaseItems();
                    mode = Mode.sleeping;
                    sleepingTime = (r.Next(minSleepingTime, maxSleepingTime) + 1) * 100;
                    System.Threading.Thread.Sleep(3);
                }
                if (hp >= 100)
                {
                    ReleaseItems();
                    mode = Mode.sleeping;
                    sleepingTime = (r.Next(minSleepingTime, maxSleepingTime) + 1) * 100;
                    System.Threading.Thread.Sleep(3);
                }
            }
            else
            {
                chargingTime = 0;
                mode = Mode.waiting;
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
