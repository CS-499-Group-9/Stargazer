using Godot;
using System;
using System.Threading.Tasks;

namespace Stargazer
{
    /// <summary>
    /// Contains the button used to toggle the visibility of the Messier Deep Space Objects
    /// </summary>
    public partial class MessierButton : Control
    {

        // TODO: DELETE THIS! This is just used right now to show how to pass the user request from the update button.
        // This will need to be cut and pasted into the UpdateButton class.
        public Func<double, double, DateTime, Task> NotifyControllerOfUserUpdate;

        // TODO: Uncomment this next line and provide code comments.
        //public Action<bool> MessierObjectsToggled;

        // TODO: DELETE THIS
        // This will need to be attached to the signal on the UpdateButton class, and get the lat/long/time from the other objects.
        private async void RunTheThing()
        {
           

        }

        // Set the button signal to this. Or it can actually be set directly to MessierObjectsToggled above.
        private void ToggleMessierObjects(bool state)
        {
            // TODO: Uncomment this next line once the messier objects are ready to go live.
            //MessierObjectsToggled(state);
        }

        // TODO: Delete this struct.
        
    }
}