"use strict";
window.addEventListener( "keydown", onKeyDown, true);
window.addEventListener( "keyup", onKeyUp, true );

window.keyDown = {};

// Key event handling for ctrl and shift click functionality 
function onKeyDown( e ) {

    if ( e.keyCode == 17 ) { // Ctrl
        window.keyDown.ctrl = true;
    }

    if ( e.keyCode == 16 ) { // Shift
        window.keyDown.shift = true;
    }
}

function onKeyUp( e ) {
    if ( e.keyCode == 17 ) { // Ctrl
        window.keyDown.ctrl = false;
    }

    if ( e.keyCode == 16 ) { // Shift
        window.keyDown.shift = false;
    }
}

// Create a global event bus for notifying Vue components
let Events = new Vue( {} );

Vue.component( "progressbar", {
    template: '<div class="progress status"> \
                    <div :class="barClass" role="progressbar" :style="barWidth"> \
                        <span>{{ this.value }} %</span> \
                    </div> \
                </div>',
    props: [ "value" ],
    computed: {
        barClass: function () {
            // Set bar color class based of percentage
            return "progress-bar style-" + Math.floor( this.value / 10 ) * 10;
        },
        barWidth: function () {
            // Set bar width based of percentage
            return "width: " + this.value + "%";
        }
    }
} )

Vue.component( "tasks", {
    template:
        '<draggable class="tasks" element="ul" v-model="userstory.TaskList" :options="draggableOptions"> \
            <li @click="select( $event, index )" v-for="( task, index ) in userstory.TaskList" draggable="true" :class="{ task: true, collapsable: true, collapsed: task.Collapsed, selected: task.Selected }" id="story"> \
                <i class="fa fa-square-o"></i> \
                <span class="group"></span>  \
                <span class="title">{{ task.NameOrTitle }}</span> \
                <span class="status">{{ task.State }}</span> \
            </li> \
            <li @click="select( $event, null )" v-if="!userstory.TaskList || userstory.TaskList.length === 0"><span class="no-items">There are no tasks in this userstory</span></li> \
         </draggable>',
    props: [ "userstory" ],
    created: function () {
        // Subscrible to event bus to listen to unselectAll events
        Events.$on( "unselectAll", this.unselectAll );
    },
    methods: {
        select: function ( event, index ) {
            let selection = this.userstory.TaskList[ index ].Selected;

            // Prevent default behaviour
            event.stopPropagation();
            // Broadcast unselectAll event
            Events.$emit( "unselectAll", event );

            // Set the selection of clicked item and open metadata card
            this.userstory.TaskList[ index ].Selected = !selection;
            MFilesAgile.methods.openMetadataCard( this.userstory.TaskList[ index ].InternalId, 103 );
        },
        unselectAll: function ( event ) {
            // Unselect all tasks of userstory
            this.userstory.TaskList.forEach( function ( task ) { task.Selected = false; } );
        },
        status: function ( index ) {
            let code = this.userstory.TaskList[ index ].TaskState;
            try {
                // Task states are defined in data-controller.js
                return taskStates[ code ];
            }
            catch ( exception ) {
                // In case of exception return 'Undefined' as task state
                // TODO: Add logging mechanism
                return taskStates[ 0 ];
            }
        }
    },
    computed: {
        draggableOptions: function () {
            return {
                group: "tasks", // Tasks can be moved only inside own tasks group
                delay: 0 // Delay from click until drag and drop functionality occurs
            }
        }
    }
} );

