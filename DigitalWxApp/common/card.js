import {
	serverUrl
} from './request.js'
import {
	useDrawPoster
} from '@/js_sdk/u-draw-poster'

///名片模板与背景
const drawObj = {
	templatelist: [{
			"name": "默认名片模板",
			"info": "企业名、姓名、头像、职位、电话号码",
			"bglist": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
			"draws": [{
				"type": "text",
				"data": "${OrgName}",
				"font": "28px sans-serif",
				"color": "#FFF",
				"x": 30,
				"y": 36
			}, {
				"type": "text",
				"data": "${RealName}",
				"font": "bold 34px sans-serif",
				"color": "#FFF",
				"x": 30,
				"y": 88
			}, {
				"type": "text",
				"data": "${PostName}",
				"font": "28px sans-serif",
				"color": "#FFF",
				"x": 30,
				"y": 146
			}, {
				"type": "text",
				"data": "${Mobile}",
				"font": "28px sans-serif",
				"color": "#FFF",
				"x": 84,
				"y": 339
			}, {
				"type": "roundImage",
				"data": "${Avatar}",
				"x": 550,
				"y": 30,
				"w": 110,
				"h": 110,
				"r": 10
			}, {
				"type": "image",
				"data": "/static/phone.png",
				"x": 30,
				"y": 334,
				"w": 34,
				"h": 34
			}]
		}, {
			"name": "默认名片模板",
			"info": "企业名、姓名、头像、职位、电话号码",
			"bglist": [11, 12, 13, 14, 15],
			"draws": [{
				"type": "text",
				"data": "${OrgName}",
				"font": "28px sans-serif",
				"color": "rgba(51, 51, 51, 1)",
				"x": 30,
				"y": 36
			}, {
				"type": "text",
				"data": "${RealName}",
				"font": "bold 34px sans-serif",
				"color": "rgba(51, 51, 51, 1)",
				"x": 30,
				"y": 88
			}, {
				"type": "text",
				"data": "${PostName}",
				"font": "28px sans-serif",
				"color": "rgba(51, 51, 51, 1)",
				"x": 30,
				"y": 146
			}, {
				"type": "text",
				"data": "${Mobile}",
				"font": "28px sans-serif",
				"color": "rgba(51, 51, 51, 1)",
				"x": 84,
				"y": 339
			}, {
				"type": "roundImage",
				"data": "${Avatar}",
				"x": 550,
				"y": 30,
				"w": 110,
				"h": 110,
				"r": 10
			}, {
				"type": "image",
				"data": "/static/phone_black.png",
				"x": 30,
				"y": 334,
				"w": 34,
				"h": 34
			}]
		}, {
			"name": "默认名片模板",
			"info": "头像、姓名、职位、企业名、电话号码、邮箱、地址",
			"bglist": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
			"draws": [{
				"type": "roundImage",
				"data": "${Avatar}",
				"x": 40,
				"y": 40,
				"w": 120,
				"h": 120,
				"r": 60
			}, {
				"type": "text",
				"data": "${RealName}",
				"font": "bold 34px MicrosoftYaHe",
				"color": "#ffffff",
				"x": 190,
				"y": 43
			}, {
				"type": "text",
				"data": "${PostName}",
				"font": "24px sans-serif",
				"color": "#ffffff",
				"x": 190,
				"y": 97
			}, {
				"type": "text",
				"data": "${OrgName}",
				"font": "24px sans-serif",
				"color": "#ffffff",
				"x": 190,
				"y": 135
			}, {
				"type": "image",
				"data": "/static/card/phone2_white.png",
				"x": 40,
				"y": 240,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Mobile}",
				"font": "24px sans-serif",
				"color": "#ffffff",
				"x": 84,
				"y": 242
			}, {
				"type": "image",
				"data": "/static/card/email_white.png",
				"x": 40,
				"y": 288,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Email}",
				"font": "24px sans-serif",
				"color": "#ffffff",
				"x": 84,
				"y": 290
			}, {
				"type": "image",
				"data": "/static/card/address_white.png",
				"x": 40,
				"y": 336,
				"w": 24,
				"h": 24
			}, {
				"type": "address",
				"data": "${AddressName}${AddressDetail}",
				"font": "20px sans-serif",
				"color": "#ffffff",
				"x": 84,
				"y": 338,
				"w": 460,
				"h": 24
			}, ]
		}, {
			"name": "默认名片模板",
			"info": "头像、姓名、职位、企业名、电话号码、邮箱、地址",
			"bglist": [11, 12, 13, 14, 15],
			"draws": [{
				"type": "roundImage",
				"data": "${Avatar}",
				"x": 40,
				"y": 40,
				"w": 120,
				"h": 120,
				"r": 60
			}, {
				"type": "text",
				"data": "${RealName}",
				"font": "bold 34px MicrosoftYaHe",
				"color": "#333333",
				"x": 190,
				"y": 43
			}, {
				"type": "text",
				"data": "${PostName}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 190,
				"y": 97
			}, {
				"type": "text",
				"data": "${OrgName}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 190,
				"y": 135
			}, {
				"type": "image",
				"data": "/static/card/phone2_black.png",
				"x": 40,
				"y": 240,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Mobile}",
				"font": "20px sans-serif",
				"color": "#333333",
				"x": 84,
				"y": 242
			}, {
				"type": "image",
				"data": "/static/card/email_black.png",
				"x": 40,
				"y": 288,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Email}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 84,
				"y": 290
			}, {
				"type": "image",
				"data": "/static/card/address_black.png",
				"x": 40,
				"y": 336,
				"w": 24,
				"h": 24
			}, {
				"type": "address",
				"data": "${AddressName}${AddressDetail}",
				"font": "20px sans-serif",
				"color": "#333333",
				"x": 84,
				"y": 338,
				"w": 460,
				"h": 24
			}]
		},
		{
			"name": "默认名片模板",
			"info": "企业名、姓名、职位、电话号码、邮箱、地址",
			"bglist": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
			"draws": [{
				"type": "text",
				"data": "${OrgName}",
				"font": "24px sans-serif",
				"color": "#ffffff",
				"x": 40,
				"y": 31
			}, {
				"type": "text",
				"data": "${RealName}",
				"font": "bold 34px MicrosoftYaHe",
				"color": "#ffffff",
				"x": 40,
				"y": 139
			}, {
				"type": "text",
				"data": "${PostName}",
				"font": "24px sans-serif",
				"color": "#ffffff",
				"x": 40,
				"y": 193
			}, {
				"type": "rightaddress",
				"data": "${AddressName}${AddressDetail}",
				"font": "20px sans-serif",
				"color": "#ffffff",
				"x": 40,
				"y": 167,
				"w": 330,
				"h": 24
			}, {
				"type": "line",
				"data": "",
				"color": "#ffffff",
				"x": 40,
				"y": 257,
				"w": 610,
				"h": 1
			}, {
				"type": "image",
				"data": "/static/card/phone2_white.png",
				"x": 40,
				"y": 298,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Mobile}",
				"font": "24px sans-serif",
				"color": "#ffffff",
				"x": 84,
				"y": 300
			}, {
				"type": "image",
				"data": "/static/card/email_white.png",
				"x": 40,
				"y": 346,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Email}",
				"font": "24px sans-serif",
				"color": "#ffffff",
				"x": 84,
				"y": 348
			}, ]
		}, {
			"name": "默认名片模板",
			"info": "企业名、姓名、职位、电话号码、邮箱、地址",
			"bglist": [11, 12, 13, 14, 15],
			"draws": [{
				"type": "text",
				"data": "${OrgName}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 40,
				"y": 31
			}, {
				"type": "text",
				"data": "${RealName}",
				"font": "bold 34px MicrosoftYaHe",
				"color": "#333333",
				"x": 40,
				"y": 139
			}, {
				"type": "text",
				"data": "${PostName}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 40,
				"y": 193
			}, {
				"type": "rightaddress",
				"data": "${AddressName}${AddressDetail}",
				"font": "20px sans-serif",
				"color": "#333333",
				"x": 40,
				"y": 167,
				"w": 330,
				"h": 24
			}, {
				"type": "line",
				"data": "",
				"color": "#333333",
				"x": 40,
				"y": 257,
				"w": 610,
				"h": 1
			}, {
				"type": "image",
				"data": "/static/card/phone2_black.png",
				"x": 40,
				"y": 298,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Mobile}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 84,
				"y": 300
			}, {
				"type": "image",
				"data": "/static/card/email_black.png",
				"x": 40,
				"y": 346,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Email}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 84,
				"y": 348
			}, ]
		}, {
			"name": "默认名片模板",
			"info": "头像、企业名、姓名、职位、电话号码、邮箱、地址",
			"bglist": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
			"draws": [{
				"type": "roundImage",
				"data": "${Avatar}",
				"x": 40,
				"y": 40,
				"w": 110,
				"h": 110,
				"r": 10
			}, {
				"type": "rightcompany",
				"data": "${OrgName}",
				"font": "26px sans-serif",
				"color": "#FFFFFF",
				"x": 410,
				"y": 40
			}, {
				"type": "rightaddress",
				"data": "${AddressName}${AddressDetail}",
				"font": "20px sans-serif",
				"color": "#ffffff",
				"x": 40,
				"y": 208,
				"w": 460,
				"h": 24
			}, {
				"type": "line",
				"data": "",
				"color": "#FFFFFF",
				"x": 40,
				"y": 258,
				"w": 610,
				"h": 1
			}, {
				"type": "text",
				"data": "${RealName}",
				"font": "bold 34px MicrosoftYaHe",
				"color": "#FFFFFF",
				"x": 40,
				"y": 292
			}, {
				"type": "text",
				"data": "${PostName}",
				"font": "24px sans-serif",
				"color": "#FFFFFF",
				"x": 40,
				"y": 346
			}, {
				"type": "image",
				"data": "/static/card/phone2_white.png",
				"x": 352,
				"y": 298,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Mobile}",
				"font": "24px sans-serif",
				"color": "#FFFFFF",
				"x": 396,
				"y": 300
			}, {
				"type": "image",
				"data": "/static/card/email_white.png",
				"x": 352,
				"y": 348,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Email}",
				"font": "24px sans-serif",
				"color": "#FFFFFF",
				"x": 396,
				"y": 350
			}]
		}, {
			"name": "默认名片模板",
			"info": "头像、企业名、姓名、职位、电话号码、邮箱、地址",
			"bglist": [11, 12, 13, 14, 15],
			"draws": [{
				"type": "roundImage",
				"data": "${Avatar}",
				"x": 40,
				"y": 40,
				"w": 110,
				"h": 110,
				"r": 10
			}, {
				"type": "rightcompany",
				"data": "${OrgName}",
				"font": "26px sans-serif",
				"color": "#333333",
				"x": 410,
				"y": 40
			}, {
				"type": "rightaddress",
				"data": "${AddressName}${AddressDetail}",
				"font": "20px sans-serif",
				"color": "#333333",
				"x": 40,
				"y": 208,
				"w": 460,
				"h": 24
			}, {
				"type": "line",
				"data": "",
				"color": "#333333",
				"x": 40,
				"y": 258,
				"w": 610,
				"h": 1
			}, {
				"type": "text",
				"data": "${RealName}",
				"font": "bold 34px MicrosoftYaHe",
				"color": "#333333",
				"x": 40,
				"y": 292
			}, {
				"type": "text",
				"data": "${PostName}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 40,
				"y": 346
			}, {
				"type": "image",
				"data": "/static/card/phone2_black.png",
				"x": 352,
				"y": 298,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Mobile}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 396,
				"y": 300
			}, {
				"type": "image",
				"data": "/static/card/email_black.png",
				"x": 352,
				"y": 348,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Email}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 396,
				"y": 350
			}]
		}, {
			"name": "默认名片模板",
			"info": "头像、企业名、姓名、职位、电话号码、邮箱、地址",
			"bglist": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
			"draws": [{
				"type": "roundImage",
				"data": "${Avatar}",
				"x": 530,
				"y": 40,
				"w": 120,
				"h": 120,
				"r": 60
			}, {
				"type": "text",
				"data": "${RealName}",
				"font": "bold 34px MicrosoftYaHe",
				"color": "#FFFFFF",
				"x": 40,
				"y": 40
			}, {
				"type": "text",
				"data": "${PostName}",
				"font": "24px sans-serif",
				"color": "#FFFFFF",
				"x": 40,
				"y": 94
			}, {
				"type": "image",
				"data": "/static/card/phone2_white.png",
				"x": 40,
				"y": 160,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Mobile}",
				"font": "24px sans-serif",
				"color": "#FFFFFF",
				"x": 84,
				"y": 160
			}, {
				"type": "image",
				"data": "/static/card/email_white.png",
				"x": 40,
				"y": 210,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Email}",
				"font": "24px sans-serif",
				"color": "#FFFFFF",
				"x": 84,
				"y": 210
			}, {
				"type": "line",
				"data": "",
				"color": "#FFFFFF",
				"x": 40,
				"y": 272,
				"w": 610,
				"h": 1
			}, {
				"type": "text",
				"data": "${OrgName}",
				"font": "24px sans-serif",
				"color": "#FFFFFF",
				"x": 40,
				"y": 302
			}, {
				"type": "address",
				"data": "${AddressName}${AddressDetail}",
				"font": "20px sans-serif",
				"color": "#ffffff",
				"x": 40,
				"y": 340,
				"w": 460,
				"h": 24
			}]
		}, {
			"name": "默认名片模板",
			"info": "头像、企业名、姓名、职位、电话号码、邮箱、地址",
			"bglist": [11, 12, 13, 14, 15],
			"draws": [{
				"type": "roundImage",
				"data": "${Avatar}",
				"x": 530,
				"y": 40,
				"w": 120,
				"h": 120,
				"r": 60
			}, {
				"type": "text",
				"data": "${RealName}",
				"font": "bold 34px MicrosoftYaHe",
				"color": "#333333",
				"x": 40,
				"y": 40
			}, {
				"type": "text",
				"data": "${PostName}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 40,
				"y": 94
			}, {
				"type": "image",
				"data": "/static/card/phone2_black.png",
				"x": 40,
				"y": 160,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Mobile}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 84,
				"y": 160
			}, {
				"type": "image",
				"data": "/static/card/email_black.png",
				"x": 40,
				"y": 210,
				"w": 24,
				"h": 24
			}, {
				"type": "text",
				"data": "${Email}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 84,
				"y": 210
			}, {
				"type": "line",
				"data": "",
				"color": "#333333",
				"x": 40,
				"y": 272,
				"w": 610,
				"h": 1
			}, {
				"type": "text",
				"data": "${OrgName}",
				"font": "24px sans-serif",
				"color": "#333333",
				"x": 40,
				"y": 302
			}, {
				"type": "address",
				"data": "${AddressName}${AddressDetail}",
				"font": "20px sans-serif",
				"color": "#333333",
				"x": 40,
				"y": 340,
				"w": 460,
				"h": 24
			}]
		}

	],
	bslist: [
		serverUrl + "/yx/1.png",
		serverUrl + "/yx/2.png",
		serverUrl + "/yx/3.png",
		serverUrl + "/yx/4.png",
		serverUrl + "/yx/5.png",
		serverUrl + "/yx/6.png",
		serverUrl + "/yx/7.png",
		serverUrl + "/yx/8.png",
		serverUrl + "/yx/9.png",
		serverUrl + "/yx/10.png"
	],
	bglist: [
		serverUrl + "/bg/1.png",
		serverUrl + "/bg/2.png",
		serverUrl + "/bg/3.png",
		serverUrl + "/bg/4.png",
		serverUrl + "/bg/5.png",
		serverUrl + "/bg/6.png",
		serverUrl + "/bg/7.png",
		serverUrl + "/bg/8.png",
		serverUrl + "/bg/9.png",
		serverUrl + "/bg/10.png",
		serverUrl + "/bg/11.png",
		serverUrl + "/bg/12.png",
		serverUrl + "/bg/13.png",
		serverUrl + "/bg/14.png",
		serverUrl + "/bg/15.png",
		serverUrl + "/bg/16.png",
	]
};

