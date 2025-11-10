export function InitBaseNodes(graph) {
    var LiteGraph = graph;

    //Constant
    function ConstantNumber() {
        this.addOutput("数值", "number");
        this.addProperty("value", 0);
        this.widget = this.addWidget("number", "value", 0, "value");
        this.size = [180, 55];
    }

    ConstantNumber.title = "数值";
    ConstantNumber.desc = "创建数值常量";

    ConstantNumber.prototype.onExecute = function () {
        this.boxcolor = "#AEA";
        this.setOutputData(0, parseFloat(this.properties["value"]));
    };
    ConstantNumber.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    ConstantNumber.prototype.getTitle = function () {
        if (this.flags.collapsed) {
            return this.properties.value;
        }
        return this.title;
    };
    ConstantNumber.prototype.onPropertyChanged = function (name, value) {
        this.widget.value = value;
    }

    ConstantNumber.prototype.onDrawBackground = function (ctx) {
        //show the current value
        this.outputs[0].label = this.properties["value"].toFixed(3);
    };

    LiteGraph.registerNodeType("常用/数值", ConstantNumber);

    function ConstantBoolean() {
        this.addOutput("布尔值", "boolean");
        this.addProperty("value", true);
        this.widget = this.addWidget("toggle", "value", true, "value");
        this.serialize_widgets = true;
        this.size = [140, 55];
    }

    ConstantBoolean.title = "布尔值";
    ConstantBoolean.desc = "创建布尔值常量";
    ConstantBoolean.prototype.getTitle = ConstantNumber.prototype.getTitle;

    ConstantBoolean.prototype.onExecute = function () {
        this.boxcolor = "#AEA";
        this.setOutputData(0, this.properties["value"]);
    };


    ConstantBoolean.prototype.onPropertyChanged = function (name, value) {
        this.widget.value = value;
    }
    ConstantBoolean.prototype.onAction = function (action) {
        this.setValue(!this.properties.value);
    }
    ConstantBoolean.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("常用/布尔值", ConstantBoolean);

    function ConstantString() {
        this.addOutput("字符串", "string");
        this.addProperty("value", "");
        this.widget = this.addWidget("text", "value", "", "value");  //link to property value
        this.size = [180, 55];
    }

    ConstantString.title = "字符串";
    ConstantString.desc = "创建字符串常量";

    ConstantString.prototype.getTitle = ConstantNumber.prototype.getTitle;

    ConstantString.prototype.onExecute = function () {
        this.boxcolor = "#AEA";
        this.setOutputData(0, this.properties["value"]);
    };
    ConstantString.prototype.onPropertyChanged = function (name, value) {
        this.widget.value = value;
    }
    ConstantString.prototype.onDropFile = function (file) {
        var that = this;
        var reader = new FileReader();
        reader.onload = function (e) {
            that.setProperty("value", e.target.result);
        }
        reader.readAsText(file);
    }
    ConstantString.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("常用/字符串", ConstantString);

    function ConstantObject() {
        this.addOutput("对象", "object");
        this.size = [120, 30];
        this._object = {};
    }

    ConstantObject.title = "对象";
    ConstantObject.desc = "创建对象";

    ConstantObject.prototype.onExecute = function () {
        this.boxcolor = "#AEA";
        this.setOutputData(0, this._object);
    };
    ConstantObject.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("常用/对象", ConstantObject);



    //to store json objects
    function JSONParse() {
        this.addInput("json字符串", "string");
        this.addOutput("输出对象", "object");
        this.size = [180, 60];
        this._str = null;
        this._obj = null;
    }

    JSONParse.title = "JSON解释器";
    JSONParse.desc = "将JSON字符串解析为对象";

    JSONParse.prototype.onExecute = function () {
        this._str = this.getInputData(0);
        if (!this._str)
            return;

        try {
            this._obj = JSON.parse(this._str);
            this.boxcolor = "#AEA";
            this.setOutputData(1, this._obj);
        } catch (err) {
            this.boxcolor = "red";
        }
    };
    JSONParse.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("常用/JSON解释器", JSONParse);

    function ConstantArray() {
        this.addOutput("数组", "array");
        this.addOutput("长度", "number");
        this.addProperty("value", "[]");
        this.widget = this.addWidget("text", "array", this.properties.value, "value");
        this.size = [180, 80];
    }

    ConstantArray.title = "数组";
    ConstantArray.desc = "创建数组对象";

    ConstantArray.prototype.onPropertyChanged = function (name, value) {
        this.widget.value = value;
    };

    ConstantArray.prototype.onExecute = function () {
        var outval = [];
        var tmpval = this.properties.value;
        try {
            if (tmpval[0] != "[")
                outval = JSON.parse("[" + tmpval + "]");
            else
                outval = JSON.parse(tmpval);
            this.boxcolor = "#AEA";
        } catch (err) {
            this.boxcolor = "red";
        }

        this.setOutputData(0, outval);
        this.setOutputData(1, outval ? (outval.length || 0) : 0);
    };

    ConstantArray.prototype.setValue = ConstantNumber.prototype.setValue;
    ConstantArray.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("常用/数组", ConstantArray);

    function SetArray() {
        this.addInput("数组", "array");
        this.addInput("任意值", "");
        this.addOutput("数组", "array");
        this.properties = { index: 0 };
        this.widget = this.addWidget("number", "i", this.properties.index, "index", { precision: 0, step: 10, min: 0 });
    }

    SetArray.title = "修改数组";
    SetArray.desc = "修改数组指定元素的值";
    SetArray.prototype.onPropertyChanged = function (name, value) {
        this.widget.value = value;
    };
    SetArray.prototype.onGetInputs = function () {
        return [["index", "number"],["触发器", LiteGraph.ACTION]];
    };
    SetArray.prototype.onExecute = function () {
        var arr = this.getInputData(0);
        if (!arr)
            return;
        var v = this.getInputData(1);
        if (v === undefined)
            return;
        if (this.inputs.length > 2) {
            this.properties.index = this.getInputData(2);
        }
        if (this.properties.index)
            arr[Math.floor(this.properties.index)] = v;
        this.boxcolor = "#AEA";
        this.setOutputData(0, arr);
    };

    LiteGraph.registerNodeType("常用/修改数组", SetArray);

    function ArrayElement() {
        this.addInput("数组", "array,string");
        this.addInput("索引", "number");
        this.addOutput("任意值", "");
        this.addProperty("index", 0);

    }

    ArrayElement.title = "取数组元素";
    ArrayElement.desc = "返回数组中的元素";

    ArrayElement.prototype.onExecute = function () {
        var array = this.getInputData(0);
        var index = this.getInputData(1);
        if (index == null)
            index = this.properties.index;
        if (array == null || index == null)
            return;
        this.boxcolor = "#AEA";
        this.setOutputData(0, array[Math.floor(Number(index))]);
    };
    ArrayElement.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("常用/取数组元素", ArrayElement);


    function ObjectProperty() {
        this.addInput("对象", "object");
        this.addOutput("属性", 0);
        this.addProperty("value", 0);
        this.widget = this.addWidget("text", "prop.", "", this.setValue.bind(this));
        this.size = [140, 30];
        this._value = null;
    }

    ObjectProperty.title = "取对象属性值";
    ObjectProperty.desc = "输出对象的属性值";

    ObjectProperty.prototype.setValue = function (v) {
        this.properties.value = v;
        this.widget.value = v;
    };

    ObjectProperty.prototype.getTitle = function () {
        if (this.flags.collapsed) {
            return "对象." + this.properties.value;
        }
        return this.title;
    };

    ObjectProperty.prototype.onPropertyChanged = function (name, value) {
        this.widget.value = value;
    };

    ObjectProperty.prototype.onExecute = function () {
        var data = this.getInputData(0);
        if (data != null) {
            this.boxcolor = "#AEA";
            this.setOutputData(0, data[this.properties.value]);
        }
    };
    ObjectProperty.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("常用/取对象属性值", ObjectProperty);

    function ObjectKeys() {
        this.addInput("对象", "object,string,array");
        this.addOutput("数组", "array");
        this.size = [140, 30];
    }

    ObjectKeys.title = "获取key数组";
    ObjectKeys.desc = "1、输出对象的属性名数组\r\n2、输出数组的索引数组";

    ObjectKeys.prototype.onExecute = function () {
        var data = this.getInputData(0);
        if (data != null) {
            this.boxcolor = "#AEA";
            if (data instanceof Array) {
                this.setOutputData(0, Object.keys(data).map(key => Number(key)));
            }
            else if(data instanceof String){
                let outarr=[];
                for(let i=0;i<data.length;i++){
                    outarr.push(i);
                }
                this.setOutputData(0, outarr);
            }
            else {
                this.setOutputData(0, Object.keys(data));
            }
        }
    };
    ObjectKeys.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("常用/获取key数组", ObjectKeys);


    function SetObject() {
        this.addInput("对象", "object");
        this.addInput("任意值", "");
        this.addOutput("对象", "object");
        this.properties = { property: "" };
        this.name_widget = this.addWidget("text", "对象.", this.properties.property, "property");
    }

    SetObject.title = "对象赋值";
    SetObject.desc = "给对象设置指定属性值";
    SetObject.prototype.onPropertyChanged = function (name, value) {
        this.name_widget.value = value;
    };
    SetObject.prototype.onExecute = function () {
        var obj = this.getInputData(0);
        if (!obj)
            return;
        var v = this.getInputData(1);
        if (v === undefined)
            return;
        if (this.properties.property)
            obj[this.properties.property] = v;
        this.boxcolor = "#AEA";
        this.setOutputData(0, obj);
    };
    SetObject.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("常用/对象赋值", SetObject);


    function MergeObjects() {
        this.addInput("A对象", "object");
        this.addInput("B对象", "object");
        this.addOutput("对象", "object");
        this.size = this.computeSize();
    }

    MergeObjects.title = "合并对象";
    MergeObjects.desc = "创建一个复制其他对象属性的对象";

    MergeObjects.prototype.onExecute = function () {
        var A = this.getInputData(0);
        var B = this.getInputData(1);
        var C = {};
        if (A)
            for (var i in A)
                C[i] = A[i];
        if (B)
            for (var i in B)
                C[i] = B[i];
        this.boxcolor = "#AEA";
        this.setOutputData(0, C);
    };
    MergeObjects.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("常用/合并对象", MergeObjects);


    function length(v) {
        if (v && v.length != null)
            return Number(v.length);
        return 0;
    }

    LiteGraph.wrapFunctionAsNode(
        "常用/获取长度",
        length,
        ["array,string"],
        "number"
    );




}
