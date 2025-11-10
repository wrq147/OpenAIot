import request from '@/common/request.js'
export const procf = {
	data() {
		return {
			lunboList: [], //轮播图列表
			search: false, //判断搜索是否要出现
			showCategory: false, //判断分类是否要出现
		}
	},
	methods:{
		setProConfig(config){
			let ProConfig = config;
			// console.log("产品设置信息", ProConfig);
			if (ProConfig) {
				let config = JSON.parse(ProConfig);
				// console.log("转化后产品设置信息this.config", config);
				let config2 = JSON.parse(ProConfig)
				if (config2[0]) {
					config = {
						oldInfo: config2
					}
				}
				if (config.lunbo) {
					if (config.lunbo.firstProImg) {
						this.lunboList.push(config.lunbo.firstProImg)
					}
					if (config.lunbo.secondProImg) {
						this.lunboList.push(config.lunbo.secondProImg)
					}
					if (config.lunbo.thirdProImg) {
						this.lunboList.push(config.lunbo.thirdProImg)
					}
				}else if (config.oldInfo) {
					config.oldInfo.map((item, index) => {
						if (item.type == 'search') {
							this.search = item.data
						}
						if (item.type == 'showCategory') {
							this.showCategory = item.data
						}
						if (item.type == 'lunbo') {
							// console.log("lunbo", item.data);
							item.data.forEach((url, i) => {
								// console.log("轮播图片链接", url.url);
								if (url.url && url.url != [] && url.url != '') {
									// console.log("轮播图片链接存在", url.url);
									let imgUrl = request.config.baseURL + url.url
									this.lunboList.push(imgUrl);
								}
							})
						}
					})
				}
				if (config.search) {
					this.search = config.search
				}
				if (config.showCategory) {
					this.showCategory = config.showCategory
				}
				
			}
		}
	}
}