export function getDrawObj() {
	return drawObj;
}

export async function drawButton(ctx, startPoint, width, height, radius, borderColor, backgroundColor, text, textColor,
	fontSize) {
		// console.log("绘制按钮",ctx);
	ctx.strokeStyle = borderColor;
	ctx.fillStyle = backgroundColor;
	ctx.beginPath();
	ctx.moveTo(startPoint.x, startPoint.y + radius);
	ctx.arcTo(startPoint.x, startPoint.y, startPoint.x + radius, startPoint.y, radius);
	ctx.lineTo(startPoint.x + width - radius, startPoint.y)
	ctx.arcTo(startPoint.x + width, startPoint.y, startPoint.x + width, startPoint.y + radius, radius);
	ctx.lineTo(startPoint.x + width, startPoint.y + height - radius)
	ctx.arcTo(startPoint.x + width, startPoint.y + height, startPoint.x + width - radius, startPoint.y + height,
		radius);
	ctx.lineTo(startPoint.x + radius, startPoint.y + height)
	ctx.arcTo(startPoint.x, startPoint.y + height, startPoint.x, startPoint.y + height - radius, radius)
	ctx.closePath();
	ctx.stroke();
	ctx.fill();
	ctx.fillStyle = textColor;
	ctx.font = `${fontSize}px sans-serif`;
	// ctx.textAlign = 'center';
	ctx.textBaseline = 'middle';

	ctx.fillText(text, startPoint.x + (width - ctx.measureText(text).width) / 2, startPoint.y + height / 2);
}