Vue.component( "userstories", {
    template:
        '<draggable class="userstories" element="ul" v-model="feature.UserStoryList" :options="draggableOptions"> \
            <li @click="select( $event, index )" v-for="( userstory, index ) in feature.UserStoryList" draggable="true" :class="{ userstory: true, collapsable: true, collapsed: userstory.Collapsed, selected: userstory.Selected }" id="story"> \
                <i @click="collapse( $event, index )" :class="collapseClasses( index )" aria-hidden="true"></i> \
                <span class="group">{{ userstory.UserStoryId }}</span>  \
                <span class="title">{{ userstory.NameOrTitle }}</span> \
                <progressbar class="" v-bind:value="progress( index )"></progressbar> \
                <span class="effort">{{ userstory.StoryPoints }}</span> \
                <tasks v-bind:userstory="userstory"></tasks> \
            </li> \
            <li v-if="!feature.UserStoryList || feature.UserStoryList.length === 0"><span class="no-items">There are no userstories in this feature</span></li> \
         </draggable>',
    props: [ "feature" ],
    data: function () {
        return {
            calculated: false,
        }
    },
    created: function () {
        // Subscrible to event bus to listen to unselectAll events
        Events.$on( "unselectAll", this.unselectAll );
    },
    mounted: function () {
        this.initialCalculations();
    },
    methods: {
        collapse: function ( event, index ) {
            // Prevent default behaviour
            event.stopPropagation();
            this.feature.UserStoryList[ index ].Collapsed = !this.feature.UserStoryList[ index ].Collapsed;
        },
        select: function ( event, index ) {
            let selection = this.feature.UserStoryList[ index ].Selected;

            // Prevent default behaviour
            event.stopPropagation();
            // Broadcast unselectAll event
            Events.$emit( "unselectAll", event );

            // Set the selection of clicked item and open metadata card
            this.feature.UserStoryList[ index ].Selected = !selection;
            MFilesAgile.methods.openMetadataCard( this.feature.UserStoryList[ index ].InternalId, 102 );
        },
        unselectAll: function ( event ) {
            // Unselect all userstories of feature
            this.feature.UserStoryList.forEach( function ( userstory ) { userstory.Selected = false; } );
        },
        collapseClasses: function ( index ) {
            // Sets the correct icon for collapsing / uncollapsing
            return this.feature.UserStoryList[ index ].Collapsed ? "fa fa-plus-square-o" : "fa fa-minus-square-o";
        },
        initialCalculations: function () {
            // Calculate the progress on userstory level from tasks
            // initial calculations have to be run because otherwise
            // progress will be calculated only after userstory is opened
            let that = this;
            this.feature.UserStoryList.forEach( function ( elem, index ) {
                that.progress( index );
            } );
            this.calculated = true;
        },
        progress: function ( index ) {
            // Calculate the progress of userstory
            let tasksDone = 0;
            this.feature.UserStoryList[ index ].TaskList.forEach( function ( task ) { if ( task.TaskState == 4 ) { tasksDone++; } } );
            let progress = ( tasksDone / Math.max( this.feature.UserStoryList[ index ].TaskList.length, 1 ) ) * 100;
            // Show with 0 decimal precision
            return progress.toFixed( 0 );

        }
    },
    computed: {
        draggableOptions: function () {
            return {
                group: "userstories", // Userstories can be moved only inside userstories and to other userstories
                delay: 0 // Delay from click until drag and drop functionality occurs
            }
        }
    }
} );

