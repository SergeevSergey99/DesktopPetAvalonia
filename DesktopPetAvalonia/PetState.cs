using System;

namespace DesktopPet
{
    public enum PetAction
    {
        Feed,
        Pet
    }

    public class PetState
    {
        public const int MAX_HUNGER = 100;
        public const int MAX_LONELINESS = 300;

        public int Hunger { get; private set; }
        public int Loneliness { get; private set; }
        public bool IsDead { get; private set; }
        public DateTime CreationTime { get; private set; }
        
        public event EventHandler? StateChanged;
        public event EventHandler? PetDied;

        public PetState()
        {
            Reset();
        }

        public void Reset()
        {
            Hunger = 0;
            Loneliness = 0;
            IsDead = false;
            CreationTime = DateTime.Now;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateState()
        {
            if (IsDead)
                return;

            if (Hunger < MAX_HUNGER) Hunger++;
            if (Loneliness < MAX_LONELINESS) Loneliness++;

            if (Hunger >= MAX_HUNGER || Loneliness >= MAX_LONELINESS)
            {
                IsDead = true;
                PetDied?.Invoke(this, EventArgs.Empty);
            }
            
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void PerformAction(PetAction action)
        {
            if (IsDead)
                return;

            switch (action)
            {
                case PetAction.Feed:
                    Hunger = 0;
                    break;
                case PetAction.Pet:
                    Loneliness = 0;
                    break;
            }
            
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public TimeSpan GetLifespan()
        {
            return DateTime.Now - CreationTime;
        }
    }
}