export async function drawCard(elem, elemdata, isShare = false,isTwoD=true) {
	// console.log("名片数据", elemdata);
	// 创建绘制工具
	let dp=null
	if(isTwoD){
		//由于此插件是腾讯自己的扩展插件，2d使用有限制，isTwoD控制绘制的名片是不是2d
		dp = await useDrawPoster(elem);
	}else{
		dp = await useDrawPoster(elem,{type:"context"});
	}
	

	dp.canvas.width = 690
	if (isShare) {
		dp.canvas.height = 552;
	} else {
		dp.canvas.height = 400;
	}


	let tmpId = elemdata["TemplateId"];
	let bkId = elemdata["TemplateBk"];
	dp.draw(async (ctx) => {
		//绘制背景图
		const pla = uni.getSystemInfoSync().platform;
		// console.log("pla", pla);
		if (pla == 'windows') {//如果是pc端的分享先绘制一层白色的底色
			if (isShare) {
				ctx.fillStyle = '#ffffff'; //线的颜色
				ctx.lineJoin = "round";
				await ctx.fillRect(0, 0, 690, 552); //画白色矩形最外层背景;
			}
		}

		await ctx.drawRoundImage(drawObj.bglist[bkId - 1], 0, 0, 690, 400, 10);

		if (isShare) {
			ctx.fillStyle = '#ffffff'; //线的颜色
			ctx.lineJoin = "round";
			await ctx.fillRect(0, 400, 690, 120); //画白色矩形最外层背景;
			await drawButton(ctx, {
				x: 195,
				y: 430
			}, 300, 60, 30, '#48ADFB', '#48ADFB', '点击查看名片', '#ffffff', 24)

		}


		//绘制描述信息
		let drawits = drawObj.templatelist[tmpId - 1].draws;
		for (let iidx = 0; iidx < drawits.length; iidx++) {
			let it = drawits[iidx];
			let ittxt = it.data;
			for (let objk in elemdata) {
				if ("${" + objk + "}" == '${Email}') {
					if (elemdata[objk] == '') {
						ittxt = ittxt.replaceAll("${" + objk + "}", "暂未填写邮箱")
					}
				} else if ("${" + objk + "}" == '${Mobile}') {
					if (elemdata[objk] == '') {
						ittxt = ittxt.replaceAll("${" + objk + "}", "暂未填写手机号码")
					}
				}
				ittxt = ittxt.replaceAll("${" + objk + "}", elemdata[objk]);

			}

			switch (it.type) {
				case "rightcompany":
				//右边的公司名称绘制
					ctx.textBaseline = "top";
					ctx.fillStyle = it.color;
					ctx.font = it.font;
					let wid = ctx.measureText(ittxt).width
					ctx.fillText(ittxt, 690 - wid - 40, it.y);
					break;
				case "line":
				//线的绘制
					// console.log("ssasasasa1", ctx, )
					ctx.fillStyle = it.color; //线的颜色
					ctx.fillRect(it.x, it.y, it.w, it.h); //画矩形
					break;
				case "text":
				//其他类型文本的绘制
					ctx.textBaseline = "top";
					ctx.fillStyle = it.color;
					ctx.font = it.font;
					ctx.fillText(ittxt, it.x, it.y);
					break;
				case "address":
					ctx.textBaseline = "top";
					ctx.fillStyle = it.color;
					ctx.font = it.font;
					let chr = ittxt.split(""); //这个方法是将一个字符串分割成字符串数组
					// console.log("ittxt", chr)
					let temp = "";
					let row = [];
					for (let a = 0; a < chr.length; a++) {
						if (ctx.measureText(temp).width < it.w) {

						} else {
							row.push(temp);
							temp = "";
						}
						temp += chr[a];
					}
					row.push(temp);
					for (let b = 0; b < row.length; b++) {
						// console.log("每一行的值", row[b], it.x, it.y + (b + 1) * 24);
						ctx.fillText(row[b], it.x, it.y + b * 20 + b * 12);
					}
					break;
				case "rightaddress":
					ctx.textBaseline = "top";
					ctx.fillStyle = it.color;
					ctx.font = it.font;
					let chr1 = ittxt.split(""); //这个方法是将一个字符串分割成字符串数组
					// console.log("ittxt", chr1)
					let temp1 = "";
					let row1 = [];
					for (let a = 0; a < chr1.length; a++) {
						if (ctx.measureText(temp1).width < it.w) {

						} else {
							row1.push(temp1);
							temp1 = "";
						}
						temp1 += chr1[a];
					}
					row1.push(temp1);
					for (let b = 0; b < row1.length; b++) {
						// console.log("每一行的值", row1[b], it.x, it.y + (b + 1) * 24,690-ctx.measureText(row1[b]).width-40);
						ctx.fillText(row1[b], 690 - ctx.measureText(row1[b]).width - 40, it.y + b * 20 +
							b * 12);
					}
					break;
				case "roundImage":
					await ctx.drawRoundImage(ittxt, it.x, it.y, it.w, it.h, it.r);
					break;
				case "image":
					await ctx.drawImage(ittxt, it.x, it.y, it.w, it.h);
					break;
			}
		}

	});

	// 由于每个任务都有可能会有异步的绘制任务, 所以得需要使用await等待绘制
	await dp.render();
	return dp;
}
