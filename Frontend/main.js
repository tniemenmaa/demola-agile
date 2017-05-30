
"use strict";

function OnNewShellUI(shellUI) {
    /// <summary>The entry point of ShellUI module.</summary>
    /// <param name="shellUI" type="MFiles.ShellUI">The new shell UI object.</param> 

    // Register to listen new shell frame creation event. This reacts to normal shell frames only (not e.g. common dialog nor embedded shell frames).
    shellUI.Events.Register(MFiles.Event.NewNormalShellFrame, newShellFrameHandler);
}

function newShellFrameHandler(shellFrame) {
    /// <summary>Event handler to handle new shell frame object creations.</summary>
    /// <param name="shellFrame" type="MFiles.ShellFrame">The new shell frame object.</param> 

    // Register to listen the started event.
    shellFrame.Events.Register(MFiles.Event.Started, getShellFrameStartedHandler(shellFrame));
}

function getShellFrameStartedHandler(shellFrame) {
    /// <summary>Gets the event handler for "started" event of a shell frame.<summary>
    /// <param name="shellFrame" type="MFiles.ShellFrame">The current shell frame object.</param>
    /// <returns type="MFiles.Event.OnStarted">The event handler object</returns>

    // Return the handler function for Started event.
    return function () {
        /// <summary>The "started" event handler implementation for a shell frame.<summary>

        // Shell frame object is now started. Check if this is the root view.
        if (shellFrame.CurrentPath == "") {

            // This is the vault root.

            // Replace the listing with a dashboard.
            shellFrame.ShowDashboard("planner", null);

            // Show the right pane dashboard.
            /*var homeTab = shellFrame.RightPane.AddTab("_home", MFiles.GetStringResource(27664), "_last");  // The string id 27664 is allocated from the resource-id space, which is mainly used through enumeration from localization.js. This is an exception.
            homeTab.ShowDashboard("home_right", null);
            homeTab.Visible = true;
            homeTab.Select();*/
        }
    };
}