Vue.component( "feature", {
    template: '<li draggable="true" @click="select" :class="{ feature: true, collapsable: true, collapsed: feature.Collapsed, selected: feature.Selected }" id="story"> \
                    <i @click="collapse( $event, index )" :class="collapseClasses( index )" aria-hidden="true"></i> \
                    <span class="group"></span>  \
                    <span class="title">{{ feature.NameOrTitle }}</span> \
                    <progressbar class="" v-bind:value="progress( index )"></progressbar> \
                    <span class="effort">{{ storypointSum( index ) }}</span> \
                    <userstories v-bind:feature="feature"></userstories> \
              </li>',
    props: [ "feature", "index" ],
    methods: {
        select: function ( event, index ) {
            let selection = this.feature.Selected;

            // Prevent default behaviour
            event.stopPropagation();
            // Broadcast unselectAll event
            Events.$emit( "unselectAll", event );

            // Set the selection of clicked item and open metadata card
            this.feature.Selected = !selection;
            MFilesAgile.methods.openMetadataCard( this.feature.InternalId, 106 );
        },
        collapse: function ( event, index ) {
            // Prevent default behaviour
            event.stopPropagation();
            this.feature.Collapsed = !this.feature.Collapsed;
        },
        collapseClasses: function ( index ) {
            // Sets the correct icon for collapsing / uncollapsing
            return this.feature.Collapsed ? "fa fa-plus-square-o" : "fa fa-minus-square-o";
        },
        storypointSum: function ( index ) {
            // Calculate storypoint sum for the feature
            let sum = 0;
            this.feature.UserStoryList.forEach( function ( story ) { sum += story.StoryPoints; } );
            return sum;
        },
        progress: function ( index ) {
            // Calculate the progress of userstory
            let sum = 0;
            this.feature.UserStoryList.forEach( function ( story ) {
                sum += story.TaskList.reduce( function ( taskA, taskB ) {
                    return taskA + ( taskB.TaskState == 4 ? 1 : 0 );
                }, 0 ) / Math.max( story.TaskList.length, 1 )
            } );
            // Show with 0 decimal precision
            return parseFloat( ( sum / this.feature.UserStoryList.length ) * 100 ).toFixed( 0 );
        }
    }
} )
Vue.component( "features", {
    template:
        '<draggable class="features" element="ul" v-model="features" :options="draggableOptions"> \
            <feature v-for="( feature, index ) in features" v-bind:feature="feature" v-bind:index="index"></feature> \
            <li class="feature" v-if="!features || features.length === 0"><span class="no-items">There are no features in this backlog</span></li> \
         </draggable>',
    props: [ "backlog" ],
    data: function () {
        return {
            features: [  ],
            lastSelection: null
        };
    },
    created: function () {
        // Subscrible to event bus to listen to backlogOpened events
        Events.$on( "backlogOpened", this.backlogOpened )
        // Subscrible to event bus to listen to unselectAll events
        Events.$on( "unselectAll", this.unselectAll )
    },
    methods: {
        unselectAll: function ( event ) {
            // Unselect all features of backlog
            this.features.forEach( function ( feature ) { feature.Selected = false; } )
        },
        collapse: function ( event, index ) {
            // Prevent default behaviour
            event.stopPropagation();
            this.features[ index ].Collapsed = !this.features[ index ].Collapsed;
        },
        collapseClasses: function ( index ) {
            // Sets the correct icon for collapsing / uncollapsing
            return this.features[ index ].Collapsed ? "fa fa-plus-square-o" : "fa fa-minus-square-o";
        },
        backlogOpened: function ( backlogId ) {
            // When backlog is opened get all features / userstories / tasks for it
            if ( this.backlog.InternalId == backlogId && this.features.length == 0 ) {
                this.features = MFilesAgile.methods.getFeatures( this.backlog.InternalId );
            }
        }
    },
    computed: {
        draggableOptions: function () {
            return {
                group: "features", // Features can be moved only inside backlogs and to other backlogs
                delay: 0 // Delay from click until drag and drop functionality occurs
            }
        }
    }
} );

Vue.component( "backlogs", {
    template:
        '<div> \
            <h4 class="content-header">{{ header }} \
                <i class="fa fa-line-chart status"></i> \
                <i class="fa fa-trophy effort"></i> \
            </h4> \
            <draggable class="backlogs planner" element="ul" v-model="backlogs" :options="draggableOptions"> \
                <li @click="select( $event, index )" v-for="( backlog, index ) in backlogs" draggable="true" id="story" :class="{ backlog: true, collapsable: true, collapsed: backlog.Collapsed, selected: backlog.Selected }" > \
                    <i @click="collapse( $event, index )" :class="collapseClasses( index )" aria-hidden="true"></i> \ \
                    <span class="title">{{ backlog.NameOrTitle }}</span> \
                    <span class="status" style="background-image:none;"></span> \
                    <span class="effort"></span> \
                    <features v-on:loaded="attachFeatures" v-bind:backlog="backlog"></features> \
                </li> \
             </draggable> \
         </div>',
    data: function () {
        return {
            backlogs: [  ],
            header: "Planner"
        }
    },
    props: [ "header", "backlogtype" ],
    created: function () {
        // When the UI is ready load all the backlogs
        this.backlogs = MFilesAgile.methods.getBacklogs( this.backlogtype )
    },
    methods: {
        collapse: function ( event, index ) {
            // Prevent default behaviour
            event.stopPropagation();
            this.backlogs[ index ].Collapsed = !this.backlogs[ index ].Collapsed;
            if ( !this.backlogs[ index ].Collapsed ) {
                // When backlog is opened broadcast backlogOpened event
                Events.$emit( "backlogOpened", this.backlogs[ index ].InternalId );
            }
        },
        select: function ( event, index ) {
            let selected = !this.backlogs[ index ].Selected;
        },
        collapseClasses: function ( index ) {
            // Sets the correct icon for collapsing / uncollapsing
            return this.backlogs[ index ].Collapsed ? "fa fa-caret-down" : "fa fa-caret-up";
        },
        attachFeatures: function ( sum ) {
            // This is called when features are rendered calculates the sum of feature points
            // Not currently shown in UI
            sum = 0;
            if ( !this.backlogs[ index ].features ) return 0;
            this.backlogs[ index ].features.forEach( function ( feature ) { sum += feature.points; } );
            return sum;
        }
    },
    computed: {
        draggableOptions: function () {
            return {
                group: "backlogs" // Backlogs can be moved only inside backlog list
            }
        }
    }
});

