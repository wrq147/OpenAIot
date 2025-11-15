export function InitStringNodes(graph) {
    var LiteGraph = graph;

    function toString(a) {
		if(a && a.constructor === Object)
		{
			try
			{
				return JSON.stringify(a);
			}
			catch (err)
			{
				return String(a);
			}
		}
        return String(a);
    }

    LiteGraph.wrapFunctionAsNode("字符串/转字符串", toString, [""], "string");

    function compare(a, b) {
        return a == b;
    }

    LiteGraph.wrapFunctionAsNode(
        "字符串/比较两字符串",
        compare,
        ["string", "string"],
        "boolean"
    );

    function concatenate(a, b) {
        if (a === undefined) {
            return b;
        }
        if (b === undefined) {
            return a;
        }
        return a + b;
    }

    LiteGraph.wrapFunctionAsNode(
        "字符串/拼接两字符串",
        concatenate,
        ["string", "string"],
        "string"
    );

    function contains(a, b) {
        if (a === undefined || b === undefined) {
            return false;
        }
        return a.indexOf(b) != -1;
    }

    LiteGraph.wrapFunctionAsNode(
        "字符串/判断是否包含",
        contains,
        ["string", "string"],
        "boolean"
    );

    function toUpperCase(a) {
        if (a != null && a.constructor === String) {
            return a.toUpperCase();
        }
        return a;
    }

    LiteGraph.wrapFunctionAsNode(
        "字符串/转大写",
        toUpperCase,
        ["string"],
        "string"
    );

    function split(str, separator) {
		if(separator == null)
			separator = this.properties.separator;
        if (str == null )
	        return [];
		if( str.constructor === String )
			return str.split(separator || " ");
        return null;
    }

    LiteGraph.wrapFunctionAsNode(
        "字符串/切割字符串",
        split,
        ["string", "string"],
        "array",
		{ separator: "," }
    );

    function toFixed(a) {
        if (a != null && a.constructor === Number) {
            return a.toFixed(this.properties.precision);
        }
        return a;
    }

    LiteGraph.wrapFunctionAsNode(
        "字符串/数字保留小数点",
        toFixed,
        ["number"],
        "string",
        { precision: 0 }
    );



}