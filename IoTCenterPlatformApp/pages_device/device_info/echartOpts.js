var dayjs = require('@/common/day.js')
import echarts from '@/pages_device/echarts.min.js'
export const echartBarOpts = {
	title:{
		show:false,
	},
	// tooltip: {
	// 	trigger: 'item',
	// 	backgroundColor: 'rgba(255, 255, 255, 0.9)',
	// 	borderColor: '#eee',
	// 	borderWidth: 1,
	// 	textStyle: {
	// 		color: '#333'
	// 	},
	// 	align: 'left',
	// 	formatter: (params) => {
	// 		const stateInfo = params.data;
	// 		return `
	//               <div style="text-align: left;">${stateInfo.name}</div>
	//               <div>
	//                 <div style="width: 10px; height: 10px; border-radius: 50%; display: inline-block; margin-right: 5px; background-color: ${stateInfo.itemStyle.color};"></div>
	//                 ${stateInfo.itemStyle.name}时间: ${dayjs(stateInfo.value[1]).format('YYYY/MM/DD HH:mm:ss')}-${dayjs(stateInfo.value[2]).format('YYYY/MM/DD HH:mm:ss')}
	//                 </div>
	//               <div style="text-align: left;">
	//                 <div style="width: 10px; height: 10px; border-radius: 50%; display: inline-block; margin-right: 5px; background-color: ${stateInfo.itemStyle.color};"></div>
	//                 ${stateInfo.itemStyle.name}: ${formatDuration(stateInfo.value[3])}
	//               </div>
	//             `;
	// 	}
	// },
	tooltip: {
		// align: 'left',
	    formatter: function (params) {
			const stateInfo = params.data;
	      return params.marker + stateInfo.itemStyle.name + '\n' + `开始: ${dayjs(stateInfo.value[1]).format('YYYY/MM/DD HH:mm:ss')}\n结束:${dayjs(stateInfo.value[2]).format('YYYY/MM/DD HH:mm:ss')}\n${formatDuration(stateInfo.value[3])}`;
	    }
	  },
	grid: [{
		left: '-7%',
		right: '3%',
		bottom: '-3%',
		top:'0%',
		// containLabel: true, //这个可以实现将数据包裹在内部，不会有竖轴的值被遮盖
	}],
	xAxis: {
		type: 'time',
		min: 0,
		max: 100,
		boundaryGap: false,
		splitLine: {
			show: false
		},
		offset:-60,
		// scale: true,
		// splitNumber: 5,
		// interval: 7200000,
		axisLine: {
			show: false // 设为 false 即可隐藏
		},
		axisLabel: {
			formatter: (value, index) => { // 第一个参数没有用 所以用_代替
				return `${dayjs(value).format('YYYY-MM-DD')}`; // 其他位置不显示标签
			},
			// showMaxLabel: true,
			// showMinLabel: true,
			// interval: 'auto',
			rotate: -90 // 旋转标签以避免重叠
		}
	},
	yAxis: {
		type: 'category',
		data: [],
		axisLabel: {
			interval: 0,
			fontSize: 18,
			fontWeight: 500,
		},
		axisLine: {
			show: false // 设为 false 即可隐藏
		}
	},
	dataZoom: [{
			type: 'slider',
			filterMode: 'weakFilter',
			showDataShadow: false,
			bottom: 80,
			labelFormatter: ''
		},
		{
			type: 'inside',
			filterMode: 'weakFilter'
		}
	],
	series: [{
		id: 'timeline',
		type: 'custom',
		renderItem: renderItem,
		itemStyle: {
			opacity: 0.8
		},
		encode: {
			x: [1, 2],
			y: 0
		},
		data: []
	}]
}
export function renderItem(params, api) { //甘特图处理
	// console.log("数据api",api);
	var categoryIndex = api.value(0);

	var start = api.coord([api.value(1), categoryIndex]);
	var end = api.coord([api.value(2), categoryIndex]);
	var height = api.size([0, 1])[1] * 0.35;
	var rectShape = echarts.graphic.clipRectByRect({
		x: start[0],
		y: start[1] - height / 2,
		width: end[0] - start[0],
		height: height
	}, {
		x: params.coordSys.x,
		y: params.coordSys.y,
		width: params.coordSys.width,
		height: params.coordSys.height
	});
	return (
		rectShape && {
			type: 'rect',
			transition: ['shape'],
			shape: rectShape,
			style: api.style()
		}
	);
}
export function formatDuration(durationMs) { //时间格式化
	const totalSeconds = Math.floor(durationMs / 1000);
	const hours = Math.floor(totalSeconds / 3600);
	const minutes = Math.floor((totalSeconds % 3600) / 60);
	const seconds = totalSeconds % 60;

	if (hours > 0) {
		return `${hours}小时${minutes}分${seconds}秒`;
	} else if (minutes > 0) {
		return `${minutes}分${seconds}秒`;
	} else {
		return `${seconds}秒`;
	}
}