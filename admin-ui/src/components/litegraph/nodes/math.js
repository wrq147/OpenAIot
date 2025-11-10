export function InitMathNodes(graph) {
    var LiteGraph = graph;


    function ToNumber() {
        this.addInput("任意值", "");
        this.addOutput("数值", "number");
    }

    ToNumber.title = "转数值";
    ToNumber.desc = "强制转换成数值类型";
    ToNumber.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    ToNumber.prototype.onExecute = function () {
        var v = this.getInputData(0);
        try {
            this.boxcolor = "#AEA";
            this.setOutputData(0, Number(v));
        }
        catch {
            this.boxcolor = "red";
        }
    };

    LiteGraph.registerNodeType("数学/转数值", ToNumber);


    function MathRand() {
        this.addOutput("数值", "number");
        this.addProperty("min", 0);
        this.addProperty("max", 1);
        this.size = [80, 30];
    }

    MathRand.title = "随机数";
    MathRand.desc = "生成随机数";

    MathRand.prototype.onExecute = function () {
        if (this.inputs) {
            for (var i = 0; i < this.inputs.length; i++) {
                var input = this.inputs[i];
                var v = this.getInputData(i);
                if (v === undefined) {
                    continue;
                }
                this.properties[input.name] = v;
            }
        }

        var min = this.properties.min;
        var max = this.properties.max;
        this._last_v = Math.random() * (max - min) + min;
        this.boxcolor = "#AEA";
        this.setOutputData(0, this._last_v);
    };

    MathRand.prototype.onDrawBackground = function (ctx) {
        //show the current value
        this.outputs[0].label = (this._last_v || 0).toFixed(3);
    };

    MathRand.prototype.onGetInputs = function () {
        return [["min", "number"], ["max", "number"], ["触发器", LiteGraph.ACTION]];
    };

    LiteGraph.registerNodeType("数学/随机数", MathRand);


    //Math clamp
    function MathClamp() {
        this.addInput("数值", "number");
        this.addOutput("数值", "number");
        this.size = [80, 30];
        this.addProperty("min", 0);
        this.addProperty("max", 1024);
    }

    MathClamp.title = "限制数值";
    MathClamp.desc = "将给定的值限制在指定的范围内";
    MathClamp.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    MathClamp.prototype.onExecute = function () {
        var v = this.getInputData(0);
        if (v == null) {
            return;
        }
        v = Math.max(this.properties.min, v);
        v = Math.min(this.properties.max, v);
        this.boxcolor = "#AEA";
        this.setOutputData(0, v);
    };

    LiteGraph.registerNodeType("数学/限制数值", MathClamp);



    //Math ABS
    function MathAbs() {
        this.addInput("数值", "number");
        this.addOutput("数值", "number");
        this.size = [120, 30];
    }

    MathAbs.title = "绝对值";
    MathAbs.desc = "取数值的绝对值";

    MathAbs.prototype.onExecute = function () {
        var v = this.getInputData(0);
        if (v == null) {
            return;
        }
        this.boxcolor = "#AEA";
        this.setOutputData(0, Math.abs(v));
    };
    MathAbs.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("数学/绝对值", MathAbs);

    //Math Floor
    function MathFloor() {
        this.addInput("数值", "number");
        this.addOutput("数值", "number");
        this.size = [80, 30];
    }

    MathFloor.title = "向下取整";
    MathFloor.desc = "用于返回小于或等于输入数值的最大整数值";

    MathFloor.prototype.onExecute = function () {
        var v = this.getInputData(0);
        if (v == null) {
            return;
        }
        this.boxcolor = "#AEA";
        this.setOutputData(0, Math.floor(v));
    };
    MathFloor.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("数学/向下取整", MathFloor);

    //Math frac
    function MathFrac() {
        this.addInput("数值", "number");
        this.addOutput("数值", "number");
        this.size = [80, 30];
    }

    MathFrac.title = "取小数";
    MathFrac.desc = "返回小数部分";

    MathFrac.prototype.onExecute = function () {
        var v = this.getInputData(0);
        if (v == null) {
            return;
        }
        this.boxcolor = "#AEA";
        this.setOutputData(0, v % 1);
    };
    MathFrac.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("数学/取小数", MathFrac);



    //Math operation
    function MathOperation() {
        this.addInput("A", "number");
        this.addInput("B", "number");
        this.addOutput("=", "number");
        this.addProperty("A", 1);
        this.addProperty("B", 1);
        this.addProperty("OP", "+", "enum", { values: MathOperation.values });
        this.widget = this.addWidget("combo","运算符",this.properties.OP,{ property: "OP", values: MathOperation.values } );
        this.size = [150, 70];
    }

    MathOperation.values = ["+", "-", "×", "/", "%", "^", "max", "min"];
    MathOperation.funcs = {
        "+": function (A, B) { return A + B; },
        "-": function (A, B) { return A - B; },
        "×": function (A, B) { return A * B; },
        "/": function (A, B) { return A / B; },
        "%": function (A, B) { return A % B; },
        "^": function (A, B) { return Math.pow(A, B); },
        "max": function (A, B) { return Math.max(A, B); },
        "min": function (A, B) { return Math.min(A, B); }
    };

    MathOperation.title = "计算器";
    MathOperation.desc = "执行简单的数学计算";
    MathOperation.size = [100, 60];

    MathOperation.prototype.getTitle = function () {
        if (this.properties.OP == "max" || this.properties.OP == "min")
            return this.properties.OP + "(A,B)";
        return "A " + this.properties.OP + " B";
    };
    MathOperation.prototype.onPropertyChanged = function (name, value) {
        this.widget.value = value;
    }

    MathOperation.prototype.onExecute = function () {
        var A = this.getInputData(0);
        var B = this.getInputData(1);
        if (A == null) {
            A = this.properties["A"];
        }

        if (B == null) {
            B = this.properties["B"];
        }

        var func = MathOperation.funcs[this.properties.OP];
        if (func == null) {
            this.boxcolor = "red";
            return;
        }
        this.boxcolor = "#AEA";
        this.setOutputData(0, func(A, B));
    };


    MathOperation.prototype.onGetInputs = function () {
        return [["触发器", LiteGraph.ACTION]];
    };
    LiteGraph.registerNodeType("数学/计算器", MathOperation);




    //Math Trigonometry
    function MathTrigonometry() {
        this.addInput("数值", "number");
        this.addOutput("数值", "number");

        this.addProperty("振幅", 1);
        this.addProperty("偏移", 0);
        this.addProperty("OP", "sin", "enum", { values: MathTrigonometry.values });
        this.widget = this.addWidget("combo","运算符",this.properties.OP,{ property: "OP", values: MathTrigonometry.values } );
        this.size = [150, 50];
    }
    MathTrigonometry.values = ["sin", "cos", "tan", "asin", "acos", "atan"];
    MathTrigonometry.title = "三角函数";
    MathTrigonometry.desc = "执行三函数计算";
    MathTrigonometry.prototype.onPropertyChanged = function (name, value) {
        this.widget.value = value;
    }
    MathTrigonometry.prototype.getTitle = function () {
        return this.properties.OP + "(数值)×振幅+偏移";
    };
    MathTrigonometry.prototype.onExecute = function () {
        var v = this.getInputData(0);
        if (v == null) {
            v = 0;
        }
        var amplitude = this.properties["振幅"];
        var slot = this.findInputSlot("振幅");
        if (slot != -1) {
            amplitude = this.getInputData(slot);
        }
        var offset = this.properties["偏移"];
        slot = this.findInputSlot("偏移");
        if (slot != -1) {
            offset = this.getInputData(slot);
        }

        var value;
        var tmpop=this.properties["OP"];
        switch (tmpop) {
            case "sin":
                value = Math.sin(v);
                break;
            case "cos":
                value = Math.cos(v);
                break;
            case "tan":
                value = Math.tan(v);
                break;
            case "asin":
                value = Math.asin(v);
                break;
            case "acos":
                value = Math.acos(v);
                break;
            case "atan":
                value = Math.atan(v);
                break;
        }
        this.boxcolor = "#AEA";
        this.setOutputData(0, amplitude * value + offset);
    };

    MathTrigonometry.prototype.onGetInputs = function () {
        return [["振幅", "number"], ["偏移", "number"], ["触发器", LiteGraph.ACTION]];
    };

    LiteGraph.registerNodeType("数学/三角函数", MathTrigonometry);

}
