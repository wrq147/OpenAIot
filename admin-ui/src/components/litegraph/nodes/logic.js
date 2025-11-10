export function InitLogicNodes(graph) {
    var LiteGraph = graph;

    
    function logicAnd(){
        this.properties = { };
        this.addInput("a", "boolean");
        this.addInput("b", "boolean");
        this.addOutput("布尔值", "boolean");
    }
    logicAnd.title = "与操作";
    logicAnd.desc = "如果所有输入均为真，则返回真";
    logicAnd.prototype.onExecute = function() {
        var ret = true;
        for (var inX in this.inputs){
            if (!this.getInputData(inX)){
                ret = false;
                break;
            }
        }
        this.boxcolor = "#AEA";
        this.setOutputData(0, ret);
    };
    logicAnd.prototype.onGetInputs = function() {
        return [["与", "boolean"],["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("逻辑/与操作", logicAnd);
    
    
    function logicOr(){
        this.properties = { };
        this.addInput("a", "boolean");
        this.addInput("b", "boolean");
        this.addOutput("布尔值", "boolean");
    }
    logicOr.title = "或操作";
    logicOr.desc = "如果至少有一个输入为真，则返回真";
    logicOr.prototype.onExecute = function() {
        var ret = false;
        for (var inX in this.inputs){
            if (this.getInputData(inX)){
                ret = true;
                break;
            }
        }
        this.boxcolor = "#AEA";
        this.setOutputData(0, ret);
    };
    logicOr.prototype.onGetInputs = function() {
        return [["或", "boolean"],["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("逻辑/或操作", logicOr);
    
    
    function logicNot(){
        this.properties = { };
        this.addInput("布尔值", "boolean");
        this.addOutput("布尔值", "boolean");
    }
    logicNot.title = "非操作";
    logicNot.desc = "返回否定逻辑";
    logicNot.prototype.onExecute = function() {
        var ret = !this.getInputData(0);
        this.boxcolor = "#AEA";
        this.setOutputData(0, ret);
    };
    logicNot.prototype.onGetInputs = function() {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("逻辑/非操作", logicNot);
    
    
    function logicCompare(){
        this.properties = { };
        this.addInput("a", "boolean");
        this.addInput("b", "boolean");
        this.addOutput("布尔值", "boolean");
    }
    logicCompare.title = "a == b";
    logicCompare.desc = "逻辑相等比较";
    logicCompare.prototype.onExecute = function() {
        var tmpa=this.getInputData(0);
        var tmpb=this.getInputData(1);
        this.boxcolor = "#AEA";
        this.setOutputData(0, tmpa==tmpb);
    };
    logicCompare.prototype.onGetInputs = function() {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("逻辑/相等判断", logicCompare);



    
    
    function GenericCompare() {
        this.addInput("A数值", "number");
        this.addInput("B数值", "number");
        this.addOutput("布尔值", "boolean");
        this.addProperty("OP", "等于", "enum", { values: GenericCompare.values });
		this.widget = this.addWidget("combo","Op.",this.properties.OP,{ property: "OP", values: GenericCompare.values } );

        this.size = this.computeSize();
    }

    GenericCompare.values = ["大于", "小于", "等于", "不等于", "小于等于", "大于等于"];
    GenericCompare.title = "数值比较";
    GenericCompare.desc = "输出A和B之间的比较结果";
    GenericCompare.prototype.onPropertyChanged = function (name, value) {
        this.widget.value = value;
    }
    GenericCompare.prototype.getTitle = function() {
        return "A值 " + this.properties.OP + " B值";
    };

    GenericCompare.prototype.onExecute = function() {
        var A = this.getInputData(0);
        var B = this.getInputData(1);
        var result = false;
        switch (this.properties.OP) {
            case "等于":
                result = A == B;
                break;
            case "不等于":
                result = A != B;
                break;
            case "大于":
                result = A > B;
                break;
            case "小于":
                result = A < B;
                break;
            case "小于等于":
                result = A <= B;
                break;
            case "大于等于":
                result = A >= B;
                break;
        }
        this.boxcolor = "#AEA";
        this.setOutputData(0, result);
    };
    GenericCompare.prototype.onGetInputs = function() {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("逻辑/数值比较", GenericCompare);
    
    
    function logicBranch(){
        this.properties = { };
        this.addInput("布尔值", "boolean");
        this.addOutput("为真", LiteGraph.EVENT);
        this.addOutput("为假", LiteGraph.EVENT);
    }
    logicBranch.title = "条件分支";
    logicBranch.desc = "根据条件执行分支";
    logicBranch.prototype.onExecute = function(param, options) {
        var condtition = this.getInputData(1);
        this.boxcolor = "#AEA";
        if (condtition){
            this.triggerSlot(0);
        }else{
            this.triggerSlot(1);
        }
    };
    logicBranch.prototype.onGetInputs = function() {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("逻辑/条件分支", logicBranch);



    
}
