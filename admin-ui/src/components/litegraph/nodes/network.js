export function InitNetworkNodes(graph) {
    var LiteGraph = graph;

//HTTP Request
function HTTPRequestNode() {
	var that = this;
	this.addInput("request", LiteGraph.ACTION);
	this.addInput("url", "string");
	this.addProperty("url", "");
	this.addOutput("ready", LiteGraph.EVENT);
    this.addOutput("data", "string");
	this.addWidget("button", "Fetch", null, this.fetch.bind(this));
	this._data = null;
	this._fetching = null;
}

HTTPRequestNode.title = "HTTP Request";
HTTPRequestNode.desc = "Fetch data through HTTP";

HTTPRequestNode.prototype.fetch = function()
{
	var url = this.properties.url;
	if(!url)
		return;

	this.boxcolor = "#FF0";
	var that = this;
	this._fetching = fetch(url)
	.then(resp=>{
		if(!resp.ok)
		{
			this.boxcolor = "#F00";
			that.trigger("error");
		}
		else
		{
			this.boxcolor = "#0F0";
			return resp.text();
		}
	})
	.then(data=>{
		that._data = data;
		that._fetching = null;
		that.trigger("ready");
	});
}

HTTPRequestNode.prototype.onAction = function(evt)
{
	if(evt == "request")
		this.fetch();
}

HTTPRequestNode.prototype.onExecute = function() {
	this.setOutputData(1, this._data);
};

HTTPRequestNode.prototype.onGetOutputs = function() {
	return [["error",LiteGraph.EVENT]];
}

HTTPRequestNode.prototype.onGetInputs = function() {
	return [["触发器", LiteGraph.ACTION]];
};

LiteGraph.registerNodeType("network/httprequest", HTTPRequestNode);


	
}
