var initPlanner = function() {
window.addEventListener('keydown', onKeyDown, true);
window.addEventListener('keyup', onKeyUp, true);

window.keyDown = {};

// Key event handling for ctrl and shift click functionality 
function onKeyDown(e) {

    if (e.keyCode == 17) { // Ctrl
        window.keyDown.ctrl = true;
    }

    if (e.keyCode == 16) { // Shift
        window.keyDown.shift = true;
    }
}

function onKeyUp(e) {
    if (e.keyCode == 17) { // Ctrl
        window.keyDown.ctrl = false;
    }

    if (e.keyCode == 16) { // Shift
        window.keyDown.shift = false;
    }
}

// Create a global event bus
Object.defineProperty(Vue.prototype, '$bus', {
    get: function() {
        return this.$root.bus;
    }
});

var Events = new Vue({});

Vue.component('progressbar', {

    props: ["value"],
    computed: {
        precentage: function() {
            return this.value + " %";
        },
        barClass: function () {
            return "progress-bar style-" + Math.floor(this.value / 10) * 10;
        },
        barWidth: function() {
            return "width: " + this.value + "%"; 
        }
    },
    template: '<div class="progress status"> \
                    <div :class="barClass" role="progressbar" :style="barWidth"> \
                        <span>{{ this.precentage }}</span> \
                    </div> \
                </div>'
})

// Components (TODO: Separate to different .js files)
Vue.component('tasks', {
    template:
        '<draggable class="tasks" element="ul" v-model="userstory.TaskList" :options="draggableOptions"> \
            <li @click="select($event, index)" v-for="(task, index) in userstory.TaskList" draggable="true" :class="{ task: true, collapsable: true, collapsed: task.Collapsed, selected: task.Selected }" id="story"> \
                <i class="fa fa-square-o"></i> \
                <span class="group"></span>  \
                <span class="title">{{ task.NameOrTitle }}</span> \
                <span class="status">{{ task.State }}</span> \
            </li> \
            <li @click="select($event, null)" v-if="!userstory.TaskList || userstory.TaskList.length === 0"><span class="no-items">There are no tasks in this userstory</span></li> \
         </draggable>',
    props: ['userstory'],
    created: function () {
        Events.$on('unselectAll', this.unselectAll);
    },
    methods: {
        select: function (event, index) {
            event.stopPropagation();

            var selection = this.userstory.TaskList[index].Selected;

            Events.$emit('unselectAll', event);

            this.userstory.TaskList[index].Selected = !selection;

            MFilesAgile.methods.openMetadataCard(this.userstory.TaskList[index].InternalId, 103);
        },
        unselectAll: function (event) {
            this.userstory.TaskList.forEach(function (task) { task.Selected = false; });
        },
        status: function (index) {
            var code = this.userstory.TaskList[index].TaskState;
            if (code == 1) {
                return "Not Started";
            }
            else if (code == 2) {
                return "In Progress";
            }
            else if (code == 3) {
                return "Impeded";
            }
            else if (code == 4) {
                return "Done";
            }
            return "N/A";
        }
    },
    computed: {
        draggableOptions: function () {
            return {
                group: 'tasks',
                delay: 100
            }
        }
    }
});

Vue.component('userstories', {
    template:
        '<draggable class="userstories" element="ul" v-model="feature.UserStoryList" :options="draggableOptions"> \
            <li @click="select($event, index)" v-for="(userstory, index) in feature.UserStoryList" draggable="true" :class="{ userstory: true, collapsable: true, collapsed: userstory.Collapsed, selected: userstory.Selected }" id="story"> \
                <i @click="collapse($event, index)" :class="collapseClasses(index)" aria-hidden="true"></i> \
                <span class="group">{{ userstory.UserStoryId }}</span>  \
                <span class="title">{{ userstory.NameOrTitle }}</span> \
                <progressbar class="" v-bind:value="progress(index)"></progressbar> \
                <span class="effort">{{ userstory.StoryPoints }}</span> \
                <tasks v-bind:userstory="userstory"></tasks> \
            </li> \
            <li v-if="!feature.UserStoryList || feature.UserStoryList.length === 0"><span class="no-items">There are no userstories in this feature</span></li> \
         </draggable>',
    props: ['feature'],
    data: function()  {
        return {
            calculated: false,
        }
    },
    created: function () {
        Events.$on('unselectAll', this.unselectAll);
    },
    mounted: function() {
        this.initialCalculations();
    },
    methods: {
        collapse: function (event, index) {
            event.stopPropagation();
            this.feature.UserStoryList[index].Collapsed = !this.feature.UserStoryList[index].Collapsed;
        },
        select: function (event, index) {
            var selection = this.feature.UserStoryList[index].Selected;
            Events.$emit('unselectAll', event);
            event.stopPropagation();
            this.feature.UserStoryList[index].Selected = !selection;
            MFilesAgile.methods.openMetadataCard(this.feature.UserStoryList[index].InternalId, 102);
        },
        unselectAll: function (event) {
            this.feature.UserStoryList.forEach(function (userstory) { userstory.Selected = false; });
        },
        collapseClasses: function (index) {
            return this.feature.UserStoryList[index].Collapsed ? "fa fa-plus-square-o" : 'fa fa-minus-square-o';
        },
        initialCalculations: function () {
            var that = this;
            this.feature.UserStoryList.forEach(function (elem, index) {
                that.progress(index);
            });
            this.calculated = true;
        },
        progress: function (index) {
            //var value = Math.round(Math.random() * 100); 
            //if (!this.calculated) {
            //    this.feature.UserStoryList[index].status = value;
            //    this.$emit('update', { featureId: this.feature.id, userstory: this.feature.UserStoryList[index] });
            //}
            //return value;
            var tasksDone = 0;
            //alert(JSON.stringify(this.feature.UserStoryList[index].TaskList));
            this.feature.UserStoryList[index].TaskList.forEach(function (task) { if (task.TaskState == 4) { tasksDone++; } });
            var progress = (tasksDone / Math.max(this.feature.UserStoryList[index].TaskList.length, 1)) * 100;
            return progress.toFixed(0);
            
        }
    },
    computed: {
        draggableOptions: function() {
            return {
                group: 'userstories',
                delay: 100
            }
        }
    }
});
Vue.component('feature', {
    props: ['feature', 'index'],
    methods: {
        select: function (event, index) {
            var selection = this.feature.Selected;
            Events.$emit('unselectAll', event);
            event.stopPropagation();          
            this.feature.Selected = !selection;
            MFilesAgile.methods.openMetadataCard(this.feature.InternalId, 106);
        },
        collapse: function (event, index) {
            event.stopPropagation();
            this.feature.Collapsed = !this.feature.Collapsed;
        },
        collapseClasses: function (index) {
            return this.feature.Collapsed ? "fa fa-plus-square-o" : 'fa fa-minus-square-o';
        },
        storypointSum: function (index) {
            var sum = 0;
            this.feature.UserStoryList.forEach(function (story) { sum += story.StoryPoints; });
            return sum;
        },
        progress: function (index) {

            var sum = 0;
            this.feature.UserStoryList.forEach(function (story) {
                sum += story.TaskList.reduce(function (taskA, taskB) {
                    return taskA + (taskB.TaskState == 4 ? 1 : 0);
                }, 0) / Math.max(story.TaskList.length, 1)
            });
            return parseFloat((sum / this.feature.UserStoryList.length) * 100).toFixed(0);
        },
        featureUpdate: function (event) {

        }
    },
    template: '<li draggable="true" @click="select" :class="{ feature: true, collapsable: true, collapsed: feature.Collapsed, selected: feature.Selected }" id="story"> \
                    <i @click="collapse($event, index)" :class="collapseClasses(index)" aria-hidden="true"></i> \
                    <span class="group"></span>  \
                    <span class="title">{{ feature.NameOrTitle }}</span> \
                    <progressbar class="" v-bind:value="progress(index)"></progressbar> \
                    <span class="effort">{{ storypointSum(index) }}</span> \
                    <userstories v-on:update="featureUpdate" v-bind:feature="feature"></userstories> \
              </li>'
})
Vue.component('features', {
    template:
        '<draggable class="features" element="ul" v-model="features" :options="draggableOptions"> \
            <feature v-for="(feature, index) in features" v-bind:feature="feature" v-bind:index="index"></feature> \
            <li class="feature" v-if="!features || features.length === 0"><span class="no-items">There are no features in this backlog</span></li> \
         </draggable>',
    props: ['backlog'],
    data: function () {
        return {
            // TODO: Move this to methods that is called when backlog is expanded
            features: [],
            lastSelection: null
        };
    },
    created: function () {
        Events.$on('backlogOpened', this.backlogOpened)
        Events.$on('unselectAll', this.unselectAll)
    },
    methods: {
        unselectAll: function(event) {
            this.features.forEach(function (feature) { feature.Selected = false; })
        },
        collapse: function (event, index) {
            event.stopPropagation();
            this.features[index].Collapsed = !this.features[index].Collapsed;
        },
        collapseClasses: function (index) {
            return this.features[index].Collapsed ? "fa fa-plus-square-o" : 'fa fa-minus-square-o';
        },
        storypointSum: function (index) {
            var sum = 0;
            this.features[index].userstories.forEach(function (story) { sum += story.points; });
            return sum;
        },
        progress: function (index) {
            alert(JSON.stringify(this.userstories));
            var sum = 0;
            this.features[index].userstories.forEach(function (story) { alert('story.Progress ' + story.Progress); sum += story.Progress ? story.Progress : 0 });
            return parseFloat(sum / this.features[index].userstories.length).toFixed(0); 
        },
        featureUpdate: function (event) {
            var featureIndex = this.features.findIndex(function (feature) { return feature.id == event.featureId });
            this.features[featureIndex].userstories.find(function (story) { return story.id === event.userstory.id }).status = event.userstory.status;
        },
        backlogOpened: function (backlogId) {
            if (this.backlog.InternalId == backlogId && this.features.length == 0) {
                this.features = MFilesAgile.methods.getFeatures(this.backlog.InternalId);

            }
        },
        openMetadataCard: function () {
            MFilesAgile.methods.openMetadataCard(106);
        }
    },
    computed: {
        draggableOptions: function() {
            return {
                group: 'features',
                delay: 100,
            }
        }
    }
});

Vue.component('backlogs', {
    template:
        '<div> \
            <h4 class="content-header">{{ header }} \
                <i class="fa fa-line-chart status"></i> \
                <i class="fa fa-trophy effort"></i> \
            </h4> \
            <draggable class="backlogs planner" element="ul" v-model="backlogs" :options="draggableOptions"> \
                <li @click="select($event, index)" v-for="(backlog, index) in backlogs" draggable="true" id="story" :class="{ backlog: true, collapsable: true, collapsed: backlog.Collapsed, selected: backlog.Selected }" > \
                    <i @click="collapse($event, index)" :class="collapseClasses(index)" aria-hidden="true"></i> \ \
                    <span class="title">{{ backlog.NameOrTitle }}</span> \
                    <span class="status" style="background-image:none;"></span> \
                    <span class="effort"></span> \
                    <features v-on:loaded="attachFeatures" v-bind:backlog="backlog"></features> \
                </li> \
             </draggable> \
         </div>',
    data: function () { 
        return {
            backlogs: [],
            lastSelection: null,
            header: 'Planner'
        }
    },
    props: ['header', 'backlogtype'],
    created: function() {
        this.backlogs = MFilesAgile.methods.getBacklogs(this.backlogtype)
    },
    methods: {
        collapse: function (event, index) {
            event.stopPropagation();
            this.backlogs[index].Collapsed = !this.backlogs[index].Collapsed;
            if (!this.backlogs[index].Collapsed) {
                Events.$emit('backlogOpened', this.backlogs[index].InternalId);
            }
        },
        select: function (event, index) {
            var selected = !this.backlogs[index].Selected;
            if (!window.keyDown.ctrl) {
                this.backlogs.forEach(function (el) { el.Selected = false })
            }

            if (window.keyDown.shift && this.lastSelection) {
                var self = this;
                var lastIndex = this.backlogs.findIndex(function (el) { return el.id === self.lastSelection.id });
                for (var i = Math.min(lastIndex, index) ; i <= Math.max(lastIndex, index) ; ++i) {
                    this.backlogs[i].Selected = true;
                }
            }
            else {
                this.backlogs[index].Selected = selected;
            }
            this.lastSelection = this.backlogs[index];
        },
        collapseClasses: function (index) {
            return this.backlogs[index].Collapsed ? "fa fa-caret-down" : 'fa fa-caret-up';
        },
        attachFeatures: function (sum) {
            var sum = 0;
            if (!this.backlogs[index].features) return 0;

            this.backlogs[index].features.forEach(function (feature) { sum += feature.points; });
            return sum;
        }
    },
    computed: {
        draggableOptions: function () {
            return {
                group: 'backlogs'
            }
        }
    }
});

Vue.component('Sprints', {
    template: 
        '<div> \
            <h4 class="content-header">{{ header }} \
                <i class="fa fa-line-chart status"></i> \
                <i class="fa fa-trophy effort"></i> \
            </h4> \
            <draggable class="backlogs planner" element="ul" v-model="backlogs" :options="draggableOptions"> \
                <li @click="select($event, index)" v-for="(backlog, index) in backlogs" draggable="true" id="story" :class="{ backlog: true, collapsable: true, collapsed: backlog.Collapsed, selected: backlog.Selected }" > \
                    <i @click="collapse($event, index)" :class="collapseClasses(index)" aria-hidden="true"></i> \ \
                    <span class="title">{{ backlog.NameOrTitle }}</span> \
                    <span class="status" style="background-image:none;"></span> \
                    <span class="effort"></span> \
                    <features v-on:loaded="attachFeatures" v-bind:backlog="backlog"></features> \
                </li> \
             </draggable> \
         </div>',
    prop: ['sprint'],


})
var app = new Vue({ 
    el: '#root',
    data: function () {
        return { backlogs: MFilesAgile.methods.getBacklogs() };
    }
})
}