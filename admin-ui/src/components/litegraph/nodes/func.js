export function InitFuncNodes(graph) {
    var LiteGraph = graph;

    function MsgOut() {
        this.addProperty("展示文字", "");
        this.strwidget = this.addWidget("text", "展示文字", "", "展示文字");
    }
    MsgOut.title = "展示文字";
    MsgOut.desc = "向客户端发送需要展示的文字信息";
    MsgOut.prototype.onPropertyChanged = function (name, value) {
        this.strwidget.value = value;
    }
    MsgOut.prototype.onGetInputs = function () {
        return [["展示文字", "string"], ["触发器", LiteGraph.ACTION]];
    };

    MsgOut.prototype.onExecute = function () {
        if (this.inputs) {
            for (var i = 0; i < this.inputs.length; i++) {
                var input = this.inputs[i];
                var value = this.getInputData(i);
                if (this.properties.hasOwnProperty(input.name)) {
                    this.properties[input.name] = value;
                }
            }
        }
        this.boxcolor = "#AEA";
        console.log(this.properties.展示的文字);
    };

    LiteGraph.registerNodeType("功能/展示文字", MsgOut);


    function GoUrl() {
        this.addProperty("网址", "");
        this.strwidget = this.addWidget("text", "网址", "", "网址");
    }
    GoUrl.title = "跳转网址";
    GoUrl.desc = "跳转到指定网址";
    GoUrl.prototype.onPropertyChanged = function (name, value) {
        this.strwidget.value = value;
    }
    GoUrl.prototype.onGetInputs = function () {
        return [["网址", "string"], ["触发器", LiteGraph.ACTION]];
    };
    GoUrl.prototype.onExecute = function () {
        if (this.inputs) {
            for (var i = 0; i < this.inputs.length; i++) {
                var input = this.inputs[i];
                var value = this.getInputData(i);
                if (this.properties.hasOwnProperty(input.name)) {
                    this.properties[input.name] = value;
                }
            }
        }
        this.boxcolor = "#AEA";
        console.log(this.properties.网址);
    };

    LiteGraph.registerNodeType("功能/跳转网址", GoUrl);


    function ResetSilenceTime() {
        this.addProperty("事件标识", "");
        this.strwidget = this.addWidget("text", "事件标识", "", "事件标识");
    }
    ResetSilenceTime.title = "重置沉默周期";
    ResetSilenceTime.desc = "重置指定事件的沉默周期";
    ResetSilenceTime.prototype.onPropertyChanged = function (name, value) {
        this.strwidget.value = value;
    }
    ResetSilenceTime.prototype.onGetInputs = function () {
        return [["事件标识", "string"], ["触发器", LiteGraph.ACTION]];
    };
    ResetSilenceTime.prototype.onExecute = function () {
        if (this.inputs) {
            for (var i = 0; i < this.inputs.length; i++) {
                var input = this.inputs[i];
                var value = this.getInputData(i);
                if (this.properties.hasOwnProperty(input.name)) {
                    this.properties[input.name] = value;
                }
            }
        }
        this.boxcolor = "#AEA";
        console.log(this.properties.事件标识);
    };

    LiteGraph.registerNodeType("功能/重置沉默周期", ResetSilenceTime);


    function RefreshPage() {
    }
    RefreshPage.title = "刷新页面";
    RefreshPage.desc = "刷新功能页面";
    RefreshPage.prototype.onExecute = function () {
        this.boxcolor = "#AEA";
    };
    RefreshPage.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("功能/刷新页面", RefreshPage);


    function FuncOut() {
        this.addInput("对象", "object");
        this.addProperty("是否成功", true);
        this.addProperty("错误消息", "");
        this.boolwidget = this.addWidget("toggle", "是否成功", true, "是否成功");
        this.strwidget = this.addWidget("text", "错误消息", "", "错误消息");
        this.size = [180, 80];
    }

    FuncOut.title = "功能回复";
    FuncOut.desc = "向客户端回复功能的执行结果";
    FuncOut.prototype.onGetInputs = function () {
        return [["是否成功", "boolean"], ["错误消息", "string"], ["触发器", LiteGraph.ACTION]];
    };

    FuncOut.prototype.onPropertyChanged = function (name, value) {
        if (name == "是否成功") {
            this.boolwidget.value = value;
        }
        else if (name == "错误消息") {
            this.strwidget.value = value;
        }

    }

    FuncOut.prototype.onExecute = function () {
        if (this.inputs) {
            for (var i = 0; i < this.inputs.length; i++) {
                var input = this.inputs[i];
                var value = this.getInputData(i);
                if (this.properties.hasOwnProperty(input.name)) {
                    this.properties[input.name] = value;
                }
            }
        }
        var iptobj = this.getInputData(0);
        this.boxcolor = "#AEA";
        console.log(JSON.stringify(iptobj));
        console.log(this.properties.是否成功);
        console.log(this.properties.错误消息);
    };

    LiteGraph.registerNodeType("功能/功能回复", FuncOut);


    function FuncParams() {
        this.addOutput("对象", "object");
        this.addProperty("test", "");
        this.strwidget = this.addWidget("text", "模拟数据", "{'key':'value'}", "test");
    }
    FuncParams.prototype.onPropertyChanged = function (name, value) {
        this.strwidget.value = value;
    }
    FuncParams.title = "输入参数";
    FuncParams.desc = "获取功能的执行参数";
    FuncParams.prototype.onExecute = function () {
        this.boxcolor = "#AEA";
        this.setOutputData(0, JSON.parse(this.properties["test"]));
    };
    LiteGraph.registerNodeType("功能/输入参数", FuncParams);


    function PublicWait() {
        this.addInput("消息标识", "string");
        this.addInput("下发数据", "string");
        this.addInput("是否为HEX", "boolean");
        this.addOutput("字符串", "string");
        this.addProperty("test", "");
        this.strwidget = this.addWidget("text", "模拟数据", "", "test");
        this.size = [180, 90];
    }
    PublicWait.prototype.onPropertyChanged = function (name, value) {
        this.strwidget.value = value;
    }
    PublicWait.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    PublicWait.title = "下发消息";
    PublicWait.desc = "推送字符串数据并输出回复的消息";
    PublicWait.prototype.onExecute = function () {
        this.boxcolor = "#AEA";
        this.setOutputData(0, this.properties["test"]);
    };
    LiteGraph.registerNodeType("功能/下发消息", PublicWait);



    function ExecuteFunc() {
        this.addInput("参数对象", "object");
        this.addOutput("对象", "object");
        this.addProperty("通讯编码", "");
        this.addProperty("功能编码", "");
        this.strwidget1 = this.addWidget("text", "通讯编码", "", "通讯编码");
        this.strwidget2 = this.addWidget("text", "功能编码", "", "功能编码");
    }
    ExecuteFunc.title = "执行功能";
    ExecuteFunc.desc = "执行指定设备的功能";
    ExecuteFunc.prototype.onPropertyChanged = function (name, value) {
        if (name == "通讯编码") {
            this.strwidget1.value = value;
        }
        else if (name == "功能编码") {
            this.strwidget2.value = value;
        }

    }
    ExecuteFunc.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    ExecuteFunc.prototype.onExecute = function () {
        if (this.inputs) {
            for (var i = 0; i < this.inputs.length; i++) {
                var input = this.inputs[i];
                var value = this.getInputData(i);
                if (this.properties.hasOwnProperty(input.name)) {
                    this.properties[input.name] = value;
                }
            }
        }
        this.boxcolor = "#AEA";
        console.log(this.properties.通讯编码);
        console.log(this.properties.功能编码);
    };

    LiteGraph.registerNodeType("功能/执行功能", ExecuteFunc);
}