Vue.component( "userstories", {
    template:
        '<draggable class="userstories" element="ul" v-model="feature.UserStoryList" :options="draggableOptions" @remove="dragRemove" @start="dragStart"> \
            <li @click="select( $event, index )" v-for="( userstory, index ) in feature.UserStoryList" draggable="true" :class="{ userstory: true, collapsable: true, collapsed: userstory.Collapsed, selected: userstory.Selected, locked: userstory.SprintId }" id="story"> \
                <i @click="collapse( $event, index )" :class="collapseClasses( index )" aria-hidden="true"></i> \
                <span class="group">{{ userstory.UserStoryId }}</span>  \
                <span class="title">{{ userstory.NameOrTitle }}</span> \
                <progressbar class="" v-bind:value="progress( index )"></progressbar> \
                <span class="effort">{{ userstory.StoryPoints }}</span> \
                <tasks v-bind:userstory="userstory"></tasks> \
            </li> \
            <li v-if="!feature.UserStoryList || feature.UserStoryList.length === 0"><span class="no-items">There are no userstories in this feature</span></li> \
         </draggable>',
    props: [ "feature" ],
    data: function () {
        return {
            calculated: false,
        }
    },
    created: function () {
        // Subscrible to event bus to listen to unselectAll events
        Events.$on( "unselectAll", this.unselectAll );
    },
    mounted: function () {
        this.initialCalculations();
    },
    methods: {
        collapse: function ( event, index ) {
            // Prevent default behaviour
            event.stopPropagation();
            this.feature.UserStoryList[ index ].Collapsed = !this.feature.UserStoryList[ index ].Collapsed;
        },
        select: function ( event, index ) {
            let selection = this.feature.UserStoryList[ index ].Selected;

            // Prevent default behaviour
            event.stopPropagation();
            // Broadcast unselectAll event
            Events.$emit( "unselectAll", event );

            // Set the selection of clicked item and open metadata card
            this.feature.UserStoryList[ index ].Selected = !selection;
            MFilesAgile.methods.openMetadataCard( this.feature.UserStoryList[ index ].InternalId, 102 );
        },
        unselectAll: function ( event ) {
            // Unselect all userstories of feature
            this.feature.UserStoryList.forEach( function ( userstory ) { userstory.Selected = false; } );
        },
        collapseClasses: function ( index ) {
            // Sets the correct icon for collapsing / uncollapsing
            return this.feature.UserStoryList[ index ].Collapsed ? "fa fa-plus-square-o" : "fa fa-minus-square-o";
        },
        initialCalculations: function () {
            // Calculate the progress on userstory level from tasks
            // initial calculations have to be run because otherwise
            // progress will be calculated only after userstory is opened
            let that = this;
            this.feature.UserStoryList.forEach( function ( elem, index ) {
                that.progress( index );
            } );
            this.calculated = true;
        },
        progress: function ( index ) {
            // Calculate the progress of userstory
            let tasksDone = 0;
            this.feature.UserStoryList[ index ].TaskList.forEach( function ( task ) { if ( task.TaskState == 4 ) { tasksDone++; } } );
            let progress = ( tasksDone / Math.max( this.feature.UserStoryList[ index ].TaskList.length, 1 ) ) * 100;
            // Show with 0 decimal precision
            return progress.toFixed( 0 );

        },
        dragStart: function (evnt) {

        },
        dragRemove: function (evnt) {
            this.feature.UserStoryList.splice(evnt.oldIndex, 0, evnt.item._underlying_vm_);
        }
    },
    computed: {
        draggableOptions: function () {
            return {
                group: "userstories", // Userstories can be moved only inside userstories and to other userstories
                delay: 0, // Delay from click until drag and drop functionality occurs
                filter: ".locked"
            }
        }
    }
} );


