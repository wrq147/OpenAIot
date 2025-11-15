//event related nodes
export function InitEventsNodes(graph) {
    var LiteGraph = graph;


    //Sequence of events
    function Sequence() {
		var that = this;
        this.addInput("", LiteGraph.ACTION);
        this.addInput("", LiteGraph.ACTION);
        this.addInput("", LiteGraph.ACTION);
        this.addOutput("", LiteGraph.EVENT);
        this.addOutput("", LiteGraph.EVENT);
        this.addOutput("", LiteGraph.EVENT);
        this.addWidget("button","+",null,function(){
	        that.addInput("", LiteGraph.ACTION);
	        that.addOutput("", LiteGraph.EVENT);
        });
        this.size = [90, 70];
        this.flags = { horizontal: true, render_box: false };
    }

    Sequence.title = "序列触发";
    Sequence.desc = "当事件到达时触发一系列事件";
    Sequence.prototype.onAction = function(action, param, options) {
        if (this.outputs) {
            options = options || {};
            for (var i = 0; i < this.outputs.length; ++i) {
				var output = this.outputs[i];
				//needs more info about this...
				if( options.action_call ) // CREATE A NEW ID FOR THE ACTION
	                options.action_call = options.action_call + "_seq_" + i;
				else
					options.action_call = this.id + "_" + (action ? action : "action")+"_seq_"+i+"_"+Math.floor(Math.random()*9999);
                this.triggerSlot(i, param, null, options);
                this.boxcolor = "#AEA";
            }
        }
    };

    LiteGraph.registerNodeType("事件/序列触发", Sequence);


    function GraphTimeout(){
        this.addOutput("执行", LiteGraph.EVENT);
        this.addProperty("延时(毫秒)", 0);
        this.widget = this.addWidget("number", "延时(毫秒)", 0, "延时(毫秒)");
        this.size = [200, 60];
    }
    GraphTimeout.prototype.onPropertyChanged = function (name, value) {
        this.widget.value = value;
    }
    GraphTimeout.title = "定时器";
    GraphTimeout.desc = "事件定时触发";
    GraphTimeout.prototype.onAction = function(action, param, options) {
        setTimeout(()=>{
            this.triggerSlot(0, param, null, options);
        },parseInt(this.properties["延时(毫秒)"]));
    }
    LiteGraph.registerNodeType("事件/定时器", GraphTimeout);
}
