using System;
using System.Collections.Generic;

namespace DesktopPet
{
    public static class PetHistory
    {
        public static List<TimeSpan> DeadPetLifespans { get; } = new List<TimeSpan>();

        public static void AddDeadPet(TimeSpan lifespan)
        {
            DeadPetLifespans.Add(lifespan);
        }
    }
}