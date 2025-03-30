using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Stargazer
{
    public class PlaySpeed
    {
        private const int slowJump = 60;
        private const int fastJump = 3600;
        public int TotalSeconds { get { return (int)timeSpan.TotalSeconds; } }
        public int Multiplier { get { return (int)timeSpan.TotalSeconds; } }

        private TimeSpan timeSpan;
        public bool IsSyncronized { get; private set; }

        public PlaySpeed()
        {
            timeSpan = TimeSpan.FromSeconds(1);
        }

        public void SyncronizeTime()
        {
            RealTime();
            IsSyncronized = true;
        }

        public TimeSpan IncreaseBySeconds(int seconds)
        {
            IsSyncronized = false;
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(seconds));
            validateTimeSpan();
            return timeSpan;
        }

        public TimeSpan IncreaseByMinutes(int minutes)
        {
            IsSyncronized = false;
            timeSpan = timeSpan.Add(TimeSpan.FromMinutes(minutes));
            validateTimeSpan();
            return timeSpan;
        }

        public TimeSpan RealTime()
        {
            IsSyncronized = false; 
            return timeSpan = TimeSpan.FromSeconds(1);
        }

        public override string ToString()
        {
            return timeSpan.ToString();
        }


        private void validateTimeSpan()
        {
            if (timeSpan.Duration().TotalSeconds < 1) timeSpan = TimeSpan.Zero;
        }

        internal TimeSpan IncreateByHours(int hours)
        {
            IsSyncronized = false;
            timeSpan = timeSpan.Add(TimeSpan.FromHours(hours));
            validateTimeSpan();
            return timeSpan ;
        }

        internal TimeSpan IncreaseByDays(int days)
        {
            IsSyncronized = false;
            timeSpan = timeSpan.Add(TimeSpan.FromDays(days));
            validateTimeSpan();
            return timeSpan ;
        }

    }
}
