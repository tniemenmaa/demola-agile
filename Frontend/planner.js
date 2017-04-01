window.addEventListener('keydown', onKeyDown, true);
window.addEventListener('keyup', onKeyUp, true);

window.keyDown = {};

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
    get() {
        return this.$root.bus;
    }
});

// Components (TODO: Separate to different .js files)
Vue.component('tasks', {
    template:
        '<draggable class="tasks" element="ul" v-model="userstory.tasks"> \
            <li @click="select($event, index)" v-for="(task, index) in userstory.tasks" draggable="true" :class="{ task: true, collapsable: true, collapsed: task.collapsed, selected: task.selected }" id="story"> \
                <span class="group">TASK</span>  \
                <span class="title">{{ task.name }}</span> \
            </li> \
            <li @click="select($event, null)" v-if="!userstory.tasks || userstory.tasks.length === 0"><span class="no-items">There are no tasks in this userstory</span></li> \
         </draggable>',
    props: ['userstory'],
    ready: function () {
        this.$bus.$on('select')
    },
    methods: {
        select: function (event, index) {
            event.stopPropagation();
            if (!index) return;

            this.userstory.tasks[index].selected = !this.userstory.tasks[index].selected;
        }
    }
});

Vue.component('userstories', {
    template:
        '<draggable class="userstories" element="ul" v-model="feature.userstories" :options="draggableOptions"> \
            <li @click="select($event, index)" v-for="(userstory, index) in feature.userstories" draggable="true" :class="{ userstory: true, collapsable: true, collapsed: userstory.collapsed, selected: userstory.selected }" id="story"> \
                <i @click="collapse($event, index)" :class="collapseClasses(index)" aria-hidden="true"></i> \
                <span class="group">USERSTORY</span>  \
                <span class="title">{{ userstory.name }}</span> \
                <tasks v-bind:userstory="userstory"></tasks> \
            </li> \
            <li v-if="!feature.userstories || feature.userstories.length === 0"><span class="no-items">There are no userstories in this feature</span></li> \
         </draggable>',
    props: ['feature'],
    methods: {
        collapse: function (event, index) {
            event.stopPropagation();
            this.feature.userstories[index].collapsed = !this.feature.userstories[index].collapsed;
        },
        select: function (event, index) {
            event.stopPropagation();
            this.feature.userstories[index].selected = !this.feature.userstories[index].selected;
        },
        collapseClasses: function (index) {
            return this.feature.userstories[index].collapsed ? "fa fa-plus-square-o" : 'fa fa-minus-square-o';
        }
    },
    computed: {
        draggableOptions() {
            return {
                group: 'userstories',
                ghostClass: 'ghost'
            }
        }
    }
});

Vue.component('features', {
    template:
        '<draggable class="features" element="ul" v-model="features" :options="draggableOptions"> \
            <li @click="select($event, index)" v-for="(feature, index) in features" draggable="true" :class="{ feature: true, collapsable: true, collapsed: feature.collapsed, selected: feature.selected }" id="story"> \
                <i @click="collapse($event, index)" :class="collapseClasses(index)" aria-hidden="true"></i> \
                <span class="group">FEATURE</span>  \
                <span class="title">{{ feature.name }}</span> \
                <userstories v-bind:feature="feature"></userstories> \
            </li> \
            <li v-if="!features || features.length === 0"><span class="no-items">There are no features in this backlog</span></li> \
         </draggable>',
    props: ['backlog'],
    data: function () {
        return {
            // TODO: Move this to methods that is called when backlog is expanded
            features: MFiles.methods.getFeatures(this.backlog.id),
            lastSelection: null
        };
    },
    methods: {
        collapse: function (event, index) {
            event.stopPropagation();
            this.features[index].collapsed = !this.features[index].collapsed;
        },
        select: function (event, index) {
            event.stopPropagation();
            var selected = !this.features[index].selected;
            if (!window.keyDown.ctrl) {
                this.features.forEach(function(el) { el.selected = false })
            }

            if (window.keyDown.shift && this.lastSelection) {
                var self = this;
                var lastIndex = this.features.findIndex(function (el) { return el.id === self.lastSelection.id });
                for (var i = Math.min(lastIndex, index) ; i < Math.max(lastIndex, index) ; ++i) {
                    this.features[i].selected = true;
                }
            }

            this.features[index].selected = selected;
            this.lastSelection = this.features[index];
        },
        collapseClasses: function (index) {
            return this.features[index].collapsed ? "fa fa-plus-square-o" : 'fa fa-minus-square-o';
        }
    },
    computed: {
        draggableOptions() {
            return {
                group: 'features',
                ghostClass: 'hidden'
            }
        }
    }
});

Vue.component('backlogs', {
    template:
        '<div> \
            <h4 class="content-header">{{ header }}</h4> \
            <draggable class="backlogs planner" element="ul" v-model="backlogs" :options="draggableOptions"> \
                <li @click="select($event, index)" v-for="(backlog, index) in backlogs" draggable="true" id="story" :class="{ backlog: true, collapsable: true, collapsed: backlog.collapsed, selected: backlog.selected }" > \
                    <i @click="collapse($event, index)" :class="collapseClasses(index)" aria-hidden="true"></i> \ \
                    <span class="title">{{ backlog.name }}</span> \
                    <features v-bind:backlog="backlog"></features> \
                </li> \
             </draggable> \
         </div>',
    data: function () { 
        return {
            backlogs: MFiles.methods.getBacklogs(),
            lastSelection: null,
            header: 'Planner'
        }
    },
    props: ['header'],
    methods: {
        collapse: function (event, index) {
            event.stopPropagation();
            this.backlogs[index].collapsed = !this.backlogs[index].collapsed;
        },
        select: function (event, index) {
            var selected = !this.backlogs[index].selected;
            if (!window.keyDown.ctrl) {
                this.backlogs.forEach(function (el) { el.selected = false })
            }

            if (window.keyDown.shift && this.lastSelection) {
                var self = this;
                var lastIndex = this.backlogs.findIndex(function (el) { return el.id === self.lastSelection.id });
                console.log('index: ' + index + ', lastIndex: ' + lastIndex);
                for (var i = Math.min(lastIndex, index) ; i <= Math.max(lastIndex, index) ; ++i) {
                    this.backlogs[i].selected = true;
                }
            }
            else {
                this.backlogs[index].selected = selected;
            }
            this.lastSelection = this.backlogs[index];
        },
        collapseClasses: function (index) {
            return this.backlogs[index].collapsed ? "fa fa-plus-square-o" : 'fa fa-minus-square-o';
        }
    },
    computed: {
        draggableOptions () {
            return {
                group: 'backlogs',
                ghostClass: 'ghost'
            }
        }
    }
});

var app = new Vue({ 
    el: '#root',
    data: function () {
        return { backlogs: MFiles.methods.getBacklogs() };
    }
})



