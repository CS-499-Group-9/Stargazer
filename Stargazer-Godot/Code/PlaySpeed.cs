using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Stargazer
{
    /// <summary>
    /// A state object to control the play speed of the simulation
    /// </summary>
    public class PlaySpeed
    {
        private const int slowJump = 60;
        private const int fastJump = 3600;
        private TimeSpan timeSpan;
        /// <summary>
        /// Gets the total seconds that should elapse in the simulation for each real world second.
        /// </summary>
        public int TotalSeconds { get { return (int)timeSpan.TotalSeconds; } }

        /// <summary>
        /// Indicates if the simulation is synchronized to real time.
        /// </summary>
        public bool IsSyncronized { get; private set; }

        /// <summary>
        /// Initializes the object
        /// </summary>
        public PlaySpeed()
        {
            timeSpan = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Synchronizes the time to real time (1:1 and current time)
        /// </summary>
        public void SynchronizeTime()
        {
            RealTime();
            IsSyncronized = true;
        }
        /// <summary>
        /// Increases or decreases the speed of the simulation in seconds.
        /// </summary>
        /// <param name="seconds">1 or -1</param>
        /// <returns>The new time span that elapses every real world second</returns>
        public TimeSpan IncreaseBySeconds(int seconds)
        {
            IsSyncronized = false;
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(seconds));
            validateTimeSpan();
            return timeSpan;
        }
        /// <summary>
        /// Increases or decreases the speed of the simulation in minutes
        /// </summary>
        /// <param name="minutes">1 or -1</param>
        /// <returns>The new time span that elapses every real world second.</returns>
        public TimeSpan IncreaseByMinutes(int minutes)
        {
            IsSyncronized = false;
            timeSpan = timeSpan.Add(TimeSpan.FromMinutes(minutes));
            validateTimeSpan();
            return timeSpan;
        }
        /// <summary>
        /// Resets the simulation to 1:1 without changing the time.
        /// </summary>
        /// <returns></returns>
        public TimeSpan RealTime()
        {
            IsSyncronized = false; 
            return timeSpan = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Used for displaying the current timespan multiplier.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return timeSpan.ToString();
        }


        private void validateTimeSpan()
        {
            if (timeSpan.Duration().TotalSeconds < 1) timeSpan = TimeSpan.Zero;
        }

        /// <summary>
        /// Increases or decreases the speed of the simulation in hours
        /// </summary>
        /// <param name="hours">1 or -1</param>
        /// <returns>The new time span that elapses every real world second.</returns>
        internal TimeSpan IncreateByHours(int hours)
        {
            IsSyncronized = false;
            timeSpan = timeSpan.Add(TimeSpan.FromHours(hours));
            validateTimeSpan();
            return timeSpan ;
        }

        /// <summary>
        /// Increases or decreases the speed of the simulation in days.
        /// </summary>
        /// <param name="days">1 or -1</param>
        /// <returns>The new time span that elapses every real world second.</returns>
        internal TimeSpan IncreaseByDays(int days)
        {
            IsSyncronized = false;
            timeSpan = timeSpan.Add(TimeSpan.FromDays(days));
            validateTimeSpan();
            return timeSpan ;
        }

    }
}