Vue.component("sprint-userstories", {
    template:
        '<draggable class="userstories sprint-userstories" element="ul" v-model="userstories" :options="draggableOptions" :data-sprint="backlog.InternalId" @add="dragAdd"> \
            <li @click="select( $event, index )" v-for="( userstory, index ) in userstories" draggable="true" :class="{ userstory: true, collapsable: true, collapsed: userstory.CollapsedInSprint, selected: userstory.Selected }" id="story"> \
                <i @click="collapse( $event, index )" :class="collapseClasses( index )" aria-hidden="true"></i> \
                <span class="group">{{ userstory.UserStoryId }}</span>  \
                <span class="group feature-group" :title="userstory.FeatureTitle">{{ userstory.FeatureId }}</span> \
                <span class="title">{{ userstory.NameOrTitle }}</span> \
                <progressbar class="" v-bind:value="progress( index )"></progressbar> \
                <span class="effort">{{ userstory.StoryPoints }}</span> \
                <tasks v-bind:userstory="userstory"></tasks> \
            </li> \
            <li v-if="!userstories || userstories.length === 0"><span class="no-items">There are no userstories in this feature</span></li> \
         </draggable>',
    props: ["backlog"],
    data: function () {
        return {
            features: [],
            calculated: false,
            initialized: false,
            userstories: [],
        }
    },
    created: function () {
        // Subscrible to event bus to listen to backlogOpened events
        Events.$on("backlogOpened", this.backlogOpened)
        // Subscrible to event bus to listen to unselectAll events
        Events.$on("unselectAll", this.unselectAll)
    },
    mounted: function () {
        this.initialCalculations();
    },
    methods: {
        collapse: function (event, index) {
            // Prevent default behaviour
            event.stopPropagation();
            this.userstories[index].CollapsedInSprint = !this.userstories[index].CollapsedInSprint;
        },
        select: function (event, index) {
            let selection = this.userstories[index].Selected;

            // Prevent default behaviour
            event.stopPropagation();
            // Broadcast unselectAll event
            Events.$emit("unselectAll", event);

            // Set the selection of clicked item and open metadata card
            this.userstories[index].Selected = !selection;
            MFilesAgile.methods.openMetadataCard(this.userstories[index].InternalId, 102);
        },
        unselectAll: function (event) {
            // Unselect all userstories of feature
            this.userstories.forEach(function (userstory) { userstory.Selected = false; });
        },
        collapseClasses: function (index) {
            // Sets the correct icon for collapsing / uncollapsing
            return this.userstories[index].CollapsedInSprint ? "fa fa-plus-square-o" : "fa fa-minus-square-o";
        },
        backlogOpened: function ( backlogId ) {
            // When backlog is opened get all features / userstories / tasks for it
            if (this.backlog.InternalId == backlogId && this.userstories.length == 0) {
                this.features = MFilesAgile.methods.getSprintFeatures(this.backlog.InternalId);
                for (let i = 0; i < this.features.length; ++i) {
                    Array.prototype.push.apply(this.userstories, this.features[i].UserStoryList);
                }
                this.userstories = this.userstories.sort(function (userstoryA, userstoryB) {
                    return userstoryA.Order - userstoryB.Order;
                });
            }
        },
        initialCalculations: function () {
            // Calculate the progress on userstory level from tasks
            // initial calculations have to be run because otherwise
            // progress will be calculated only after userstory is opened
            let that = this;
            this.userstories.forEach(function (elem, index) {
                that.progress(index);
            });
            this.calculated = true;
        },
        progress: function (index) {
            // Calculate the progress of userstory
            let tasksDone = 0;
            this.userstories[index].TaskList.forEach(function (task) { if (task.TaskState == 4) { tasksDone++; } });
            let progress = (tasksDone / Math.max(this.userstories[index].TaskList.length, 1)) * 100;
            // Show with 0 decimal precision
            return progress.toFixed(0);

        },
        dragAdd: function (evnt) {
            console.log('add item to sprint');
            evnt.item._underlying_vm_.SprintId = this.backlog.InternalId;
        }
    },
    computed: {
        draggableOptions: function () {
            return {
                group: "userstories", // Userstories can be moved only inside userstories and to other userstories
                delay: 0 // Delay from click until drag and drop functionality occurs
            }
        }
        //userstories: function () {
        //    if ( this.features == null ) return [];
        //    var userstoriesBuffer = [];
        //    for ( let i = 0; i < this.features.length; ++i ) {
        //        for ( let j = 0; j < this.features[ i ].UserStoryList.length; ++j ) {
        //            this.features[ i ].UserStoryList[ j ].FeatureId = this.features[ i ].FeatureId;
        //            this.features[ i ].UserStoryList[ j ].FeatureTitle = this.features[ i ].NameOrTitle;
        //        }
        //        Array.prototype.push.apply( userstoriesBuffer, this.features[ i ].UserStoryList );
        //    }
        //    if ( !this.initialized && userstoriesBuffer.length !== 0 ) {
        //        this.initialized = true;
        //        console.log('sort');
        //        userstoriesBuffer = userstoriesBuffer.sort(function (userstoryA, userstoryB) {
        //            return userstoryA.Order - userstoryB.Order;
        //        } );
        //    }
        //    return userstoriesBuffer
        //}
    }
});

