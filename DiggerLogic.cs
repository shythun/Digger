using System;
using System.Windows.Forms;

namespace Digger
{
    class Terrain : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new Digger.CreatureCommand();
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject is Player;
        }

        public int GetDrawingPriority()
        {
            return 2;
        }

        public string GetImageFileName()
        {
            return "Terrain.png";
        }
    }


    class Player : ICreature, IMovingObject
    {
        public CreatureCommand Act(int x, int y)
        {
            if (Game.KeyPressed == Keys.Left && ObjectCanMoveTo(x - 1, y))
                return new Digger.CreatureCommand { DeltaX = -1 };

            if (Game.KeyPressed == Keys.Right && ObjectCanMoveTo(x + 1, y))
                return new Digger.CreatureCommand { DeltaX = 1 };

            if (Game.KeyPressed == Keys.Down && ObjectCanMoveTo(x, y + 1))
                return new Digger.CreatureCommand { DeltaY = 1 };

            if (Game.KeyPressed == Keys.Up && ObjectCanMoveTo(x, y - 1))
                return new Digger.CreatureCommand { DeltaY = -1 };

            return new Digger.CreatureCommand();
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Sack) return true;
            if (conflictedObject is Monster) return true;

            return false;
        }

        public int GetDrawingPriority()
        {
            return 1;
        }

        public string GetImageFileName()
        {
            return "Digger.png";
        }

        public bool ObjectCanMoveTo(int x, int y)
        {
            if (x < 0) return false;
            if (y < 0) return false;

            if (x > Game.MapWidth - 1) return false;
            if (y > Game.MapHeight - 1) return false;

            if (Game.Map[x, y] is Sack) return false;

            return true;
        }
    }


    class Sack : ICreature, IMovingObject
    {
        private int fallDistance;

        public CreatureCommand Act(int x, int y)
        {
            if (ObjectCanMoveTo(x, y + 1))
            {
                fallDistance++;
                return new Digger.CreatureCommand { DeltaY = 1 };
            }
            else
            {
                if(fallDistance > 1)
                    return new Digger.CreatureCommand { TransformTo = new Gold() };

                fallDistance = 0;
            }

            return new Digger.CreatureCommand();
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return false;
        }

        public int GetDrawingPriority()
        {
            return 2;
        }

        public string GetImageFileName()
        {
            return "Sack.png";
        }

        public bool ObjectCanMoveTo(int x, int y)
        {
            if (y > Game.MapHeight - 1)  return false;

            if (Game.Map[x, y] is Sack)      return false;
            if (Game.Map[x, y] is Terrain)   return false;
            if (Game.Map[x, y] is Gold)      return false;
            if (Game.Map[x, y] is Player)    return fallDistance > 0;
            if (Game.Map[x, y] is Monster)   return fallDistance > 0;

            return true;
        }
    }


    class Gold : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new Digger.CreatureCommand();
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Player)
            {
                Game.Scores += 10;
                return true;
            }

            if (conflictedObject is Monster) return true;

            return false;
        }

        public int GetDrawingPriority()
        {
            return 0;
        }

        public string GetImageFileName()
        {
            return "Gold.png";
        }
    }


    class Monster : ICreature, IMovingObject
    {
        public CreatureCommand Act(int x, int y)
        {
            if (FindDigger(out int i, out int j))
            {
                if (x > i && ObjectCanMoveTo(x - 1, y))
                    return new Digger.CreatureCommand() { DeltaX = -1 };

                if (x < i && ObjectCanMoveTo(x + 1, y))
                    return new Digger.CreatureCommand() { DeltaX = 1 };

                if (y > j && ObjectCanMoveTo(x, y - 1))
                    return new Digger.CreatureCommand() { DeltaY = -1 };

                if (y < j && ObjectCanMoveTo(x, y + 1))
                    return new Digger.CreatureCommand() { DeltaY = 1 };
            }

            return new Digger.CreatureCommand();
        }

        private static bool FindDigger(out int i, out int j)
        {
            i = 0;
            j = 0;
            for (i = 0; i < Game.MapWidth; i++)
                for (j = 0; j < Game.MapHeight; j++)
                    if (Game.Map[i, j] is Player)
                        return true;

            return false;
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Sack)    return true;
            if (conflictedObject is Monster) return true;

            return false;
        }

        public int GetDrawingPriority()
        {
            return 0;
        }

        public string GetImageFileName()
        {
            return "Monster.png";
        }

        public bool ObjectCanMoveTo(int x, int y)
        {
            if (x < 0) return false;
            if (y < 0) return false;

            if (x > Game.MapWidth - 1)   return false;
            if (y > Game.MapHeight - 1)  return false;

            if (Game.Map[x, y] is Sack)      return false;
            if (Game.Map[x, y] is Terrain)   return false;
            if (Game.Map[x, y] is Monster)   return false;

            return true;
        }
    }


    public interface IMovingObject
    {
        bool ObjectCanMoveTo(int x, int y);
    }
}
