<template>
	<view style="width: 100%;height: 100vh;background-color: rgba(22, 26, 38, 1);">
		
	</view>
</template>

<script>
	export default {
		data() {
			return {
				
			}
		},
		onLoad(options) {
			this.$nextTick(()=>{
				if(options.url){
					uni.switchTab({
						url:options.url
					})
				}else{
					uni.switchTab({
						url:'/pages/home/home'
					})
				}
				
			})
		},
		methods: {
			
		}
	}
</script>

<style>

</style>
