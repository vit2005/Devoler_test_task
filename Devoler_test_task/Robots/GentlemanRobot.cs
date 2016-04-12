using System.Collections.Generic;

namespace Devoler_test_task
{
    class GentlemanRobot : Robot
    {
        List<Robot> neighbors;
        bool canRecharge;

        public GentlemanRobot(string title = null, int hp = 50, List<Item> items = null) : base("G-" + title, hp, items)
        {
            neighbors = new List<Robot>();
            canRecharge = true;
        }

        public void SetNeighbors(int id, List<Robot> list)
        {
            if (id == 0)
            {
                neighbors.Add(list[list.Count - 1]);
                neighbors.Add(list[1]);
            }
            else if (id == list.Count-1)
            {
                neighbors.Add(list[id-1]);
                neighbors.Add(list[0]);
            }
            else
            {
                neighbors.Add(list[id - 1]);
                neighbors.Add(list[id + 1]);
            }
        }

        protected override void DoWaitingStrategy()
        {
            canRecharge = true;
            for (int i = 0; i < neighbors.Count; i++)
            {
                CheckNeighborHp(i);
            }
            if (canRecharge && CanStartRecharge() && hp < 100)
            {
                mode = Mode.charging;
                GrabItems();
            }
        }

        protected override void DoChargingStrategy()
        {
            canRecharge = true;
            for (int i = 0; i < neighbors.Count; i++)
            {
                CheckNeighborHp(i);
            }
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
                    mode = Mode.waiting;
                }
                if (!canRecharge)
                {
                    mode = Mode.waiting;
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
            
        }

        void CheckNeighborHp(int id)
        {
            if (neighbors[id].HP > 0 && neighbors[id].HP < hp)
            {
                canRecharge = false;
                if (ItemsToRecharge[id].Owner == this)
                    ItemsToRecharge[id].Owner = null;
            }
        }
    }
}
