(function() {
    "use strict";

    function buildDraggable(Sortable) {
        function removeNode (node) {
            node.parentElement.removeChild(node)
        }

        function insertNodeAt (fatherNode, node, position) {
            if (position < fatherNode.children.length) {
                fatherNode.insertBefore(node, fatherNode.children[position])
            } else {
                fatherNode.appendChild(node)
            }
        }

        function computeVmIndex (vnodes, element) {
            /*return vnodes.map(elt => elt.elm).indexOf(element)*/
            return vnodes.map(function(elt) { return elt.elm }).indexOf(element)
        }

        function computeIndexes (slots, children) {
            /*return (!slots)? [] : Array.prototype.map.call(children, elt => computeVmIndex(slots, elt))*/
            return (!slots)? [] : Array.prototype.map.call(children, function(elt) { return computeVmIndex(slots, elt)})
        }

        function emit (evtName, evtData) {
            this.$emit( evtName.toLowerCase(), evtData)
        }

        function delegateAndEmit (evtName) {
            /*return (evtData) => {
                if (this.realList!==null) {
                    this['onDrag' + evtName](evtData)
                }
                emit.call(this, evtName, evtData)
            }*Y
                        /*return (evtData) => { */
            var that = this;
            return function(evtData) {
                if (that.realList!==null) {
                    that['onDrag' + evtName](evtData)
                }
                emit.call(that, evtName, evtData)
            }
        }

        const eventsListened = ['Start', 'Add', 'Remove', 'Update', 'End']
        const eventsToEmit = ['Choose', 'Sort', 'Filter', 'Clone']
        /*const readonlyProperties = ['Move', ...eventsListened, ...eventsToEmit].map(evt => 'on'+evt)*/
        const readonlyProperties = ['Move', 'Start', 'Add', 'Remove', 'Update', 'End', 'Choose', 'Sort', 'Filter', 'Clone'].map(function(evt) { return 'on'+evt })
        var draggingElement = null
  
        const props = {
            options: Object,
            list: { 
                type: Array,
                required: false,
                default: null
            },
            value: { 
                type: Array,
                required: false,
                default: null
            },
            clone: {
                type: Function,
                /*default : (original) => { return original;}*/
                default : function(original) { return original;}
            },
            element: {
                type: String,
                default: 'div'
            },
            move: {
                type: Function,
                default: null
            }
        }

        const draggableComponent = {
            props: props,

            data: function() {
                return {
                    transitionMode: false
                }
            },

            render: function(h) {
                if (this.$slots.default && this.$slots.default.length===1) {
                    const child = this.$slots.default[0]
                    if (child.componentOptions && child.componentOptions.tag==="transition-group") { 
                        this.transitionMode = true
                    }
                }
                return h(this.element, null, this.$slots.default);
            },

            mounted: function() {
                var optionsAdded = {};
                //eventsListened.forEach( elt => {
                //    optionsAdded['on' + elt] = delegateAndEmit.call(this, elt)
                //});
                var that = this;
                eventsListened.forEach( function(elt) {
                    optionsAdded['on' + elt] = delegateAndEmit.call(that, elt)
                });

                //eventsToEmit.forEach( elt => {
                //    optionsAdded['on' + elt] = emit.bind(this, elt)
                //});
                eventsToEmit.forEach( function(elt) {
                    optionsAdded['on' + elt] = emit.bind(that, elt)
                });

                //const options = Object.assign({}, this.options, optionsAdded, { onMove: evt => {return this.onDragMove(evt);} })
                const options = Object.assign({}, this.options, optionsAdded, { onMove:function(evt) {return that.onDragMove(evt);} })
                this._sortable = new Sortable(this.rootContainer, options)
                this.computeIndexes()
            },

            beforeDestroy: function() {
                this._sortable.destroy()
            },

            computed : {
                rootContainer: function () {
                    return this.transitionMode? this.$el.children[0] : this.$el;
                },

                isCloning: function () {
                    return (!!this.options) && (!!this.options.group !== null) && (this.options.group.pull === 'clone')
                },

                realList: function () {
                    return (!!this.list) ? this.list : this.value;
                }
            },

            watch: {
                options: function (newOptionValue){
                    for(var property in newOptionValue) {
                        if (readonlyProperties.indexOf(property)==-1) {
                            this._sortable.option(property, newOptionValue[property] );
                        }        
                    }         
                },

                realList: function () {
                    this.computeIndexes()
                }
            },

            methods: {
                getChildrenNodes: function () {
                    const rawNodes = this.$slots.default
                    return this.transitionMode? rawNodes[0].child.$slots.default : rawNodes
                },

                computeIndexes: function () {
                    var that = this;
                    //this.$nextTick( () => {
                    //    this.visibleIndexes = computeIndexes(this.getChildrenNodes(), this.rootContainer.children)
                    //})
                    this.$nextTick( function() {
                        that.visibleIndexes = computeIndexes(that.getChildrenNodes(), that.rootContainer.children)
                    })
                },

                getUnderlyingVm: function (htmlElt) {
                    const index = computeVmIndex(this.getChildrenNodes(), htmlElt)
                    const element = this.realList[index]
                    return {index: index, element: element}
                },

                getUnderlyingPotencialDraggableComponent: function (obj) {
                    var __vue__ = obj.__vue__;
                if (!__vue__ || !__vue__.$options || __vue__.$options._componentTag!=="transition-group"){
              return __vue__
    }
    return __vue__.$parent
},

        emitChanges: function (evt) {
          var that = this;
          this.$nextTick( function() {
            that.$emit('change', evt)
});       
},

alterList: function (onList) {
    if (!!this.list) {
        onList(this.list)
    }
    else {
        //const newList = [...this.value]
        const newList = this.value.slice(0)
        onList(newList)
        this.$emit('input', newList)
    }  
},

        spliceList: function () {
            //const spliceList = function(list) { return list.splice(...arguments) }
            const spliceList = function(list) { return list.splice(Array.from(arguments)) }
            this.alterList(spliceList)
        },

        updatePosition: function (oldIndex, newIndex) {
            const updatePosition = function(list) { return list.splice(newIndex, 0, list.splice(oldIndex, 1)[0]) }
            this.alterList(updatePosition)
        },

        getRelatedContextFromMoveEvent: function (obj) {
            var to = obj.to;
            var related = obj.related;
            const component = this.getUnderlyingPotencialDraggableComponent(to)
            if (!component) {
                return {component: component}
            }
            const list = component.realList
            const context = {list: list, component: component}
            if (to !== related && list && component.getUnderlyingVm) {
                const destination = component.getUnderlyingVm(related)
                return Object.assign(destination, context)
            }

            return context
        },

        getVmIndex: function (domIndex) {
            const indexes = this.visibleIndexes
            const numberIndexes = indexes.length
            return (domIndex > numberIndexes - 1) ? numberIndexes : indexes[domIndex]
        },

        onDragStart: function (evt) {
            this.context = this.getUnderlyingVm(evt.item)
            evt.item._underlying_vm_ = this.clone(this.context.element)
            draggingElement = evt.item
        },

        onDragAdd: function (evt) {
            const element = evt.item._underlying_vm_
            if (element === undefined) {
                return
            }
            removeNode(evt.item)
            const newIndex = this.getVmIndex(evt.newIndex)
            this.spliceList(newIndex, 0, element)
            this.computeIndexes()
            const added = {element: element, newIndex: newIndex}
            this.emitChanges({added: added})
        },

        onDragRemove: function (evt) {
            insertNodeAt(this.rootContainer, evt.item, evt.oldIndex)
            if (this.isCloning) {
                removeNode(evt.clone)
                return
            }
            const oldIndex = this.context.index
            this.spliceList(oldIndex, 1)
            const removed = {element: this.context.element, oldIndex: oldIndex}
            this.emitChanges({removed: removed})
        },

        onDragUpdate: function (evt) {
            removeNode(evt.item)
            insertNodeAt(evt.from, evt.item, evt.oldIndex)
            const oldIndex = this.context.index
            const newIndex = this.getVmIndex(evt.newIndex)
            this.updatePosition(oldIndex, newIndex)
            const moved = {element: this.context.element, oldIndex: oldIndex, newIndex: newIndex}
            this.emitChanges({moved: moved})
        },

        computeFutureIndex: function (relatedContext, evt) {
            if (!relatedContext.element){
                return 0
            }
            //const domChildren = [...evt.to.children]
            const domChildren = evt.to.children.slice(0)
            const currentDOMIndex = domChildren.indexOf(evt.related)
            const currentIndex = relatedContext.component.getVmIndex(currentDOMIndex)
            const draggedInList = domChildren.indexOf(draggingElement) != -1
            return draggedInList? currentIndex : currentIndex+1
        },

        onDragMove: function (evt) {
            const onMove = this.move
            if (!onMove || !this.realList) {
                return true
            }

            const relatedContext = this.getRelatedContextFromMoveEvent(evt)
            const draggedContext = this.context
            const futureIndex = this.computeFutureIndex(relatedContext, evt)
            Object.assign(draggedContext, { futureIndex: futureIndex })
            Object.assign(evt, {relatedContext: relatedContext, draggableContext: draggedContext})
            return onMove(evt)
        },

        onDragEnd: function (evt) {
            this.computeIndexes()
            draggingElement = null
        }
}
}    
return draggableComponent
}

if (typeof exports == "object") {
    var Sortable =  require("sortablejs")
    module.exports = buildDraggable(Sortable)
} else if (typeof define == "function" && define.amd) {
    define(['sortablejs'], function(Sortable) { return buildDraggable(Sortable);});
} else if ( window && (window.Vue) && (window.Sortable)) {
    var draggable = buildDraggable(window.Sortable)
    Vue.component('draggable', draggable)
}
})();