Vue.component("sprints", {
    template:
        '<div> \
            <h4 class="content-header">{{ header }} \
                <i class="fa fa-line-chart status"></i> \
                <i class="fa fa-trophy effort"></i> \
            </h4> \
            <draggable class="backlogs planner" element="ul" v-model="backlogs" :options="draggableOptions"> \
                <li @click="select( $event, index )" v-for="( backlog, index ) in backlogs" draggable="true" id="story" :class="{ backlog: true, collapsable: true, collapsed: backlog.Collapsed, selected: backlog.Selected }" > \
                    <i @click="collapse( $event, index )" :class="collapseClasses( index )" aria-hidden="true"></i> \ \
                    <span class="title">{{ backlog.NameOrTitle }}</span> \
                    <span class="status" style="background-image:none;"></span> \
                    <span class="effort"></span> \
                    <sprint-userstories v-on:loaded="attachFeatures" v-bind:backlog="backlog"></sprint-userstories> \
                </li> \
             </draggable> \
         </div>',
    data: function () {
        return {
            backlogs: [],
            header: "Planner"
        }
    },
    props: ["header", "backlogtype"],
    created: function () {
        // When the UI is ready load all the backlogs
        this.backlogs = MFilesAgile.methods.getBacklogs(this.backlogtype)
    },
    methods: {
        collapse: function (event, index) {
            // Prevent default behaviour
            event.stopPropagation();
            this.backlogs[index].Collapsed = !this.backlogs[index].Collapsed;
            if (!this.backlogs[index].Collapsed) {
                // When backlog is opened broadcast backlogOpened event
                Events.$emit("backlogOpened", this.backlogs[index].InternalId);
            }
        },
        select: function (event, index) {
            let selected = !this.backlogs[index].Selected;
        },
        collapseClasses: function (index) {
            // Sets the correct icon for collapsing / uncollapsing
            return this.backlogs[index].Collapsed ? "fa fa-caret-down" : "fa fa-caret-up";
        },
        attachFeatures: function (sum) {
            // This is called when features are rendered calculates the sum of feature points
            // Not currently shown in UI
            sum = 0;
            if (!this.backlogs[index].features) return 0;
            this.backlogs[index].features.forEach(function (feature) { sum += feature.points; });
            return sum;
        }
    },
    computed: {
        draggableOptions: function () {
            return {
                group: "backlogs" // Backlogs can be moved only inside backlog list
            }
        }
    }
});

let initPlanner = function () {
    let app = new Vue( {
        el: "#root",
        data: function () {
            return { backlogs: MFilesAgile.methods.getBacklogs() };
        }
    } )
}